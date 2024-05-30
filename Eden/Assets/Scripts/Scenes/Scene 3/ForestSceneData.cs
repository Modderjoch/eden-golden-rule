using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class ForestSceneData : GameSceneData
{
    [Header("Additional Objects")]
    private GameObject trashProgress;
    private GameObject swipeArea;
    private GameObject seedSpawnpoint;
    private GameObject swipeAnimation;

    [SerializeField] private float itemForTreeRate = 3;
    [SerializeField] private float grassLerpDuration = 3;

    [SerializeField] private Material grassMaterial;
    [SerializeField] private Material grassClumpsMaterial;

    [SerializeField] private List<ParticleSystem> particleSystemsBase;
    [SerializeField] private List<ParticleSystem> particleSystemsExtra;
    [SerializeField] private List<TreeSwitcher> treeSwitcher;

    private TrashProgress trashProgressScript;
    private PopUpScript popUp;
    private SwipeScript swipeScript;

    private List<GameSceneAdditionalObject> additionalObjects;
    private AudioManager audioManager;
    private GameManager gameManager;

    private int deadForestItems;
    private int actionPoint;

    private float lerpTimer = 0f;

    private bool transitionGrass = false;

    private bool rotateEnvironment = false;

    protected void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.P))
        {
            AudioManager.Instance.StopAllVoiceOvers();
        }
