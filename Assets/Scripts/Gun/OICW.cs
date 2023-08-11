using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class OICW : Gun
{
    public static string s_gunName = "OICW";
    public static int s_gunDamageMin = 48;
    public static int s_gunDamageMax = 61;
    public static int s_criticalDamage = 205;
    public static float s_fireRate = 0.09f;
    public static float s_spread = 2.5f;
    public static float s_reloadTime = 1.9f;
    public static float s_decelerationSpeed = 0.85f;
    public static int s_maxAmmo = 400;
    public static int s_ammo = 40;
    public static int s_currentAmmo = 40;
    public static float s_recoilAmount = 1.5f;
    public static float s_recoilTime = 0.15f;

    public static int grenadeMaxAmmo = 6;
    public int grenadeAmmo = grenadeMaxAmmo;
    public static int grenadeDamageMin = 500;
    public static int grenadeDamageMax = 700;
    public static int grenadeCriticalDamage = 999;
    public static float grenadeStifness = 0.7f;

    public static float grenadeCoolTime = 5f;

    public GameObject grenade;
    public GameObject gui;

    bool isReady = true;
    float time = grenadeCoolTime;

    public AudioClip grenadeFireClip;

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

        KnockBack = 1.7f; // 3.5f
        Stiffness = 0.12f;

        gui = GameObject.Find("GUI");
    }

    public override void Update()
    {
        base.Update();

        StartCoroutine(GrenadeLauncherShot());
        GrenadeTimer();
    }

    IEnumerator GrenadeLauncherShot()
    {
        if (Input.GetMouseButtonDown(1) && PV.IsMine && isReady && grenadeAmmo > 0)
        {
            GameObject grenade = PhotonNetwork.Instantiate("Grenade", gunHole.transform.position, Quaternion.identity);
            ui.grenadeAmmo -= 1;
            --grenadeAmmo;

            grenade.transform.parent = this.transform;
            isReady = false;
            PV.RPC("GrenadeFireSound", RpcTarget.All);
            gui.transform.GetChild(2).GetChild(4).gameObject.SetActive(false);
        }
        yield return null;
    }

    void GrenadeTimer()
    {
        if (isReady)
        {
            return;
        }
        else
        {
            time -= Time.deltaTime;
        }

        if (time <= 0)
        {
            isReady = true;
            gui.transform.GetChild(2).GetChild(4).gameObject.SetActive(true);
            time = grenadeCoolTime;
        }
    }

    public override void OnEnable()
    {
        base.OnEnable();

        if (PV.IsMine)
        {
            base.OnEnable();
            gui.transform.GetChild(2).gameObject.SetActive(true);
        }
    }

    public void OnDisable()
    {
        if (PV.IsMine)
        {
            gui.transform.GetChild(2).gameObject.SetActive(false);
        }
    }

    [PunRPC]
    public void GrenadeFireSound()
    {
        audioSource1.PlayOneShot(grenadeFireClip);
    }
}

