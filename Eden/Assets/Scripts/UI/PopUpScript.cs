using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUpScript : MonoBehaviour
{
    [SerializeField] private TMP_Text popUpMsg;
    public string popUpText = "gamer";

    private void OnEnable()
    {
        
        popUpMsg.text = popUpText;
    }

}