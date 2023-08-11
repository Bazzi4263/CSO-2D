using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum WEAPONTYPE
{
    E_MP5 = 0,
    E_XM1014,
    E_M4A1,
    E_M249,
    E_G3SG1,
    E_AWP,
    E_AK47,
    E_M16A4,
    E_M870,
    E_SCARH,
    E_UMP45,
    E_PKP,
    E_DOUBLEBARREL,
    E_END
}

public class WeaponSelect : MonoBehaviourPun
{
    public PhotonView PV;
    public GameObject player;

    public Image img_Weapon;
    public Sprite img_MP5;
    public Sprite img_XM1014;
    public Sprite img_M4A1;
    public Sprite img_M249;
    public Sprite img_G3SG1;
    public Sprite img_AWP;
    public Sprite img_AK47;
    public Sprite img_M16A4;
    public Sprite img_M870;
    public Sprite img_SCARH;
    public Sprite img_UMP45;
    public Sprite img_PKP;
    public Sprite img_DoubleBarrel;
    public Sprite img_Random;

    public Image bar_Damage;
    public Image bar_Accuracy;
    public Image bar_Weight;
    public Image bar_FireRate;
    public Text text_Ammo;

    WEAPONTYPE weaponType;

    int playerID;

    private void OnEnable()
    {
        weaponType = (WEAPONTYPE)(Random.Range(0, (int)WEAPONTYPE.E_END));
        img_Weapon.sprite = img_Random;

        foreach (GameObject human in GameObject.FindGameObjectsWithTag("Human"))
        {
            if (human.GetPhotonView().IsMine && human.GetComponent<HumanScript>() != null)
            {
                playerID = human.GetPhotonView().ViewID;
            }
        }
    }

    public void MP5()
    {
        img_Weapon.sprite = img_MP5;
        bar_Damage.fillAmount = ((float)SMG.s_gunDamageMin + (float)SMG.s_gunDamageMax) / 200;
        bar_Accuracy.fillAmount = 1 - SMG.s_spread / 10;
        bar_Weight.fillAmount = SMG.s_decelerationSpeed + 0.3f;
        bar_FireRate.fillAmount = 1 - (SMG.s_fireRate * 5f);
        text_Ammo.text = SMG.s_ammo.ToString();
        weaponType = WEAPONTYPE.E_MP5;
    }

    public void XM1014()
    {
        img_Weapon.sprite = img_XM1014;
        bar_Damage.fillAmount = ((float)Shotgun.s_gunDamageMin + (float)Shotgun.s_gunDamageMax) / 200 * 3;
        bar_Accuracy.fillAmount = 1 - Shotgun.s_spread / 10 + 0.1f;
        bar_Weight.fillAmount = 1 - Shotgun.s_decelerationSpeed + 0.3f;
        bar_FireRate.fillAmount = 1 - (Shotgun.s_fireRate * 2f);
        text_Ammo.text = Shotgun.s_ammo.ToString();
        weaponType = WEAPONTYPE.E_XM1014;
    }

    public void M4A1()
    {
        img_Weapon.sprite = img_M4A1;
        bar_Damage.fillAmount = ((float)Rifle.s_gunDamageMin + (float)Rifle.s_gunDamageMax) / 200;
        bar_Accuracy.fillAmount = 1 - Rifle.s_spread / 10;
        bar_Weight.fillAmount = 1 - Rifle.s_decelerationSpeed + 0.3f;
        bar_FireRate.fillAmount = 1 - (Rifle.s_fireRate * 5f);
        text_Ammo.text = Rifle.s_ammo.ToString();
        weaponType = WEAPONTYPE.E_M4A1;
    }

    public void M249()
    {
        img_Weapon.sprite = img_M249;
        bar_Damage.fillAmount = ((float)LMG.s_gunDamageMin + (float)LMG.s_gunDamageMax) / 200;
        bar_Accuracy.fillAmount = 1 - LMG.s_spread / 10;
        bar_Weight.fillAmount = 1 - LMG.s_decelerationSpeed + 0.3f;
        bar_FireRate.fillAmount = (1 - (LMG.s_fireRate * 5f));
        text_Ammo.text = LMG.s_ammo.ToString();
        weaponType = WEAPONTYPE.E_M249;
    }

