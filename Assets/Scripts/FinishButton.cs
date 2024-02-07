using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishButton : MonoBehaviour
{
    public void FinishGame()
    {

        LobbyController.Instance.CloseLobby();
    }
}
