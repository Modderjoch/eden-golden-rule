using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject mainMenuUI;
    [SerializeField] TMP_Dropdown playerIndexDropdown;

    [SerializeField] private XRReferenceImageLibrary referenceLibrary;
    [SerializeField] private ImageTracking imageTracking;
    private ARTrackedImageManager trackedImageManager;

    [SerializeField] private List<GameScene> scenes;
    [SerializeField] private List<GameScene> modifiableScenes;

    [SerializeField, Range(0, 4)] private int playerIndex = 0;

    private static GameManager instance;
    private UIManager uiManager;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();

                if (instance == null)
                {
                    GameObject obj = new GameObject("GameManager");
                    instance = obj.AddComponent<GameManager>();
                }
            }

            return instance;
        }
    }

    protected void Awake()
    {
        trackedImageManager = FindObjectOfType<ARTrackedImageManager>();
        uiManager = UIManager.Instance;

        if (referenceLibrary == null)
        {
            referenceLibrary = trackedImageManager.referenceLibrary as XRReferenceImageLibrary;
        }

        CloneScenes();
    }

    /// <summary>
    /// Method to start the game from the Main Menu,
    /// involves removing the menu UI, setting all prefabs according
    /// to the player starting pillar (playerindex)
    /// </summary>
    public void StartGame()
    {
        SetPlayerIndex(playerIndexDropdown.value);

        Destroy(mainMenuUI);

        SetSpawnablePrefabs();

        imageTracking.SetSpawnablePrefabs();
        uiManager.SetSpawnablePrefabs();
    }

    public void SetActiveScene(GameObject scenePrefab)
    {
        foreach(GameScene scene in modifiableScenes)
        {
            SceneState.State state = scene.sceneState.state;

            if (scenePrefab.name == scene.sceneEnvironmentPrefab.name)
            {
                if(scene.sceneState.state == SceneState.State.Active)
                {
                    break;
                }

                scene.sceneState.state = SceneState.State.Active;
            }
            else
            {
                if(scene.sceneState.state is SceneState.State.Active)
                {
                    scene.sceneState.state = SceneState.State.Inactive;
                }
            }
        }

        uiManager.ShowUI(modifiableScenes, scenePrefab.name);
    }

    private void SetPlayerIndex(int index)
    {
        playerIndex = index;
    }

    private void SetSpawnablePrefabs()
    {
        imageTracking.PlaceablePrefabs = ReturnPrefabs();
        uiManager.SceneUIs = ReturnPrefabsUI();
    }

    private List<GameObject> ReturnPrefabs()
    {
        List<GameObject> prefabs = new List<GameObject>();

        for (int i = 0; i < modifiableScenes.Count; i++)
        {
            prefabs.Add(modifiableScenes[i].sceneEnvironmentPrefab);
            modifiableScenes[i].sceneEnvironmentPrefab.name = GetSceneName(playerIndex, i);
        }

        return prefabs;
    }

    private List<GameObject> ReturnPrefabsUI()
    {
        List<GameObject> prefabs = new List<GameObject>();

        for (int i = 0; i < modifiableScenes.Count; i++)
        {
            prefabs.Add(modifiableScenes[i].sceneUIPrefab);
            modifiableScenes[i].sceneUIPrefab.name = GetSceneName(playerIndex, i);
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

    private GameObject GetSceneUI(GameObject scenePrefab)
    {
        foreach(GameScene scene in modifiableScenes)
        {
            if(scene.sceneEnvironmentPrefab == scenePrefab)
            {
                return scene.sceneUIPrefab;
            }
        }

        return null;
    }

    private void CloneScenes()
    {
        modifiableScenes = new List<GameScene>();

        foreach(GameScene scene in scenes)
        {
            modifiableScenes.Add(scene.Clone());
        }
    }
}