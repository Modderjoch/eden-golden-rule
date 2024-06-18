// Copyright Oebe Rademaker All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bracelet : Interactable
{
    public event Action OnBraceletCollected;

    public void CollectBracelet()
    {
        AudioManager.Instance.Play("Paper1");

        OnBraceletCollected.Invoke();
    }
}
