// Sourced from https://www.youtube.com/watch?v=7O9bAFyGvH8 and slightly modified

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeScript : MonoBehaviour
{
    Vector2 startPos, endPos; // Touch start position, touch end position, swipe direction
    Vector3 direction;
    float touchTimeStart, touchTimeFinish, timeInterval; // To calculate swipe time to sontrol throw force in Z direction

    [SerializeField] float throwForceX = 1f; // To control throw force in X and Y directions
    [SerializeField] float throwForceY = 1f;
    [SerializeField] float throwForceZ = 50f; // To control throw force in Z direction

    [SerializeField] float timeToDestroy = 4f; // Time before the thrown will be destroyed
    [SerializeField] float minSwipeDistance = 50f; // Minimum swipe distance threshold

    Rigidbody rb;

    private bool trackSwipe = true;

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

                if(swipeDistance >= minSwipeDistance)
                {
                    transform.SetParent(null);

                    trackSwipe = false;

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

                    Destroy(gameObject, timeToDestroy);
                }
            }
        }
    }
}
