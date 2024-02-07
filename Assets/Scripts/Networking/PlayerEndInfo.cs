using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerEndInfo : MonoBehaviour
{
    public TextMeshProUGUI playerNameTMP;
    public TextMeshProUGUI pointsTMP;
    public string playerName;
    public string points;

    private void Start()
    {
        playerNameTMP.text = playerName;
        pointsTMP.text = points;
    }

}
