using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectRole2 : MonoBehaviour
{
    public Sprite[] roleSprites;
    public Image[] roleImage;

    [HideInInspector]
    public int[] index;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        index = new int[2] { 0, 1 };
    }

    private void Start()
    {
        index[0] = 0; index[1] = 1;
        roleImage[0].sprite = roleSprites[index[0]];
        roleImage[1].sprite = roleSprites[index[1]];
    }

    private void Update()
    {
        if (!Menu.isFullScreen)
        {
            Menu.screenSize = new int[2] { Screen.width, Screen.height };
        }
    }
    public void Next1()
    {
        index[0] = (index[0] + 1) % 5;
        while (index[0] == index[1])
        {
            index[0] = (index[0] + 1) % 5;
        }
        roleImage[0].sprite = roleSprites[index[0]];
    }

    public void Previous1()
    {
        if (index[0] == 0)
            index[0] = 4;
        else
            index[0]--;
        while (index[0] == index[1])
        {
            if (index[0] == 0)
                index[0] = 4;
            else
                index[0]--;
        }

        roleImage[0].sprite = roleSprites[index[0]];
    }

    public void Next2()
    {
        index[1] = (index[1] + 1) % 5;
        while (index[1] == index[0])
        {
            index[1] = (index[1] + 1) % 5;
        }
        roleImage[1].sprite = roleSprites[index[1]];
    }

    public void Previous2()
    {
        if (index[1] == 0)
            index[1] = 4;
        else
            index[1]--;
        while (index[1] == index[0])
        {
            if (index[1] == 0)
                index[1] = 4;
            else
                index[1]--;
        }

        roleImage[1].sprite = roleSprites[index[1]];
    }


    public void StartGame()
    {
        SceneManager.LoadScene(4);
    }

    public void ReturnToMainMenu()
    {
        Destroy(gameObject);
        SceneManager.LoadScene(0);
    }
}
