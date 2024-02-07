using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PatronDisplay : MonoBehaviour
{
    public Image backgroundImage;

    public TextMeshProUGUI blackReq;
    public TextMeshProUGUI greenReq;
    public TextMeshProUGUI redReq;
    public TextMeshProUGUI blueReq;
    public TextMeshProUGUI whiteReq;

    public Patron patron;

    private void Start()
    {
        backgroundImage.sprite = patron.image;
        SetReqTextAndVisibility(blackReq, patron.blackReq);
        SetReqTextAndVisibility(greenReq, patron.greenReq);
        SetReqTextAndVisibility(redReq, patron.redReq);
        SetReqTextAndVisibility(blueReq, patron.blueReq);
        SetReqTextAndVisibility(whiteReq, patron.whiteReq);
    }


    void SetReqTextAndVisibility(TextMeshProUGUI reqText, int reqValue)
    {
        if (reqValue > 0)
        {
            reqText.text = reqValue.ToString();
        }
        else
        {
            reqText.transform.parent.gameObject.SetActive(false);
        }
    }
}
