using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Simple class that is dependent on assigned variables, 
/// allows the user to scale and offset the object.
/// </summary>
public class TestPanel : MonoBehaviour
{
    public Slider scaleSlider;
    public Slider offsetSlider;
    public GameObject uiParent;
    public Text objectName;

    private BoxCollider boxCollider;

    protected void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    public void ToggleUI()
    {
        objectName.text = name;

        scaleSlider.onValueChanged.RemoveAllListeners();
        offsetSlider.onValueChanged.RemoveAllListeners();
        
        scaleSlider.onValueChanged.AddListener(SetScale);
        offsetSlider.onValueChanged.AddListener(SetOffsetY);
        
        offsetSlider.value = 0;
        
        if(scaleSlider.value != transform.localScale.x)
        {
            scaleSlider.value = transform.localScale.x;
        }
    }

    public void SetScale(float scale)
    {
        transform.localScale = new Vector3(scale, scale, scale);
        boxCollider.transform.localScale = new Vector3(scale, scale, scale);
    }

    public void SetOffsetY(float offset)
    {
        Transform childTransform = transform.GetChild(0).transform;
        Vector3 newPosition = new Vector3(childTransform.localPosition.x, offset, childTransform.localPosition.z);
        childTransform.localPosition = newPosition;
    }
}
