using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TrashProgress : MonoBehaviour
{
    [SerializeField] private Image progressBar;
    [SerializeField] private TMP_Text progressText;

    private int totalScore = 0;
    private int currentScore = 0;

    public event Action OnScoreReached; // Event to be invoked when currentScore equals totalScore
    public event Action OnScoreAdded;

    protected void Start()
    {
        RefreshUI();
    }

    public void SetTotalScore(int score)
    {
        totalScore += score;

        RefreshUI();
    }

    public void AddScore(Trash trash)
    {
        currentScore += trash.Score;

        OnScoreAdded.Invoke();

        RefreshUI();

        if (currentScore == totalScore)
        {
            OnScoreReached.Invoke();
        }
    }

    public int ReturnTotalScore()
    {
        return totalScore;
    }

    public int ReturnCurrentScore()
    {
        return currentScore;
    }

    private void RefreshUI()
    {
        progressBar.fillAmount = currentScore / (float)totalScore;
        progressText.text = currentScore + "/" + totalScore;
    }
}
