// Copyright Oebe Rademaker All rights reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public Image progressBarFill;
    public float progressValue = 0f;

    // Function to update the progress bar
    public void SetProgress(float value)
    {
        progressValue = Mathf.Clamp01(value);
        progressBarFill.fillAmount = progressValue;
    }
}
