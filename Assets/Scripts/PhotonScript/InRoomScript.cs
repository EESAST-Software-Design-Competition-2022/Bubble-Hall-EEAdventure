using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InRoomScript : MonoBehaviourPunCallbacks
{
    public Image[] stage;
    public Image[] playerImage;
    public GameObject StartGameBtn;
    public Sprite[] roleSprites;
    //public TextMeshProUGUI[] PlayerInfo;
    public Image[] PlayerStatus;
    public Sprite[] StatusSprites;
    Player[] allplayers;
    public int RoomPeopleNum = 0;
    public bool AllReady = true;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            playerImage[i].sprite = roleSprites[0];
        }
        ExitGames.Client.Photon.Hashtable IsReady = new ExitGames.Client.Photon.Hashtable { { "isReady", false } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(IsReady);
    }

    // Update is called once per frame
    void Update()
    {
        updateinfo();
        showReadyBtn();
    }

    void showReadyBtn()
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient)//判断是否是房主，是就显示开始游戏按钮，不是就显示准备按钮
        {
            StartGameBtn.SetActive(true);

        }
        else
        {
            StartGameBtn.SetActive(false);
        }
    }

    /// <summary>
    /// 当加入房间时
    /// </summary>
    public override void OnJoinedRoom()
    {
        int i = 0;
        object index;
        object IsReady;
        allplayers = PhotonNetwork.PlayerList;
        foreach (var item in allplayers)
        {
            if (item.CustomProperties.TryGetValue("RoleIndex", out index))
            {
                if ((int)index == 5)
                {
                    playerImage[i].sprite = roleSprites[0];
                }
                else
                {
                    playerImage[i].sprite = roleSprites[(int)index + 1];
                }
            }
            else
            {
                playerImage[i].sprite = roleSprites[0];
            }
            if (item.CustomProperties.TryGetValue("isReady", out IsReady))
            {
                if ((bool)IsReady)
                    PlayerStatus[i].sprite = StatusSprites[0];
                else
                    PlayerStatus[i].sprite = StatusSprites[1];
            }
            else
            {
                PlayerStatus[i].sprite = StatusSprites[1];
            }
            i++;
        }
        for (int j = i; j < 4; j++)
        {
            playerImage[j].sprite = roleSprites[0];
            PlayerStatus[j].sprite = StatusSprites[2];
        }
    }
    /// 当离开房间时，如果房间内还有人，则设置其他人为房主
    /// </summary>
    public override void OnLeftRoom()
    {

        //foreach (var item in mplayers)
        //{

        //    if (item.GetComponentInChildren<TextMeshProUGUI>().text.Contains(PhotonNetwork.LocalPlayer.NickName))
        //    {

        //        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        //        {

        //            foreach (var p in allplayers)
        //            {

        //                if (p != PhotonNetwork.LocalPlayer)
        //                {

        //                    PhotonNetwork.CurrentRoom.SetMasterClient(p);
        //                    break;
        //                }
        //            }

        //        }

        //        Destroy(item);
        //        break;
        //    }
        //}

    }

    public void updateinfo()
    {
        int i = 0;
        object index;
        object IsReady;
        allplayers = PhotonNetwork.PlayerList;
        AllReady = true;
        foreach (var item in allplayers)
        {
            if (item.CustomProperties.TryGetValue("RoleIndex", out index))
            {
                if ((int)index == 5)
                {
                    playerImage[i].sprite = roleSprites[0];
                }
                else
                {
                    playerImage[i].sprite = roleSprites[(int)index + 1];
                }
            }
            else
            {
                playerImage[i].sprite = roleSprites[0];
            }
            if (item.CustomProperties.TryGetValue("isReady", out IsReady))
            {
                if ((bool)IsReady)
                    PlayerStatus[i].sprite = StatusSprites[0];
                else
                {
                    PlayerStatus[i].sprite = StatusSprites[1];
                    AllReady = false;
                }

            }
            else
            {
                //PlayerInfo[i].text = item.NickName + "(" + "is preparing" + ")";
                PlayerStatus[i].sprite = StatusSprites[1];
                AllReady = false;
            }
            i++;
        }
        for (int j = i; j < 4; j++)
        {
            playerImage[j].sprite = roleSprites[0];
            //PlayerInfo[j].text = "No Player";
            PlayerStatus[j].sprite = StatusSprites[2];
        }
        if (i != RoomPeopleNum)
            RoomPeopleNum = i;
    }

    //开始游戏
    public void StartGameButton()
    {
        //ExitGames.Client.Photon.Hashtable PlayerNum = new ExitGames.Client.Photon.Hashtable { { "PlayerNum", RoomPeopleNum } };
        //PhotonNetwork.LocalPlayer.SetCustomProperties(PlayerNum);
        if (AllReady)
        {
            photonView.RPC("LoadGameScene1", RpcTarget.All);
        }
    }


    [PunRPC]
    void LoadGameScene1()
    {
        PhotonNetwork.LoadLevel(4);
    }
}
