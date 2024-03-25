using UnityEngine;

public class SeedManager : MonoBehaviour
{
    public static SeedManager Instance;

    public GameObject seedPrefab;
    public float seedThrowForce = 5f;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void ThrowSeed(Vector2 spawnPosition)
    {
        GameObject newSeed = Instantiate(seedPrefab, spawnPosition, Quaternion.identity);
        Rigidbody seedRigidbody = newSeed.GetComponent<Rigidbody>();
        Vector3 throwDirection = Vector3.up;
        seedRigidbody.AddForce(throwDirection * seedThrowForce, ForceMode.Impulse);
    }
}
