using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tools : MonoBehaviour
{
    //引用
    GameManager gameManager;
    //属性
    public int x, y;
    public float disappearTime = 30f;
    //游戏资源
    public AudioClip toolAudio;

    private void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void Update()
    {
        disappearTime -= Time.deltaTime;
        if (disappearTime < 0)
        {
            gameManager.itemsType[x, y] = GameManager.ItemType.EMPTY;
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //播放音频
        AudioSource.PlayClipAtPoint(toolAudio, new Vector3(x, y, 0));

        if (CompareTag("tool_bomb"))
        {
            if(collision.GetComponent<Person>().bombNumber < 4)
                collision.GetComponent<Person>().bombNumber++;
        }
        if (CompareTag("tool_potion"))
        {
            if(collision.GetComponent<Person>().bombRadius < 4)
                collision.GetComponent<Person>().bombRadius++;
        }
        if (CompareTag("tool_shoes"))
        {
            if(collision.GetComponent<Person>().speed < 4.0f)
                collision.GetComponent<Person>().speed += 1.0f;
        }
        if (CompareTag("tool_heart"))
        {
            if (collision.GetComponent<Person>().life < 4)
                collision.GetComponent<Person>().life++;
        }

        gameManager.itemsType[x, y] = GameManager.ItemType.EMPTY;
        Destroy(gameObject);
    }
}
