using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class HandGun : SingleGun
{
    public override void Awake()
    {
        base.Awake();
        
        gunName = "H&K USP";
        gunDamageMin = 24;
        gunDamageMax = 41;
        criticalDamage = 134;
        fireRate = 0.14f;
        spread = 2f;
        reloadTime = 1.3f;
        maxAmmo = 200;
        ammo = 12;
        currentAmmo = ammo;
        decelerationSpeed = 0;
        recoilAmount = 1f;
        recoilTime = 0.1f;

        KnockBack = 1.5f;
        Stiffness = 0.2f;
    }
}
