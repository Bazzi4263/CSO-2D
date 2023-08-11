using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Rifle : Gun
{
    public static string s_gunName = "M4A1";
    public static int s_gunDamageMin = 22;
    public static int s_gunDamageMax = 35;
    public static int s_criticalDamage = 113;
    public static float s_fireRate = 0.09f;
    public static float s_spread = 4.6f;
    public static float s_reloadTime = 2.5f;
    public static float s_decelerationSpeed = 0.92f;
    public static int s_maxAmmo = 300;
    public static int s_ammo = 30;
    public static int s_currentAmmo = 30;
    public static float s_recoilAmount = 2.5f;
    public static float s_recoilTime = 0.15f;
    public static float s_knockBack = 1.1f;
    public static float s_Stiffness = 0.06f;

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

        KnockBack = 1.1f;
        Stiffness = 0.06f;
    }
}
