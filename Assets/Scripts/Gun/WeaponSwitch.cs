using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WeaponSwitch : MonoBehaviourPun
{
    public PhotonView PV;

    public static int weaponNum;

    public static bool isFirstWeapon = true;

    GameObject[] humans;
    public static int mineID;

    private void Start()
    {
        humans = new GameObject[PhotonNetwork.PlayerList.Length];

        humans = GameObject.FindGameObjectsWithTag("Human");

        for (int i = 0; i < humans.Length; i++)
        {
            if (humans[i].GetPhotonView().IsMine)
            {
                mineID = humans[i].GetPhotonView().ViewID;
                break;
            }
        }
    }

    void Update()
    {
        Swap();
    }

    public void Swap()
    {
        if (this.gameObject.tag == "Human" && PV.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) && !isFirstWeapon)
            {
                this.GetComponentInChildren<Gun>().CameraShake(0, 0);
                PV.RPC("SwapFirstWeapon", RpcTarget.All, mineID, weaponNum);
            }
        
            if (Input.GetKeyDown(KeyCode.Alpha2) && isFirstWeapon)
            {
                this.GetComponentInChildren<Gun>().CameraShake(0, 0);
                PV.RPC("SwapSecondWeapon", RpcTarget.All, mineID, weaponNum);
            }
        }
    }

    [PunRPC]
    public void SwapFirstWeapon(int _id, int _weaponNum)
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (humans[i].GetPhotonView().ViewID == _id)
            {
                if (PV.IsMine)
                {
                    isFirstWeapon = true;
                }
                humans[i].transform.GetChild(_weaponNum).gameObject.SetActive(true);
                humans[i].transform.GetChild(1).gameObject.SetActive(false);
                return;
            }
        }
    }

    [PunRPC]
    public void SwapSecondWeapon(int _id, int _weaponNum)
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (humans[i].GetPhotonView().ViewID == _id)
            {
                if (PV.IsMine)
                {
                    isFirstWeapon = false;
                }
                humans[i].transform.GetChild(_weaponNum).gameObject.SetActive(false);
                humans[i].transform.GetChild(1).gameObject.SetActive(true);
                return;
            }
        }
    }
}
