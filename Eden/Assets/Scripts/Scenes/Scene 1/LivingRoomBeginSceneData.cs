using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LivingRoomBeginSceneData : GameSceneData
{
    [SerializeField] private Compass compass;

    private PopUpScript popUp;

    private List<GameSceneAdditionalObject> additionalObjects;
    private AudioManager audioManager;
    private GameManager gameManager;

    // Store the event handlers to unsubscribe later
    private System.Action environmentActivatedHandler;
    private System.Action voiceOverFinishedHandler;
    private System.Action compassCollectedHandler;

#if UNITY_EDITOR
    protected void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            AudioManager.Instance.StopAllVoiceOvers();
        }
    }
#endif

    public override void OnSceneEnter()
    {
        audioManager = AudioManager.Instance;
        gameManager = GameManager.Instance;
        additionalObjects = gameManager.GetAddditionalObjects();

        popUp = gameManager.PopUp.GetComponent<PopUpScript>();

        environmentActivatedHandler = StartVoiceOver;
        gameManager.Scenes[0].OnEnvironmentActivated += environmentActivatedHandler;
    }

    private void OnDisable()
    {
        UnsubscribeFromAll();
    }

    private void StartVoiceOver()
    {
        // First we de-activate the old objects
        gameManager.QRScanningUI.SetActive(false);

        // Then we unsubscribe from previous events
        gameManager.Scenes[0].OnEnvironmentActivated -= environmentActivatedHandler;

        // Then we activate new objects and call the needed methods
        audioManager.PlayVoiceOver("LivingRoomBeginScenePart1" + LocalizationSettings.SelectedLocale.Formatter);
        audioManager.Play("Confirm");

        // Then we subscribe to new events
        voiceOverFinishedHandler = StartCompassCollection;
        audioManager.OnVoiceOverFinished += voiceOverFinishedHandler;
    }

    private void StartCompassCollection()
    {
        // First we de-activate the old objects

        // Then we unsubscribe from previous events
        audioManager.OnVoiceOverFinished -= voiceOverFinishedHandler;

        // Then we activate new objects and call the needed methods
        compass.gameObject.SetActive(true);

        // Then we subscribe to new events
        compassCollectedHandler = StartOneLegVoiceOver;
        compass.OnCompassCollected += compassCollectedHandler;
    }

    private void StartOneLegVoiceOver()
    {
        // First we de-activate the old objects
        compass.gameObject.SetActive(false);

        // Then we unsubscribe from previous events
        compass.OnCompassCollected -= compassCollectedHandler;

        // Then we activate new objects and call the needed methods
        audioManager.PlayVoiceOver("LivingRoomBeginScenePart2" + LocalizationSettings.SelectedLocale.Formatter);

        // Then we subscribe to new events
        voiceOverFinishedHandler = OnSceneExit;
        audioManager.OnVoiceOverFinished += voiceOverFinishedHandler;
    }

    public override void OnSceneExit()
    {
        // First we de-activate the old objects

        // Then we unsubscribe from previous events
        audioManager.OnVoiceOverFinished -= voiceOverFinishedHandler;

        // Then we activate new objects and call the needed methods
        popUp.PopUpEntry(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("FindGrandma").Result, 4);
        gameManager.NextScene();
        gameManager.QRScanningUI.SetActive(true);
        Debug.Log("Finished scene");

        // No new event subscriptions here as the scene is ending
    }

    public override void UnsubscribeFromAll()
    {
        // Unsubscribe from events
        if (gameManager != null)
        {
            gameManager.Scenes[0].OnEnvironmentActivated -= environmentActivatedHandler;
        }

        if (audioManager != null)
        {
            audioManager.OnVoiceOverFinished -= voiceOverFinishedHandler;
        }

        if (compass != null)
        {
            compass.OnCompassCollected -= compassCollectedHandler;
        }

        AudioManager.Instance.StopAllVoiceOvers();
    }
}

