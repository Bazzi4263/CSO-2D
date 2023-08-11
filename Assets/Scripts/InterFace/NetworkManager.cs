using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using Unity.Mathematics;
using System;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public InputField NicknameInput;
    public GameObject connectPanel;
    public GameObject lobby;
    public GameObject Black;
    public GameObject GameChat;
    public GameObject botPannel;

    public PhotonView PV;

    public Text[] texts;

    GameObject player;

    private void Awake()
    {
        PhotonNetwork.SendRate = 90;
        PhotonNetwork.SerializationRate = 60;
    }

    public void Connect()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }

        if (string.IsNullOrWhiteSpace(NicknameInput.text))
            return;
        else
        {
            GameManager.isChatEnd = false;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public void QuitGame()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
        else
        {
            Application.Quit();
        }
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.LocalPlayer.NickName = NicknameInput.text;
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 10 }, null);
    }

    public override void OnJoinedRoom()
    {
        connectPanel.SetActive(false);
        RoomRenewal();
        lobby.SetActive(true);
        botPannel.SetActive(true);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        RoomRenewal();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RoomRenewal();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        connectPanel.SetActive(true);
        lobby.SetActive(false);
    }

    public void Disconect()
    {
        PhotonNetwork.Disconnect();
    }

    public void RoomRenewal()
    {
        botPannel.SetActive(true);

        foreach (Text text in texts)
        {
            text.text = "";
        }

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (PhotonNetwork.PlayerList[i] == PhotonNetwork.MasterClient)
                texts[i].text = "<color=blue>" + PhotonNetwork.PlayerList[i].NickName + "</color>";
            else
                texts[i].text = PhotonNetwork.PlayerList[i].NickName;
        }
    }

    public void GameStartButton()
    {
        if (PhotonNetwork.IsMasterClient == true)
        {
            PV.RPC("GameStartRPC", RpcTarget.All);
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }
    }

    [PunRPC]
    public void GameStartRPC()
    {
        int randomint = UnityEngine.Random.Range(0, GameManager.instance.startLocatns.Length); 
        lobby.SetActive(false);
        PhotonNetwork.Instantiate("Human", GameManager.instance.startLocatns[randomint].position, Quaternion.identity);
        Black.SetActive(true);
        GameManager.instance.StartGame();
        GameChat.SetActive(true);
    }
}
