// Sourced from https://www.youtube.com/watch?v=7O9bAFyGvH8 and slightly modified

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeScript : MonoBehaviour
{
    private const string GROUND_TAG = "Ground";

    [SerializeField] private GameObject flowerPrefab;

    [Header("Swiping")]
    [SerializeField] float minSwipeDistance = 50f;
    [HideInInspector] public RectTransform swipeArea;
    private Vector2 startPos; 
    private Vector2 endPos;
    private Vector3 direction;
    private bool trackSwipe = true;

    [Header("Timing")]
    private float touchTimeStart;
    private float touchTimeFinish;
    private float timeInterval;

    [Header("Throwing")]
    [SerializeField] float throwForceX = 1f;
    [SerializeField] float throwForceY = 1f;
    [SerializeField] float throwForceZ = 50f;
    [SerializeField] float timeToDestroy = 4f;

    public event Action OnSwipeDetected;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (trackSwipe)
        {
            Vector3 startpointSwipe = Vector3.zero;

            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                // Getting touch position and marking time when you touch the screen
                touchTimeStart = Time.time;
                startPos = Input.GetTouch(0).position;
            }

            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                // Getting release finger position
                endPos = Input.GetTouch(0).position;

                // Calculating swipe direction in 2D space
                direction = startPos - endPos;

                float swipeDistance = direction.magnitude;

                Vector2 swipeAreaLocalPos = (Vector2)swipeArea.position - new Vector2(swipeArea.rect.width * 0.5f, swipeArea.rect.height * 0.5f);
                Rect swipeBoundary = new Rect(swipeAreaLocalPos.x, swipeAreaLocalPos.y, swipeArea.rect.width, swipeArea.rect.height);


                if (swipeBoundary.Contains(startPos) && swipeDistance >= minSwipeDistance)
                {
                    HandleSwipe();
                }
            }
        }
    }

    private void HandleSwipe()
    {
        transform.SetParent(null);

        trackSwipe = false;

        if (OnSwipeDetected != null && OnSwipeDetected.GetInvocationList().Length > 0)
        {
            OnSwipeDetected.Invoke();
        }

        // Marking time when you release it
        touchTimeFinish = Time.time;

        // Calculate swipe time interval 
        timeInterval = touchTimeFinish - touchTimeStart;

        Vector2 xyForceDirection = new Vector2(-direction.x, -direction.y);

        Vector3 force = new Vector3(xyForceDirection.x * throwForceX, xyForceDirection.y * throwForceY, 0) +
                    Camera.main.transform.forward * (timeInterval * throwForceZ);

        // Add force to objects rigidbody in 3D space depending on swipe time, direction and throw forces
        rb.isKinematic = false;
        rb.AddRelativeForce(force);

        gameObject.transform.localScale = new Vector3(.01f, .01f, .01f);

        Destroy(gameObject, timeToDestroy);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag == GROUND_TAG)
        {
            Vector3 collisionPoint = collision.contacts[0].point;

            float rotationY = UnityEngine.Random.Range(0f, 360f);

            Quaternion rotation = Quaternion.Euler(0, rotationY, 0);

            GameObject flower = Instantiate(flowerPrefab, collisionPoint, rotation);

            flower.transform.SetParent(collision.transform.parent);

            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject, timeToDestroy);
        }
    }
}
