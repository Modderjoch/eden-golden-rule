using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperController : MonoBehaviour
{
    [SerializeField] private List<PaperSheet> papers;

    public void BlowPapers()
    {
        foreach(PaperSheet paper in papers)
        {
            paper.Blow();
        }
    }
}
