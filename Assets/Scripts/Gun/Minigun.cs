using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

public class Minigun : Gun
{
    public static string s_gunName = "Minigun";
    public static int s_gunDamageMin = 47;
    public static int s_gunDamageMax = 58;
    public static int s_criticalDamage = 141;
    public static float s_fireRate = 0.045f;
    public static float s_spread = 3.8f;
    public static float s_reloadTime = 3f;
    public static int s_maxAmmo = 250;
    public static int s_ammo = 250;
    public static int s_currentAmmo = 250;
    public static float s_decelerationSpeed = 0.7f;
    public static float s_recoilAmount = 2f;
    public static float s_recoilTime = 0.15f;

    public AudioClip rotaionClip;
    public AudioClip rotationingClip;

    bool isPrepFire = false;
    float t_movespeed;

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

        KnockBack = 5;
        Stiffness = 0.2f;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        isPrepFire = false;
        t_movespeed = human.GetComponent<HumanScript>().maxSpeed;
    }

    public override void Update()
    {
        base.Update();
        StartCoroutine(SetBoolStartFire());
    }

    public override void Shot()
    {
        if (PV.IsMine)
        {
            if (Input.GetMouseButton(0) && currentAmmo > 0 && !isReload && !isFire)
            {
                isFire = true;
                StartCoroutine(Fire());
            }
            else if (Input.GetMouseButton(1) && currentAmmo > 0 && !isReload && !isFire)
            {
                isFire = true;
                StartCoroutine(Rotation());
            }
        }
    }

    public override IEnumerator Fire()
    {
        if (!isPrepFire)
        {
            StartCoroutine(Rotation());
            yield break;
        }

        if (audioSource1.isPlaying)
        {
            PV.RPC("StopRotationSound", RpcTarget.All);
        }

        CameraShake(recoilAmount, recoilTime);
        AddBullet();
        --currentAmmo;
        PV.RPC("FireSound", RpcTarget.All);
        if (PV.IsMine) ui.currentAmmo = currentAmmo;
        yield return new WaitForSeconds(fireRate);
        isFire = false;
    }

    private IEnumerator Rotation()
    {
        if (t_movespeed == human.GetComponent<HumanScript>().maxSpeed)
        {
            human.GetComponent<HumanScript>().maxSpeed -= 2;
        }

        PV.RPC("RotationSound", RpcTarget.All, isPrepFire);

        if (isPrepFire)
        {
            yield return new WaitForSeconds(0.1f);
            isFire = false;
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
            isFire = false;
            yield return new WaitForSeconds(0.5f);
            isPrepFire = true;
        }
    }

    private IEnumerator SetBoolStartFire()
    {
        if (PV.IsMine)
        {
            if (!isFire)
            {
                yield return new WaitForSeconds(0.1f);
                if (!isFire)
                {
                    PV.RPC("StopRotationSound", RpcTarget.All);
                    isPrepFire = false;
                    if (human.GetComponent<HumanScript>().maxSpeed != t_movespeed)
                    {
                        human.GetComponent<HumanScript>().maxSpeed += 2;
                    }
                }
            }
        }
    }

    [PunRPC]
    public void RotationSound(bool _isStartFire)
    {
        if (!_isStartFire)
        {
            if (audioSource1.isPlaying)
            {
                return;
            }
            else
            {
                audioSource1.clip = rotaionClip;
                audioSource1.Play();
            }
        }
        else
        {
            if (audioSource1.clip == rotaionClip)
            {
                audioSource1.clip = rotationingClip;
                audioSource1.Play();
            }
            else if(!audioSource1.isPlaying)
            {
                audioSource1.clip = rotationingClip;
                audioSource1.Play();
            }
        }
    }

    [PunRPC]
    public void StopRotationSound()
    {
        audioSource1.clip = null;
    }
}
