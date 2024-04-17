using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestSceneData : GameSceneData
{
    [Header("Additional Objects")]
    private GameObject trashProgress;
    private GameObject swipeArea;
    private GameObject seedSpawnpoint;
    private GameObject popUp;

    private TrashProgress trashProgressScript;

    private List<GameSceneAdditionalObject> additionalObjects;
    private AudioManager audioManager;
    private GameManager gameManager;

    public override void OnSceneEnter()
    {
        audioManager = AudioManager.Instance;
        gameManager = GameManager.Instance;
        additionalObjects = gameManager.GetAddditionalObjects();

        trashProgress = additionalObjects[0].additionalObject;
        swipeArea = additionalObjects[1].additionalObject;
        seedSpawnpoint = additionalObjects[2].additionalObject;
        popUp = additionalObjects[3].additionalObject;

        trashProgressScript = trashProgress.GetComponent<TrashProgress>();

        foreach(GameSceneAdditionalObject additionalObject in additionalObjects)
        {
            additionalObject.additionalObject.gameObject.SetActive(false);
        }

        audioManager.PlayVoiceOver("ForestScenePart1" + gameManager.Language.currentLanguage);
        audioManager.OnVoiceOverFinished += StartTrashPicking;
    }

    private void StartTrashPicking()
    {
        // First we de-activate the old objects

        // Then we unsubscribe from previous events
        audioManager.OnVoiceOverFinished -= StartTrashPicking;

        // Then we activate new objects and call the needed methods
        trashProgress.SetActive(true);

        // Then we subscribe to new events
        trashProgressScript.OnScoreReached += StartSeedVoiceOver;
    }

    private void StartSeedVoiceOver()
    {        
        // First we de-activate the old objects
        trashProgress.SetActive(false);
        
        // Then we unsubscribe from previous events
        trashProgressScript.OnScoreReached -=StartSeedVoiceOver;
        
        // Then we activate new objects and call the needed methods
        audioManager.PlayVoiceOver("ForestScenePart2" + gameManager.Language.currentLanguage);

        // Then we subscribe to new events
        audioManager.OnVoiceOverFinished += StartSeedThrowing;
    }

    private void StartSeedThrowing()
    {
        // First we de-activate the old objects

        // Then we unsubscribe from previous events
        audioManager.OnVoiceOverFinished -= StartSeedThrowing;

        // Then we activate new objects and call the needed methods
        swipeArea.SetActive(true);
        seedSpawnpoint.SetActive(true);

        // Then we subscribe to new events
        seedSpawnpoint.GetComponent<SpawnSeed>().OnSeedsDepleted += EndScene;
    }

    private void EndScene()
    {
        // First we de-activate the old objects
        swipeArea.SetActive(false);
        seedSpawnpoint.SetActive(false);

        // Then we unsubscribe from previous events
        seedSpawnpoint.GetComponent<SpawnSeed>().OnSeedsDepleted -= EndScene;

        // Then we activate new objects and call the needed methods
        audioManager.PlayVoiceOver("ForestScenePart3" + gameManager.Language.currentLanguage);

        // Then we subscribe to new events
        audioManager.OnVoiceOverFinished += OnSceneExit;
    }

    public override void OnSceneExit()
    {
        // First we de-activate the old objects

        // Then we unsubscribe from previous events
        audioManager.OnVoiceOverFinished -= OnSceneExit;

        // Then we activate new objects and call the needed methods
        popUp.SetActive(true);
        gameManager.NextScene();
        Debug.Log("Finished scene");

        // Then we subscribe to new events
    }
}
