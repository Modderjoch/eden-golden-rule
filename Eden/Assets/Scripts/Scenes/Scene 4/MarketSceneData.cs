using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class MarketSceneData : GameSceneData
{
    [Header("Additional Objects")]
    private Bracelet bracelet;

    private PopUpScript popUp;

    private List<GameSceneAdditionalObject> additionalObjects;
    private AudioManager audioManager;
    private GameManager gameManager;

    private bool rotateEnvironment = false;

    // Store the event handlers to unsubscribe later
    private System.Action environmentActivatedHandler;
    private System.Action voiceOverFinishedHandler;
    private System.Action braceletCollectedHandler;

#if UNITY_EDITOR
    protected void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            AudioManager.Instance.StopAllVoiceOvers();
        }

        if (rotateEnvironment)
        {
            Vector3 relativePos = transform.position - Camera.main.transform.position;

            relativePos.y = 0;

            Quaternion rotation = Quaternion.LookRotation(relativePos);
            transform.rotation = rotation;
        }

        rotateEnvironment = false;
    }
#endif

    public override void OnSceneEnter()
    {
        audioManager = AudioManager.Instance;
        gameManager = GameManager.Instance;
        additionalObjects = gameManager.GetAddditionalObjects();

        if (LocalizationSettings.SelectedLocale.Formatter.ToString() == "en-US")
        {
            bracelet = additionalObjects[0].additionalObject.GetComponent<Bracelet>();
        }
        else
        {
            bracelet = additionalObjects[1].additionalObject.GetComponent<Bracelet>();
        }

        popUp = gameManager.PopUp.GetComponent<PopUpScript>();

        environmentActivatedHandler = StartVoiceOver;
        gameManager.Scenes[3].OnEnvironmentActivated += environmentActivatedHandler;
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
        gameManager.Scenes[3].OnEnvironmentActivated -= environmentActivatedHandler;

        // Then we activate new objects and call the needed methods
        audioManager.PlayVoiceOver("MarketScenePart1" + LocalizationSettings.SelectedLocale.Formatter);
        audioManager.Play("Confirm");

        rotateEnvironment = true;
        CoroutineHandler.Instance.StartCoroutine(DisableRotation(.1f));

        // Then we subscribe to new events
        voiceOverFinishedHandler = StartBreadFamilyVoiceOver;
        audioManager.OnVoiceOverFinished += voiceOverFinishedHandler;
    }

    private void StartBreadFamilyVoiceOver()
    {
        // First we de-activate the old objects

        // Then we unsubscribe from previous events
        audioManager.OnVoiceOverFinished -= voiceOverFinishedHandler;

        // Then we activate new objects and call the needed methods
        audioManager.PlayVoiceOver("MarketScenePart2" + LocalizationSettings.SelectedLocale.Formatter);

        // Then we subscribe to new events
        voiceOverFinishedHandler = StartRevealVoiceOver;
        audioManager.OnVoiceOverFinished += voiceOverFinishedHandler;
    }

    private void StartRevealVoiceOver()
    {
        // First we de-activate the old objects

        // Then we unsubscribe from previous events
        audioManager.OnVoiceOverFinished -= voiceOverFinishedHandler;

        // Then we activate new objects and call the needed methods
        audioManager.PlayVoiceOver("MarketScenePart3" + LocalizationSettings.SelectedLocale.Formatter);

        // Then we subscribe to new events
        voiceOverFinishedHandler = StartGoldenRuleVoiceOver;
        audioManager.OnVoiceOverFinished += voiceOverFinishedHandler;
    }

    private void StartGoldenRuleVoiceOver()
    {
        // First we de-activate the old objects

        // Then we unsubscribe from previous events
        audioManager.OnVoiceOverFinished -= voiceOverFinishedHandler;

        // Then we activate new objects and call the needed methods
        audioManager.PlayVoiceOver("MarketScenePart4" + LocalizationSettings.SelectedLocale.Formatter);

        // Then we subscribe to new events
        voiceOverFinishedHandler = StartBraceletInteraction;
        audioManager.OnVoiceOverFinished += voiceOverFinishedHandler;
    }

    private void StartBraceletInteraction()
    {
        // First we de-activate the old objects

        // Then we unsubscribe from previous events
        audioManager.OnVoiceOverFinished -= voiceOverFinishedHandler;

        // Then we activate new objects and call the needed methods
        bracelet.gameObject.SetActive(true);
        popUp.PopUpEntry(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("BraceletCollection").Result, 4);

        // Then we subscribe to new events
        braceletCollectedHandler = StartBraceletVoiceOver;
        bracelet.OnBraceletCollected += braceletCollectedHandler;
    }

    private void StartBraceletVoiceOver()
    {
        // First we de-activate the old objects
        bracelet.gameObject.SetActive(false);

        // Then we unsubscribe from previous events
        bracelet.OnBraceletCollected -= braceletCollectedHandler;

        // Then we activate new objects and call the needed methods
        audioManager.PlayVoiceOver("MarketScenePart5" + LocalizationSettings.SelectedLocale.Formatter);

        // Then we subscribe to new events
        voiceOverFinishedHandler = StartGratitudeVoiceOver;
        audioManager.OnVoiceOverFinished += voiceOverFinishedHandler;
    }

    private void StartGratitudeVoiceOver()
    {
        // First we de-activate the old objects

        // Then we unsubscribe from previous events
        audioManager.OnVoiceOverFinished -= voiceOverFinishedHandler;

        // Then we activate new objects and call the needed methods
        audioManager.PlayVoiceOver("MarketScenePart6" + LocalizationSettings.SelectedLocale.Formatter);

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
        popUp.PopUpEntry(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("FindHome").Result, 4);
        gameManager.NextScene();
        gameManager.QRScanningUI.SetActive(true);
        Debug.Log("Finished scene");

        // Then we subscribe to new events
    }

    private IEnumerator DisableRotation(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        rotateEnvironment = false;
    }

    public override void UnsubscribeFromAll()
    {
        // Unsubscribe from events
        if (gameManager != null)
        {
            gameManager.Scenes[3].OnEnvironmentActivated -= environmentActivatedHandler;
        }

        if (audioManager != null)
        {
            audioManager.OnVoiceOverFinished -= voiceOverFinishedHandler;
        }

        if (bracelet != null)
        {
            bracelet.OnBraceletCollected -= braceletCollectedHandler;
        }
    }
}
