using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public int x, y;
    public int index;//第几个传送门
    public GameManager gameManager;

    private void Awake()
    {
        gameManager = GameManager.Instance;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Role") && collision.GetComponent<Person>().canEnterDoor<=0)
        {
            int targetIndex = index;
            //Debug.Log("Enter:" + index);
            while (targetIndex == index)
            {
                targetIndex = Random.Range(0, gameManager.doors.Count);
            }
            //Debug.Log("Target:" + targetIndex);
            GameManager.Location target = gameManager.doors[targetIndex];

            collision.GetComponent<Transform>().position = gameManager.CorrectPosition(target.x, target.y);
            collision.GetComponent<Person>().canEnterDoor = 3f;
        }
        

    }
}
