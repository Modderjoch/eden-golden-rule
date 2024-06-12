// Copyright Oebe Rademaker All rights reserved

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToMiddle : MonoBehaviour
{
    public bool MiddleMove => moveToMiddle;

    private bool moveToMiddle = false;
    private RectTransform rectTransform;
    private Vector2 targetPosition;
    public float speed = 5f;

    public GameObject[] otherPacks;

    public event Action OnMiddleReached;

    [SerializeField] 

    protected void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        targetPosition = new Vector2(0, -400);
    }

    protected void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha8))
        {
            StartMoveToMiddle();
        }

        if (moveToMiddle)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, targetPosition, speed * Time.deltaTime);

            if (Vector2.Distance(rectTransform.anchoredPosition, targetPosition) < 0.1f)
            {
                rectTransform.anchoredPosition = targetPosition;
                OnMiddleReached.Invoke();
                moveToMiddle = false;
            }
        }
    }

    public void StartMoveToMiddle()
    {
        Debug.Log("Moving to middle");

        moveToMiddle = true;
    }

    public void DeactivateOtherPacks()
    {
        for (int i = 0; i < otherPacks.Length; i++)
        {
            otherPacks[i].gameObject.SetActive(false);
        }
    }
}
