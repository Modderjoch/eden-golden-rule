// Copyright Oebe Rademaker All rights reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LivingRoomBeginSceneData : GameSceneData
{
    [Header("Additional Objects")]
    [SerializeField] CharacterTextureReplacing motherTextureReplacing;
    private Compass compass;

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

    public override void OnSceneEnter()
    {
        audioManager = AudioManager.Instance;
        gameManager = GameManager.Instance;
        additionalObjects = gameManager.GetAddditionalObjects();

        compass = additionalObjects[0].additionalObject.GetComponent<Compass>();

        popUp = gameManager.PopUp.GetComponent<PopUpScript>();

        gameManager.Scenes[0].OnEnvironmentActivated += StartVoiceOver;
    }

    private void StartVoiceOver()
    {
        // First we de-activate the old objects
        gameManager.QRScanningUI.SetActive(false);

        // Then we unsubscribe from previous events
        gameManager.Scenes[0].OnEnvironmentActivated -= StartVoiceOver;

        // Then we activate new objects and call the needed methods
        rotateEnvironment = true;
        CoroutineHandler.Instance.StartCoroutine(DisableRotation(.1f));
        audioManager.PlayVoiceOver("LivingRoomBeginScenePart1" + LocalizationSettings.SelectedLocale.Formatter);
        audioManager.Play("Confirm");
        motherTextureReplacing.SetPose("Entry" + LocalizationSettings.SelectedLocale.Formatter);

        // Then we subscribe to new events
        audioManager.OnVoiceOverFinished += StartCompassCollection;
    }

    private void StartCompassCollection()
    {
        // First we de-activate the old objects

        // Then we unsubscribe from previous events
        audioManager.OnVoiceOverFinished -= StartCompassCollection;

        // Then we activate new objects and call the needed methods
        compass.gameObject.SetActive(true);
        popUp.PopUpEntry(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("CompassCollection").Result, 3);

        // Then we subscribe to new events
        compass.OnCompassCollected += StartOneLegVoiceOver;
    }

    private void StartOneLegVoiceOver()
    {
        // First we de-activate the old objects
        compass.gameObject.SetActive(false);

        // Then we unsubscribe from previous events
        compass.OnCompassCollected -= StartOneLegVoiceOver;

        // Then we activate new objects and call the needed methods
        audioManager.PlayVoiceOver("LivingRoomBeginScenePart2" + LocalizationSettings.SelectedLocale.Formatter);
        motherTextureReplacing.SetPose("Exit" + LocalizationSettings.SelectedLocale.Formatter);

        // Then we subscribe to new events
        audioManager.OnVoiceOverFinished += OnSceneExit;
    }

    public override void OnSceneExit()
    {
        // First we de-activate the old objects

        // Then we unsubscribe from previous events
        audioManager.OnVoiceOverFinished -= OnSceneExit;

        // Then we activate new objects and call the needed methods
        //popUp.PopUpEntry(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("FindGrandma").Result, 4);
        gameManager.NextScene();
        gameManager.Compass.SetInteger("sceneprogress", 1);

        // Then we subscribe to new events
    }

    private IEnumerator DisableRotation(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        rotateEnvironment = false;
    }
}
