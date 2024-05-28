using System;
using UnityEngine;

public class SpawnSeed : MonoBehaviour
{
    [SerializeField] private GameObject spawnPrefab;
    [SerializeField] private float spawnDelay;
    [SerializeField] private RectTransform swipeArea;
    [SerializeField] private int numberOfSeeds = 10;

    private bool isInstantiating = false;

    public event Action OnSeedsDepleted;

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
            seed.GetComponent<SwipeScript>().swipeArea = swipeArea;
        }

        numberOfSeeds--;

        if (numberOfSeeds <= 0)
        {
            if (OnSeedsDepleted != null)
            {
                OnSeedsDepleted.Invoke();
            }
        }

        isInstantiating = false;
    }
}
