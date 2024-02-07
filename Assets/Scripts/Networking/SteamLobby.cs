using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using TMPro;
using UnityEngine.SceneManagement;

public class SteamLobby : MonoBehaviour
{

    public static SteamLobby instance;

    protected Callback<LobbyCreated_t> LobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> JoinRequest;
    protected Callback<LobbyEnter_t> LobbyEntered;

    protected Callback<LobbyMatchList_t> LobbyList;
    protected Callback<LobbyDataUpdate_t> LobbyDataUpdated;

    public List<CSteamID> LobbyIDs = new List<CSteamID>();

    public ulong CurrentLobbyID;
    private const string HostAddressKey = "HostAddress";
    private CustomNetworkManager manager;



    private void Awake()
    {
        if(instance == null)
            instance = this;
    }
    private void Start()
    {
        if (!SteamManager.Initialized)
            return;
        Inicialize();

    }
    public void CloseLobby()
    {
        manager.StopHost();
    }
    private void Inicialize()
    {
        manager = GetComponent<CustomNetworkManager>();

        LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        JoinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
        LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);

        LobbyList = Callback<LobbyMatchList_t>.Create(OnGetLobbyList);
        LobbyDataUpdated = Callback<LobbyDataUpdate_t>.Create(OnGetLobbyData);
    }

    public void HostLobby()
    {
        if (SteamMatchmaking.GetLobbyOwner((CSteamID)CurrentLobbyID) != SteamUser.GetSteamID())
        {
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, manager.maxConnections);
        }
        else
            SceneManager.LoadScene(1);
        
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
            return;

        Debug.Log("Created Lobby");
        
        manager.StartHost();

        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey, SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name", SteamFriends.GetPersonaName().ToString() + "'s safe heist");     
    }



    private void OnJoinRequest(GameLobbyJoinRequested_t callback)
    {
        Debug.Log("Request to join");
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        
        CurrentLobbyID = callback.m_ulSteamIDLobby;

        if (NetworkServer.active)
            return;

        manager.networkAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey);
        manager.StartClient();

    }

    public void JoinLobby(CSteamID lobbyID)
    {
        SteamMatchmaking.JoinLobby(lobbyID);
    }

    public void GetLobbiesList()
    {
        if(LobbyIDs.Count > 0) {
            LobbyIDs.Clear();
        }

        SteamMatchmaking.AddRequestLobbyListResultCountFilter(60);
        SteamMatchmaking.RequestLobbyList();
    }

    void OnGetLobbyList(LobbyMatchList_t result)
    {
        if(LobbiesListManager.instance.listOfLobbies.Count > 0) { LobbiesListManager.instance.DestroyLobbies(); }

        for(int i=0; i < result.m_nLobbiesMatching; i++)
        {
            
            CSteamID lobbyID = SteamMatchmaking.GetLobbyByIndex(i);
            if (SteamMatchmaking.GetLobbyData(lobbyID, "name").Contains("safe heist"))
            {
                LobbyIDs.Add(lobbyID);
                SteamMatchmaking.RequestLobbyData(lobbyID);
            }
        }
    }

    void OnGetLobbyData(LobbyDataUpdate_t result)
    {
        LobbiesListManager.instance.DisplayLobbies(LobbyIDs, result);
    }

}