#endif

        if(transitionGrass)
        {
            lerpTimer += Time.deltaTime;

            float lerpValue = Mathf.Clamp01(lerpTimer / grassLerpDuration);

            grassMaterial.SetFloat("_GrassLerp", lerpValue);

            grassClumpsMaterial.SetFloat("_GrassClump_Lerp", lerpValue);

            if(lerpTimer > grassLerpDuration)
            {
                transitionGrass = false;
            }
        }
    }

    public override void OnSceneEnter()
    {
        audioManager = AudioManager.Instance;
        gameManager = GameManager.Instance;
        additionalObjects = gameManager.GetAddditionalObjects();

        trashProgress = additionalObjects[0].additionalObject;
        swipeArea = additionalObjects[1].additionalObject;
        seedSpawnpoint = additionalObjects[2].additionalObject;
        swipeAnimation = additionalObjects[3].additionalObject;

        trashProgressScript = trashProgress.GetComponent<TrashProgress>();
        popUp = gameManager.PopUp.GetComponent<PopUpScript>();
        swipeScript = seedSpawnpoint.GetComponentInChildren<SwipeScript>();

        deadForestItems = treeSwitcher.Count;

        grassMaterial.SetFloat("_GrassLerp", 0);
        grassClumpsMaterial.SetFloat("_GrassClump_Lerp", 0);


        gameManager.Scenes[2].OnEnvironmentActivated += StartVoiceOver;
    }

    protected void FixedUpdate()
    {
        if (rotateEnvironment)
        {
            Vector3 relativePos = transform.position - Camera.main.transform.position;

            relativePos.y = 0;

            Quaternion rotation = Quaternion.LookRotation(relativePos);
            transform.rotation = rotation;
        }
    }

    private void StartVoiceOver()
    {
        // First we de-activate the old objects

        // Then we unsubscribe from previous events
        gameManager.Scenes[2].OnEnvironmentActivated -= StartVoiceOver;

        // Then we activate new objects and call the needed methods
        audioManager.PlayVoiceOver("ForestScenePart1" + LocalizationSettings.SelectedLocale.Formatter);
        gameManager.QRScanningUI.SetActive(false);
        audioManager.Play("Confirm");

        rotateEnvironment = true;
        CoroutineHandler.Instance.StartCoroutine(DisableRotation(.1f));

        // Then we subscribe to new events
        audioManager.OnVoiceOverFinished += StartMotherWolfAppearance;
    }

    private void StartMotherWolfAppearance()
    {
        // First we de-activate the old objects

        // Then we unsubscribe from previous events
        audioManager.OnVoiceOverFinished -= StartMotherWolfAppearance;

        // Then we activate new objects and call the needed methods
        audioManager.PlayVoiceOver("ForestScenePart2" + LocalizationSettings.SelectedLocale.Formatter);

        // Then we subscribe to new events
        audioManager.OnVoiceOverFinished += StartTrashPicking;
    }

    private void StartTrashPicking()
    {
        // First we de-activate the old objects

        // Then we unsubscribe from previous events
        audioManager.OnVoiceOverFinished -= StartTrashPicking;

        // Then we activate new objects and call the needed methods
        trashProgress.SetActive(true);
        actionPoint = Mathf.FloorToInt(trashProgressScript.ReturnTotalScore() / deadForestItems + itemForTreeRate);
        popUp.PopUpEntry(LocalizationSettings.StringDatabase.GetLocalizedStringAsync("TrashCollection").Result, 3);

        // Then we subscribe to new events
        trashProgressScript.OnScoreReached += StartSeedVoiceOver;
        trashProgressScript.OnScoreAdded += HandleTrashCollection;
    }

    private void StartSeedVoiceOver()
    {        
        // First we de-activate the old objects
        trashProgress.SetActive(false);

        // Then we unsubscribe from previous events
        trashProgressScript.OnScoreReached -=StartSeedVoiceOver;
        trashProgressScript.OnScoreAdded -= HandleTrashCollection;

        // Then we activate new objects and call the needed methods
        transitionGrass = true;
        audioManager.PlayVoiceOver("ForestScenePart3" + LocalizationSettings.SelectedLocale.Formatter);

        // Then we subscribe to new events
        audioManager.OnVoiceOverFinished += StartSeedThrowing;
    }

    private void StartSeedThrowing()
    {
        // First we de-activate the old objects

        // Then we unsubscribe from previous events
        audioManager.OnVoiceOverFinished -= StartSeedThrowing;

        // Then we activate new objects and call the needed methods
        swipeAnimation.SetActive(true);
        swipeArea.SetActive(true);
        seedSpawnpoint.SetActive(true);
        popUp.PopUpEntry(LocalizationSettings.StringDatabase.GetLocalizedString("SeedPlanting"), 5);

        // Then we subscribe to new events
        seedSpawnpoint.GetComponent<SpawnSeed>().OnSeedsDepleted += EndScene;
        swipeScript.OnSwipeDetected += DisableSwipeAnimation;
    }

    private void EndScene()
    {
        // First we de-activate the old objects
        swipeArea.SetActive(false);
        seedSpawnpoint.SetActive(false);
        swipeAnimation.SetActive(false);

        // Then we unsubscribe from previous events
        seedSpawnpoint.GetComponent<SpawnSeed>().OnSeedsDepleted -= EndScene;

        // Then we activate new objects and call the needed methods
        audioManager.PlayVoiceOver("ForestScenePart4" + LocalizationSettings.SelectedLocale.Formatter);

        // Then we subscribe to new events
        audioManager.OnVoiceOverFinished += OnSceneExit;
    }

    public override void OnSceneExit()
    {
        // First we de-activate the old objects

        // Then we unsubscribe from previous events
        audioManager.OnVoiceOverFinished -= OnSceneExit;

        // Then we activate new objects and call the needed methods
        popUp.PopUpEntry("Well done!", 3);
        gameManager.NextScene();
        gameManager.QRScanningUI.SetActive(true);
        Debug.Log("Finished scene");

        // Then we subscribe to new events
    }

    private void DisableSwipeAnimation()
    {
        swipeAnimation.SetActive(false);
        swipeScript.OnSwipeDetected -= DisableSwipeAnimation;
    }

    private IEnumerator DisableRotation(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        rotateEnvironment = false;
    }


    private void HandleTrashCollection()
    {
        int currentScore = trashProgressScript.ReturnCurrentScore();

        if (currentScore > 0 && currentScore <= 5)
        {
            particleSystemsBase[0].Stop();
            particleSystemsBase[1].Stop();
            particleSystemsBase[2].Stop();
        }
        else if (currentScore > 5 && currentScore <= 10)
        {
            particleSystemsBase[3].Stop();
            particleSystemsBase[4].Stop();
            particleSystemsBase[5].Stop();
            particleSystemsBase[6].Stop();

            particleSystemsExtra[0].Stop();
            particleSystemsExtra[1].Stop();
            particleSystemsExtra[2].Stop();
            particleSystemsExtra[3].Stop();
        }
        else if (currentScore > 10 && currentScore <= 15)
        {
            particleSystemsExtra[4].Stop();
            particleSystemsExtra[5].Stop();
            particleSystemsExtra[6].Stop();
        }
        else if (currentScore > 15 && currentScore <= 20)
        {
            treeSwitcher[0].ActivateTransition();
            treeSwitcher[1].ActivateTransition();
        }
        else if (currentScore > 20 && currentScore <= 25)
        {
            treeSwitcher[2].ActivateTransition();
            treeSwitcher[10].ActivateTransition();
        }
        else if (currentScore > 25 && currentScore <= 30)
        {
            treeSwitcher[3].ActivateTransition();
        }
        else if (currentScore > 30 && currentScore <= 35)
        {
            treeSwitcher[4].ActivateTransition();
            treeSwitcher[5].ActivateTransition();
        }
        else if (currentScore > 35 && currentScore <= 40)
        {
            treeSwitcher[6].ActivateTransition();
        }
        else if (currentScore > 40 && currentScore <= 45)
        {
            treeSwitcher[7].ActivateTransition();
            treeSwitcher[11].ActivateTransition();
        }
        else if (currentScore > 45 && currentScore <= 50)
        {
            treeSwitcher[8].ActivateTransition();
            treeSwitcher[9].ActivateTransition();
        }
    }
}
