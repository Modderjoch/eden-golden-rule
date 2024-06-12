// Copyright Oebe Rademaker All rights reserved

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LivingRoomEndSceneData : GameSceneData
{
    private PopUpScript popUp;

    private List<GameSceneAdditionalObject> additionalObjects;
    private AudioManager audioManager;
    private GameManager gameManager;

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

    // Store the event handlers to unsubscribe later
    private System.Action environmentActivatedHandler;
    private System.Action voiceOverFinishedHandler;

    protected void OnDisable()
    {
        
    }

    public override void OnSceneEnter()
    {
        audioManager = AudioManager.Instance;
        gameManager = GameManager.Instance;
        additionalObjects = gameManager.GetAddditionalObjects();

        popUp = gameManager.PopUp.GetComponent<PopUpScript>();

        environmentActivatedHandler = StartVoiceOver;
        gameManager.Scenes[4].OnEnvironmentActivated += environmentActivatedHandler;
    }

    private void StartVoiceOver()
    {
        // First we de-activate the old objects
        gameManager.QRScanningUI.SetActive(false);

        // Then we unsubscribe from previous events
        gameManager.Scenes[4].OnEnvironmentActivated -= environmentActivatedHandler;

        // Then we activate new objects and call the needed methods

        rotateEnvironment = true;
        CoroutineHandler.Instance.StartCoroutine(DisableRotation(.1f));
        audioManager.PlayVoiceOver("LivingRoomEndScenePart1" + LocalizationSettings.SelectedLocale.Formatter);
        audioManager.Play("Confirm");

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
        popUp.PopUpEntry(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("Finish").Result, 5);
        CoroutineHandler.Instance.StartCoroutine(gameManager.ResetGame(10f));
        Debug.Log("Finished scene");

        // Then we subscribe to new events
    }

    public override void UnsubscribeFromAll()
    {
        // Unsubscribe from events
        if (gameManager != null)
        {
            gameManager.Scenes[4].OnEnvironmentActivated -= environmentActivatedHandler;
        }

        if (audioManager != null)
        {
            audioManager.OnVoiceOverFinished -= voiceOverFinishedHandler;
        }

        AudioManager.Instance.StopAllVoiceOvers();

    private IEnumerator DisableRotation(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        rotateEnvironment = false;

    }
}
