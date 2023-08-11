using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UIElements;
using Cinemachine;

public class Sniper : SingleGun
{
    public static string s_gunName = "AWP";
    public static int s_gunDamageMin = 272;
    public static int s_gunDamageMax = 295;
    public static int s_criticalDamage = 561;
    public static float s_fireRate = 1.4f;
    public static float s_spread = 10f;
    public static float s_Zoomspread = 0.2f;
    public static float s_reloadTime = 2.8f;
    public static float s_decelerationSpeed = 0.75f;
    public static int s_maxAmmo = 180;
    public static int s_ammo = 10;
    public static int s_currentAmmo = 10;
    public static float s_recoilAmount = 3f;
    public static float s_recoilTime = 0.2f;

    bool isZoom = false;

    public float zoomSpread;
    public float zoomSpeed;

    public GameObject fov;
    public GameObject black;

    float t_spread;

    public PolygonCollider2D confiner;
    public PolygonCollider2D zoomConfiner;

    public override void Awake()
    {
        base.Awake();

        gunName = s_gunName;
        gunDamageMin = s_gunDamageMin;
        gunDamageMax = s_gunDamageMax;
        criticalDamage = s_criticalDamage;
        fireRate = s_fireRate;
        spread = s_spread;
        zoomSpread = s_Zoomspread;
        reloadTime = s_reloadTime;
        maxAmmo = s_maxAmmo;
        ammo = s_ammo;
        currentAmmo = s_currentAmmo;
        decelerationSpeed = s_decelerationSpeed;
        zoomSpeed = 0.4f;
        t_spread = spread;
        recoilAmount = s_recoilAmount;
        recoilTime = s_recoilTime;

        KnockBack = 5;
        Stiffness = 0.9f;

        fov = GameObject.FindWithTag("FOV");
        black = GameObject.FindWithTag("Black");
    }

    public override void Update()
    {
        base.Update();
        Zoom();
    }

    public override void AddBullet()
    {
        GameObject bullet = PhotonNetwork.Instantiate("SniperBullet", gunHole.transform.position, Quaternion.identity);
        bullet.transform.parent = this.transform;
    }

    public void Zoom()
    {
        if (PV.IsMine)
        {
            if (Input.GetMouseButtonDown(1) && !isZoom)
            {
                fov.GetComponent<FieldOfView>().fov = 30;
                fov.GetComponent<FieldOfView>().viewDistance = 50;
                fov.GetComponent<FieldOfView>().rayCount = 300;
                GameObject.FindWithTag("CMCamera").GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = 14;
                GameObject.FindWithTag("CMCamera").GetComponent<CameraFollowing>().CC.m_BoundingShape2D = zoomConfiner;
                black.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 1);
                fov.GetComponent<FieldOfView>().isZoom = true;
                spread = zoomSpread;
                isZoom = true;
            }
            else if(Input.GetMouseButtonDown(1) && isZoom)
            {
                fov.GetComponent<FieldOfView>().fov = 360;
                fov.GetComponent<FieldOfView>().viewDistance = 35;
                fov.GetComponent<FieldOfView>().rayCount = 800;
                GameObject.FindWithTag("CMCamera").GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = 7;
                GameObject.FindWithTag("CMCamera").GetComponent<CameraFollowing>().CC.m_BoundingShape2D = confiner;
                black.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0.7f);
                fov.GetComponent<FieldOfView>().isZoom = false;
                spread = t_spread;
                isZoom = false;
            }
        }
    }

    public void OnDisable()
    {
        if (PV.IsMine)
        {
            fov.GetComponent<FieldOfView>().fov = 360;
            fov.GetComponent<FieldOfView>().viewDistance = 35;
            fov.GetComponent<FieldOfView>().rayCount = 800;
            GameObject.FindWithTag("CMCamera").GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = 7;
            black.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0.7f);
            fov.GetComponent<FieldOfView>().isZoom = false;
            spread = t_spread;
            isZoom = false;
        }
    }

    public override void DeclareSpeed()
    {
        if (isZoom)
        {
            human.GetComponent<HumanScript>().maxSpeed = human.GetComponent<HumanScript>().t_moveSpeed;
            human.GetComponent<HumanScript>().maxSpeed *= zoomSpeed;
        }
        else
        {
            human.GetComponent<HumanScript>().maxSpeed = human.GetComponent<HumanScript>().t_moveSpeed;
            human.GetComponent<HumanScript>().maxSpeed *= decelerationSpeed;
        }
    }
}
