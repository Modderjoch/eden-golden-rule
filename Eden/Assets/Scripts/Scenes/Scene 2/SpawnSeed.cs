using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSeed : MonoBehaviour
{
    [SerializeField] private GameObject spawnPrefab;
    [SerializeField] private float spawnDelay;
    [SerializeField] private RectTransform swipeArea;
    [SerializeField] private int numberOfSeeds = 10;
    [SerializeField] private List<GameObject> availableFlowers = new List<GameObject>();
    private List<GameObject> usedFlowers = new List<GameObject>();

    private bool isInstantiating = false;

    public event Action OnSeedsDepleted;
    public event Action OnSeedsChosen;

    public void SetFlowerPrefab(GameObject prefab)
    {
        if(prefab.name != "null")
        {
            usedFlowers.Add(prefab);
        }
        else
        {
            usedFlowers.AddRange(availableFlowers);
        }

        gameObject.GetComponentInChildren<SwipeScript>().SetFlowerPrefab(ReturnFlower());

        OnSeedsChosen.Invoke();
    }

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

            SwipeScript seedSwipe = seed.GetComponent<SwipeScript>();

            seedSwipe.swipeArea = swipeArea;
            seedSwipe.SetFlowerPrefab(ReturnFlower());
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

    private GameObject ReturnFlower()
    {
        int randomIndex = UnityEngine.Random.Range(0, usedFlowers.Count);

        return usedFlowers[randomIndex];
    }
}
