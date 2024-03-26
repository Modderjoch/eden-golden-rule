using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARTrackedImageManager))]
public class ImageTracking : MonoBehaviour
{
    public List<GameObject> PlaceablePrefabs
    {
        get { return placeablePrefabs; }
        set { placeablePrefabs = value; }
    }

    private List<GameObject> placeablePrefabs;
    private Dictionary<string, GameObject> spawnedPrefabs;
    [SerializeField] private ARTrackedImageManager trackedImageManager;


    protected void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += ImageChanged;
    }

    protected void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= ImageChanged;
    }

    /// <summary>
    /// Method to spawn all spawnable prefabs, set their name and add them to
    /// a list to keep track.
    /// </summary>
    public void SetSpawnablePrefabs()
    {
        spawnedPrefabs = new Dictionary<string, GameObject>();

        foreach (GameObject prefab in placeablePrefabs)
        {
            GameObject newPrefab = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            newPrefab.name = prefab.name;
            newPrefab.gameObject.name = prefab.name;
            spawnedPrefabs.Add(prefab.name, newPrefab);

            newPrefab.SetActive(false);
        }
    }

    private void ImageChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach(ARTrackedImage trackedImage in eventArgs.added)
        {
            UpdateImage(trackedImage);
        }

        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            UpdateImage(trackedImage);
        }

        foreach (ARTrackedImage trackedImage in eventArgs.removed)
        {
            spawnedPrefabs[trackedImage.name].SetActive(false);
        }
    }

    private void UpdateImage(ARTrackedImage trackedImage)
    {
        string name = trackedImage.referenceImage.name;
        Vector3 position = trackedImage.transform.position;

        if (name != null && spawnedPrefabs != null)
        {
            GameObject prefab = spawnedPrefabs[name];
            GameManager.Instance.SetActiveScene(prefab);
            prefab.transform.position = position;
            prefab.transform.rotation = trackedImage.transform.rotation;
            prefab.SetActive(true);

            foreach (GameObject go in spawnedPrefabs.Values)
            {
                if (go.name != name)
                {
                    go.SetActive(false);
                }
            }
        }
        else
        {
            return;
        }
    }
}

