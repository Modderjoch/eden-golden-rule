using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class GardenSceneData : GameSceneData
{
    [Header("Additional Objects")]
    private GameObject paperProgress;
    private GameObject paperAnimation;

    private PopUpScript popUp;
    private

    private List<GameSceneAdditionalObject> additionalObjects;
    private AudioManager audioManager;
    private GameManager gameManager;

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

        paperProgress = additionalObjects[0].additionalObject;
        paperAnimation = additionalObjects[1].additionalObject;

        gameManager.Scenes[1].OnEnvironmentActivated += StartVoiceOver;
    }

    private void StartVoiceOver()
    {
        // First we de-activate the old objects

        // Then we unsubscribe from previous events
        gameManager.Scenes[1].OnEnvironmentActivated -= StartVoiceOver;

        // Then we activate new objects and call the needed methods
        audioManager.PlayVoiceOver("GardenScenePart1" + LocalizationSettings.SelectedLocale.Formatter);
        gameManager.QRScanningUI.SetActive(false);

        // Then we subscribe to new events
        audioManager.OnVoiceOverFinished += HelpVoiceOver;
    }

    private void HelpVoiceOver()
    {
        // First we de-activate the old objects

        // Then we unsubscribe from previous events
        audioManager.OnVoiceOverFinished -= HelpVoiceOver;

        // Then we activate new objects and call the needed methods
        audioManager.PlayVoiceOver("GardenScenePart2" + LocalizationSettings.SelectedLocale.Formatter);

        // Then we subscribe to new events
        audioManager.OnVoiceOverFinished += StartPaperCollecting;
    }

    private void StartPaperCollecting()
    {
        // First we de-activate the old objects

        // Then we unsubscribe from previous events
        audioManager.OnVoiceOverFinished -= StartPaperCollecting;

        // Then we activate new objects and call the needed methods
        paperProgress.SetActive(true);
        paperAnimation.SetActive(true);

        // Then we subscribe to new events

    }

    private void StartPaperCollectedVoiceOver()
    {
        // First we de-activate the old objects
        paperProgress.SetActive(false);
        paperAnimation.SetActive(false);

        // Then we unsubscribe from previous events


        // Then we activate new objects and call the needed methods

        // Then we subscribe to new events
    }

    private void StartContinueVoiceOver()
    {
        // First we de-activate the old objects

        // Then we unsubscribe from previous events

        // Then we activate new objects and call the needed methods

        // Then we subscribe to new events
    }

    private void StartMotherWolfVoiceOver()
    {
        // First we de-activate the old objects

        // Then we unsubscribe from previous events

        // Then we activate new objects and call the needed methods

        // Then we subscribe to new events
    }

    public override void OnSceneExit()
    {
        // First we de-activate the old objects

        // Then we unsubscribe from previous events

        // Then we activate new objects and call the needed methods
        popUp.PopUpEntry("Find mother Wolf", 4);
        gameManager.NextScene();
        Debug.Log("Finished scene");

        // Then we subscribe to new events
    }
}
