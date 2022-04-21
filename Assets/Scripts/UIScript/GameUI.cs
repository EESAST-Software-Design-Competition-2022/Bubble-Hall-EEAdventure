using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class GameUI : MonoBehaviourPun
{
    //��Ϸ��Դ
    public Image[] panels;
    public Image gameOverPanel;
    public Image gameWinPanel;
    public Image shopPanel;
    public Image messagePanel;
    public Image shade;
    public Image shouqing;
    public TextMeshProUGUI[] playerTags;
    public Sprite[] panelSprites;
    public TextMeshProUGUI[] values;
    public TextMeshProUGUI[] coinValues;
    public Sprite[] buttonSprites;
    public Button pauseOrContinueButton;
    public Button returnButton;


    public GameObject temp;

    //����
    GameManager gameManager;
    //����
    private int[] roleNum = new int[4];
    private int total = 0;
    private Person[] persons = new Person[4];
    private bool status = true;//true:������Ϸ false:��ͣ
    public bool Online = false;
    public bool IsMaster = false;
    public bool inital = false;
    public float delayTime = 2.0f;
    private void Awake()
    {
        status = true;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameManager.ui = this;
        if (PhotonNetwork.IsConnected && Menu.mode == 3)
        {
            Online = true;
            if (PhotonNetwork.IsMasterClient)
                IsMaster = true;
        }
    }

    private void Start()
    {

        if (!Online)
        {
            total = gameManager.roleList.Count;
            for (int i = 0; i < total; i++)
            {
                persons[i] = gameManager.roleList.GetT(i).GetComponent<Person>();
                roleNum[i] = persons[i].NO;
                panels[i].sprite = panelSprites[persons[i].NO];
                if (persons[i].PlayerNO > 0)
                {
                    playerTags[i].gameObject.SetActive(true);
                    playerTags[i].SetText("P" + persons[i].PlayerNO.ToString());
                }
                else
                {
                    playerTags[i].gameObject.SetActive(false);
                }
            }
            shade.gameObject.SetActive(false);
            gameOverPanel.gameObject.SetActive(false);
            gameWinPanel.gameObject.SetActive(false);
        }
        else if (Online && IsMaster)
        {
            shade.gameObject.SetActive(false);
            gameOverPanel.gameObject.SetActive(false);
            gameWinPanel.gameObject.SetActive(false);
        }
        else if (Online && !IsMaster)
        {
            shade.gameObject.SetActive(false);
            gameOverPanel.gameObject.SetActive(false);
            gameWinPanel.gameObject.SetActive(false);
        }
        if(shopPanel!=null)
            shopPanel.gameObject.SetActive(false);
        if(shouqing!=null)
            shouqing.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!Online)
        {
            for (int i = 0; i < total; i++)
            {
                if (gameManager.roleList.GetT(i) == null)
                {
                    panels[i].sprite = panelSprites[roleNum[i] + 5];
                    persons[i] = null;
                }
                if (persons[i] != null)
                {
                    values[i * 4].SetText(persons[i].life.ToString());
                    values[i * 4 + 1].SetText(persons[i].bombNumber.ToString());
                    values[i * 4 + 2].SetText(persons[i].bombRadius.ToString());
                    values[i * 4 + 3].SetText(((int)persons[i].speed).ToString());
                    if(Menu.mode == 2)
                    {
                        coinValues[i].SetText(persons[i].coin.ToString());
                    }
                }

            }
        }
        else if (Online && IsMaster)
        {
            total = gameManager.roleList.Count;
            if (total == 4 && !inital)
            {
                int j = 1;
                for (int i = 0; i < total; i++)
                {
                    persons[i] = gameManager.roleList.GetT(i).GetComponent<Person>();
                    roleNum[i] = persons[i].NO;
                    panels[i].sprite = panelSprites[persons[i].NO];
                    if (persons[i].PlayerNO > 0)
                    {
                        playerTags[i].gameObject.SetActive(true);
                        playerTags[i].SetText("P" + (j).ToString());
                        photonView.RPC("SetPlayerTags", RpcTarget.Others, i, j);
                        j++;
                    }
                    else
                    {
                        playerTags[i].gameObject.SetActive(false);
                    }
                    photonView.RPC("SetInitalInfo", RpcTarget.Others, i, persons[i].NO, persons[i].PlayerNO);
                }
                inital = true;
            }
            for (int i = 0; i < total; i++)
            {
                if (gameManager.roleList.GetT(i) == null)
                {
                    panels[i].sprite = panelSprites[roleNum[i] + 5];
                    persons[i] = null;
                }
                if (persons[i] != null)
                {
                    values[i * 4].SetText(persons[i].life.ToString());
                    values[i * 4 + 1].SetText(persons[i].bombNumber.ToString());
                    values[i * 4 + 2].SetText(persons[i].bombRadius.ToString());
                    values[i * 4 + 3].SetText(((int)persons[i].speed).ToString());
                }
                if (delayTime > 0)
                {
                    delayTime -= Time.deltaTime;
                    return;
                }
                bool alive = true;
                if (gameManager.roleList.GetT(i) == null)
                    alive = false;
                if (persons[i] != null)
                {
                    if (persons[i].life == 0)
                    {
                        Debug.Log("persons[i].life==0");
                    }
                    photonView.RPC("UpdateRoleInfo", RpcTarget.Others, alive, i, persons[i].life, persons[i].bombNumber, persons[i].bombRadius, persons[i].speed, roleNum[i]);
                }

                else
                {
                    photonView.RPC("UpdateRoleInfo", RpcTarget.Others, alive, i, 0, 0, 0, (float)0, roleNum[i]);
                }


            }
        }
    }

    [PunRPC]
    public void SetInitalInfo(int i, int NO, int PlayerNO)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            panels[i].sprite = panelSprites[NO];
            if (PlayerNO > 0)
            {
                playerTags[i].gameObject.SetActive(true);
                //playerTags[i].SetText("P" + PlayerNO.ToString());
            }
            else
            {
                playerTags[i].gameObject.SetActive(false);
            }
        }

    }

    [PunRPC]
    public void UpdateRoleInfo(bool alive, int i, int role_life, int role_bombNumber, int role_bombRadius, float role_speed, int role_num)
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (!alive)
        {
            panels[i].sprite = panelSprites[role_num + 5];
            persons[i] = null;
            return;
        }
        if (alive)
        {
            values[i * 4].SetText(role_life.ToString());
            values[i * 4 + 1].SetText(role_bombNumber.ToString());
            values[i * 4 + 2].SetText(role_bombRadius.ToString());
            values[i * 4 + 3].SetText(((int)role_speed).ToString());
        }
        Debug.Log(gameManager.roleList.Count);
        for (int j = 0; j < 4; j++)
        {
            if (gameManager.roleList.GetT(j) == null || gameManager.roleList.GetT(j).GetComponent<Person>() == null)
                return;
            if (gameManager.roleList.GetT(j).GetComponent<Person>().NO != role_num)
                continue;
            else
            {
                if (role_life == 0 && role_num == 0)
                    Debug.Log("role_life==0");
                gameManager.roleList.GetT(j).GetComponent<Person>().life = role_life;
                gameManager.roleList.GetT(j).GetComponent<Person>().speed = role_speed;
                gameManager.roleList.GetT(j).GetComponent<Person>().bombNumber = role_bombNumber;
                gameManager.roleList.GetT(j).GetComponent<Person>().bombRadius = role_bombRadius;
            }
        }
    }
    [PunRPC]
    public void SetPlayerTags(int i, int j)
    {
        playerTags[i].gameObject.SetActive(true);
        playerTags[i].SetText("P" + j.ToString());
    }

    public void UnfoldShop()
    {
        shopPanel.gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    public void closeShop()
    {
        shopPanel.gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    public void PauseOrContinue()
    {
        if (status)
        {
            status = false;
            pauseOrContinueButton.GetComponent<Image>().sprite = buttonSprites[1];
            gameManager.Pause();
        }
        else
        {
            status = true;
            pauseOrContinueButton.GetComponent<Image>().sprite = buttonSprites[0];
            gameManager.Continue();
        }
    }

    public void ReturnToMainMenu()
    {
        if (PhotonNetwork.IsConnected)
            PhotonNetwork.Disconnect();
        SceneManager.LoadScene(0);
    }

    public void Defeat()
    {
        pauseOrContinueButton.enabled = false;
        returnButton.enabled = false;
        shade.gameObject.SetActive(true);
        gameOverPanel.gameObject.SetActive(true);
    }

    public void Victory()
    {

        Debug.Log("Victory");
        pauseOrContinueButton.enabled = false;
        returnButton.enabled = false;
        shade.gameObject.SetActive(true);
        gameWinPanel.gameObject.SetActive(true);
    }

    public void Restart()
    {
        SceneManager.LoadScene(1);
        if (PhotonNetwork.IsConnected)
            PhotonNetwork.Disconnect();

    }

    public void BuyWeapon1()
    {
        GameObject playerObject = gameManager.roleList.GetT(0);
        Person player = playerObject.GetComponent<Person>();
        if (!player.canPushBomb)
        {
            if (player.coin >= 15)
            {
                player.coin -= 15;
                player.canPushBomb = true;
                shouqing.gameObject.SetActive(true);
            }
            else
            {
                BuyFailure();
            }
        }
        
    }

    public void BuyWeapon2()
    {
        GameObject playerObject = gameManager.roleList.GetT(0);
        Person player = playerObject.GetComponent<Person>();

        if (player.coin >= 15)
        {
            player.coin -= 15;
            player.haveGun = true;
        }
        else
        {
            BuyFailure();
        }
    }

    public void BuyWeapon3()
    {
        GameObject playerObject = gameManager.roleList.GetT(0);
        Person player = playerObject.GetComponent<Person>();

        if (player.coin >= 100)
        {
            player.coin -= 100;
            player.haveUnknownWeapon = true;
        }
        else
        {
            BuyFailure();
        }
    }

    public void BuyFailure()
    {
        shopPanel.gameObject.SetActive(false);
        messagePanel.gameObject.SetActive(true);
    }

    public void closeMessagePanel()
    {
        messagePanel.gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

}
