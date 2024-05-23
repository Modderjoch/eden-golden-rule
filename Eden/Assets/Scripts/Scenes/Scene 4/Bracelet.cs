using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bracelet : Interactable
{
    public event Action OnBraceletCollected;

    public void CollectBracelet()
    {
        OnBraceletCollected.Invoke();
    }
}
