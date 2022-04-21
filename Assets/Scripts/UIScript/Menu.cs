using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public Button startButton;
    public Button exitButton;
    public Button[] submenuButton;
    public Button settingButton;
    public Image settingPanel;
    public Toggle soundToggle;
    public Toggle fullScreenToggle;
    public Image wuxiPanel;
    public Button[] yesOrNoButton;
    public Image IntroPanel;

    public static int mode = -1;//0:单人 1:双人 2:联机
    public static bool sound = true;//true:有音效 false:无音效
    public static bool isFullScreen;//true:全屏
    public static int[] screenSize = new int[2] { 800, 600 };

    private int index;

    private void Awake()
    {
        index = 0;
        startButton.gameObject.SetActive(true);
        exitButton.gameObject.SetActive(true);
        settingButton.gameObject.SetActive(true);
        for (int i = 0; i < 4; i++)
        {
            submenuButton[i].gameObject.SetActive(false);
        }
        settingPanel.gameObject.SetActive(false);
        wuxiPanel.gameObject.SetActive(false);
        IntroPanel.gameObject.SetActive(false);
        soundToggle.isOn = sound;
        OnClickSound();
        isFullScreen = Screen.fullScreen;
        fullScreenToggle.isOn = isFullScreen;
    }

    private void Update()
    {
        if (!isFullScreen)
        {
            screenSize = new int[2] { Screen.width, Screen.height };
        }
    }

    public void GoToSubMenu()
    {
        index = 1;
        startButton.gameObject.SetActive(false);
        exitButton.gameObject.SetActive(false);
        settingButton.gameObject.SetActive(true);
        for (int i = 0; i < 4; i++)
        {
            submenuButton[i].gameObject.SetActive(true);
        }
        settingPanel.gameObject.SetActive(false);
        wuxiPanel.gameObject.SetActive(false);
        IntroPanel.gameObject.SetActive(false);
    }

    public void ReturnToMainMenu()
    {
        index = 0;
        startButton.gameObject.SetActive(true);
        exitButton.gameObject.SetActive(true);
        settingButton.gameObject.SetActive(true);
        for (int i = 0; i < 4; i++)
        {
            submenuButton[i].gameObject.SetActive(false);
        }
        settingPanel.gameObject.SetActive(false);
        wuxiPanel.gameObject.SetActive(false);
        IntroPanel.gameObject.SetActive(false);
    }

    public void PlaySingleGame()
    {
        startButton.gameObject.SetActive(false);
        exitButton.gameObject.SetActive(false);
        settingButton.gameObject.SetActive(false);
        for (int i = 0; i < 4; i++)
        {
            submenuButton[i].gameObject.SetActive(false);
        }
        wuxiPanel.gameObject.SetActive(true);
        IntroPanel.gameObject.SetActive(false);
    }

    public void PlaySingleGameOrdinary()
    {
        mode = 0;
        SceneManager.LoadScene(1);
    }

    public void PlaySingleGameWuxi()
    {
        mode = 2;
        SceneManager.LoadScene(1);
    }

    public void PlayDoubleGame()
    {
        mode = 1;
        SceneManager.LoadScene(2);
    }
    public void OnlineGame()
    {
        mode = 3;
        SceneManager.LoadScene(3);
    }
    public void Exit()
    {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;

#else
        Application.Quit();
#endif
    }

    public void UnfoldIntroPanel()
    {
        wuxiPanel.gameObject.SetActive(false);
        IntroPanel.gameObject.SetActive(true);
    }

    public void UnfoldSetting()
    {
        startButton.gameObject.SetActive(false);
        exitButton.gameObject.SetActive(false);
        settingButton.gameObject.SetActive(false);
        for (int i = 0; i < 4; i++)
        {
            submenuButton[i].gameObject.SetActive(false);
        }
        settingPanel.gameObject.SetActive(true);
        wuxiPanel.gameObject.SetActive(false);
    }

    public void FoldSetting()
    {
        if (index == 0)
        {
            ReturnToMainMenu();
        }
        else
        {
            GoToSubMenu();
        }
    }

    public void OnClickSound()
    {

        if (soundToggle.isOn)
        {
            sound = true;
            AudioListener.pause = false;
        }
        else
        {
            sound = false;
            AudioListener.pause = true;
        }

    }

    public void OnClickFullscreen()
    {
        if (fullScreenToggle.isOn)
        {
            Screen.fullScreen = true;
            isFullScreen = true;
            Resolution[] resolutions = Screen.resolutions;
            Screen.SetResolution(resolutions[resolutions.Length - 1].width, resolutions[resolutions.Length - 1].height, true);
        }
        else
        {
            Screen.fullScreen = false;
            isFullScreen = false;
            Screen.SetResolution(screenSize[0], screenSize[1], false);
        }
    }
}
