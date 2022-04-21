using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBtn : MonoBehaviourPunCallbacks
{

    public string roomName;
    void Start()
    {


    }

    /// <summary>
    /// 点击按钮，加入该房间
    /// </summary>
    public void JoinRoomBtn()
    {


        PhotonNetwork.JoinRoom(roomName);//加入房间

        NetworkManager._instance.SelectRolePanel.SetActive(true);//显示选择角色面板
        NetworkManager._instance.LobbyPanel.SetActive(false);//隐藏大厅面板


    }

}