    public void G3SG1()
    {
        img_Weapon.sprite = img_G3SG1;
        bar_Damage.fillAmount = ((float)DMR.s_gunDamageMin + (float)DMR.s_gunDamageMax) / 230;
        bar_Accuracy.fillAmount = 1 - DMR.s_Zoomspread / 10;
        bar_Weight.fillAmount = 1 - DMR.s_decelerationSpeed + 0.3f;
        bar_FireRate.fillAmount = 1 - (DMR.s_fireRate * 2f);
        text_Ammo.text = DMR.s_ammo.ToString();
        weaponType = WEAPONTYPE.E_G3SG1;
    }

    public void AWP()
    {
        img_Weapon.sprite = img_AWP;
        bar_Damage.fillAmount = ((float)Sniper.s_gunDamageMin + (float)Sniper.s_gunDamageMax) / 200;
        bar_Accuracy.fillAmount = 1 - Sniper.s_Zoomspread / 10;
        bar_Weight.fillAmount = 1 - Sniper.s_decelerationSpeed + 0.3f;
        bar_FireRate.fillAmount = (1 - (Sniper.s_fireRate * 0.5f));
        text_Ammo.text = Sniper.s_ammo.ToString();
        weaponType = WEAPONTYPE.E_AWP;
    }

    public void F_AK47()
    {
        img_Weapon.sprite = img_AK47;
        bar_Damage.fillAmount = ((float)AK47.s_gunDamageMin + (float)AK47.s_gunDamageMax) / 200;
        bar_Accuracy.fillAmount = 1 - AK47.s_spread / 10;
        bar_Weight.fillAmount = 1 - AK47.s_decelerationSpeed + 0.3f;
        bar_FireRate.fillAmount = (1 - (AK47.s_fireRate * 5f));
        text_Ammo.text = AK47.s_ammo.ToString();
        weaponType = WEAPONTYPE.E_AK47;
    }

    public void F_M16A4()
    {
        img_Weapon.sprite = img_M16A4;
        bar_Damage.fillAmount = ((float)M16A4.s_gunDamageMin + (float)M16A4.s_gunDamageMax) / 200;
        bar_Accuracy.fillAmount = 1 - M16A4.s_spread / 10;
        bar_Weight.fillAmount = 1 - M16A4.s_decelerationSpeed + 0.3f;
        bar_FireRate.fillAmount = (1 - (M16A4.s_fireRate * 5f));
        text_Ammo.text = M16A4.s_ammo.ToString();
        weaponType = WEAPONTYPE.E_M16A4;
    }

    public void F_M870()
    {
        img_Weapon.sprite = img_M870;
        bar_Damage.fillAmount = ((float)M870.s_gunDamageMin + (float)M870.s_gunDamageMax) / 200 * 3;
        bar_Accuracy.fillAmount = 1 - M870.s_spread / 10;
        bar_Weight.fillAmount = 1 - M870.s_decelerationSpeed + 0.3f;
        bar_FireRate.fillAmount = 1 - (M870.s_fireRate * 0.5f);
        text_Ammo.text = M870.s_ammo.ToString();
        weaponType = WEAPONTYPE.E_M870;
    }

    public void F_SCARH()
    {
        img_Weapon.sprite = img_SCARH;
        bar_Damage.fillAmount = ((float)SCARH.s_gunDamageMin + (float)SCARH.s_gunDamageMax) / 200;
        bar_Accuracy.fillAmount = 1 - SCARH.s_spread / 10;
        bar_Weight.fillAmount = 1 - SCARH.s_decelerationSpeed + 0.3f;
        bar_FireRate.fillAmount = (1 - (SCARH.s_fireRate * 5f));
        text_Ammo.text = SCARH.s_ammo.ToString();
        weaponType = WEAPONTYPE.E_SCARH;
    }

    public void F_UMP45()
    {
        img_Weapon.sprite = img_UMP45;
        bar_Damage.fillAmount = ((float)UMP45.s_gunDamageMin + (float)UMP45.s_gunDamageMax) / 200;
        bar_Accuracy.fillAmount = 1 - UMP45.s_spread / 10;
        bar_Weight.fillAmount = UMP45.s_decelerationSpeed + 0.3f;
        bar_FireRate.fillAmount = (1 - (UMP45.s_fireRate * 5f));
        text_Ammo.text = UMP45.s_ammo.ToString();
        weaponType = WEAPONTYPE.E_UMP45;
    }

    public void F_PKP()
    {
        img_Weapon.sprite = img_PKP;
        bar_Damage.fillAmount = ((float)PKP.s_gunDamageMin + (float)PKP.s_gunDamageMax) / 200;
        bar_Accuracy.fillAmount = 1 - PKP.s_spread / 10;
        bar_Weight.fillAmount = 1 - PKP.s_decelerationSpeed + 0.3f;
        bar_FireRate.fillAmount = (1 - (PKP.s_fireRate * 5f));
        text_Ammo.text = PKP.s_ammo.ToString();
        weaponType = WEAPONTYPE.E_PKP;
    }

