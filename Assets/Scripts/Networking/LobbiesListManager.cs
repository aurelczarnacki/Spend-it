using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbiesListManager : MonoBehaviour
{
    public static LobbiesListManager instance;

    public GameObject lobbiesMenu;
    public RectTransform lobbyDataItemPrefab;
    public RectTransform lobbyListContent;

    public GameObject lobbiesButton, hostButton;

    public List<GameObject> listOfLobbies = new List<GameObject>();

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    public void DestroyLobbies()
    {
        foreach(GameObject item in listOfLobbies)
        {
            Destroy(item);
        }
        listOfLobbies.Clear();
    }

    public void DisplayLobbies(List<CSteamID> lobbyIDs, LobbyDataUpdate_t result)
    {
        DestroyLobbies();

        for (int i = 0; i < lobbyIDs.Count; i++)
        {
            if (lobbyIDs[i].m_SteamID == result.m_ulSteamIDLobby)
            {
                RectTransform createdItem = Instantiate(lobbyDataItemPrefab, lobbyListContent);

                createdItem.GetComponent<LobbyDataEntry>().lobbyID = (CSteamID)lobbyIDs[i].m_SteamID;
                createdItem.GetComponent<LobbyDataEntry>().lobbyName = SteamMatchmaking.GetLobbyData((CSteamID)lobbyIDs[i].m_SteamID, "name");
                createdItem.GetComponent<LobbyDataEntry>().SetLobbyData();

                listOfLobbies.Add(createdItem.gameObject);
            }
        }
    }

    public void GetListOfLobbies()
    {
        SteamLobby.instance.GetLobbiesList();
    }


}
