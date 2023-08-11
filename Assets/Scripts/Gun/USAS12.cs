using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class USAS12 : Gun
{
    public int pellet;

    public static string s_gunName = "USAS-12";
    public static int s_gunDamageMin = 25;
    public static int s_gunDamageMax = 35;
    public static int s_criticalDamage = 43;
    public static float s_fireRate = 0.15f;
    public static float s_spread = 6f;
    public static float s_reloadTime = 2.8f;
    public static float s_decelerationSpeed = 0.8f;
    public static int s_maxAmmo = 120;
    public static int s_ammo = 25;
    public static int s_currentAmmo = 25;
    public static int s_pelet = 9;
    public static float s_recoilAmount = 3.5f;
    public static float s_recoilTime = 0.15f;
    public static float s_shotgunTime = 0.5f;

    public override void Awake()
    {
        base.Awake();

        gunName = s_gunName;
        gunDamageMin = s_gunDamageMin;
        gunDamageMax = s_gunDamageMax;
        criticalDamage = s_criticalDamage;
        fireRate = s_fireRate;
        spread = s_spread;
        reloadTime = s_reloadTime;
        maxAmmo = s_maxAmmo;
        ammo = s_ammo;
        currentAmmo = ammo;
        decelerationSpeed = s_decelerationSpeed;
        pellet = s_pelet;
        recoilAmount = s_recoilAmount;
        recoilTime = s_recoilTime;

        shotgunTime = s_shotgunTime;

        KnockBack = 4;
        Stiffness = 0.3f;
    }

    public override void Shot()
    {
        if (PV.IsMine)
        {
            if (Input.GetMouseButton(0) && currentAmmo > 0 && !isFire)
            {
                isFire = true;
                isReload = false;
                StartCoroutine(Fire());
            }
        }
    }

    public override void AddBullet()
    {
        for (int i = 0; i < pellet; i++)
        {
            GameObject _bullet = PhotonNetwork.Instantiate("Bullet", gunHole.transform.position, Quaternion.identity);
            _bullet.transform.parent = this.transform;
        }
    }
}