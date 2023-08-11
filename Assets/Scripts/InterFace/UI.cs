using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class UI : MonoBehaviour
{
    public PhotonView PV;

    public Text hpText;
    public Text currentAmmoText;
    public Text MaxAmmoText;
    public Text grenadeAmmoText;

    public int hp = 100;
    public int currentAmmo;
    public int MaxAmmo;
    public int grenadeAmmo;

    void Start()
    {
        grenadeAmmo = OICW.grenadeMaxAmmo;
    }

    private void Update()
    {
        hpText.text = hp.ToString();
        currentAmmoText.text = currentAmmo.ToString();
        MaxAmmoText.text = MaxAmmo.ToString();
        grenadeAmmoText.text = grenadeAmmo.ToString();
    }
}
