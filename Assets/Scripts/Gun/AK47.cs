using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class AK47 : Gun
{
    public static string s_gunName = "AK47";
    public static int s_gunDamageMin = 32;
    public static int s_gunDamageMax = 41;
    public static int s_criticalDamage = 148;
    public static float s_fireRate = 0.1f;
    public static float s_spread = 6.1f;
    public static float s_reloadTime = 1.8f;
    public static float s_decelerationSpeed = 0.9f;
    public static int s_maxAmmo = 360;
    public static int s_ammo = 30;
    public static int s_currentAmmo = 30;
    public static float s_recoilAmount = 3f;
    public static float s_recoilTime = 0.15f;

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
        recoilAmount = s_recoilAmount;
        recoilTime = s_recoilTime;

        KnockBack = 1.2f; // 3.5f
        Stiffness = 0.07f;
    }
}
