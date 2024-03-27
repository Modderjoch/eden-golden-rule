using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateBar : MonoBehaviour
{
    public ProgressBar progressBar;

    void Start()
    {
        // Access the ProgressBar script attached to the GameObject
        progressBar = GetComponent<ProgressBar>();
    }

    void Update()
    {
        // Example usage: updating the progress bar
        progressBar.SetProgress(Time.time / 10f); // Progress based on time
    }
}