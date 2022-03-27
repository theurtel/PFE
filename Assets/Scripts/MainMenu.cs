using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadSceneAsync("SampleScene");
    }

    public void SkinMenu()
    {
        SceneManager.LoadSceneAsync("SkinMenu");
    }

    public void SettingsMenu()
    {
        SceneManager.LoadSceneAsync("SettingsMenu");
    }

    public void StartSwipeVersion()
    {
        SceneManager.LoadSceneAsync("SampleSceneSwipe");
    }

    public void backToMenu()
    {
        SceneManager.LoadSceneAsync("Menu");
    }
}
