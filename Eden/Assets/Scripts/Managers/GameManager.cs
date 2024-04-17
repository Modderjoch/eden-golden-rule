using System;
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

    [SerializeField] private List<Button> languages;

    [SerializeField] private List<GameScene> scenes;
    [SerializeField] private List<GameScene> modifiableScenes;

    public TrashProgress trashProgress;

    public List<GameSceneAdditionalObject> additionalObjects = new List<GameSceneAdditionalObject>();

    [SerializeField, Range(0, 4)] private int playerIndex = 0;

    public Language Language => language;

    private static GameManager instance;
    private UIManager uiManager;
    private Language language;

    [SerializeField] private int startSceneIndex = 0;

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

    protected void Start()
    {
        trackedImageManager = FindObjectOfType<ARTrackedImageManager>();
        uiManager = UIManager.Instance;

        if (referenceLibrary == null)
        {
            referenceLibrary = trackedImageManager.referenceLibrary as XRReferenceImageLibrary;
        }

        CloneScenes();
        AddAdditionalObjects();
    }

    protected void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("Setting scene");

            SetActiveScene(startSceneIndex);
        }
    }

    /// <summary>
    /// Method to start the game from the Main Menu,
    /// involves removing the menu UI, setting all prefabs according
    /// to the player starting pillar (playerindex)
    /// </summary>
    public void StartGame()
    {
        SetPlayerIndex(playerIndexDropdown.value);

        SetLanguage();

        Destroy(mainMenuUI);

        SetSpawnablePrefabs();

        SetActiveScene(startSceneIndex);

        imageTracking.SetSpawnablePrefabs();
        //uiManager.SetSpawnablePrefabs();
    }

    /// <summary>
    /// Method to get the currently active scene.
    /// </summary>
    /// <returns>The currently active <see cref="GameScene"/></returns>
    public GameScene GetActiveScene()
    {
        foreach(GameScene scene in modifiableScenes)
        {
            if(scene.sceneState.state == SceneState.State.Active)
            {
                return scene;
            }
        }

        return null;
    }

    public List<GameSceneAdditionalObject> GetAddditionalObjects()
    {
        foreach (GameScene scene in modifiableScenes)
        {
            if (scene.sceneState.state == SceneState.State.Active)
            {
                return scene.additionalObjects;
            }
        }

        return null;
    }

    public void NextScene()
    {
        startSceneIndex++;

        SetActiveScene(startSceneIndex);
    }

    public void SetActiveScene(int index)
    {
        foreach(GameScene scene in modifiableScenes)
        {
            if(scene.sceneIndex == index)
            {
                scene.sceneState.state = SceneState.State.Active;

                foreach (GameSceneAdditionalObject additionalObject in scene.additionalObjects)
                {
                    additionalObject.additionalObject.SetActive(true);
                }

                if(scene.sceneEnvironmentPrefab.GetComponent<GameSceneData>() != null)
                {
                    scene.sceneEnvironmentPrefab.GetComponent<GameSceneData>().OnSceneEnter();
                }

                //Debug.Log("Set scene " + scene.name + " to active");
            }
            else
            {
                scene.sceneState.state = SceneState.State.Inactive;

                foreach (GameSceneAdditionalObject additionalObject in scene.additionalObjects)
                {
                    additionalObject.additionalObject.SetActive(false);
                }

                //Debug.Log("Set scene " + scene.name + " to inactive");
            }
        }

        this.startSceneIndex++;
    }

    /// <summary>
    /// Sets the passed-in prefab to the active scene
    /// </summary>
    /// <param name="scenePrefab">The scene prefab that should be set active</param>
    public bool SetActiveScene(GameObject scenePrefab)
    {
        foreach(GameScene scene in modifiableScenes)
        {
            SceneState.State state = scene.sceneState.state;

            if(GetActiveScene().sceneEnvironmentPrefab.name != scenePrefab.name)
            {
                Debug.Log("Not the active scene");

                return false;
            }

            if (scenePrefab.name == scene.sceneEnvironmentPrefab.name)
            {
                if(scene.sceneState.state == SceneState.State.Active)
                {
                    break;
                }

                scene.sceneState.state = SceneState.State.Active;

                foreach(GameSceneAdditionalObject additionalObject in scene.additionalObjects)
                {
                    additionalObject.additionalObject.SetActive(true);
                }
            }
            else
            {
                if(scene.sceneState.state is SceneState.State.Active)
                {
                    scene.sceneState.state = SceneState.State.Inactive;

                    foreach (GameSceneAdditionalObject additionalObject in scene.additionalObjects)
                    {
                        additionalObject.additionalObject.SetActive(false);
                    }
                }
            }
        }

        return true;
    }

    private void SetLanguage()
    {
        Language newLanguage = null;

        foreach(Button language in languages)
        {
            if(!language.enabled)
            {
                newLanguage = new Language(language.name);
            }
        }

        language = newLanguage;
        Debug.Log(language.currentLanguage);
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

        //Debug.Log(sceneName);

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

    private void AddAdditionalObjects()
    {
        foreach(GameScene scene in modifiableScenes)
        {
            scene.additionalObjects = new List<GameSceneAdditionalObject>();

            foreach(GameSceneAdditionalObject additionalObject in additionalObjects)
            {
                if(scene.sceneIndex == additionalObject.sceneIndex)
                {
                    scene.additionalObjects.Add(additionalObject);
                }
            }
        }
    }
}
