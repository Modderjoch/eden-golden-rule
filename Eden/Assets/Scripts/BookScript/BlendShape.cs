using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlendShape : MonoBehaviour
{ 
    private float mSize = 0.0f;
    [SerializeField] SkinnedMeshRenderer bookPages;

    // Start is called before the first frame update
    void Start()
    {
        
        bookPages = bookPages.GetComponent<SkinnedMeshRenderer>();
    }

    public void RestartScript()
    {
        mSize = 0.0f;
        InvokeRepeating("Scale", 0.0f, 0.02f);
    }

    void Scale()
    {
        if(mSize >= 100.0f)
        {
            CancelInvoke("Scale");
        }

        bookPages.SetBlendShapeWeight(0, mSize++);
        
    }

}
