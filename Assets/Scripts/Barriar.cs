using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Barriar : MonoBehaviourPun
{
    public Sprite[] barriarSprites;
    private int index;

    private void Start()
    {

        if (!PhotonNetwork.IsConnected)
        {
            index = Random.Range(0, 3);
            GetComponent<SpriteRenderer>().sprite = barriarSprites[index * 2];
            transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = barriarSprites[index * 2 + 1];
        }
        else
        {
            if (PhotonNetwork.IsMasterClient)
            {
                index = Random.Range(0, 3);
                GetComponent<SpriteRenderer>().sprite = barriarSprites[index * 2];
                transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = barriarSprites[index * 2 + 1];
                photonView.RPC("changeSprite", RpcTarget.Others, index);
            }

        }
    }

    [PunRPC]
    public void changeSprite(int i)
    {
        index = i;
        GetComponent<SpriteRenderer>().sprite = barriarSprites[i*2];
        transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = barriarSprites[i * 2 + 1];
    }

}
