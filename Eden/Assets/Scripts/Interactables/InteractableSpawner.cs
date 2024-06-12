using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class InteractableSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> prefabToSpawnList;
    [SerializeField] private int numberOfObjectsToSpawn = 3;

    [SerializeField] private Transform parentToSpawnTo;
    [SerializeField] private Transform transformToUse;

    public float minDistance = 10f;

    public float objectScale = 1f;

    private NavMeshSurface navMeshSurface;

    protected void Start()
    {
        navMeshSurface = GetComponent<NavMeshSurface>();

        if (navMeshSurface == null)
        {
            Debug.LogError("NavMeshSurface not found on the GameObject!");
            return;
        }

        // Build the NavMesh before instantiating objects
        navMeshSurface.BuildNavMesh();
        InstantiateObjects();
    }

    private void SpawnObjectsOnCubeSurface()
    {
        for (int i = 0; i < numberOfObjectsToSpawn; i++)
        {
            int randomPrefabIndex = Random.Range(0, prefabToSpawnList.Count);
            GameObject prefabToSpawn = prefabToSpawnList[randomPrefabIndex];

            Vector3 randomPosition = new Vector3(
                Random.Range(0, 1),
                Random.Range(0, 1),
                Random.Range(0, 1)
            );

            Quaternion randomRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

            GameObject instantiatedPrefab = Instantiate(prefabToSpawn, randomPosition, randomRotation, parentToSpawnTo);
            instantiatedPrefab.transform.localPosition = randomPosition;
            instantiatedPrefab.transform.localScale = instantiatedPrefab.transform.localScale * objectScale;
        }
    }

    private void InstantiateObjects()
    {
        for (int i = 0; i < numberOfObjectsToSpawn; i++)
        {
            int randomPrefabIndex = Random.Range(0, prefabToSpawnList.Count);
            GameObject prefabToSpawn = prefabToSpawnList[randomPrefabIndex];

            Vector3 randomPointOnNavMesh = GetRandomPointOnNavMesh();

            Vector3 higherPosition = new Vector3(randomPointOnNavMesh.x, randomPointOnNavMesh.y + .2f, randomPointOnNavMesh.z);

            GameObject instantiatedPrefab = Instantiate(prefabToSpawn, higherPosition, Quaternion.identity, parentToSpawnTo);

            instantiatedPrefab.transform.localScale = instantiatedPrefab.transform.localScale * objectScale;
        }
    }

    Vector3 GetRandomPointOnNavMesh()
    {
        NavMeshHit hit;
        Vector3 randomPoint = Vector3.zero;

        // Try to find a random point on the NavMesh within a certain radius
        for (int attempts = 0; attempts < 30; attempts++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * minDistance;
            randomPoint = new Vector3(transform.position.x + randomCircle.x, -.1f, transform.position.z + randomCircle.y);

            if (NavMesh.SamplePosition(randomPoint, out hit, minDistance, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }

        // Return the original position if no valid point is found
        return transform.position;
    }
}
