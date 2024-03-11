using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SimpleUpDown : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 45f;

    protected void FixedUpdate()
    {
        float verticalMovement = Mathf.Sin(Time.time * 1f) / moveSpeed;
        float rotation = Time.time * rotationSpeed;

        Transform child = transform.GetChild(0).transform;

        child.localPosition = new Vector3(0, verticalMovement, 0);
        child.localRotation = Quaternion.Euler(0, rotation, 0);
    }
}
