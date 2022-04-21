using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class SelectRoleManager : MonoBehaviour
{
    public Sprite[] roleSprites;
    public Image roleImage;
    Player[] allplayers;
    [HideInInspector]
    public static int index = 0;
    int[] otherindex;
    int i = 0;
    int j = 0;
    private List<int> leftRoleNOList = new List<int> { 0, 1, 2, 3, 4 };
    void Awake()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        index = 5;
        roleImage.sprite = roleSprites[index];
        otherindex = new int[3] { 0, 1, 2 };
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable { { "RoleIndex", index } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        ExitGames.Client.Photon.Hashtable IsReady = new ExitGames.Client.Photon.Hashtable { { "isReady", false } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(IsReady);
    }

    // Update is called once per frame
    private void Update()
    {
        allplayers = PhotonNetwork.PlayerList;//获取其余玩家信息
        object temp;
        leftRoleNOList = new List<int> { 0, 1, 2, 3, 4 };
        foreach (var p in allplayers)
        {
            if (p != PhotonNetwork.LocalPlayer)
            {
                j++;
                if (p.CustomProperties.TryGetValue("RoleIndex", out temp))
                {
                    otherindex[i++] = (int)temp;
                    leftRoleNOList.Remove((int)temp);
                }
            }
            else if (p == PhotonNetwork.LocalPlayer)
            {
                ExitGames.Client.Photon.Hashtable LocalIndex = new ExitGames.Client.Photon.Hashtable { { "LocalIndex", j } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(LocalIndex);
            }

        }
        i = 0; j = 0;
    }
    public void NextBtn()
    {
        index = index % 5;
        index = (index + 1) % 5;
        if (leftRoleNOList.Contains(index))
        {
            roleImage.sprite = roleSprites[index];
        }
        else
        {
            while (!leftRoleNOList.Contains(index))
            {
                index = (index + 1) % 5;
            }
            roleImage.sprite = roleSprites[index];

        }
        //        Debug.Log(index);
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable { { "RoleIndex", index } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    public void PreviousBtn()
    {
        if (index == 0)
            index = 4;
        else
            index--;
        if (leftRoleNOList.Contains(index))
        {
            roleImage.sprite = roleSprites[index];
        }
        else
        {
            while (!leftRoleNOList.Contains(index))
            {
                index--;
                if (index == -1)
                    index = 4;
            }
            roleImage.sprite = roleSprites[index];

        }
        //        Debug.Log(index);
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable { { "RoleIndex", index } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }
    public void ReadyBtn()
    {
        if (index < 0 || index > 4)
            return;
        ExitGames.Client.Photon.Hashtable IsReady = new ExitGames.Client.Photon.Hashtable { { "isReady", true } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(IsReady);
        NetworkManager._instance.SelectRolePanel.SetActive(false);
        NetworkManager._instance.InRoomPanel.SetActive(true);
    }
}
