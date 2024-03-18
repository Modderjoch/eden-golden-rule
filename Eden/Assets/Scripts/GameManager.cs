using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class GameManager : MonoBehaviour
{
    [SerializeField] private XRReferenceImageLibrary referenceLibrary;
    [SerializeField] private ImageTracking imageTracking;
    private ARTrackedImageManager trackedImageManager;

    [SerializeField] private List<GameScene> scenes;
    private Dictionary<XRReferenceImage, GameScene> scenesDictionary = new Dictionary<XRReferenceImage, GameScene>();

    [SerializeField, Range(0, 4)] private int playerIndex = 0;

    protected void Awake()
    {
        trackedImageManager = FindObjectOfType<ARTrackedImageManager>();

        if (referenceLibrary == null)
        {
            referenceLibrary = trackedImageManager.referenceLibrary as XRReferenceImageLibrary;
        }        
    }

    private void SetSpawnablePrefabs()
    {
        imageTracking.PlaceablePrefabs = ReturnPrefabs();
    }

    private List<GameObject> ReturnPrefabs()
    {
        List<GameObject> prefabs = new List<GameObject>();

        for (int i = 0; i < scenes.Count; i++)
        {
            prefabs.Add(scenes[i].sceneEnvironmentPrefab);
            scenes[i].sceneEnvironmentPrefab.name = GetSceneName(playerIndex, i);
        }

        return prefabs;
    }

    private string GetSceneName(int playerIndex, int sceneIndex)
    {
        int nextSceneNumber = (sceneIndex + playerIndex) % 5 + 1;
        string sceneName = "scene" + nextSceneNumber;

        Debug.Log(sceneName);

        return sceneName;
    }
}
