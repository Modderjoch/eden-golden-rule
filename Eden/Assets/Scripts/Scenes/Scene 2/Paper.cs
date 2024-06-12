// Copyright Oebe Rademaker All rights reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paper : Interactable
{
    public int Score => score;

    [SerializeField] private int score = 1;

    public Paper(int score)
    {
        this.score = score;
    }

    protected void Awake()
    {
        GameManager.Instance.PaperProgress.SetTotalScore(score);
    }

    public bool PickUp()
    {
        int randomIndex = Random.Range(0, 3);

        AudioManager.Instance.Play("Paper" +  randomIndex);

        Destroy(gameObject);

        return true;
    }
}
