// Copyright Oebe Rademaker All rights reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LivingRoomBeginSceneData : GameSceneData
{
    [Header("Additional Objects")]
    private Compass compass;

    private PopUpScript popUp;

    private List<GameSceneAdditionalObject> additionalObjects;
    private AudioManager audioManager;
    private GameManager gameManager;

    // Store the event handlers to unsubscribe later
    private System.Action environmentActivatedHandler;
    private System.Action voiceOverFinishedHandler;
    private System.Action compassCollectedHandler;

    private bool rotateEnvironment = false;

    protected void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.P))
        {
            AudioManager.Instance.StopAllVoiceOvers();
        }
#endif
        if (rotateEnvironment)
        {
            Vector3 relativePos = transform.position - Camera.main.transform.position;

            relativePos.y = 0;

            Quaternion rotation = Quaternion.LookRotation(relativePos);
            transform.rotation = rotation;
        }
    }

    public override void OnSceneEnter()
    {
        audioManager = AudioManager.Instance;
        gameManager = GameManager.Instance;
        additionalObjects = gameManager.GetAddditionalObjects();

        compass = additionalObjects[0].additionalObject.GetComponent<Compass>();

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
        rotateEnvironment = true;
        CoroutineHandler.Instance.StartCoroutine(DisableRotation(.1f));
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

        // Then we subscribe to new events
        compassCollectedHandler = StartOneLegVoiceOver;
        compass.OnCompassCollected += compassCollectedHandler;
        compass.gameObject.SetActive(true);
        popUp.PopUpEntry(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("CompassCollection").Result, 3);
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

    private IEnumerator DisableRotation(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        rotateEnvironment = false;
    }
}

