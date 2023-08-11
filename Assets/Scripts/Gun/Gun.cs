using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Cinemachine;
using System.Globalization;

public class Gun : MonoBehaviourPunCallbacks
{
    public GameObject human;
    public GameObject gunHole;

    [HideInInspector]public string gunName = "";
    [HideInInspector]public int gunDamageMin;
    [HideInInspector]public int gunDamageMax;
    [HideInInspector]public int criticalDamage;
    [HideInInspector]public float fireRate;
    [HideInInspector]public float spread;
    [HideInInspector]public float reloadTime;
    [HideInInspector]public float decelerationSpeed;
    [HideInInspector]public int maxAmmo;
    [HideInInspector]public int ammo;
    [HideInInspector]public int currentAmmo;
    [HideInInspector]int t_currentAmmo;
    [HideInInspector]public float recoilAmount;
    [HideInInspector]public float recoilTime;

    [HideInInspector]public float KnockBack;
    [HideInInspector]public float Stiffness;

    public float shotgunTime = 0;

    public bool recoilStart = false;
    bool isFirstFire = true;

    public PhotonView PV;

    protected bool isFire = false;
    protected bool isReload = false;

    public UI ui;
    public AudioSource audioSource1;

    public AudioClip fireClip;
    public AudioClip reloadClip;

    public ParticleSystem muzzleFlashParticle;

    protected CinemachineVirtualCamera CM;
    protected float shakeTime;

    public virtual void Awake()
    {
        ui = GameObject.FindGameObjectWithTag("GameManager").GetComponent<UI>();
        CM = GameObject.FindGameObjectWithTag("CMCamera").GetComponent<CinemachineVirtualCamera>();
    }

    public virtual void OnEnable()
    {
        DeclareSpeed();
        isFire = false;
        isReload = false;
        recoilStart = false;
        isFirstFire = true;
        if (PV.IsMine) ui.currentAmmo = currentAmmo;
        if (PV.IsMine) ui.MaxAmmo = maxAmmo;
    }

    public virtual void Update()
    {
        Shot();
        RecoilCheck();
        StartCoroutine(RecoverRecoil());
        TryReload();
        ShakeTimer();
    }

    public void ShakeTimer()
    {
        if (shakeTime > 0)
        {
            shakeTime -= Time.deltaTime;

            if (shakeTime <= 0f)
            {
                CinemachineBasicMultiChannelPerlin CP = CM.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

                CP.m_AmplitudeGain = 0f;
            }
        }
    }

    public virtual void Shot()
    {
        if (PV.IsMine)
        {
            if (Input.GetMouseButton(0) && currentAmmo > 0 && !isReload && !isFire)
            {
                isFire = true;
                StartCoroutine(Fire());
            }
        }
    }

    public virtual IEnumerator Fire()
    {
        CameraShake(recoilAmount, recoilTime);
        PV.RPC("FireSound", RpcTarget.All);
        AddBullet();
        --currentAmmo;
        if (PV.IsMine) ui.currentAmmo = currentAmmo;
        yield return new WaitForSeconds(fireRate);
        isFire = false;
    }

    public virtual void AddBullet()
    {
        GameObject bullet = PhotonNetwork.Instantiate("Bullet", gunHole.transform.position, Quaternion.identity);
        bullet.transform.parent = this.transform;
    }

    public void RecoilCheck()
    {
        if (isFire && isFirstFire)
        {
            t_currentAmmo = currentAmmo;
            isFirstFire = false;
        }

        if (t_currentAmmo > currentAmmo + 4)
        {
            recoilStart = true;
        }
    }

    IEnumerator RecoverRecoil()
    {
        if (recoilStart)
        {
            int t_ammo = currentAmmo;
            yield return new WaitForSeconds(0.3f);
            if (t_ammo == currentAmmo)
            {
                recoilStart = false;
                isFirstFire = true;
            }
        }
    }

    public virtual void TryReload()
    {
        if (currentAmmo == 0 && maxAmmo > 0 && !isReload && PV.IsMine)
        {
            StopCoroutine(Reload());
            StartCoroutine(Reload());
        }

        if (Input.GetKeyDown(KeyCode.R) && maxAmmo > 0 && !isReload && PV.IsMine && currentAmmo != ammo)
        {
            StopCoroutine(Reload());
            StartCoroutine(Reload());
        }
    }

    public virtual IEnumerator Reload()
    {
        isReload = true;
        ReloadSound();
        yield return new WaitForSeconds(reloadTime);
        if (maxAmmo < ammo)
        {
            currentAmmo = maxAmmo;
            if (PV.IsMine) ui.currentAmmo = currentAmmo;
            maxAmmo = 0;
            if (PV.IsMine) ui.MaxAmmo = maxAmmo;
            isReload = false;
        }
        else
        {
            maxAmmo -= ammo - currentAmmo;
            if (PV.IsMine) ui.MaxAmmo = maxAmmo;
            currentAmmo = ammo;
            if (PV.IsMine) ui.currentAmmo = currentAmmo;
            isReload = false;
        }
    }

    public virtual void DeclareSpeed()
    {
        if (decelerationSpeed == 0)
        {
            human.GetComponent<HumanScript>().maxSpeed = human.GetComponent<HumanScript>().t_moveSpeed;
        }
        else
        {
            human.GetComponent<HumanScript>().maxSpeed = human.GetComponent<HumanScript>().t_moveSpeed;
            human.GetComponent<HumanScript>().maxSpeed *= decelerationSpeed;
        }
    }

    public virtual void CameraShake(float _intensity, float _time)
    {
        CinemachineBasicMultiChannelPerlin CP = CM.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float distance = Vector3.Distance(transform.position, mousePos);
        CP.m_AmplitudeGain = _intensity * (distance / 10);
        shakeTime = _time;
    }

    [PunRPC]
    public virtual void FireSound()
    {
        if (name != "SCAR-H")
        {
            muzzleFlashParticle.Play();
        }
        audioSource1.PlayOneShot(fireClip);
    }

    public virtual void ReloadSound()
    {
        audioSource1.PlayOneShot(reloadClip);
    }   
}
