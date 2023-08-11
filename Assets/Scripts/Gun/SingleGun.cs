using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class SingleGun : Gun
{   
    public override void Shot()
    {
        if (PV.IsMine)
        {
            if (Input.GetMouseButtonDown(0) && currentAmmo > 0 && !isReload && !isFire)
            {
                isFire = true;
                StartCoroutine(Fire());
            }
        }
    }  
}
