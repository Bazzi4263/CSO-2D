using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using Cinemachine;
//using Photon.Pun.UtilityScripts;
//using TMPro;
//using System.Runtime.Remoting.Metadata.W3cXsd2001;

public class ZombieScript : MonoBehaviourPunCallbacks, IPunObservable
{
    public bool isHit = false;

    public FieldOfView fieldOfView;
    public SpriteRenderer spriteRenderer;
    public Sprite frontSprite;
    public Sprite behindSprite;
    public Rigidbody2D RB;
    public PhotonView PV;
    public GameObject spectator;
    public float maxSpeed;
    public float acceleration;
    public int maxHP;
    public int HP;
    public Text nickName;

    public float upAxis;
    public float rightAxis;

    int t_upAxis;
    int t_rightAxis;

    bool reverseX;
    bool reverseY;
    float reverseTimer = 0.1f;

    public AudioClip footstepClip;
    public AudioClip deathClip;
    public AudioClip healClip;

    float healTimer = 5f;
    float currentHealTime = 5f;

    GameObject black;

    UI ui;

    bool canMove = true;
    public bool isMove;

    float currentTime = 0;

    AudioSource audioSource;

    bool isHealing = false;
    bool canHeal = false;
    public bool isFirstZombie = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        black = GameObject.FindWithTag("Black");
        ui = GameObject.FindGameObjectWithTag("GameManager").GetComponent<UI>();
    }

    private void Start()
    {
        nickName.text = PV.IsMine ? PhotonNetwork.NickName : PV.Owner.NickName;
        maxHP = HP;

        if (PV.IsMine)
        {
            GameObject CM = GameObject.FindGameObjectWithTag("CMCamera");
            CM.GetComponent<CameraFollowing>().targetObject = null;
            CM.GetComponent<CameraFollowing>().SetFollower(this.gameObject);
            CM.GetComponent<CameraFollowing>().CC.m_BoundingShape2D = null;

            CinemachineBasicMultiChannelPerlin CP = CM.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            CP.m_AmplitudeGain = 0f;

            gameObject.GetComponent<ZombieScript>().fieldOfView = GameObject.FindWithTag("FOV").GetComponent<FieldOfView>();
            SetOrigin();
        }
    }

    private void Update()
    {
        Look();
        CheckHP();
        CheckStifness();
        StartCoroutine(ReverseTimer());
        StartCoroutine(HealHP());
        HealTimer();
        if (PV.IsMine) ui.hp = HP;
        if (PV.IsMine) fieldOfView.SetOrigin(transform.position);
    }

    private void FixedUpdate()
    {
        Move();
    }

    // 플레이어 이동
    public void Move()
    {
        if (PV.IsMine && !isHit && canMove)
        {
            if (RB.velocity.magnitude < maxSpeed)
            {
                upAxis = Input.GetAxisRaw("Vertical");
                rightAxis = Input.GetAxisRaw("Horizontal");

                if (upAxis == 0)
                {
                    RB.velocity = new Vector2(RB.velocity.x, 0);
                }
                if (rightAxis == 0)
                {
                    RB.velocity = new Vector2(0, RB.velocity.y);
                }

                RB.AddForce(new Vector2(rightAxis * acceleration, upAxis * acceleration));
            }
            else
            {
                upAxis = Input.GetAxisRaw("Vertical");
                rightAxis = Input.GetAxisRaw("Horizontal");

                RB.velocity = new Vector2(maxSpeed * rightAxis, RB.velocity.y);
                RB.velocity = new Vector2(RB.velocity.x, maxSpeed * upAxis);
            }

            if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
            {
                PV.RPC("PlayFootstepSound", RpcTarget.All);
            }
        }

        if (RB.velocity.magnitude != 0 && PV.IsMine)
        {
            isMove = true;
        }
        else if (PV.IsMine)
        {
            isMove = false;
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

    public void CheckRaycastZombie(float upAxis, float rightAxis)
    {
        if (upAxis != 0 || rightAxis != 0)
        {
            if (upAxis > 0)
            {
                RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector3.up, 0.85f);

                foreach (var hit in hits)
                {
                    if (hit.collider != null)
                    {
                        if ((hit.transform.CompareTag("Zombie") || hit.transform.CompareTag("ZombieAI")) && hit.transform.gameObject != this.transform.gameObject)
                        {
                            canMove = false;
                        }
                        else
                        {
                            canMove = true;
                        }
                    }
                }            
            }

            if (upAxis < 0)
            {
                RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector3.down, 0.85f);

                foreach (var hit in hits)
                {
                    if (hit.collider != null)
                    {
                        if ((hit.transform.CompareTag("Zombie") || hit.transform.CompareTag("ZombieAI")) && hit.transform.gameObject != this.transform.gameObject)
                        {
                            canMove = false;
                        }
                        else
                        {
                            canMove = true;
                        }
                    }
                }
            }

            if (rightAxis > 0)
            {
                RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector3.right, 0.85f);

                foreach (var hit in hits)
                {
                    if (hit.collider != null)
                    {
                        if ((hit.transform.CompareTag("Zombie") || hit.transform.CompareTag("ZombieAI")) && hit.transform.gameObject != this.transform.gameObject)
                        {
                            canMove = false;
                        }
                        else
                        {
                            canMove = true;
                        }
                    }
                }
            }

            if (rightAxis < 0)
            {
                RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector3.left, 0.85f);

                foreach (var hit in hits)
                {
                    if (hit.collider != null)
                    {
                        if ((hit.transform.CompareTag("Zombie") || hit.transform.CompareTag("ZombieAI")) && hit.transform.gameObject != this.transform.gameObject)
                        {
                            canMove = false;
                        }
                        else
                        {
                            canMove = true;
                        }
                    }
                }
            }
        }
    }

    // 플레이어 시점
    public void Look()
    {
        if (PV.IsMine)
        {
            if (Camera.main.ScreenToWorldPoint(Input.mousePosition).y < this.transform.position.y)
                PV.RPC("LookFrontSynchronize", RpcTarget.All, this.gameObject.GetPhotonView().ViewID);
            else
                PV.RPC("LookBehindSynchronize", RpcTarget.All, this.gameObject.GetPhotonView().ViewID);
        }
    }

    public void SetOrigin()
    {

        fieldOfView.fov = 360;
        fieldOfView.viewDistance = 35;
        fieldOfView.rayCount = 800;
        fieldOfView.isZoom = false;
        GameObject.FindWithTag("CMCamera").GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = 7;
        black.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0.7f);

    }

    [PunRPC]
    public void Hit(int _dmgMin, int _dmgMax, int _criticalDmg, float _stifness, float _dirx,float _diry, float _knockBack)
    {
        if (PV.IsMine)
        {
            StopCoroutine(HealHP());
            isHit = true;
            canHeal = false;
            currentHealTime = healTimer;

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
            SetStiffness(_stifness);
            EffectScript.instance.GetComponent<PhotonView>().RPC("DamageIndicator", RpcTarget.All, isCritical, dmg, transform.position);
            PV.RPC("Damage", RpcTarget.All, dmg);
        }
    }

    IEnumerator KnockBack(float _knockBack, Vector3 _dir, float _stifness)
    {
        if (_knockBack == 0)
        {
            RB.velocity = Vector2.zero;
            yield break;
        }

        if (_stifness > 0.1f)
        {
            RB.velocity = new Vector2(_dir.x * _knockBack, _dir.y * _knockBack);
            yield return new WaitForSeconds(0.1f);
            RB.velocity = new Vector2(0, 0);
        }
        else
        {
            RB.velocity = new Vector2(_dir.x * _knockBack * (0.1f / _stifness), _dir.y * _knockBack * (0.1f / _stifness));
            yield return new WaitForSeconds(_stifness);
            RB.velocity = new Vector2(0, 0);
        }
    }

    public void SetStiffness(float _time)
    {
        if (PV.IsMine)
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
        if (!PV.IsMine)
        {
            return;
        }

        if (!isHit)
        {
            return;
        }

        if (isFirstZombie)
        {
            if (currentTime > 0.05f)
            {
                currentTime -= Time.deltaTime;
                isHit = true;
            }
            else
            {
                isHit = false;
            }

            return;
        }

        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            isHit = true;
        }
        else
        {
            isHit = false;
        }
    }

    private void CheckHP()
    {
        if (HP <= 0)
        {
            HP = 0;
            if (PV.IsMine)
            {
                GameObject.Instantiate(spectator, transform.position, Quaternion.identity);
                EffectScript.instance.GetComponent<PhotonView>().RPC("DeathEffect", RpcTarget.All, transform.position);
                PV.RPC("DestroyRPC", RpcTarget.AllBuffered);
            }

            GameManager.instance.ZombieDeathSound();
        }
    }

    IEnumerator HealHP()
    {
        if (!isMove && isFirstZombie && PV.IsMine && !isHealing && !isHit && canHeal)
        {
            isHealing = true;
            while (!isMove && HP < maxHP && !isHit && canHeal)
            {
                PV.RPC("PlusHP", RpcTarget.All, 300);
                audioSource.PlayOneShot(healClip);
                yield return new WaitForSeconds(1f);
            }
            isHealing = false;
        }
    }

    void HealTimer()
    {
        if (!PV.IsMine)
        {
            return;
        }

        if (isMove)
        {
            currentHealTime = healTimer;
            canHeal = false;
            return;
        }

        if (currentHealTime > 0 && !canHeal)
        {
            canHeal = false;
            currentHealTime -= Time.deltaTime;
        }
        else
        {
            canHeal = true;
        }
    }

    public void OnDestroy()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            --GameManager.instance.zombieCount;
        }
    }

    IEnumerator ReverseTimer()
    {
        if (true)
        {

        }

        if (reverseX)
        {
            yield return new WaitForSeconds(0.1f);
            reverseX = false;
        }

        if (reverseY)
        {
            yield return new WaitForSeconds(0.1f);
            reverseY = false;
        }
    }

    [PunRPC]
    private void PlusHP(int _hp)
    {
        HP += _hp;
        if (HP > maxHP)
        {
            HP = maxHP;
        }
    }

    [PunRPC]
    private void SetHP(int _hp)
    {
        HP = _hp;
    }

    [PunRPC]
    private void Damage(int _dmg)
    {
        HP -= _dmg;
    }

    [PunRPC]
    private void DestroyRPC() => Destroy(gameObject);

    #region 시점 동기화
    [PunRPC]
    public void LookFrontSynchronize(int _playerID)
    {
        GameObject _player = PhotonView.Find(_playerID).gameObject;
        SpriteRenderer _spriteRenderer = _player.GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = frontSprite;
    }

    [PunRPC]
    public void LookBehindSynchronize(int _playerID)
    {
        GameObject _player = PhotonView.Find(_playerID).gameObject;
        SpriteRenderer _spriteRenderer = _player.GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = behindSprite;
    }
    #endregion

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