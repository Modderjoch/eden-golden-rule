// Copyright Oebe Rademaker All rights reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class GardenSceneData : GameSceneData
{
    [Header("Additional Objects")]
    private GameObject paperProgress;
    private GameObject paperAnimation;

    [SerializeField] private GameObject decoyCollider;
    [SerializeField] private PaperController paperController;
    [SerializeField] private CharacterTextureReplacing grandmaCharacterTexture;
    [SerializeField] private GameObject book;
    [SerializeField] private GameObject paper;

    private PopUpScript popUp;
    private PaperProgress paperProgressScript;
    private Animator bookAnimator;

    private List<GameSceneAdditionalObject> additionalObjects;
    private AudioManager audioManager;
    private GameManager gameManager;

    private bool rotateEnvironment = false;

    // Store the event handlers to unsubscribe later
    private System.Action environmentActivatedHandler;
    private System.Action voiceOverFinishedHandler;
    private System.Action paperScoreReachedHandler;
    private System.Action paperScoreAddedHandler;
    private System.Action continueVoiceOverHandler;
    private System.Action motherWolfVoiceOverHandler;
    private System.Action sceneExitHandler;

#if UNITY_EDITOR
    protected void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            AudioManager.Instance.StopAllVoiceOvers();
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            CoroutineHandler.Instance.StartCoroutine(CollectPaperAutomatically());
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

    protected void OnDisable()
    {
        UnsubscribeFromAll();
    }

    public override void OnSceneEnter()
    {
        audioManager = AudioManager.Instance;
        gameManager = GameManager.Instance;
        additionalObjects = gameManager.GetAddditionalObjects();

        paperProgress = additionalObjects[0].additionalObject;
        paperAnimation = additionalObjects[1].additionalObject;

        paperProgressScript = paperProgress.GetComponent<PaperProgress>();
        popUp = gameManager.PopUp.GetComponent<PopUpScript>();
        bookAnimator = book.GetComponent<Animator>();

        environmentActivatedHandler = StartVoiceOver;
        gameManager.Scenes[1].OnEnvironmentActivated += environmentActivatedHandler;
    }

    private void StartVoiceOver()
    {
        // First we de-activate the old objects

        // Then we unsubscribe from previous events
        gameManager.Scenes[1].OnEnvironmentActivated -= environmentActivatedHandler;

        // Then we activate new objects and call the needed methods
        audioManager.PlayVoiceOver("GardenScenePart1" + LocalizationSettings.SelectedLocale.Formatter);
        gameManager.QRScanningUI.SetActive(false);
        grandmaCharacterTexture.SetPose("Pose1");
        audioManager.Play("Confirm");

        rotateEnvironment = true;
        CoroutineHandler.Instance.StartCoroutine(DisableRotation(.1f));
        CoroutineHandler.Instance.StartCoroutine(OpenBook(12f, true));

        // Then we subscribe to new events
        voiceOverFinishedHandler = StartPaperCollection;
        audioManager.OnVoiceOverFinished += voiceOverFinishedHandler;
    }

    private void StartPaperCollection()
    {
        // First we de-activate the old objects

        // Then we unsubscribe from previous events
        audioManager.OnVoiceOverFinished -= voiceOverFinishedHandler;

        // Then we activate new objects and call the needed methods
        audioManager.PlayVoiceOver("GardenScenePart2" + LocalizationSettings.SelectedLocale.Formatter);
        popUp.PopUpEntry(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("PaperCollection").Result, 3);
        audioManager.Play("PaperFlying");

        paperProgress.SetActive(true);
        paperAnimation.SetActive(true);
        paperController.BlowPapers();
        Destroy(decoyCollider);
        grandmaCharacterTexture.SetPose("Pose2");
        paper.SetActive(false);

        // Then we subscribe to new events
        paperScoreReachedHandler = StartPaperCollectedVoiceOver;
        paperProgressScript.OnScoreReached += paperScoreReachedHandler;

        paperScoreAddedHandler = DisablePickUpHint;
        paperProgressScript.OnScoreAdded += paperScoreAddedHandler;
    }

    private void StartPaperCollectedVoiceOver()
    {
        // First we de-activate the old objects
        paperProgress.SetActive(false);
        paperAnimation.SetActive(false);

        // Then we unsubscribe from previous events
        paperProgressScript.OnScoreReached -= paperScoreReachedHandler;

        // Then we activate new objects and call the needed methods
        audioManager.PlayVoiceOver("GardenScenePart3" + LocalizationSettings.SelectedLocale.Formatter);
        grandmaCharacterTexture.SetPose("Pose3");
        CoroutineHandler.Instance.StartCoroutine(OpenBook(6.5f, false));
        paper.SetActive(true);

        // Then we subscribe to new events
        continueVoiceOverHandler = StartContinueVoiceOver;
        audioManager.OnVoiceOverFinished += continueVoiceOverHandler;
    }

    private void StartContinueVoiceOver()
    {
        // First we de-activate the old objects

        // Then we unsubscribe from previous events
        audioManager.OnVoiceOverFinished -= continueVoiceOverHandler;

        // Then we activate new objects and call the needed methods
        audioManager.PlayVoiceOver("GardenScenePart4" + LocalizationSettings.SelectedLocale.Formatter);
        grandmaCharacterTexture.SetPose("Pose4");

        // Then we subscribe to new events
        motherWolfVoiceOverHandler = StartMotherWolfVoiceOver;
        audioManager.OnVoiceOverFinished += motherWolfVoiceOverHandler;
    }

    private void StartMotherWolfVoiceOver()
    {
        // First we de-activate the old objects

        // Then we unsubscribe from previous events
        audioManager.OnVoiceOverFinished -= motherWolfVoiceOverHandler;

        // Then we activate new objects and call the needed methods
        audioManager.PlayVoiceOver("GardenScenePart5" + LocalizationSettings.SelectedLocale.Formatter);
        grandmaCharacterTexture.SetPose("Pose5");

        // Then we subscribe to new events
        sceneExitHandler = OnSceneExit;
        audioManager.OnVoiceOverFinished += sceneExitHandler;
    }

    public override void OnSceneExit()
    {
        // First we de-activate the old objects

        // Then we unsubscribe from previous events
        audioManager.OnVoiceOverFinished -= sceneExitHandler;

        // Then we activate new objects and call the needed methods
        popUp.PopUpEntry(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("FindMotherWolf").Result, 4);
        gameManager.NextScene();
        gameManager.QRScanningUI.SetActive(true);
        grandmaCharacterTexture.SetPose("Pose1");
        Debug.Log("Finished scene");

        // Then we subscribe to new events
    }

    private IEnumerator CollectPaperAutomatically()
    {
        Paper paper = new Paper(1);

        for (int i = 0; i < paperProgressScript.ReturnTotalScore(); i++)
        {
            paperProgressScript.AddScore(paper);
            yield return null;
        }
    }


    private IEnumerator DisableRotation(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        rotateEnvironment = false;
    }

    private IEnumerator OpenBook(float seconds, bool active)
    {
        yield return new WaitForSeconds(seconds);

        book.SetActive(active);
    }

    private void DisablePickUpHint()
    {
        paperAnimation.SetActive(false);
        paperProgressScript.OnScoreAdded -= paperScoreAddedHandler;
    }

    public override void UnsubscribeFromAll()
    {
        // Unsubscribe from events
        if (gameManager != null)
        {
            gameManager.Scenes[1].OnEnvironmentActivated -= environmentActivatedHandler;
        }

        if (audioManager != null)
        {
            audioManager.OnVoiceOverFinished -= voiceOverFinishedHandler;
            audioManager.OnVoiceOverFinished -= continueVoiceOverHandler;
            audioManager.OnVoiceOverFinished -= motherWolfVoiceOverHandler;
            audioManager.OnVoiceOverFinished -= sceneExitHandler;
        }

        if (paperProgressScript != null)
        {
            paperProgressScript.OnScoreReached -= paperScoreReachedHandler;
            paperProgressScript.OnScoreAdded -= paperScoreAddedHandler;
        }

        AudioManager.Instance.StopAllVoiceOvers();
    }
}
