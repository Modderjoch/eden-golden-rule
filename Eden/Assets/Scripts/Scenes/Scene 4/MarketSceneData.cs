// Copyright Oebe Rademaker All rights reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class MarketSceneData : GameSceneData
{
    [Header("Additional Objects")]
    [SerializeField] private List<CharacterTextureReplacing> characterTextures = new List<CharacterTextureReplacing>();
    [SerializeField] private CharacterTextureReplacing hillelTextureReplacing;
    [SerializeField] private CharacterTextureReplacing edenTextureReplacing;
    private Bracelet bracelet;

    private PopUpScript popUp;

    private List<GameSceneAdditionalObject> additionalObjects;
    private AudioManager audioManager;
    private GameManager gameManager;

    private bool rotateEnvironment = false;

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
        audioManager.Play("Confirm");

        rotateEnvironment = true;
        CoroutineHandler.Instance.StartCoroutine(DisableRotation(.1f));

        SetPoseForCharacters("Angry" + LocalizationSettings.SelectedLocale.Formatter);

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
        SetPoseForCharacters("Explaining" + LocalizationSettings.SelectedLocale.Formatter);

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
        SetPoseForCharacters("Standing on leg" + LocalizationSettings.SelectedLocale.Formatter);

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
        SetPoseForCharacters("Listening" + LocalizationSettings.SelectedLocale.Formatter);

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
        popUp.PopUpEntry(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("BraceletCollection").Result, 4);

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
        hillelTextureReplacing.SetPose("Giving" + LocalizationSettings.SelectedLocale.Formatter);
        edenTextureReplacing.SetPose("Giving" + LocalizationSettings.SelectedLocale.Formatter);

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
        SetPoseForCharacters("Standing on leg again" + LocalizationSettings.SelectedLocale.Formatter);

        // Then we subscribe to new events
        audioManager.OnVoiceOverFinished += OnSceneExit;
    }

    public override void OnSceneExit()
    {
        // First we de-activate the old objects

        // Then we unsubscribe from previous events
        audioManager.OnVoiceOverFinished -= OnSceneExit;

        // Then we activate new objects and call the needed methods
        //popUp.PopUpEntry(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("FindHome").Result, 4);
        gameManager.NextScene();
        gameManager.Compass.SetInteger("sceneprogress", 4);
        Debug.Log("Finished scene");

        // Then we subscribe to new events
    }

    private IEnumerator DisableRotation(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        rotateEnvironment = false;
    }

    private void SetPoseForCharacters(string pose)
    {
        foreach(CharacterTextureReplacing character in characterTextures)
        {
            character.SetPose(pose);
        }
    }
}
