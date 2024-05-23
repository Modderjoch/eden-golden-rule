using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paper : Interactable
{
    public int Score => score;

    [SerializeField] private int score = 1;

    protected void Awake()
    {
        GameManager.Instance.PaperProgress.SetTotalScore(score);
    }

    public bool PickUp()
    {
        Destroy(gameObject);

        return true;
    }
}
