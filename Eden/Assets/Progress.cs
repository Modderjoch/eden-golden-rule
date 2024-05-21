using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class Progress : MonoBehaviour
{
    [SerializeField] private Image progressBar;
    [SerializeField] private TMP_Text progressText;

    private int totalScore = 0;
    private int currentScore = 0;

    public event Action OnScoreReached;
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

    public void AddScore(Interactable interactable)
    {
        if(interactable is Trash trash)
        {
            currentScore += trash.Score;
        }
        else
        {
            if(interactable is Paper paper)
            {
                currentScore += paper.Score;
            }
        }

        if (OnScoreAdded != null)
        {
            OnScoreAdded.Invoke();
        }

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
