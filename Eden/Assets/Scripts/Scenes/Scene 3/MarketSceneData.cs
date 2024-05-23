using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class MarketSceneData : GameSceneData
{
    [Header("Additional Objects")]

    [SerializeField] private Bracelet bracelet;

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

        gameManager.Scenes[3].OnEnvironmentActivated += StartVoiceOver;
    }

    private void StartVoiceOver()
    {
        // First we de-activate the old objects
        gameManager.QRScanningUI.SetActive(false);

        // Then we unsubscribe from previous events
        gameManager.Scenes[3].OnEnvironmentActivated -= StartVoiceOver;

        // Then we activate new objects and call the needed methods
        audioManager.PlayVoiceOver("MarketScenePart1" + LocalizationSettings.SelectedLocale.Formatter);

        // Then we subscribe to new events
        audioManager.OnVoiceOverFinished += StartBreadFamilyVoiceOver;
    }

    private void StartBreadFamilyVoiceOver()
    {
        // First we de-activate the old objects

        // Then we unsubscribe from previous events
        audioManager.OnVoiceOverFinished -= StartBreadFamilyVoiceOver;

        // Then we activate new objects and call the needed methods
        audioManager.PlayVoiceOver("MarketScenePart2" + LocalizationSettings.SelectedLocale.Formatter);

        // Then we subscribe to new events
        audioManager.OnVoiceOverFinished += StartRevealVoiceOver;
    }

    private void StartRevealVoiceOver()
    {
        // First we de-activate the old objects

        // Then we unsubscribe from previous events
        audioManager.OnVoiceOverFinished -= StartRevealVoiceOver;

        // Then we activate new objects and call the needed methods
        audioManager.PlayVoiceOver("MarketScenePart3" + LocalizationSettings.SelectedLocale.Formatter);

        // Then we subscribe to new events
        audioManager.OnVoiceOverFinished += StartGoldenRuleVoiceOver;
    }

    private void StartGoldenRuleVoiceOver()
    {
        // First we de-activate the old objects

        // Then we unsubscribe from previous events
        audioManager.OnVoiceOverFinished -= StartGoldenRuleVoiceOver;

        // Then we activate new objects and call the needed methods
        audioManager.PlayVoiceOver("MarketScenePart4" + LocalizationSettings.SelectedLocale.Formatter);

        // Then we subscribe to new events
        audioManager.OnVoiceOverFinished += StartBraceletInteraction;
    }

    private void StartBraceletInteraction()
    {
        // First we de-activate the old objects

        // Then we unsubscribe from previous events
        audioManager.OnVoiceOverFinished -= StartBraceletInteraction;

        // Then we activate new objects and call the needed methods
        bracelet.gameObject.SetActive(true);
        popUp.PopUpEntry(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("BraceletCollection").Result, 2);

        // Then we subscribe to new events
        bracelet.OnBraceletCollected += StartBraceletVoiceOver;
    }

    private void StartBraceletVoiceOver()
    {
        // First we de-activate the old objects
        bracelet.gameObject.SetActive(false);

        // Then we unsubscribe from previous events
        bracelet.OnBraceletCollected -= StartBraceletVoiceOver;

        // Then we activate new objects and call the needed methods
        audioManager.PlayVoiceOver("MarketScenePart5" + LocalizationSettings.SelectedLocale.Formatter);

        // Then we subscribe to new events
        audioManager.OnVoiceOverFinished += StartGratitudeVoiceOver;
    }

    private void StartGratitudeVoiceOver()
    {
        // First we de-activate the old objects

        // Then we unsubscribe from previous events
        audioManager.OnVoiceOverFinished -= StartGratitudeVoiceOver;

        // Then we activate new objects and call the needed methods
        audioManager.PlayVoiceOver("MarketScenePart6" + LocalizationSettings.SelectedLocale.Formatter);

        // Then we subscribe to new events
        audioManager.OnVoiceOverFinished += OnSceneExit;
    }

    public override void OnSceneExit()
    {
        // First we de-activate the old objects

        // Then we unsubscribe from previous events
        audioManager.OnVoiceOverFinished -= OnSceneExit;

        // Then we activate new objects and call the needed methods
        popUp.PopUpEntry(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("FindHome").Result, 4);
        gameManager.NextScene();
        gameManager.QRScanningUI.SetActive(true);

        Debug.Log("Finished scene");

        // Then we subscribe to new events
    }
}
