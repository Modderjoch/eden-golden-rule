// Copyright Oebe Rademaker All rights reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject deadTree;
    [SerializeField] private GameObject liveTree;

    [SerializeField] private float speed = 1.0f;
    [SerializeField] private AnimationCurve blendCurve;

    private Vector3 initialScale;
    public bool transitionActivated = false;
    private float blend = 0;

    protected void Start()
    {
        initialScale = transform.GetChild(0).transform.localScale;
    }

    private void Update()
    {
        if (transitionActivated)
        {
            blend = Mathf.Clamp01(blend + Time.deltaTime * speed);

            float modifiedBlend = blendCurve.Evaluate(blend);

            deadTree.transform.localScale = initialScale * (1 - modifiedBlend);

            liveTree.transform.localScale = initialScale * modifiedBlend;

            if (blend >= 1)
            {
                transitionActivated = false;
            }
        }
    }

    public void ActivateTransition()
    {
        transitionActivated = true;
    }

    public void ToggleTrees()
    {
        if(deadTree.transform.localScale == Vector3.zero)
        {
            initialScale = liveTree.transform.localScale;
        }
        else { initialScale = deadTree.transform.localScale; }

        if (deadTree.transform.localScale == initialScale)
        {
            deadTree.transform.localScale = Vector3.zero;
            liveTree.transform.localScale = initialScale;
        }
        else
        {
            deadTree.transform.localScale = initialScale;
            liveTree.transform.localScale = Vector3.zero;
        }
    }
}