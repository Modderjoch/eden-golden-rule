using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class ForestSceneData : GameSceneData
{
    [Header("Additional Objects")]
    private GameObject trashProgress;
    private GameObject swipeArea;
    private GameObject seedSpawnpoint;
    private GameObject swipeAnimation;

    [SerializeField] private List<GameObject> particleSystems;

    private TrashProgress trashProgressScript;
    private PopUpScript popUp;
    private SwipeScript swipeScript;

    private List<GameSceneAdditionalObject> additionalObjects;
    private AudioManager audioManager;
    private GameManager gameManager;

    private bool isSwipeDetected = false;

    public event Action OnTrashPicked;

    protected void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            AudioManager.Instance.StopAllVoiceOvers();
        }
    }

    public override void OnSceneEnter()
    {
        audioManager = AudioManager.Instance;
        gameManager = GameManager.Instance;
        additionalObjects = gameManager.GetAddditionalObjects();

        trashProgress = additionalObjects[0].additionalObject;
        swipeArea = additionalObjects[1].additionalObject;
        seedSpawnpoint = additionalObjects[2].additionalObject;
        swipeAnimation = additionalObjects[3].additionalObject;

        trashProgressScript = trashProgress.GetComponent<TrashProgress>();
        popUp = gameManager.PopUp.GetComponent<PopUpScript>();
        swipeScript = seedSpawnpoint.GetComponentInChildren<SwipeScript>();

        foreach (GameObject particleSystem in particleSystems)
        {
            particleSystem.SetActive(true);
        }

        gameManager.Scenes[2].OnEnvironmentActivated += StartVoiceOver;
    }

    private void StartVoiceOver()
    {
        // First we de-activate the old objects

        // Then we unsubscribe from previous events
        gameManager.Scenes[2].OnEnvironmentActivated -= StartVoiceOver;

        // Then we activate new objects and call the needed methods
        audioManager.PlayVoiceOver("ForestScenePart1" + LocalizationSettings.SelectedLocale.Formatter);

        // Then we subscribe to new events
        audioManager.OnVoiceOverFinished += StartTrashPicking;
    }

    private void StartTrashPicking()
    {
        // First we de-activate the old objects

        // Then we unsubscribe from previous events
        audioManager.OnVoiceOverFinished -= StartTrashPicking;

        // Then we activate new objects and call the needed methods
        trashProgress.SetActive(true);
        popUp.PopUpEntry(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("TrashCollection").Result, 3);

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
        audioManager.PlayVoiceOver("ForestScenePart2" + LocalizationSettings.SelectedLocale.Formatter);

        // Then we subscribe to new events
        audioManager.OnVoiceOverFinished += StartSeedThrowing;
    }

    private void StartSeedThrowing()
    {
        // First we de-activate the old objects

        // Then we unsubscribe from previous events
        audioManager.OnVoiceOverFinished -= StartSeedThrowing;

        // Then we activate new objects and call the needed methods
        swipeAnimation.SetActive(true);
        swipeArea.SetActive(true);
        seedSpawnpoint.SetActive(true);
        popUp.PopUpEntry(LocalizationSettings.StringDatabase.GetLocalizedString("SeedPlanting"), 5);

        // Then we subscribe to new events
        seedSpawnpoint.GetComponent<SpawnSeed>().OnSeedsDepleted += EndScene;
        swipeScript.OnSwipeDetected += DisableSwipeAnimation;
    }

    private void EndScene()
    {
        // First we de-activate the old objects
        swipeArea.SetActive(false);
        seedSpawnpoint.SetActive(false);
        swipeAnimation.SetActive(false);

        // Then we unsubscribe from previous events
        seedSpawnpoint.GetComponent<SpawnSeed>().OnSeedsDepleted -= EndScene;

        // Then we activate new objects and call the needed methods
        audioManager.PlayVoiceOver("ForestScenePart3" + LocalizationSettings.SelectedLocale.Formatter);

        // Then we subscribe to new events
        audioManager.OnVoiceOverFinished += OnSceneExit;
    }

    public override void OnSceneExit()
    {
        // First we de-activate the old objects

        // Then we unsubscribe from previous events
        audioManager.OnVoiceOverFinished -= OnSceneExit;

        // Then we activate new objects and call the needed methods
        popUp.PopUpEntry("Well done!", 3);
        gameManager.NextScene();
        Debug.Log("Finished scene");

        // Then we subscribe to new events
    }

    private void DisableSwipeAnimation()
    {
        swipeAnimation.SetActive(false);
        swipeScript.OnSwipeDetected -= DisableSwipeAnimation;
    }
}
