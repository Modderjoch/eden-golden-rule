// Copyright Oebe Rademaker All rights reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TrashProgress trashProgress;
    [SerializeField] private PaperProgress paperProgress;

    private static ScoreManager instance;

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
                trashProgress.AddScore(trash);
            }
            else if(interactable is Paper paper)
            {
                paperProgress.AddScore(paper);
            }
        }
    }
}
