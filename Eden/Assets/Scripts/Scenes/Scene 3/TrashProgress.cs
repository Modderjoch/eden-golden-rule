// Copyright Oebe Rademaker All rights reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashProgress : Progress
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
    }

    protected void Update()
    {
        // If the progress is less than 1, continue moving
        if (progress < 1f)
        {
            // Calculate how much to move this frame
            float step = moveSpeed * Time.deltaTime;

            // Update the progress
            progress = Mathf.Min(progress + step / distanceToMove, 1f);

            // Interpolate the position
            rectTransform.anchoredPosition = Vector2.Lerp(initialPosition, targetPosition, progress);
        }
    }
}
