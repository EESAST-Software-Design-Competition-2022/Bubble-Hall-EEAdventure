using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectRole : MonoBehaviour
{
    public Sprite[] roleSprites;
    public Image roleImage;

    [HideInInspector]
    public int index = 0;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        index = 0;
        roleImage.sprite = roleSprites[index];
    }


    private void Update()
    {
        if (!Menu.isFullScreen)
        {
            Menu.screenSize = new int[2] { Screen.width, Screen.height };
        }
    }

    public void Next()
    {
        index = (index + 1) % 5;
        roleImage.sprite = roleSprites[index];
    }

    public void Previous()
    {
        if (index == 0)
            index = 4;
        else
            index--;
        roleImage.sprite = roleSprites[index];
    }

    public void StartGame()
    {
        if(Menu.mode == 0)
        {
            SceneManager.LoadScene(4);
        }
        if (Menu.mode == 2)
        {
            SceneManager.LoadScene(5);
        }

    }

    public void ReturnToMainMenu()
    {
        Destroy(gameObject);
        SceneManager.LoadScene(0);
    }
}
