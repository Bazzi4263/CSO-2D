using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class BotPannel : MonoBehaviourPun
{
    public Text botCountText;

    void Start()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            this.gameObject.SetActive(false);
        }
    }

    void OnEnable()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            this.gameObject.SetActive(false);
        }
    }

    public void Update()
    {
        botCountText.text = GameManager.instance.botCount.ToString();
    }

    public void PlusButton()
    {
        GameManager.instance.botCount++;
    }

    public void MinusButton()
    {
        if (GameManager.instance.botCount <= 0)
        {
            return;
        }

        GameManager.instance.botCount--;
    }
}
