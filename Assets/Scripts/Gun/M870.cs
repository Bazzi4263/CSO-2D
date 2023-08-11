using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class M870 : SingleGun
{
    public int pellet;

    public static string s_gunName = "M870";
    public static int s_gunDamageMin = 29;
    public static int s_gunDamageMax = 31;
    public static int s_criticalDamage = 48;
    public static float s_fireRate = 1.1f;
    public static float s_spread = 4.5f;
    public static float s_reloadTime = 0.37f;
    public static float s_decelerationSpeed = 0.91f;
    public static int s_maxAmmo = 80;
    public static int s_ammo = 8;
    public static int s_currentAmmo = 8;
    public static int s_pelet = 9;
    public static float s_recoilAmount = 6f;
    public static float s_recoilTime = 0.15f;
    public static float s_shotgunTime = 0.6f;

    public AudioClip shotgunPush;

    Coroutine reloadCoroutine;

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

        KnockBack = 10f; // 9
        Stiffness = 0.22f; // 0.09
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Shot()
    {
        if (PV.IsMine)
        {
            if (Input.GetMouseButtonDown(0) && currentAmmo > 0 && !isFire)
            {
                isFire = true;
                isReload = false;
                if (reloadCoroutine != null) StopCoroutine(reloadCoroutine);
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

    public override void TryReload()
    {
        if (currentAmmo == 0 && maxAmmo > 0 && !isReload && PV.IsMine)
        {
            if (reloadCoroutine != null) StopCoroutine(reloadCoroutine);
            reloadCoroutine = StartCoroutine(Reload());
        }

        if (Input.GetKeyDown(KeyCode.R) && maxAmmo > 0 && !isReload && PV.IsMine && currentAmmo != ammo)
        {
            if (reloadCoroutine != null) StopCoroutine(reloadCoroutine);
            reloadCoroutine = StartCoroutine(Reload());
        }
    }

    public override IEnumerator Reload()
    {
        isReload = true;
        while (currentAmmo < ammo && isReload && maxAmmo > 0)
        {
            if (isFire)
            {
                break;
            }

            yield return new WaitForSeconds(reloadTime);
            ++currentAmmo;

            if (currentAmmo > ammo)
            {
                --currentAmmo;
                break;
            }

            audioSource1.PlayOneShot(shotgunPush);
            --maxAmmo;
            if (PV.IsMine) ui.MaxAmmo = maxAmmo;
            if (PV.IsMine) ui.currentAmmo = currentAmmo;

            if (currentAmmo == ammo)
            {
                audioSource1.PlayOneShot(reloadClip);
            }
        }

        isReload = false;
    }
}