    public void F_DoubleBarrel()
    {
        img_Weapon.sprite = img_DoubleBarrel;
        bar_Damage.fillAmount = ((float)DoubleBarrel.s_gunDamageMin + (float)DoubleBarrel.s_gunDamageMax) / 200;
        bar_Accuracy.fillAmount = 1 - DoubleBarrel.s_spread / 10;
        bar_Weight.fillAmount = 1 - DoubleBarrel.s_decelerationSpeed + 0.3f;
        bar_FireRate.fillAmount = (1 - (DoubleBarrel.s_fireRate * 5f));
        text_Ammo.text = DoubleBarrel.s_ammo.ToString();
        weaponType = WEAPONTYPE.E_DOUBLEBARREL;
    }

    public void Select()
    {
        switch (weaponType)
        {
            case WEAPONTYPE.E_MP5:
                PV.RPC("ActiveWeapon", RpcTarget.All, 2, playerID);
                WeaponSwitch.weaponNum = 2;
                this.gameObject.SetActive(false);
                break;
            case WEAPONTYPE.E_XM1014:
                PV.RPC("ActiveWeapon", RpcTarget.All, 3, playerID);
                WeaponSwitch.weaponNum = 3;
                this.gameObject.SetActive(false);
                break;
            case WEAPONTYPE.E_M4A1:
                PV.RPC("ActiveWeapon", RpcTarget.All, 4, playerID);
                WeaponSwitch.weaponNum = 4;
                this.gameObject.SetActive(false);
                break;
            case WEAPONTYPE.E_M249:
                PV.RPC("ActiveWeapon", RpcTarget.All, 5, playerID);
                WeaponSwitch.weaponNum = 5;
                this.gameObject.SetActive(false);
                break;
            case WEAPONTYPE.E_G3SG1:
                PV.RPC("ActiveWeapon", RpcTarget.All, 6, playerID);
                WeaponSwitch.weaponNum = 6;
                this.gameObject.SetActive(false);
                break;
            case WEAPONTYPE.E_AWP:
                PV.RPC("ActiveWeapon", RpcTarget.All, 7, playerID);
                WeaponSwitch.weaponNum = 7;
                this.gameObject.SetActive(false);
                break;
            case WEAPONTYPE.E_AK47:
                PV.RPC("ActiveWeapon", RpcTarget.All, 10, playerID);
                WeaponSwitch.weaponNum = 10;
                this.gameObject.SetActive(false);
                break;
            case WEAPONTYPE.E_M16A4:
                PV.RPC("ActiveWeapon", RpcTarget.All, 13, playerID);
                WeaponSwitch.weaponNum = 13;
                this.gameObject.SetActive(false);
                break;
            case WEAPONTYPE.E_M870:
                PV.RPC("ActiveWeapon", RpcTarget.All, 14, playerID);
                WeaponSwitch.weaponNum = 14;
                this.gameObject.SetActive(false);
                break;
            case WEAPONTYPE.E_SCARH:
                PV.RPC("ActiveWeapon", RpcTarget.All, 15, playerID);
                WeaponSwitch.weaponNum = 15;
                this.gameObject.SetActive(false);
                break;
            case WEAPONTYPE.E_UMP45:
                PV.RPC("ActiveWeapon", RpcTarget.All, 16, playerID);
                WeaponSwitch.weaponNum = 16;
                this.gameObject.SetActive(false);
                break;
            case WEAPONTYPE.E_PKP:
                PV.RPC("ActiveWeapon", RpcTarget.All, 17, playerID);
                WeaponSwitch.weaponNum = 17;
                this.gameObject.SetActive(false);
                break;
            case WEAPONTYPE.E_DOUBLEBARREL:
                PV.RPC("ActiveWeapon", RpcTarget.All, 18, playerID);
                WeaponSwitch.weaponNum = 18;
                this.gameObject.SetActive(false);
                break;
            default:
                break;
        }
    }

    [PunRPC]
    private void ActiveWeapon(int _num, int _id)
    {
        foreach (GameObject human in GameObject.FindGameObjectsWithTag("Human"))
        {
            if (human.GetPhotonView().ViewID == _id)
            {
                human.transform.GetChild(_num).gameObject.SetActive(true);
            }
        }
    }
}
