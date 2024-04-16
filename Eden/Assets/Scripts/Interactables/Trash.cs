using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trash : Interactable
{
    public int Score => score;

    [SerializeField] private int score = 1;

    [SerializeField] private TrashProgress trashProgress;

    protected void Awake()
    {
        GameManager.Instance.trashProgress.SetTotalScore(score);
    }

    public bool PickUp()
    {
        Destroy(gameObject);

        return true;
    }
}
