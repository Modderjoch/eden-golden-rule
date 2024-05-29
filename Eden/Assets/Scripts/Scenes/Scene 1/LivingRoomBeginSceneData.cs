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

        gameManager.Scenes[0].OnEnvironmentActivated += StartVoiceOver;
    }

    private void StartVoiceOver()
    {
        // First we de-activate the old objects
        gameManager.QRScanningUI.SetActive(false);

        // Then we unsubscribe from previous events
        gameManager.Scenes[0].OnEnvironmentActivated -= StartVoiceOver;

        // Then we activate new objects and call the needed methods
        audioManager.PlayVoiceOver("LivingRoomBeginScenePart1" + LocalizationSettings.SelectedLocale.Formatter);
        audioManager.Play("Confirm");

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

        // Then we subscribe to new events
        audioManager.OnVoiceOverFinished += OnSceneExit;
    }

    public override void OnSceneExit()
    {
        // First we de-activate the old objects

        // Then we unsubscribe from previous events
        audioManager.OnVoiceOverFinished -= OnSceneExit;

        // Then we activate new objects and call the needed methods
        popUp.PopUpEntry(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("FindGrandma").Result, 4);
        gameManager.NextScene();
        gameManager.QRScanningUI.SetActive(true);

        Debug.Log("Finished scene");

        // Then we subscribe to new events
    }
}
