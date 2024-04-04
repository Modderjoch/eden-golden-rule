using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trash : Interactable
{
    public int Score => score;
    
    public TrashType trashType;

    [SerializeField] private int score = 1;

    public enum TrashType
    {
        Bottle,
        Beer,
        Box,
        Chips,
        Can,
        Bag,
    }

    public bool PickUp()
    {
        Destroy(gameObject);

        return true;
    }
}
