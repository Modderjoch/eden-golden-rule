using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSeed : MonoBehaviour
{
    [SerializeField] private GameObject spawnPrefab;

    private void OnTransformChildrenChanged()
    {
        if (spawnPrefab != null)
        {
            Instantiate(spawnPrefab, transform);
        }
    }
}
