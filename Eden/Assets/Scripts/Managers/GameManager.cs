// Copyright Oebe Rademaker All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class GameManager : MonoBehaviour
{
    public GameObject PopUp => popUp;
    public TrashProgress TrashProgress => trashProgress;
    public PaperProgress PaperProgress => paperProgress;
    public List<GameScene> Scenes => modifiableScenes;
    public GameObject QRScanningUI => qrScanningUI;
    public GameObject MainMenuUI => mainMenuUI;
    public List<GameSceneAdditionalObject> AdditionalObjects => additionalObjects;


    [Header("AR Foundation")]
    [SerializeField] private XRReferenceImageLibrary referenceLibrary;
    [SerializeField] private ImageTracking imageTracking;
    private ARTrackedImageManager trackedImageManager;

    [Header("Data")]
    [SerializeField] private List<Button> languages;
    [SerializeField] private List<GameSceneAdditionalObject> additionalObjects = new List<GameSceneAdditionalObject>();
    [SerializeField, Range(0, 4)] private int playerIndex = 0;
    [SerializeField] private int startSceneIndex = 0;
    [SerializeField] private TrashProgress trashProgress;
    [SerializeField] private PaperProgress paperProgress;

    [Header("Scenes")]
    [SerializeField] private List<GameScene> scenes;
    [SerializeField] private List<GameScene> modifiableScenes;

    [Header("UI")]
    [SerializeField] private GameObject popUp;
    [SerializeField] GameObject mainMenuUI;
    [SerializeField] GameObject qrScanningUI;
    [SerializeField] TMP_Dropdown playerIndexDropdown;
    [SerializeField] SetNextLocationImage setNextLocationImage; 

    private static GameManager instance;
    private UIManager uiManager;

    private GameSceneData activeSceneData;

    private bool isFirstStart = true;

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
        SetLanguage();

        trackedImageManager = FindObjectOfType<ARTrackedImageManager>();
        uiManager = UIManager.Instance;

        if (referenceLibrary == null)
        {
            referenceLibrary = trackedImageManager.referenceLibrary as XRReferenceImageLibrary;
        }

        CloneScenes();
        AddAdditionalObjects();

        Application.targetFrameRate = 60;
    }

#if UNITY_EDITOR
    protected void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("Setting scene");

            NextScene();
        }
    }
#endif

    /// <summary>
    /// Method to start the game from the Main Menu,
    /// involves removing the menu UI, setting all prefabs according
    /// to the player starting pillar (playerindex)
    /// </summary>
    public void StartGame()
    {
        SetPlayerIndex(playerIndexDropdown.value);

        mainMenuUI.SetActive(false);

        SetLanguage();

        if (isFirstStart)
        {
            SetSpawnablePrefabs();

            List<GameObject> newPrefabs = imageTracking.SetSpawnablePrefabs();

            for (int i = 0; i < modifiableScenes.Count; i++)
            {
                modifiableScenes[i].sceneEnvironmentPrefab = newPrefabs[i];
            }
            
            SetActiveScene(startSceneIndex);
        }


        isFirstStart = false;
    }

    public IEnumerator ResetGame()
    {
        trashProgress.ResetScore();
        paperProgress.ResetScore();

        CloneScenes();

        AddAdditionalObjects();

        imageTracking.RemoveEnvironments(true);

        yield return new WaitUntil(imageTracking.EnvironmentsAreRemoved);

        imageTracking.RemoveEnvironments(false);
        
        SetSpawnablePrefabs();

        List<GameObject> newPrefabs = imageTracking.SetSpawnablePrefabs();

        for (int i = 0; i < modifiableScenes.Count; i++)
        {
            modifiableScenes[i].sceneEnvironmentPrefab = newPrefabs[i];
        }

        NextScene();
        
        mainMenuUI.SetActive(true);
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

    /// <summary>
    /// Returns a list of <see cref="GameSceneAdditionalObject"/>
    /// </summary>
    /// <returns>A list of <see cref="GameSceneAdditionalObject"/> which hold the additional GameObjects</returns>
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

    /// <summary>
    /// Used for increasing the scene index and setting the next scene
    /// </summary>
    public void NextScene()
    {
        startSceneIndex++;

        if(startSceneIndex >= modifiableScenes.Count)
        {
            startSceneIndex = 0;
        }

        SetActiveScene(startSceneIndex);
    }

    /// <summary>
    /// Used to activate a scene, calling <see cref="GameSceneData.OnSceneEnter"/>
    /// </summary>
    /// <param name="index">The index of the scene to activate</param>
    public void SetActiveScene(int index)
    {
        foreach(GameScene scene in modifiableScenes)
        {
            if(scene.sceneIndex == index)
            {
                scene.sceneState.state = SceneState.State.Active;

                if(scene.sceneEnvironmentPrefab.GetComponent<GameSceneData>() != null)
                {
                    scene.sceneEnvironmentPrefab.GetComponent<GameSceneData>().OnSceneEnter();
                }
            }
            else
            {
                scene.sceneState.state = SceneState.State.Inactive;

                foreach (GameSceneAdditionalObject additionalObject in scene.additionalObjects)
                {
                    additionalObject.additionalObject.SetActive(false);
                }
            }
        }

        for (int i = 0; i < referenceLibrary.count; i++)
        {
            if(i == startSceneIndex + playerIndex)
            {
                Sprite sprite = Sprite.Create(referenceLibrary[i].texture, new Rect(0, 0, referenceLibrary[i].texture.width, referenceLibrary[i].texture.height), Vector2.zero);
                setNextLocationImage.SetNextImage(sprite);
            }

        }
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
                return false;
            }

            if (scenePrefab.name == scene.sceneEnvironmentPrefab.name)
            {
                scene.sceneState.state = SceneState.State.Active;

                scene.environmentState.state = EnvironmentState.State.Shown;

                scene.ActivateEnvironment();
            }
            else
            {
                if(scene.sceneState.state is SceneState.State.Active)
                {
                    scene.sceneState.state = SceneState.State.Inactive;

                    scene.environmentState.state = EnvironmentState.State.Hidden;

                    foreach (GameSceneAdditionalObject additionalObject in scene.additionalObjects)
                    {
                        additionalObject.additionalObject.SetActive(false);
                    }
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Used to set the language based on
    /// the inactive button in the main menu
    /// </summary>
    public void SetLanguage()
    {
        string newLanguageID = "";

        foreach(Button language in languages)
        {
            if(!language.enabled)
            {
                newLanguageID = language.name;
            }
        }

        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale(newLanguageID);
    }

    /// <summary>
    /// Used to rotate the environment towards the player
    /// </summary>
    /// <param name="transform">The environment to rotate</param>
    public void SetRotation(Transform transform)
    {
        Vector3 relativePos = transform.position - Camera.main.transform.position;
        relativePos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(relativePos);
        transform.rotation = rotation;
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

    public int GetSceneIndex(string sceneName)
    {
        foreach(GameScene scene in modifiableScenes)
        {
            if(scene.sceneName == sceneName)
            {
                return scene.sceneIndex;
            }
        }

        return -1;
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
