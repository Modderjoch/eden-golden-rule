using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUpScript : MonoBehaviour
{
    private Animator animator;

    [SerializeField] private TMP_Text popUpMsg;
    public string popUpText = "gamer";

    private void Start()
    {
        animator = GetComponent<Animator>();
        popUpMsg.text = popUpText;
    }

    public void PopUpEntry(string text, int x)
    {
        popUpMsg.text = text;
        animator.SetTrigger("entry");
        StartCoroutine(PopUpClose(x));
    }
    
    IEnumerator PopUpClose(int x)
    {
        yield return new WaitForSeconds(x);
        animator.SetTrigger("close");
    }
}