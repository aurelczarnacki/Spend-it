using Steamworks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyDataEntry : MonoBehaviour
{

    public CSteamID lobbyID;
    public string lobbyName;
    public TextMeshProUGUI lobbyNameText;
    public GameObject lobbyGameObject;

    public void Start()
    {

    }

    public void SetLobbyData()
    {
        if(lobbyName == "")
        {
            lobbyNameText.text = "Empty";
        }
        else
        {
            lobbyNameText.text = lobbyName;
        }

        lobbyNameText.text = lobbyName;

    }

    public void JoinLobby()
    {
        SteamLobby.instance.JoinLobby(lobbyID);
        SceneManager.LoadScene(1);
    }
}
