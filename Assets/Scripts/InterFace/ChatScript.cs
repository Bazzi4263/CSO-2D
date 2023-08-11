using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

public class ChatScript : MonoBehaviourPunCallbacks
{
    public TMP_Text chatText;
    public TMP_InputField chatInputField;

    public PhotonView PV;

    private void Update()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (chatInputField.text == "")
            {
                chatInputField.ActivateInputField();
            }
            else
            {
                chatInputField.ActivateInputField();
                GameManager.isChatEnd = false;
                chatInputField.text = "";
            }
        }
        if (GameManager.isChatEnd)
        {
            chatInputField.DeactivateInputField();
            GameManager.isChatEnd = false;
            PV.RPC("ChatRPC", RpcTarget.All, PhotonNetwork.NickName, chatInputField.text);
        }
        return;
#endif
        
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (chatInputField.text != "")
            {
                chatInputField.DeactivateInputField();
                PV.RPC("ChatRPC", RpcTarget.All, PhotonNetwork.NickName, chatInputField.text);
            }
            else
            {
                chatInputField.ActivateInputField();
            }
        }
    }

    [PunRPC]
    public void ChatRPC(string _player, string _msg)
    {
        if (PV.IsMine) chatInputField.text = "";
        chatText.text += _player + ": " + _msg + "\n";
    }
}
