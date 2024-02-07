using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostLobbyButton : MonoBehaviour
{

    public void OnHostClick()
    {
        SteamLobby.instance.HostLobby();
    }
}
