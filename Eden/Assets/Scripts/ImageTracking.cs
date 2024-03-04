using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARTrackedImageManager))]
public class ImageTracking : MonoBehaviour
{
    [SerializeField]
    private GameObject[] placeablePrefabs;

    private Dictionary<string, GameObject> spawnedPrefabs = new Dictionary<string, GameObject>();
    private ARTrackedImageManager trackedImageManager;

    protected void Awake()
    {
        trackedImageManager = FindObjectOfType<ARTrackedImageManager>();

        foreach(GameObject prefab in placeablePrefabs)
        {
            GameObject newPrefab = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            newPrefab.name = prefab.name;
            TestPanel testPanel = newPrefab.GetComponent<TestPanel>();

            testPanel.scaleSlider = InputManager.Instance.ScaleSlider;
            testPanel.offsetSliderX = InputManager.Instance.OffsetSliderX;
            testPanel.offsetSliderY = InputManager.Instance.OffsetSliderY;
            testPanel.offsetSliderZ = InputManager.Instance.OffsetSliderZ;
            testPanel.uiParent = InputManager.Instance.UIParent;
            testPanel.objectName = InputManager.Instance.ObjectName;

            spawnedPrefabs.Add(prefab.name, newPrefab);
        }
    }

    protected void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += ImageChanged;
    }

    protected void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= ImageChanged;
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

        if (name != null)
        {
            GameObject prefab = spawnedPrefabs[name];
            prefab.transform.position = position;
            prefab.SetActive(true);

            //foreach(GameObject go in spawnedPrefabs.Values)
            //{
            //    if(go.name != name)
            //    {
            //        go.SetActive(false);
            //    }
            //}
        }
        else
        {
            return;
        }
    }
}

