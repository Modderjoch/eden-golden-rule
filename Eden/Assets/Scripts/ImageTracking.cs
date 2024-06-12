// Copyright Oebe Rademaker All rights reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARTrackedImageManager))]
public class ImageTracking : MonoBehaviour
{
    public Slider scaleSlider;
    public GameObject uiParent;
    public Text objectName;
    public Transform sceneParent;

    public List<GameObject> PlaceablePrefabs
    {
        get { return placeablePrefabs; }
        set { placeablePrefabs = value; }
    }

    private List<GameObject> placeablePrefabs;
    private Dictionary<string, GameObject> spawnedPrefabs;
    [SerializeField] private ARTrackedImageManager trackedImageManager;

    private bool removeEnvironments = false;

    protected void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += ImageChanged;
    }

    protected void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= ImageChanged;
    }

    protected void Update()
    {
        if (removeEnvironments)
        {
            while (sceneParent.childCount > 0)
            {
                DestroyImmediate(sceneParent.GetChild(0).gameObject);
            }

            removeEnvironments = false;
        }
    }

    public bool EnvironmentsAreRemoved()
    {
        if(sceneParent.childCount == 0)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Method to spawn all spawnable prefabs, set their name and add them to
    /// a list to keep track.
    /// </summary>
    public List<GameObject> SetSpawnablePrefabs()
    {
        spawnedPrefabs = new Dictionary<string, GameObject>();
        List<GameObject> result = new List<GameObject>();

        foreach (GameObject prefab in placeablePrefabs)
        {
            GameObject newPrefab = Instantiate(prefab, Vector3.zero, prefab.transform.rotation, sceneParent);
            newPrefab.name = prefab.name;
            spawnedPrefabs.Add(prefab.name, newPrefab);
            result.Add(newPrefab);

            newPrefab.SetActive(false);
        }

        return result;
    }

    public void RemoveEnvironments(bool value)
    {
        removeEnvironments = value;
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

        if (removeEnvironments)
        {
            return;
        }

        if (name != null && spawnedPrefabs != null)
        {
            GameObject prefab = spawnedPrefabs[name];
            
            if(prefab != null)
            {
                if (!GameManager.Instance.SetActiveScene(prefab))
                {
                    return;
                }
            }
            else
            {
                return;
            }            
            
            prefab.transform.position = position;
            
            //prefab.transform.rotation = trackedImage.transform.rotation;

            if(prefab.GetComponent<TestPanel>() != null)
            {
                TestPanel testPanel = prefab.GetComponent<TestPanel>();
                testPanel.scaleSlider = scaleSlider;
                testPanel.objectName = objectName;

                testPanel.ToggleUI();

                uiParent.SetActive(true);
            }
            else
            {
                uiParent.SetActive(false);
            }        

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

