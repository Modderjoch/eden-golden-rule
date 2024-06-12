using UnityEngine;

public class GroundCollision : MonoBehaviour
{
    [SerializeField] private GameObject flowerPrefab;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Seed"))
        {
            SpawnFlower(collision.contacts[0].point);
            Destroy(collision.gameObject);
        }
    }

    private void SpawnFlower(Vector3 spawnPosition)
    {
        Instantiate(flowerPrefab, spawnPosition, Quaternion.identity);
    }
}
