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

    public GameScene Clone()
    {
        GameScene clone = ScriptableObject.CreateInstance<GameScene>();
        clone.name = sceneName;
        clone.sceneName = this.sceneName;
        clone.sceneIndex = this.sceneIndex;
        clone.sceneUIPrefab = this.sceneUIPrefab;
        clone.sceneEnvironmentPrefab = this.sceneEnvironmentPrefab;
        clone.sceneState = new SceneState { state = this.sceneState.state };
        return clone;
    }
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
