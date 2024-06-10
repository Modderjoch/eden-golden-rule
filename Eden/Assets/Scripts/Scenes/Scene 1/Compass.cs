// Copyright Oebe Rademaker All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compass : Interactable
{
    public event Action OnCompassCollected;

    public void CollectCompass()
    {
        OnCompassCollected.Invoke();
    }
}
