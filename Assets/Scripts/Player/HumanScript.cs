using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HumanScript : MonoBehaviourPunCallbacks, IPunObservable
{
    public static int HP = 100;

    public GameObject zombie;
    public FieldOfView fieldOfView;
    public SpriteRenderer spriteRenderer;
    public Sprite frontSprite;
    public Sprite behindSprite;
    public Rigidbody2D RB;
    public PhotonView PV;
    public float maxSpeed;
    public float acceleration;
    public float t_moveSpeed;
    public GameObject go_Light;
    public Text nickName;
    public bool isMove;
    public PolygonCollider2D boxCollider2D;

    float upAxis;
    float rightAxis;

    int t_upAxis = 0;
    int t_rightAxis = 0;

    bool reverseX;
    bool reverseY;
    float reverseTimer = 0.1f;

    public AudioClip[] startClips;
    public AudioClip footstepClip;

    bool isActive = true;

    AudioSource audioSource;

    private void Start()
    {
        this.gameObject.SetActive(true);

        audioSource = GetComponent<AudioSource>();

        nickName.text = PV.IsMine ? PhotonNetwork.NickName : PV.Owner.NickName;

        t_moveSpeed = maxSpeed;

        if (PV.IsMine)
        {     
            audioSource.clip = startClips[Random.Range(0, 2)];
            audioSource.volume = 0.4f;
            audioSource.Play();
            audioSource.volume = 1f;

            GameObject black = GameObject.FindWithTag("Black");
            GameObject.FindWithTag("CMCamera").GetComponent<CameraFollowing>().targetObject = this.gameObject;
            GameObject.FindWithTag("CMCamera").GetComponent<CameraFollowing>().CC.m_BoundingShape2D = this.boxCollider2D;
            GameObject.FindWithTag("CMCamera").GetComponent<CameraFollowing>().SetFollower(GameObject.Find("PosObject"));
            gameObject.GetComponent<HumanScript>().fieldOfView = GameObject.FindWithTag("FOV").GetComponent<FieldOfView>();
            black.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0.7f);
            if (fieldOfView.isZoom)
            {
                fieldOfView.fov = 360;
                fieldOfView.viewDistance = 30;
                fieldOfView.rayCount = 800;
                fieldOfView.isZoom = false;
            }
        }
    }

    private void Update()
    {
        StartCoroutine(ReverseTimer());
        Look();
        SetOrigin();

        //if (PhotonNetwork.IsMasterClient)
        //{
        //    if (Input.GetKeyDown(KeyCode.K))
        //    {
        //        PhotonNetwork.Instantiate("ZombieAI", new Vector3(0, -10f, 0), Quaternion.identity);
        //        GameManager.instance.zombieCount++;
        //    }

        //    if (Input.GetKeyDown(KeyCode.H))
        //    {
        //        PhotonNetwork.Instantiate("HumanAI", new Vector3(0, -10f, 0), Quaternion.identity);
        //    }
        //}
    }

    private void FixedUpdate()
    {
        Move();
    }

    // 플레이어 이동
    public void Move()
    {
        if (PV.IsMine)
        {
            Vector2 dir = Vector3.ClampMagnitude(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")), maxSpeed);

            if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
            {
                PV.RPC("PlayFootstepSound", RpcTarget.All);
            }

            RB.velocity = dir * maxSpeed;

            //if (RB.velocity.magnitude < maxSpeed)*
            //{
            //    upAxis = Input.GetAxis("Vertical");
            //    rightAxis = Input.GetAxis("Horizontal");

            //    RB.velocity = new Vector2(maxSpeed * rightAxis, RB.velocity.y);
            //    RB.velocity = new Vector2(RB.velocity.x, maxSpeed  upAxis);
            //}
            //else
            //{
            //    upAxis = Input.GetAxisRaw("Vertical");
            //    rightAxis = Input.GetAxisRaw("Horizontal");

            //    RB.velocity = new Vector2(maxSpeed * rightAxis, RB.velocity.y);
            //    RB.velocity = new Vector2(RB.velocity.x, maxSpeed * upAxis);
            //}
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
        fieldOfView.SetOrigin(transform.position);

        if (fieldOfView.isZoom)
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPosition.z = 0;

            Vector3 aimDir = (worldPosition - transform.position).normalized;

            fieldOfView.SetAimDirection(aimDir);

            go_Light.SetActive(true);
        }
        else
        {
            go_Light.SetActive(false);
        }
    }

    IEnumerator ReverseTimer()
    {
        if (t_rightAxis * rightAxis < 0)
        {
            reverseX = true;
        }
        if (t_upAxis * upAxis < 0)
        {
            reverseY = true;
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
            zombie.GetComponent<ZombieScript>().HP = 6000;
            zombie.GetComponent<ZombieScript>().isFirstZombie = true;
        }
        else if (_isFirst && (PhotonNetwork.PlayerList.Length + GameManager.instance.botCount) > 2)
        {
            zombie.GetComponent<ZombieScript>().HP += (PhotonNetwork.PlayerList.Length + GameManager.instance.botCount) * 500;
            zombie.GetComponent<ZombieScript>().isFirstZombie = true;
        }
        else if (PhotonNetwork.PlayerList.Length + GameManager.instance.botCount > 5 && !_isFirst)
        {
            zombie.GetComponent<ZombieScript>().HP += (PhotonNetwork.PlayerList.Length + GameManager.instance.botCount - 5) * 100;
        }

        zombie.transform.parent = null;
        zombie.SetActive(true);

        GameManager.instance.isStartGame = true;        
        if(!_isFirst) GameManager.instance.ZombieInfectSound();
        isActive = false;
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

    [PunRPC]
    void PlayFootstepSound()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.clip = footstepClip;
            audioSource.Play();
        }
    }

    void OnDestroy()
    {
        if (isActive && PhotonNetwork.IsMasterClient)
        {
            GameManager.instance.humanCount--;
        }
    }

    [PunRPC]
    void DestroyRPC() => Destroy(gameObject);

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
