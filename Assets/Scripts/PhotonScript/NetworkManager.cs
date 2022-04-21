using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks
{

    #region 私有变量
    #endregion
    #region 公开变量
    public static NetworkManager _instance;//单例
    public TMP_InputField nameInputField;//名字输入框
    public TMP_InputField RoomnameInputField;//房间名输入框
    //public GameObject readyBtn;//准备按钮
    //public GameObject startBtn;//开始游戏按钮

    public GameObject NamePanel;//名字设置面板
    public GameObject LobbyPanel;//大厅面板
    public GameObject SelectRolePanel;//选角色面板
    public GameObject InRoomPanel;//房间内部面板
    //public GameObject RoomPanel;//房间面板
    public GameObject StartInitPanel;//开始初始化面板

    #endregion

    #region Mono函数
    private void Awake()
    {

        if (_instance == null)
        {

            _instance = this;
        }
    }

    private void Start()
    {

        PhotonNetwork.ConnectUsingSettings();//初始化设置，连接服务器
    }

    private void Update()
    {

        if (PhotonNetwork.LocalPlayer.IsMasterClient)//判断是否是房主，是就显示开始游戏按钮，不是就显示准备按钮
        {

            //readyBtn.SetActive(false);
            //startBtn.SetActive(true);
        }
        else
        {

            //readyBtn.SetActive(true);
            //startBtn.SetActive(false);
        }
    }
    #endregion
    #region 按钮函数



    /// <summary>
    /// 名字设置按钮
    /// </summary>
    public void SetNicknameButton()
    {


        if (nameInputField.text.Length < 2)
            return;
        PhotonNetwork.NickName = nameInputField.text;//将输入的名字上传至网络
        if (PhotonNetwork.InLobby)//判断是否在大厅内，在就显示大厅，隐藏名字面板
        {

            LobbyPanel.SetActive(true);
            NamePanel.SetActive(false);
            Debug.Log("已经在大厅中");
        }


    }

    /// <summary>
    /// 创建或加入房间按钮
    /// </summary>
    public void joinOrCreateRoomBtn()
    {

        if (RoomnameInputField.text.Length <= 2)
        {

            return;
        }
        LobbyPanel.SetActive(false);
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 4
        };//房间最大人数4人
        PhotonNetwork.JoinOrCreateRoom(RoomnameInputField.text, roomOptions, default);
        SelectRolePanel.SetActive(true);
        //RoomPanel.SetActive(true);

    }

    public void ReturnToMainMenu()
    {
        if (PhotonNetwork.IsConnected)
            PhotonNetwork.Disconnect();
        SceneManager.LoadScene(0);
    }

    #endregion

    #region Photon函数


    public override void OnJoinedRoom()
    {
        //if (PhotonNetwork.LocalPlayer.IsMasterClient)//判断是否是房主，是就显示开始游戏按钮，不是就显示准备按钮
        //{

        //    readyBtn.SetActive(false);
        //    startBtn.SetActive(true);
        //}
        //else
        //{

        //    readyBtn.SetActive(true);
        //    startBtn.SetActive(false);
        //}

    }
    /// <summary>
    /// 当成功连接该服务器
    /// </summary>
    public override void OnConnectedToMaster()
    {

        NamePanel.SetActive(true);//显示名字面板
        StartInitPanel.SetActive(false);//隐藏开始初始化面板
        PhotonNetwork.JoinLobby();//加入大厅
        Debug.Log("加入大厅成功");
    }


    #endregion
}