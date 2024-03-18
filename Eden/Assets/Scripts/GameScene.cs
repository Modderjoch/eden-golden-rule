using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "New Scene", menuName = "Eden/Scene", order = 1)]
public class GameScene : ScriptableObject
{
    public string sceneName;
    public int sceneIndex;

    public GameObject sceneUIPrefab;
    public GameObject sceneEnvironmentPrefab;
    public SceneState sceneState;
}

[Serializable]
public class SceneState
{
    public State state;

    public enum State
    {
        Active,
        Inactive,
        Hidden
    }
}
