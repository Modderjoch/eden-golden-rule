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

        gameManager.Scenes[1].OnEnvironmentActivated += StartVoiceOver;
    }

    private void StartVoiceOver()
    {
        // First we de-activate the old objects

        // Then we unsubscribe from previous events
        gameManager.Scenes[1].OnEnvironmentActivated -= StartVoiceOver;

        // Then we activate new objects and call the needed methods
        audioManager.PlayVoiceOver("GardenScenePart1" + LocalizationSettings.SelectedLocale.Formatter);
        gameManager.QRScanningUI.SetActive(false);
        grandmaCharacterTexture.SetPose("Pose1");
        audioManager.Play("Confirm");

        rotateEnvironment = true;
        CoroutineHandler.Instance.StartCoroutine(DisableRotation(.1f));
        CoroutineHandler.Instance.StartCoroutine(OpenBook(12f, true));

        // Then we subscribe to new events
        audioManager.OnVoiceOverFinished += StartPaperCollection;
    }

    private void StartPaperCollection()
    {
        // First we de-activate the old objects

        // Then we unsubscribe from previous events
        audioManager.OnVoiceOverFinished -= StartPaperCollection;

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
        paperProgressScript.OnScoreReached += StartPaperCollectedVoiceOver;
        paperProgressScript.OnScoreAdded += DisablePickUpHint;
    }

    private void StartPaperCollectedVoiceOver()
    {
        // First we de-activate the old objects
        paperProgress.SetActive(false);
        paperAnimation.SetActive(false);

        // Then we unsubscribe from previous events
        paperProgressScript.OnScoreReached -= StartPaperCollectedVoiceOver;

        // Then we activate new objects and call the needed methods
        audioManager.PlayVoiceOver("GardenScenePart3" + LocalizationSettings.SelectedLocale.Formatter);
        grandmaCharacterTexture.SetPose("Pose3");
        CoroutineHandler.Instance.StartCoroutine(OpenBook(6.5f, false));
        paper.SetActive(true);

        // Then we subscribe to new events
        audioManager.OnVoiceOverFinished += StartContinueVoiceOver;
    }

    private void StartContinueVoiceOver()
    {
        // First we de-activate the old objects

        // Then we unsubscribe from previous events
        audioManager.OnVoiceOverFinished -= StartContinueVoiceOver;

        // Then we activate new objects and call the needed methods
        audioManager.PlayVoiceOver("GardenScenePart4" + LocalizationSettings.SelectedLocale.Formatter);
        grandmaCharacterTexture.SetPose("Pose4");

        // Then we subscribe to new events
        audioManager.OnVoiceOverFinished += StartMotherWolfVoiceOver;
    }

    private void StartMotherWolfVoiceOver()
    {
        // First we de-activate the old objects

        // Then we unsubscribe from previous events
        audioManager.OnVoiceOverFinished -= StartMotherWolfVoiceOver;

        // Then we activate new objects and call the needed methods
        audioManager.PlayVoiceOver("GardenScenePart5" + LocalizationSettings.SelectedLocale.Formatter);
        grandmaCharacterTexture.SetPose("Pose5");

        // Then we subscribe to new events
        audioManager.OnVoiceOverFinished += OnSceneExit;
    }

    public override void OnSceneExit()
    {
        // First we de-activate the old objects

        // Then we unsubscribe from previous events
        audioManager.OnVoiceOverFinished -= OnSceneExit;

        // Then we activate new objects and call the needed methods
        //popUp.PopUpEntry(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("FindMotherWolf").Result, 4);
        gameManager.NextScene();
        gameManager.Compass.SetInteger("sceneprogress", 2);
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

        if(book != null)
        book.SetActive(active);
    }


    private void DisablePickUpHint()
    {
        paperAnimation.SetActive(false);
        paperProgressScript.OnScoreAdded -= DisablePickUpHint;
    }
}
