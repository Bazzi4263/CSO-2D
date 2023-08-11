using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using Photon.Realtime;

public class ZombieAI : MonoBehaviour, IPunObservable
{
    public PhotonView PV;
    NavMeshAgent agent;

    public bool isHit = false;

    public Rigidbody2D RB;

    public int maxHP;
    public int HP;

    public AudioClip footstepClip;
    public AudioClip deathClip;

    public bool isMove;

    float currentTime = 0;

    AudioSource audioSource;

    public GameObject target;

    public List<GameObject> humans;

    bool isDestroy = false;

    private int offMeshArea = 0;
    private int climbMeshArea = 2;
    private float moveSpeed = 400;
    private float climbSpeed = 0.4f;
    
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (PhotonNetwork.IsMasterClient)
        {
            humans = new List<GameObject>(GameObject.FindGameObjectsWithTag("Human"));
        }
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        maxHP = HP;

        if (PhotonNetwork.IsMasterClient)
        { 
            StartCoroutine(FindTargetCoroutine());
            StartCoroutine(StartWarp());
            StartCoroutine(StartClimb());
        }
    }

    IEnumerator StartWarp()
    {
        while (true)
        {
            yield return new WaitUntil(() => IsOnWarp());

            yield return StartCoroutine(Warp());
        }
    }

    IEnumerator StartClimb()
    {
        while (true)
        {
            yield return new WaitUntil(() => IsOnClimb());

            yield return StartCoroutine(Climb());
        }
    }

    public bool IsOnClimb()
    {
        if (agent.isOnOffMeshLink)
        {
            OffMeshLinkData linkData = agent.currentOffMeshLinkData;

            if (linkData.offMeshLink != null && linkData.offMeshLink.area == climbMeshArea)
            {
                return true;
            }
        }

        return false;
    }

    public bool IsOnWarp()
    {
        if (agent.isOnOffMeshLink)
        {
            OffMeshLinkData linkData = agent.currentOffMeshLinkData;

            if (linkData.offMeshLink != null && linkData.offMeshLink.area == offMeshArea)
            {
                return true;
            }
        }

        return false;
    }

    IEnumerator Warp()
    {
        if (!isDestroy) agent.isStopped = true;

        OffMeshLinkData linkData = agent.currentOffMeshLinkData;
        Vector3 start = linkData.startPos;
        Vector3 end = linkData.endPos;

        float warpTime = Vector2.Distance(start, end) / moveSpeed;
        float currentTime = 0f;
        float percent = 0;

        while (percent < 1)
        {
            currentTime += Time.deltaTime;
            percent = currentTime / warpTime;

            transform.position = Vector3.Lerp(start, end, percent);

            yield return null;
        }

        agent.CompleteOffMeshLink();

        if (!isDestroy) agent.isStopped = false;
    }

    IEnumerator Climb()
    {
        agent.isStopped = true;

        OffMeshLinkData linkData = agent.currentOffMeshLinkData;
        Vector3 start = linkData.startPos;
        Vector3 end = linkData.endPos;

        float warpTime = Vector2.Distance(start, end) / climbSpeed;
        float currentTime = 0f;
        float percent = 0;

        while (percent < 1)
        {
            currentTime += Time.deltaTime;
            percent = currentTime / warpTime;

            transform.position = Vector3.Lerp(start, end, percent);

            yield return null;
        }

        agent.CompleteOffMeshLink();

        agent.isStopped = false;
    }

    void Update()
    {
        CheckHP();
        CheckStifness();

        Move();
        CheckRaycastZombie();
    }

    public void Move()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        if (!isDestroy && !isHit)
        {
            if (!agent.isStopped)
            {
                PV.RPC("PlayFootstepSound", RpcTarget.All);
            }

            if (target != null)
            {
                agent.SetDestination(target.transform.position);
            }
        }
    }

    public void CheckRaycastZombie()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        RaycastHit2D[] uphits = Physics2D.RaycastAll(transform.position, Vector3.up, 0.85f);

        foreach (var hit in uphits)
        {
            if (hit.collider != null)
            {
                if ((hit.transform.CompareTag("Zombie") || hit.transform.CompareTag("ZombieAI")) && hit.transform.gameObject != this.transform.gameObject)
                {
                    if (agent.velocity.y > 0)
                    {
                        agent.velocity = new Vector2(agent.velocity.x, 0);
                    }
                }
            }
        }


        RaycastHit2D[] downhits = Physics2D.RaycastAll(transform.position, Vector3.down, 0.85f);

        foreach (var hit in downhits)
        {
            if (hit.collider != null)
            {
                if ((hit.transform.CompareTag("Zombie") || hit.transform.CompareTag("ZombieAI")) && hit.transform.gameObject != this.transform.gameObject)
                {
                    if (agent.velocity.y < 0)
                    {
                        agent.velocity = new Vector2(agent.velocity.x, 0);
                    }
                }
            }
        }



        RaycastHit2D[] righthits = Physics2D.RaycastAll(transform.position, Vector3.right, 0.85f);

        foreach (var hit in righthits)
        {
            if (hit.collider != null)
            {
                if ((hit.transform.CompareTag("Zombie") || hit.transform.CompareTag("ZombieAI")) && hit.transform.gameObject != this.transform.gameObject)
                {
                    if (agent.velocity.x > 0)
                    {
                        agent.velocity = new Vector2(0, agent.velocity.y);
                    }
                }
            }
        }



        RaycastHit2D[] lefthits = Physics2D.RaycastAll(transform.position, Vector3.left, 0.85f);

        foreach (var hit in lefthits)
        {
            if (hit.collider != null)
            {
                if ((hit.transform.CompareTag("Zombie") || hit.transform.CompareTag("ZombieAI")) && hit.transform.gameObject != this.transform.gameObject)
                {
                    if (agent.velocity.x < 0)
                    {
                        agent.velocity = new Vector2(0, agent.velocity.y);
                    }
                }
            }
        }


    }

    IEnumerator FindTargetCoroutine()
    {
        while (true)
        {
            NavMeshPath path = new NavMeshPath();
            List<Transform> humansTransform = new List<Transform>();
            humans = new List<GameObject>(GameObject.FindGameObjectsWithTag("Human"));

            foreach (GameObject human in humans)
            {
                Transform t = human.transform;
                t.transform.position = new Vector3(t.position.x, t.position.y, 0);
                humansTransform.Add(t);
            }

            Transform[] targets = humansTransform.ToArray();

            GameObject closestTarget = null;
            float closestTargetDistance = float.MaxValue;

            for (int i = 0; i < targets.Length; i++)
            {
                if (isDestroy)
                {
                    break;
                }

                if (agent.CalculatePath(humans[i].transform.position, path))
                {
                    float distance = Vector2.SqrMagnitude(transform.position - path.corners[0]);

                    for (int j = 1; j < path.corners.Length; j++)
                    {
                        distance += Vector2.SqrMagnitude(path.corners[j - 1] - path.corners[j]);
                    }

                    if (distance < closestTargetDistance)
                    {
                        closestTargetDistance = distance;
                        closestTarget = targets[i].gameObject;
                    }
                }
            }

            if (closestTarget != null)
            {
                target = closestTarget;
            }
            else
            {
                target = null;
            }


            yield return new WaitForSeconds(1f);
        }
    }

    [PunRPC]
    public void Hit(int _dmgMin, int _dmgMax, int _criticalDmg, float _stifness, float _dirx, float _diry, float _knockBack)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (!isDestroy) agent.SetDestination(transform.position);
            if (!isDestroy) agent.isStopped = true;
            isHit = true;

            EffectScript.instance.GetComponent<PhotonView>().RPC("BloodEffect", RpcTarget.All, transform.position);
            bool isCritical = false;

            int randomNum = Random.Range(0, 100);

            int dmg;

            if (randomNum < 10)
            {
                dmg = _criticalDmg;
                isCritical = true;
            }
            else
            {
                int randomDmg = Random.Range(_dmgMin, _dmgMax);
                dmg = randomDmg;
            }

            Vector3 dir = new Vector3(_dirx, _diry, 0);

            StartCoroutine(KnockBack(_knockBack, dir, _stifness));
            SetStiffness(_stifness + 0.1f);
            EffectScript.instance.GetComponent<PhotonView>().RPC("DamageIndicator", RpcTarget.All, isCritical, dmg, transform.position);
            PV.RPC("Damage", RpcTarget.All, dmg);
        }
    }

    [PunRPC]
    void PlayFootstepSound()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.clip = footstepClip;
            audioSource.Play();
        }
    }

    IEnumerator KnockBack(float _knockBack, Vector3 _dir, float _stifness)
    {
        if (_knockBack == 0)
        {
            agent.velocity = Vector2.zero;
            yield break;
        }

        if (_stifness > 0.1f)
        {
            agent.velocity = new Vector2(_dir.x * _knockBack, _dir.y * _knockBack);
            yield return new WaitForSeconds(0.1f);
            agent.velocity = new Vector2(0, 0);
        }
        else
        {
            agent.velocity = new Vector2(_dir.x * _knockBack * (0.1f / _stifness), _dir.y * _knockBack * (0.1f / _stifness));
            yield return new WaitForSeconds(_stifness);
            agent.velocity = new Vector2(0, 0);
        }   
    }

    public void SetStiffness(float _time)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (currentTime > _time)
            {
                return;
            }

            currentTime = _time;
        }
    }

    public void CheckStifness()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        if (!isHit)
        {
            return;
        }

        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            if (!isDestroy) agent.isStopped = true;
            isHit = true;
        }
        else
        {
            isHit = false;
            if (!isDestroy) agent.isStopped = false;
        }
    }

    private void CheckHP()
    {
        if (HP <= 0)
        {
            HP = 0;
            if (PhotonNetwork.IsMasterClient)
            {
                EffectScript.instance.GetComponent<PhotonView>().RPC("DeathEffect", RpcTarget.All, transform.position);
                PV.RPC("DestroyRPC", RpcTarget.AllBuffered);
            }

            GameManager.instance.ZombieDeathSound();
        }
    }

    public void OnDestroy()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            --GameManager.instance.zombieCount;
        }
    }

    [PunRPC]
    private void Damage(int _dmg)
    {
        HP -= _dmg;
    }

    [PunRPC]
    private void DestroyRPC()
    {
        agent.enabled = false;
        isDestroy = true;
        Destroy(gameObject);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isMove);
        }
        else
        {
            isMove = (bool)stream.ReceiveNext();
        }
    }
}
