using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Cinemachine;

public class DoubleBarrel : SingleGun
{
    public int pellet;

    public static string s_gunName = "Double Barrel";
    public static int s_gunDamageMin = 34;
    public static int s_gunDamageMax = 44;
    public static int s_criticalDamage = 84;
    public static float s_fireRate = 0.05f;
    public static float s_spread = 2f;
    public static float s_reloadTime = 1.9f;
    public static float s_decelerationSpeed = 0.96f;
    public static int s_maxAmmo = 80;
    public static int s_ammo = 2;
    public static int s_currentAmmo = 2;
    public static int s_pelet = 10;
    public static float s_recoilAmount = 20f;
    public static float s_recoilTime = 0.25f;
    public static float s_shotgunTime = 1f;

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

        KnockBack = 20f; // 9
        Stiffness = 0.1f; // 0.09
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
                StartCoroutine(Fire());
            }
        }
    }

    public override void CameraShake(float _intensity, float _time)
    {
        CinemachineBasicMultiChannelPerlin CP = CM.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float distance = Vector3.Distance(transform.position, mousePos);
        CP.m_AmplitudeGain = _intensity * (distance / 6);
        shakeTime = _time;
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