using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave : MonoBehaviour
{
    public int x, y;
    public int isEnd;//0:middle 1:end -1:start
    public Sprite[] waveSprites;

    void Start()
    {
        if (isEnd == 0)
        {
             GetComponent<SpriteRenderer>().sprite = waveSprites[1];
        }
        else if (isEnd == 1)
        {

             GetComponent<SpriteRenderer>().sprite = waveSprites[2];
        }


        Invoke(nameof(DestoryWave), 0.3f);
    }

    private void DestoryWave()
    {
        Destroy(gameObject);
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
    
}
