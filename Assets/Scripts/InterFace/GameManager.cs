using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.UIElements;
using Cinemachine;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance = null;

    public GameObject timer_Obj;
    public Text timer;
    public int time;
    private int t_time;
    public PhotonView PV;
    public GameObject gui;
    public GameObject gameChat;
    public Text winnerText;

    public Transform[] startLocatns;

    public int zombieCount = 0;
    public int humanCount;

    public GameObject weaponSelect;

    public bool isStartGame = false;

    UI ui;
    public AudioSource audioSource;
    public AudioSource audioSource2;
    public AudioClip BGM;
    public AudioClip timerCLip;
    public AudioClip supplyClip;
    public AudioClip humanWinClip;
    public AudioClip zombieWinClip;
    public AudioClip zombieInfectClip;
    public AudioClip zombieDeathClip;
    public AudioClip explosionClip;
    public UnityEngine.UI.Image supplyBoxImage;

    bool isEnd = false;
    public static bool isChatEnd = false;

    int number1;
    int number2;

    private CinemachineVirtualCamera CM;

    public Texture2D cursorTexture;

    public GameObject cameraPosition;

    public int botCount = 0;

    public Text humanCountText;
    public Text zombieCountText;

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient && !isEnd)
        {
            CheckHumanCount();
            CheckZombieCount();
            PV.RPC("UpdateCountText", RpcTarget.All, humanCount, zombieCount);
        }
    }

    [PunRPC]
    private void UpdateCountText(int _humanCount, int _zombieCount)
    {
        humanCountText.text = _humanCount.ToString();
        zombieCountText.text = _zombieCount.ToString();
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        CM = GameObject.FindGameObjectWithTag("CMCamera").GetComponent<CinemachineVirtualCamera>();

        t_time = time;
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        t_time = time;
        ui = GetComponent<UI>();
    }

    public static GameManager gm
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }

    private void Start()
    {
        audioSource.clip = BGM;
        audioSource.Play();

#if UNITY_WEBGL
        UnityEngine.Cursor.SetCursor(cursorTexture, new Vector2(16, 16), CursorMode.ForceSoftware);
#else
        UnityEngine.Cursor.SetCursor(cursorTexture, new Vector2(16, 16), CursorMode.ForceSoftware);
#endif
    }

    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            humanCount = PhotonNetwork.PlayerList.Length + botCount;
            zombieCount = 0;
            for (int i = 0; i < botCount; i++)
            {
                int randomint = Random.Range(0, startLocatns.Length);
                PhotonNetwork.Instantiate("HumanAI", startLocatns[randomint].position, Quaternion.identity);
            }
            
        }
        audioSource.Stop();
        humanCountText.gameObject.SetActive(true);
        zombieCountText.gameObject.SetActive(true);
        gui.SetActive(true);
        gui.transform.GetChild(2).gameObject.SetActive(false);
        weaponSelect.SetActive(true);
        StartCoroutine(SetTimerStart());
    }

    IEnumerator SetTimerStart()
    {
        timer_Obj.SetActive(true);
        audioSource.clip = timerCLip;
        audioSource.Play();

        time = t_time;

        while (time > 0)
        {
            timer.text = time.ToString();
            yield return new WaitForSeconds(1f);
            --time;
        }

        timer_Obj.SetActive(false);
        SetZombieRandomly(); 
    }

    IEnumerator SetTimer()
    {
        timer_Obj.SetActive(true);
        time = 180;
        if(PhotonNetwork.IsMasterClient) PV.RPC("TimeSync", RpcTarget.All);
        while (time > 0)
        {
            timer.text = time.ToString();
            yield return new WaitForSeconds(1f);
            --time;

            if (time % 45 == 0 && PhotonNetwork.IsMasterClient)
            {
                int boxpostion = Random.Range(0, startLocatns.Length); 
                PhotonNetwork.Instantiate("SupplyBox", startLocatns[boxpostion].position , Quaternion.identity);
                PV.RPC("SetSupplyBoxImage", RpcTarget.All, startLocatns[boxpostion].position);
                PV.RPC("SupplySound", RpcTarget.All);
            }

            if (time % 5 == 0 && PhotonNetwork.IsMasterClient)
            {
                humanCount = GameObject.FindGameObjectsWithTag("Human").Length;
                zombieCount = GameObject.FindGameObjectsWithTag("Zombie").Length + GameObject.FindGameObjectsWithTag("ZombieAI").Length;
            }

            if (time == 0)
            {
                PV.RPC("HumanWin", RpcTarget.All);
                break;
            }
        }
    }

    [PunRPC]
    IEnumerator SetSupplyBoxImage(Vector3 _boxPos)
    {
        GameObject human = null;

        foreach (GameObject _human in GameObject.FindGameObjectsWithTag("Human"))
        {
            if (_human.GetPhotonView().IsMine)
            {
                human = _human;
                break;           
            }
        }

        if (human == null)
        {
            yield break;
        }

        for (int i = 0; i < 6; i++)
        {
            Vector2 angle = (_boxPos - human.transform.position).normalized;
            Vector2 dir = Vector2.zero;
            supplyBoxImage.gameObject.SetActive(true);

            dir = new Vector2((angle.x / 2) + 0.5f, (angle.y / 2) + 0.5f);

            supplyBoxImage.rectTransform.anchorMin = new Vector2(dir.x, dir.y);
            supplyBoxImage.rectTransform.anchorMax = new Vector2(dir.x, dir.y);
            supplyBoxImage.rectTransform.anchoredPosition = Vector3.zero;
            yield return new WaitForSeconds(0.5f);
            supplyBoxImage.gameObject.SetActive(false);
        }
    }

    public void SetZombieRandomly()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            int count = (PhotonNetwork.PlayerList.Length + botCount) / 5;
            if ((PhotonNetwork.PlayerList.Length + botCount) % 5 != 0) count += 1;
            Debug.Log(count);
            GameObject[] humans = GameObject.FindGameObjectsWithTag("Human");
            List<int> ids = new List<int>();

            foreach (GameObject human in humans)
            {
                ids.Add(human.GetPhotonView().ViewID);
            }

            for (int i = 0; i < count; i++)
            {
                int rand = Random.Range(0, ids.Count);
                int id = ids[rand];

                foreach (GameObject human in humans)
                {
                    if (human.GetPhotonView().ViewID == id)
                    {
                        if (human.GetComponent<HumanScript>() == null)
                        {
                            human.GetComponent<HumanAI>().PV.RPC("InfectToZombie", RpcTarget.All, true);
                        }
                        else
                        {
                            human.GetComponent<HumanScript>().PV.RPC("InfectToZombie", RpcTarget.All, true);
                        }
                    }
                }

                ids.RemoveAt(rand);
            }
        }

        StartCoroutine(SetTimer());
    }

    IEnumerator SetRandomNumber()
    {
        while (true)
        {
            Player[] players = PhotonNetwork.PlayerList;

            number1 = Random.Range(0, players.Length);
            number2 = Random.Range(0, players.Length);

            if (number1 != number2)
            {
                break;
            }

            yield return null;
        }
    }

    public void CheckZombieCount()
    {
        if (isStartGame && zombieCount == 0)
        {
            PV.RPC("HumanWin", RpcTarget.All);
        }
    }

    public void CheckHumanCount()
    {
        if (isStartGame && humanCount == 0)
        {
            PV.RPC("ZombieWin", RpcTarget.All);
        }
    }

    public void ZombieInfectSound()
    {
        audioSource.PlayOneShot(zombieInfectClip);
    }

    [PunRPC]
    IEnumerator ZombieWin()
    {
        isEnd = true;
        winnerText.color = new Color(1, 0, 0, 1);
        winnerText.text = "Zombie Win!";
        audioSource.clip = zombieWinClip;
        audioSource.Play();
        yield return new WaitForSeconds(3f);
        PhotonNetwork.Disconnect();
        ResetGame();
    }

    [PunRPC]
    IEnumerator HumanWin()
    {
        isEnd = true;
        winnerText.color = new Color(0.2f, 0.2f, 0.2f, 1);
        winnerText.text = "Human Win!";
        audioSource.clip = humanWinClip;
        audioSource.Play();
        yield return new WaitForSeconds(3f);
        PhotonNetwork.Disconnect();
        ResetGame();
    }

    [PunRPC]
    public void SupplySound()
    {
        audioSource.Stop();
        audioSource.clip = supplyClip;
        audioSource.Play();
    }

    public void ResetGame()
    {
        foreach (GameObject human in GameObject.FindGameObjectsWithTag("Human"))
        {
            DestroyImmediate(human);
        }

        foreach (GameObject zombie in GameObject.FindGameObjectsWithTag("Zombie"))
        {
            DestroyImmediate(zombie);
        }

        DestroyImmediate(GameObject.FindGameObjectWithTag("Spectator"));
        StopAllCoroutines();

        CinemachineBasicMultiChannelPerlin CP = CM.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        CP.m_AmplitudeGain = 0;
        humanCount = 0;
        zombieCount = 0;
        time = t_time;
        winnerText.text = "";
        gui.SetActive(false);
        gameChat.SetActive(false);
        humanCountText.gameObject.SetActive(false);
        zombieCountText.gameObject.SetActive(false);
        isStartGame = false;
        ui.hp = 100;
        timer_Obj.SetActive(false);
        audioSource.clip = BGM;
        audioSource.Play();
        isEnd = false;
        GameObject.FindGameObjectWithTag("CMCamera").GetComponent<CameraFollowing>().SetFollower(cameraPosition);
    }

    [PunRPC]
    public void TimeSync()
    {
        time = 180;
    }

    public void ZombieDeathSound()
    {
        audioSource2.PlayOneShot(zombieDeathClip);
    }
}
