// Copyright Oebe Rademaker All rights reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperProgress : Progress
{
    private RectTransform rectTransform;

    // Distance to move
    public float distanceToMove = 300f;

    // Speed of movement
    public float moveSpeed = 100f; // Units per second

    // Initial position
    private Vector2 initialPosition;

    // Target position
    private Vector2 targetPosition;

    // Progress tracker
    private float progress = 0f;

    protected void OnEnable()
    {
        rectTransform = GetComponent<RectTransform>();
        initialPosition = rectTransform.anchoredPosition;
        targetPosition = initialPosition + new Vector2(0, distanceToMove);
        CoroutineHandler.Instance.StartCoroutine(MoveUp());
    }

    protected void OnDisable()
    {
        CoroutineHandler.Instance.StartCoroutine(MoveDown());
    }

    private IEnumerator MoveUp()
    {
        progress = 0f;
        while (progress < 1f)
        {
            float step = moveSpeed * Time.deltaTime;
            progress = Mathf.Min(progress + step / distanceToMove, 1f);
            rectTransform.anchoredPosition = Vector2.Lerp(initialPosition, targetPosition, progress);
            yield return null;
        }
    }

    private IEnumerator MoveDown()
    {
        Vector2 startPosition = rectTransform.anchoredPosition;
        Vector2 endPosition = initialPosition;
        progress = 0f;

        while (progress < 1f)
        {
            float step = moveSpeed * Time.deltaTime;
            progress = Mathf.Min(progress + step / distanceToMove, 1f);
            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, endPosition, progress);
            yield return null;
        }
    }
}
