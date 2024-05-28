using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameSceneData : MonoBehaviour
{
    public abstract void OnSceneEnter();
    public abstract void OnSceneExit();
}
