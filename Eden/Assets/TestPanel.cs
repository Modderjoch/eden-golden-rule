using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestPanel : MonoBehaviour
{
    public Slider scaleSlider;
    public Slider offsetSliderX;
    public Slider offsetSliderY;
    public Slider offsetSliderZ;
    public GameObject uiParent;
    public Text objectName;

    private bool showUI = false;

    private BoxCollider boxCollider;

    protected void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    public void ToggleUI()
    {
        objectName.text = name;

        scaleSlider.onValueChanged.RemoveAllListeners();
        offsetSliderX.onValueChanged.RemoveAllListeners();
        offsetSliderY.onValueChanged.RemoveAllListeners();
        offsetSliderZ.onValueChanged.RemoveAllListeners();
        
        scaleSlider.onValueChanged.AddListener(SetScale);
        offsetSliderX.onValueChanged.AddListener(SetOffsetX);
        offsetSliderY.onValueChanged.AddListener(SetOffsetY);
        offsetSliderZ.onValueChanged.AddListener(SetOffsetZ);
        
        offsetSliderX.value = 0;
        offsetSliderY.value = 0;
        offsetSliderZ.value = 0;
        
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

    public void SetOffsetX(float offset)
    {
        Transform childTransform = transform.GetChild(0).transform;
        Vector3 newPosition = new Vector3(offset, childTransform.localPosition.y, childTransform.localPosition.z);
        childTransform.localPosition = newPosition;
    }

    public void SetOffsetY(float offset)
    {
        Transform childTransform = transform.GetChild(0).transform;
        Vector3 newPosition = new Vector3(childTransform.localPosition.x, offset, childTransform.localPosition.z);
        childTransform.localPosition = newPosition;
    }

    public void SetOffsetZ(float offset)
    {
        Transform childTransform = transform.GetChild(0).transform;
        Vector3 newPosition = new Vector3(childTransform.localPosition.x, childTransform.localPosition.y, offset);
        childTransform.localPosition = newPosition;
    }

}
