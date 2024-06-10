using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateHeadTowardsCamera : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private Transform head;


    protected void FixedUpdate()
    {
        // Determine which direction to rotate towards
        Vector3 targetDirection = Camera.main.transform.position - head.position;

        // The step size is equal to speed times frame time.
        float singleStep = speed * Time.deltaTime;

        // Rotate the forward vector towards the target direction by one step
        Vector3 newDirection = Vector3.RotateTowards(head.forward, targetDirection, singleStep, 0.0f);

        // Draw a ray pointing at our target
        Debug.DrawRay(head.position, newDirection, Color.red);

        // Calculate a rotation a step closer to the target and apply rotation to this object
        head.rotation = Quaternion.LookRotation(newDirection);

        // Clamp the rotation to prevent full 360-degree rotation and excessive looking up/down
        float minHorizontalAngle = -140f; // minimum angle to rotate left from the initial forward direction
        float maxHorizontalAngle = 0f;  // maximum angle to rotate right from the initial forward direction
        float minVerticalAngle = 40f;   // minimum angle to look down
        float maxVerticalAngle = 100f;    // maximum angle to look up
        head.rotation = ClampRotation(head.rotation, minHorizontalAngle, maxHorizontalAngle, minVerticalAngle, maxVerticalAngle);
    }

    private Quaternion ClampRotation(Quaternion q, float minYaw, float maxYaw, float minPitch, float maxPitch)
    {
        Vector3 euler = q.eulerAngles;

        // Clamp the yaw (Y-axis rotation)
        float yaw = euler.y > 180 ? euler.y - 360 : euler.y; // Convert to [-180, 180] range
        yaw = Mathf.Clamp(yaw, minYaw, maxYaw); // Clamp the yaw
        euler.y = yaw;

        // Clamp the pitch (X-axis rotation)
        float pitch = euler.x > 180 ? euler.x - 360 : euler.x; // Convert to [-180, 180] range
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch); // Clamp the pitch
        euler.x = pitch;

        return Quaternion.Euler(euler);
    }

}
