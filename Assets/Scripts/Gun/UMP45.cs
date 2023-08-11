using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class UMP45 : Gun
{
    public static string s_gunName = "UMP45";
    public static int s_gunDamageMin = 20;
    public static int s_gunDamageMax = 33;
    public static int s_criticalDamage = 80;
    public static float s_fireRate = 0.1f;
    public static float s_spread = 2.1f;
    public static float s_reloadTime = 2.2f;
    public static float s_decelerationSpeed = 0f;
    public static int s_maxAmmo = 300;
    public static int s_ammo = 25;
    public static int s_currentAmmo = 25;
    public static float s_recoilAmount = 1f;
    public static float s_recoilTime = 0.1f;

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

        KnockBack = 0.9f;
        Stiffness = 0.043f;
    }
}
