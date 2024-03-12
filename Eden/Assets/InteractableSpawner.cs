using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> prefabToSpawnList;
    [SerializeField] private int numberOfObjectsToSpawn = 3;

    [SerializeField] private Transform parentToSpawnTo;

    protected void Start()
    {
        SpawnObjectsOnCubeSurface();
    }

    private void SpawnObjectsOnCubeSurface()
    {
        for (int i = 0; i < numberOfObjectsToSpawn; i++)
        {
            int randomPrefabIndex = Random.Range(0, prefabToSpawnList.Count);
            GameObject prefabToSpawn = prefabToSpawnList[randomPrefabIndex];

                Vector3 randomPosition = new Vector3(
                Random.Range(transform.position.x - transform.localScale.x / 2, transform.position.x + transform.localScale.x / 2),
                Random.Range(transform.position.y + 0.025f, transform.position.y + 0.025f),
                Random.Range(transform.position.z - transform.localScale.z / 2, transform.position.z + transform.localScale.z / 2)
            );

            Quaternion randomRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

            Instantiate(prefabToSpawn, randomPosition, randomRotation, parentToSpawnTo);
        }
    }
}
