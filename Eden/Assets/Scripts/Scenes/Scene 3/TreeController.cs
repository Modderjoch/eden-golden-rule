// Copyright Oebe Rademaker All rights reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeController : MonoBehaviour
{
    [Range(0, 1)] public float treeBlend = 0;

    public List<TreeSwitcher> list = new List<TreeSwitcher>();

    public bool doTransition = true;

    protected void Update()
    {
        if (doTransition)
        {
            foreach (TreeSwitcher s in list)
            {
                s.ActivateTransition();
            }
        }
    }
}
