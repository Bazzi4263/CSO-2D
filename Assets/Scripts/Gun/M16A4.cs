using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class M16A4 : SingleGun
{
    public static string s_gunName = "M16A4";
    public static int s_gunDamageMin = 23;
    public static int s_gunDamageMax = 36;
    public static int s_criticalDamage = 113;
    public static float s_fireRate = 0.15f;
    public static float s_spread = 0.8f;
    public static float s_reloadTime = 2.5f;
    public static float s_decelerationSpeed = 0.92f;
    public static int s_maxAmmo = 300;
    public static int s_ammo = 30;
    public static int s_currentAmmo = 30;
    public static float s_recoilAmount = 3.5f;
    public static float s_recoilTime = 0.17f;
           
    public static float s_KnockBack = 1.1f;
    public static float s_Stiffness = 0.06f;

    public override void Awake()
    {
        base.Awake();

        gunName = "M16A4";
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
        recoilAmount = s_recoilAmount;
        recoilTime = s_recoilTime;

        KnockBack = s_KnockBack;
        Stiffness = s_Stiffness;
    }

    public override IEnumerator Fire()
    {
        CameraShake(recoilAmount, recoilTime);
        for (int i = 0; i < 3; i++)
        {
            AddBullet();
            PV.RPC("FireSound", RpcTarget.All);
            --currentAmmo;
            if (PV.IsMine) ui.currentAmmo = currentAmmo;
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(fireRate);
        isFire = false;
    }
}
