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

    [SerializeField] CharacterTextureReplacing motherTextureReplacing;


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

        popUp = gameManager.PopUp.GetComponent<PopUpScript>();

        gameManager.Scenes[4].OnEnvironmentActivated += StartVoiceOver;
    }

    private void StartVoiceOver()
    {
        // First we de-activate the old objects
        gameManager.QRScanningUI.SetActive(false);

        // Then we unsubscribe from previous events
        gameManager.Scenes[4].OnEnvironmentActivated -= StartVoiceOver;

        // Then we activate new objects and call the needed methods
        rotateEnvironment = true;
        CoroutineHandler.Instance.StartCoroutine(DisableRotation(.1f));
        audioManager.PlayVoiceOver("LivingRoomEndScenePart1" + LocalizationSettings.SelectedLocale.Formatter);
        audioManager.Play("Confirm");
        motherTextureReplacing.SetPose("Entry");

        // Then we subscribe to new events
        audioManager.OnVoiceOverFinished += StartSongVoiceOver;
    }

    private void StartSongVoiceOver()
    {
        // First we de-activate the old objects

        // Then we unsubscribe from previous events
        audioManager.OnVoiceOverFinished -= StartSongVoiceOver;

        // Then we activate new objects and call the needed methods
        audioManager.PlayVoiceOver("LivingRoomEndScenePart2" + LocalizationSettings.SelectedLocale.Formatter);
        gameManager.Compass.SetInteger("sceneprogress", 5);

        // Then we subscribe to new events
        audioManager.OnVoiceOverFinished += OnSceneExit;
    }

    public override void OnSceneExit()
    {
        // First we de-activate the old objects

        // Then we unsubscribe from previous events
        audioManager.OnVoiceOverFinished -= OnSceneExit;

        // Then we activate new objects and call the needed methods
        CoroutineHandler.Instance.StartCoroutine(gameManager.ResetGame(10f));
        Debug.Log("Finished scene");

        // Then we subscribe to new events
    }

    private IEnumerator DisableRotation(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        rotateEnvironment = false;
    }
}
