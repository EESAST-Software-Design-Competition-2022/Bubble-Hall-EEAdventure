using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Photon.Realtime;

public class Control : MonoBehaviourPun
{
    public int localIndex;
    private GameManager gameManager = GameManager.Instance;
    private GameObject roleObject;
    private Person role;
    public Player[] allplayers;
    private void Awake()
    {

        allplayers = PhotonNetwork.PlayerList;
        object Playerindex;
        foreach (var p in allplayers)
        {
            if (p == PhotonNetwork.LocalPlayer && p.CustomProperties.TryGetValue("LocalIndex", out Playerindex))
            {
                localIndex = (int)Playerindex;
            }
        }
    }

    private void FixedUpdate()
    {
        ControlByKey();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            photonView.RPC("ControlFire", RpcTarget.MasterClient, localIndex);
        }
    }
    public void ControlByKey()
    {

        //{
        //    photonView.RPC("ControlFire", RpcTarget.MasterClient, localIndex);
        //}
        //if (Input.GetKeyDown(KeyCode.Space))

        //control moving
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        if (h != 0) v = 0;
        if (h > 0)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("ControlMove", RpcTarget.MasterClient, 2, localIndex);
            }

        }
        if (h < 0)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("ControlMove", RpcTarget.MasterClient, 1, localIndex);
            }

        }
        if (v > 0)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("ControlMove", RpcTarget.MasterClient, 3, localIndex);
            }

        }
        if (v < 0)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("ControlMove", RpcTarget.MasterClient, 0, localIndex);
            }

        }
        if (h == 0 && v == 0)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("ControlMove", RpcTarget.MasterClient, -1, localIndex);
            }

        }


    }
    [PunRPC]
    public void ControlMove(int direction, int Playerindex)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            roleObject = gameManager.roleList.GetT(Playerindex);
            if (roleObject != null)
            {
                role = roleObject.GetComponent<Person>();
                if (role != null)
                {
                    role.Move(direction);
                }
            }
        }
    }
    [PunRPC]
    public void ControlFire(int Playerindex)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            roleObject = gameManager.roleList.GetT(Playerindex);
            if (roleObject != null)
                role = roleObject.GetComponent<Person>();
            if (role != null)
            {
                role.Fire();
            }
        }
    }
}
