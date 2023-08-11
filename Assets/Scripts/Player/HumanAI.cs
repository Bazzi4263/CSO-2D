using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class HumanAI : MonoBehaviourPunCallbacks
{
    enum WAYPOINT
    {
        FALLAWAY = 0,
        ROOF,
        HOUSE,
        CHURCH,
        RANDOM,
        END
    }

    public AIRifle aiRifle;

    private NavMeshAgent agent;
    private AudioSource audioSource;

    public PhotonView PV;
    public AudioClip footstepClip;

    public Transform[] randomWaypoints;
    int waypointIndex = 1;
    public Transform target;
    bool isEnd = false;

    public GameObject zombie;

    private int offMeshArea = 0;
    private int climbMeshArea = 2;
    private float moveSpeed = 400;
    private float climbSpeed = 3f;

    WAYPOINT waypoint;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();

        if (PhotonNetwork.PlayerList.Length + GameManager.instance.botCount <= 15)
        {
            waypoint = (WAYPOINT)(Random.Range(0, (int)WAYPOINT.RANDOM));
        }
        else
        {
            waypoint = (WAYPOINT)(Random.Range(0, (int)WAYPOINT.END));
        }
    }

    private void Start()
    {
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        StartCoroutine(StartWarp());
        StartCoroutine(StartClimb());

        switch (waypoint)
        {
            case WAYPOINT.FALLAWAY:
                target = GameObject.Find("A Faraway Place").transform;
                agent.SetDestination(target.position);
                break;
            case WAYPOINT.ROOF:
                target = GameObject.Find("Roof").transform;
                agent.SetDestination(target.position);
                break;
            case WAYPOINT.HOUSE:
                target = GameObject.Find("House").transform;
                agent.SetDestination(target.position);
                break;
            case WAYPOINT.CHURCH:
                target = GameObject.Find("Church").transform;
                agent.SetDestination(target.position);
                break;
            case WAYPOINT.RANDOM:
                randomWaypoints = GameObject.Find("RandomWaypoint").GetComponentsInChildren<Transform>();
                target = randomWaypoints[1];
                agent.SetDestination(target.position);
                break;
            default:
                break;
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
        agent.isStopped = true;

        OffMeshLinkData linkData = agent.currentOffMeshLinkData;
        Vector3 start = linkData.startPos;
        Vector3 end = linkData.endPos;

        float warpTime = Vector2.Distance(start, end) / moveSpeed;
        float currentTime = 0f;
        float percent = 0;

        while ( percent < 1)
        {
            currentTime += Time.deltaTime;
            percent = currentTime / warpTime;

            transform.position = Vector3.Lerp(start, end, percent);

            yield return null;
        }

        agent.CompleteOffMeshLink();

        agent.isStopped = false;
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

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        if (target == null || isEnd)
        {
            return;
        }

        if (!agent.isStopped)
        {
            PV.RPC("PlayFootstepSound", RpcTarget.All);
        }

        if (aiRifle.target != null && waypoint == WAYPOINT.RANDOM)
        {
            agent.isStopped = true;
        }
        else if (aiRifle.target == null && waypoint == WAYPOINT.RANDOM)
        {
            agent.isStopped = false;
        }

        CheckDistance();
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

    private void CheckDistance()
    {
        if (Vector2.Distance(target.position, transform.position) < 1f && waypoint == WAYPOINT.RANDOM)
        {
            target = randomWaypoints[waypointIndex];
            UpdateWaypoint();
            agent.SetDestination(target.position);
        }
        else if (Vector2.Distance(target.position, transform.position) < 1.5f && waypoint != WAYPOINT.RANDOM)
        {
            agent.SetDestination(transform.position);
        }
        else if (Vector2.Distance(target.position, transform.position) > 1.5f && waypoint != WAYPOINT.RANDOM)
        {
            agent.SetDestination(target.transform.position);
        }
    }

    public void Escape()
    {
        switch (waypoint)
        {
            case WAYPOINT.FALLAWAY:
                target = GameObject.Find("House").transform;
                agent.SetDestination(target.transform.position);
                break;
            case WAYPOINT.ROOF:
                target = GameObject.Find("A Faraway Place").transform;
                agent.SetDestination(target.transform.position);
                break;
            case WAYPOINT.HOUSE:
                target = GameObject.Find("Roof").transform;
                agent.SetDestination(target.transform.position);
                break;
            case WAYPOINT.CHURCH:
                target = GameObject.Find("Roof").transform;
                agent.SetDestination(target.transform.position);
                break;
            case WAYPOINT.RANDOM:
                break;
            default:
                break;
        }
    }

    private void UpdateWaypoint()
    {
        waypointIndex++;
        if (waypointIndex == randomWaypoints.Length)
        {
            waypointIndex = 1;
        }
    }


    [PunRPC]
    public void InfectToZombie(bool _isFirst)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            ++GameManager.instance.zombieCount;
            --GameManager.instance.humanCount;
            InformZombie();
        }

        if (_isFirst && PhotonNetwork.PlayerList.Length + GameManager.instance.botCount == 5)
        {
            zombie.GetComponent<ZombieAI>().HP = 6000;
        }
        else if (_isFirst && PhotonNetwork.PlayerList.Length + GameManager.instance.botCount > 2)
        {
            zombie.GetComponent<ZombieAI>().HP += (PhotonNetwork.PlayerList.Length + GameManager.instance.botCount) * 500;
        }
        else if (PhotonNetwork.PlayerList.Length + GameManager.instance.botCount > 5 && !_isFirst)
        {
            zombie.GetComponent<ZombieAI>().HP += (PhotonNetwork.PlayerList.Length + GameManager.instance.botCount - 5) * 100;
        }

        zombie.transform.parent = null;
        zombie.SetActive(true);

        GameManager.instance.isStartGame = true;
        if (!_isFirst) GameManager.instance.ZombieInfectSound();
        this.gameObject.SetActive(false);
    }

    private void InformZombie()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 5f);

        foreach (Collider2D hit in hits)
        {
            if (hit.GetComponent<HumanAI>() != null)
            {
                hit.GetComponent<HumanAI>().Escape();
            }
        }
    }
}
