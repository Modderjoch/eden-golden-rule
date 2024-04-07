using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TrashProgress trashProgress;

    private static ScoreManager instance;

    private int forestScore = 0;
    private int gardenScore = 0;

    private Dictionary<Trash.TrashType, int> forestScores = new Dictionary<Trash.TrashType, int>();

    public static ScoreManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ScoreManager>();

                if (instance == null)
                {
                    GameObject obj = new GameObject("ScoreManager");
                    instance = obj.AddComponent<ScoreManager>();
                }
            }

            return instance;
        }
    }

    protected void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public void AddScore(Interactable interactable)
    {
        if(interactable != null)
        {
            if(interactable is Trash trash)
            {
                forestScore += trash.Score;

                trashProgress.AddScore(trash);

                if (forestScores.ContainsKey(trash.trashType))
                {
                    forestScores[trash.trashType] += trash.Score;
                }
                else
                {
                    forestScores.Add(trash.trashType, trash.Score);
                }

                DebugForestScores();
            }
            else if(interactable is Paper paper)
            {
                gardenScore += paper.Score;
            }
        }
    }

    public void DebugForestScores()
    {
        foreach (KeyValuePair<Trash.TrashType, int> pair in forestScores)
        {
            Debug.Log("Trash Type: " + pair.Key + ", Score: " + pair.Value);
        }
    }
}
