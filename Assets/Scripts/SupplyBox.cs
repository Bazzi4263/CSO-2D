using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupplyBox : MonoBehaviourPun
{
    public PhotonView PV;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Human") && collision.gameObject.GetPhotonView().IsMine && collision.GetComponent<HumanScript>() != null)
        {
            int randomNum = Random.Range(0, 100);

            if (randomNum < 20)
            {
                if (WeaponSwitch.weaponNum == 8)
                {
                    collision.transform.GetChild(8).GetComponent<Barret>().maxAmmo = 105;
                    GameManager.instance.GetComponent<UI>().MaxAmmo = collision.transform.GetChild(8).GetComponent<Barret>().maxAmmo;
                }
                else
                {
                    PV.RPC("DisActiveRPC", RpcTarget.All, WeaponSwitch.mineID, WeaponSwitch.weaponNum);
                    WeaponSwitch.weaponNum = 8;
                    collision.GetComponent<PhotonView>().RPC("SwapFirstWeapon", RpcTarget.All, WeaponSwitch.mineID, WeaponSwitch.weaponNum);
                    collision.transform.GetChild(8).GetComponent<Barret>().ammo = 15;
                    collision.transform.GetChild(8).GetComponent<Barret>().maxAmmo = 90;
                }
            }
            else if (randomNum < 40)
            {
                if (WeaponSwitch.weaponNum == 9)
                {
                    collision.transform.GetChild(9).GetComponent<Minigun>().maxAmmo = 500;
                    GameManager.instance.GetComponent<UI>().MaxAmmo = collision.transform.GetChild(9).GetComponent<Minigun>().maxAmmo;
                }
                else
                {
                    PV.RPC("DisActiveRPC", RpcTarget.All, WeaponSwitch.mineID, WeaponSwitch.weaponNum);
                    WeaponSwitch.weaponNum = 9;
                    collision.GetComponent<PhotonView>().RPC("SwapFirstWeapon", RpcTarget.All, WeaponSwitch.mineID, WeaponSwitch.weaponNum);
                    collision.transform.GetChild(9).GetComponent<Minigun>().ammo = 250;
                    collision.transform.GetChild(9).GetComponent<Minigun>().maxAmmo = 250;
                }
            }
            else if(randomNum < 60)
            {
                if (WeaponSwitch.weaponNum == 11)
                {
                    collision.transform.GetChild(11).GetComponent<USAS12>().maxAmmo = 240;
                    GameManager.instance.GetComponent<UI>().MaxAmmo = collision.transform.GetChild(9).GetComponent<USAS12>().maxAmmo;
                }
                else
                {
                    PV.RPC("DisActiveRPC", RpcTarget.All, WeaponSwitch.mineID, WeaponSwitch.weaponNum);
                    WeaponSwitch.weaponNum = 11;
                    collision.GetComponent<PhotonView>().RPC("SwapFirstWeapon", RpcTarget.All, WeaponSwitch.mineID, WeaponSwitch.weaponNum);
                    collision.transform.GetChild(11).GetComponent<USAS12>().ammo = 15;
                    collision.transform.GetChild(11).GetComponent<USAS12>().maxAmmo = 120;
                }
            }
            else
            {
                if (WeaponSwitch.weaponNum == 12)
                {
                    collision.transform.GetChild(12).GetComponent<USAS12>().maxAmmo = 600;
                    GameManager.instance.GetComponent<UI>().MaxAmmo = collision.transform.GetChild(9).GetComponent<OICW>().maxAmmo;
                }
                else
                {
                    PV.RPC("DisActiveRPC", RpcTarget.All, WeaponSwitch.mineID, WeaponSwitch.weaponNum);
                    WeaponSwitch.weaponNum = 12;
                    collision.GetComponent<PhotonView>().RPC("SwapFirstWeapon", RpcTarget.All, WeaponSwitch.mineID, WeaponSwitch.weaponNum);
                }
            }

            PV.RPC("DestroyRPC", RpcTarget.All);
        }
    }

    [PunRPC]
    void DestroyRPC() => Destroy(gameObject);

    [PunRPC]
    public void DisActiveRPC(int _id, int num)
    {
        PhotonView.Find(_id).transform.GetChild(num).gameObject.SetActive(false);
    }
}
