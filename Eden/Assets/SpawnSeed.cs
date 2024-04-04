using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSeed : MonoBehaviour
{
    [SerializeField] private GameObject spawnPrefab;
    [SerializeField] private float spawnDelay;

    [SerializeField] private RectTransform swipeArea;

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
            GameObject seed = Instantiate(spawnPrefab, transform.position, transform.rotation, transform);
            seed.GetComponent<SwipeScript>().swipeArea = this.swipeArea;
        }

        isInstantiating = false;
    }
}
