using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Explosion : MonoBehaviourPun
{
    public int x, y;
    public int isEnd;//0:middle 1:end -1:center
    public Sprite[] explosionSprites;
    // Start is called before the first frame update
    void Start()
    {
        if (isEnd == 0)
        {
            if (!PhotonNetwork.IsConnected)
            {
                    GetComponent<SpriteRenderer>().sprite = explosionSprites[1];
                
            }
            else
            {
                photonView.RPC("changeexplosionsprite", RpcTarget.All, 1);
            }
        }
        else if (isEnd == 1)
        {
            if (!PhotonNetwork.IsConnected)
            {
                    GetComponent<SpriteRenderer>().sprite = explosionSprites[2];
                
            }
            else
            {
                photonView.RPC("changeexplosionsprite", RpcTarget.All, 2);
            }
        } 
        

        Invoke(nameof(DestoryExplosion), 0.3f);
    }

    private void DestoryExplosion()
    {
        Destroy(gameObject);
        for(int i = 0; i < 2000; i++)
        {

        }
        GameManager.Instance.explosionRange[x, y]--;

    }

    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Role"))
        {
            if (!collision.GetComponent<Person>().isDefended)
            {
                collision.GetComponent<Person>().ReduceLife();
            }

        }

    }
    [PunRPC]
    public void changeexplosionsprite(int i)
    {
        GetComponent<SpriteRenderer>().sprite = explosionSprites[i];
    }
}
