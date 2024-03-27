using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSeed : MonoBehaviour
{
    [SerializeField] private GameObject spawnPrefab;
    [SerializeField] private float spawnDelay;

    private bool isInstantiating = false;

    private void OnTransformChildrenChanged()
    {
        if (!isInstantiating)
        {
            isInstantiating = true;
            Invoke("InstantiateObject", spawnDelay);
        }
    }

    private void InstantiateObject()
    {
        if (spawnPrefab != null)
        {
            Instantiate(spawnPrefab, transform.position, transform.rotation, transform);
        }
        isInstantiating = false;
    }
}
