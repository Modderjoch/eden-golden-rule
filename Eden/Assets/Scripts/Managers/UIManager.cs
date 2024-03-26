using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manager that is responsible for updating the UI. 
/// For scores see <see cref="ScoreManager"/>
/// </summary>
public class UIManager : MonoBehaviour
{
    public List<GameObject> SceneUIs
    {
        get { return sceneUIs; }
        set { sceneUIs = value; }
    }

    [SerializeField] private Transform uiParent;

    private static UIManager instance;

    private List<GameObject> sceneUIs;
    private Dictionary<string, GameObject> spawnedUIs;

    public static UIManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UIManager>();

                if (instance == null)
                {
                    GameObject obj = new GameObject("UIManager");
                    instance = obj.AddComponent<UIManager>();
                }
            }

            return instance;
        }
    }

    protected void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    /// <summary>
    /// Method to spawn all spawnable prefabs, set their name and add them to
    /// a list to keep track.
    /// </summary>
    public void SetSpawnablePrefabs()
    {
        spawnedUIs = new Dictionary<string, GameObject>();

        foreach (GameObject prefab in sceneUIs)
        {
            GameObject newPrefab = Instantiate(prefab, uiParent);
            newPrefab.name = prefab.name;
            newPrefab.gameObject.name = prefab.name;
            spawnedUIs.Add(prefab.name, newPrefab);

            newPrefab.SetActive(false);
        }
    }

    public void ShowUI(List<GameScene> scenes, string sceneName)
    {
        foreach (GameScene scene in scenes)
        {
            foreach(var entry in spawnedUIs)
            {
                entry.Value.SetActive(false);

                if (entry.Key == sceneName)
                {
                    entry.Value.SetActive(true);
                }
            }
        }
    }

}
