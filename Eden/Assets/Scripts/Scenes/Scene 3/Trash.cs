// Copyright Oebe Rademaker All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trash : Interactable
{
    public int Score => score;

    [SerializeField] private int score = 1;

    [SerializeField] private TrashProgress trashProgress;

    [SerializeField] private string audioFile;

    public Trash(int score)
    {
        this.score = score;
    }

    protected void Awake()
    {
        GameManager.Instance.TrashProgress.SetTotalScore(score);
    }

    public bool PickUp()
    {
        AudioManager.Instance.Play(audioFile);

        Destroy(gameObject);

        return true;
    }
}
