// Copyright Oebe Rademaker All rights reserved.

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
            if(paper != null)

            paper.gameObject.SetActive(true);

            if(paper.gameObject.activeSelf)
            {
                paper.Blow();
                paper.transform.parent = null;
            }
        }
    }
}
