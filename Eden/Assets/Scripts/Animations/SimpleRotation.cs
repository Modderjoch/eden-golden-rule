using UnityEngine;

public class SimpleRotation : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 45f;

    protected void FixedUpdate()
    {
        float rotation = Time.time * rotationSpeed;

        Transform child = transform.GetChild(0).transform;

        child.localRotation = Quaternion.Euler(0, rotation, 0);
    }
}
