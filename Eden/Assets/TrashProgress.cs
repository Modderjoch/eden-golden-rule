using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrashProgress : MonoBehaviour
{
    [SerializeField] private Image progressBar;
    [SerializeField] private TMP_Text progressText;

    private int totalScore = 0;
    private int currentScore = 0;

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

        RefreshUI();

        Debug.Log("Score is now: " + currentScore + " from " + (currentScore - trash.Score));
    }

    /// <summary>
    /// Returns the total score, which is all scores of every trash object
    /// added together.
    /// </summary>
    /// <returns>The total score possible</returns>
    public int ReturnTotalScore()
    {
        return totalScore;
    }

    /// <summary>
    /// Returns the current score, which is all scores of every thus far collected object
    /// added together.
    /// </summary>
    /// <returns>The current score</returns>
    public int ReturnCurrentScore()
    {
        return currentScore;
    }

    private void RefreshUI()
    {
        progressBar.fillAmount = currentScore / (float)totalScore;
        progressText.text = currentScore + " / " + totalScore;
    }
}
