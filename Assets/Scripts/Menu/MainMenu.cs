using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject joinGame;
    public GameObject settings;
    private GameObject currentlyActive;

    public void OnSettingsClick()
    {
        CloseActive();
        settings.SetActive(true);
        currentlyActive = settings;
    }

    public void OnJoinClick()
    {
        CloseActive();
        joinGame.SetActive(true);
        currentlyActive = joinGame;
    }

    public void OnExitButtonClick()
    {
        Application.Quit();
    }

    public void OnBackButton()
    {
        CloseActive();
    }

    public void CloseActive()
    {
        if (currentlyActive != null)
            currentlyActive.SetActive(false);
    }
}
