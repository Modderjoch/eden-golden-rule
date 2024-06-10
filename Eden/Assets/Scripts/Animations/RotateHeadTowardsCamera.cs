// Copyright Oebe Rademaker All rights reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateHeadTowardsCamera : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private Transform head;


    protected void FixedUpdate()
    {
        Vector3 targetDirection = Camera.main.transform.position - head.position;

        float singleStep = speed * Time.deltaTime;

        Vector3 newDirection = Vector3.RotateTowards(head.forward, targetDirection, singleStep, 0.0f);

        Debug.DrawRay(head.position, newDirection, Color.red);

        head.rotation = Quaternion.LookRotation(newDirection);

        float minHorizontalAngle = -140f;
        float maxHorizontalAngle = 0f; 
        float minVerticalAngle = 40f; 
        float maxVerticalAngle = 100f;
        head.rotation = ClampRotation(head.rotation, minHorizontalAngle, maxHorizontalAngle, minVerticalAngle, maxVerticalAngle);
    }

    private Quaternion ClampRotation(Quaternion q, float minYaw, float maxYaw, float minPitch, float maxPitch)
    {
        Vector3 euler = q.eulerAngles;

        float yaw = euler.y > 180 ? euler.y - 360 : euler.y;
        yaw = Mathf.Clamp(yaw, minYaw, maxYaw);
        euler.y = yaw;

        float pitch = euler.x > 180 ? euler.x - 360 : euler.x;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        euler.x = pitch;

        return Quaternion.Euler(euler);
    }
}
