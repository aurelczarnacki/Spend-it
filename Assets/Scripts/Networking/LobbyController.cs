using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Steamworks;
using Mirror.Examples.AdditiveLevels;
using Mirror;
using System.Linq;
using UnityEngine.SceneManagement;

public class LobbyController : MonoBehaviour
{
    public static LobbyController Instance;

    public TextMeshProUGUI LobbyNameText;

    public RectTransform PlayerListViewContent;
    public RectTransform PlayerListItemPrefab;
    public GameObject LocalPlayerObject;

    public ulong CurrentLobbyID;
    public bool PlayerItemCreated = false;
    private List<PlayerListItem> PlayerListItems = new List<PlayerListItem>();
    public PlayerObjectController LocalPlayerObjectController;

    private CustomNetworkManager manager;
    private CustomNetworkManager Manager
    {
        get
        {
            if (manager != null)
            {
                return manager;
            }
            return manager = CustomNetworkManager.singleton as CustomNetworkManager;
        }
    }

    private void Awake()
    {
        if(Instance == null) { Instance = this; }
    }

    public void UpdateLobbyName()
    {
        CurrentLobbyID = Manager.GetComponent<SteamLobby>().CurrentLobbyID;
        LobbyNameText.text = SteamMatchmaking.GetLobbyData(new CSteamID(CurrentLobbyID), "name");
    }

    public void UpdatePlayerList()
    {
        if(!PlayerItemCreated) { CreateHostPlayerItem(); }
        if(PlayerListItems.Count < Manager.GamePlayers.Count) { CreateClientPlayerItem(); }
        if(PlayerListItems.Count > Manager.GamePlayers.Count) { RemovePlayerItem(); }
        if(PlayerListItems.Count == Manager.GamePlayers.Count) { UpdatePlayerItem(); }
    }

    public void FindLocalPlayer()
    {
        LocalPlayerObject = GameObject.Find("LocalGamePlayer");
        LocalPlayerObjectController = LocalPlayerObject.GetComponent<PlayerObjectController>();
    }

    public void CreateHostPlayerItem()
    {
        foreach(PlayerObjectController player in Manager.GamePlayers)
        {
            RectTransform NewPlayerItem = Instantiate(PlayerListItemPrefab, PlayerListViewContent);
            PlayerListItem NewPlayerItemScript = NewPlayerItem.GetComponent<PlayerListItem>();

            NewPlayerItemScript.PlayerName = player.PlayerName;
            NewPlayerItemScript.ConnectionID = player.ConnectionID;
            NewPlayerItemScript.PlayerSteamID = player.PlayerSteamID;
            NewPlayerItemScript.SetPlayerValues();


            PlayerListItems.Add(NewPlayerItemScript);
        }
        PlayerItemCreated = true;
    }

    public void CreateClientPlayerItem()
    {
        foreach(PlayerObjectController player in Manager.GamePlayers)
        {
            if(!PlayerListItems.Any(b => b.ConnectionID == player.ConnectionID))
            {
                RectTransform NewPlayerItem = Instantiate(PlayerListItemPrefab, PlayerListViewContent);
                PlayerListItem NewPlayerItemScript = NewPlayerItem.GetComponent<PlayerListItem>();

                NewPlayerItemScript.PlayerName = player.PlayerName;
                NewPlayerItemScript.ConnectionID = player.ConnectionID;
                NewPlayerItemScript.PlayerSteamID = player.PlayerSteamID;
                NewPlayerItemScript.SetPlayerValues();


                PlayerListItems.Add(NewPlayerItemScript);
            }
        }
    }

    public void RemovePlayerItem()
    {
        foreach (PlayerListItem playerListItem in PlayerListItems)
        {
            if(!Manager.GamePlayers.Any(b => b.ConnectionID == playerListItem.ConnectionID)) {

                GameObject ObjectToRemove = playerListItem.gameObject;
                Destroy(ObjectToRemove);
                ObjectToRemove = null;
            }
        }
    }

    public void UpdatePlayerItem()
    {
        foreach (PlayerObjectController player in Manager.GamePlayers)
        {
            foreach(PlayerListItem PlayerListItemScript in PlayerListItems)
            {
                if(PlayerListItemScript.ConnectionID == player.ConnectionID)
                {
                    PlayerListItemScript.PlayerName = player.PlayerName;
                    PlayerListItemScript.SetPlayerValues();
                }
            }
        }
    }
    public void CloseLobby()
    {
        SteamMatchmaking.LeaveLobby((CSteamID)CurrentLobbyID);
        SceneManager.LoadScene(0);
        SteamLobby.instance.CloseLobby();
    }

    public void StartGame(string SceneName)
    {
        LocalPlayerObjectController.CanStartGame(SceneName);
    }
}
