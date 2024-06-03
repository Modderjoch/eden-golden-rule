using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToMiddle : MonoBehaviour
{
    public bool MiddleMove => moveToMiddle;

    private bool moveToMiddle = false;
    private RectTransform rectTransform;
    private Vector2 targetPosition = Vector2.zero;
    public float speed = 5f;

    [SerializeField] 

    protected void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    protected void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha8))
        {
            StartMoveToMiddle();
        }

        if (moveToMiddle)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, targetPosition, speed * Time.deltaTime);

            if (Vector2.Distance(rectTransform.anchoredPosition, targetPosition) < 0.1f)
            {
                rectTransform.anchoredPosition = targetPosition;
                moveToMiddle = false;
            }
        }
    }

    public void StartMoveToMiddle()
    {
        Debug.Log("Moving to middle");

        moveToMiddle = true;
    }
}
