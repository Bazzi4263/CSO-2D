using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PKP : Gun
{
    public static string s_gunName = "PKP";
    public static int s_gunDamageMin = 36;
    public static int s_gunDamageMax = 42;
    public static int s_criticalDamage = 125;
    public static float s_fireRate = 0.12f;
    public static float s_spread = 5.4f;
    public static float s_reloadTime = 5f;
    public static int s_maxAmmo = 200;
    public static int s_ammo = 100;
    public static int s_currentAmmo = 100;
    public static float s_decelerationSpeed = 0.69f;
    public static float s_recoilAmount = 3.8f;
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

        KnockBack = 1.2f;
        Stiffness = 0.07f;
    }
}
