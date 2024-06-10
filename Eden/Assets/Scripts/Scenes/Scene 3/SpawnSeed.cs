using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSeed : MonoBehaviour
{
    [SerializeField] private GameObject spawnPrefab;
    [SerializeField] private float spawnDelay;
    [SerializeField] private RectTransform swipeArea;
    [SerializeField] private int numberOfSeeds = 10;
    [SerializeField] private List<GameObject> availableFlowers = new List<GameObject>();
    private List<GameObject> usedFlowers = new List<GameObject>();

    private bool isInstantiating = false;

    public event Action OnSeedsDepleted;
    public event Action OnSeedsChosen;

    [SerializeField] private MoveToMiddle moveToMiddle;
    [SerializeField] private Animator animator;


    public void SetMoveToMiddle(MoveToMiddle moveToMiddle) 
    {
        this.moveToMiddle = moveToMiddle;
    }

    public void SetFlowerPrefab(GameObject prefab)
    {
        if(prefab.name != "null")
        {
            usedFlowers.Add(prefab);
        }
        else
        {
            usedFlowers.AddRange(availableFlowers);
        }

        gameObject.GetComponentInChildren<SwipeScript>().SetFlowerPrefab(ReturnFlower());

        moveToMiddle.DeactivateOtherPacks();

        moveToMiddle.OnMiddleReached += OpenPack;

        SetAnimator();
    }

    public void DepleteSeeds()
    {
        numberOfSeeds = 0;
    }

    private void OpenPack()
    {
        animator.SetTrigger("Open");

        AnimatorStateInfo animationState = animator.GetCurrentAnimatorStateInfo(0);
        float animationDuration = animationState.length;

        CoroutineHandler.Instance.StartCoroutine(SeedPackOpened(animationDuration));
    }

    private IEnumerator SeedPackOpened(float time)
    {
        yield return new WaitForSeconds(time);

        OnSeedsChosen.Invoke();
    }

    private void OnTransformChildrenChanged()
    {
        if (!isInstantiating)
        {
            isInstantiating = true;
            Invoke("InstantiateObject", spawnDelay);
        }
    }

    private void SetAnimator()
    {
        GameObject[] packs = GameObject.FindGameObjectsWithTag("FlowerPack");

        foreach (GameObject pack in packs)
        {
            if (pack.activeSelf)
            {
                animator = pack.GetComponent<Animator>();
            }
        }
    }

    private void InstantiateObject()
    {
        if (spawnPrefab != null)
        {
            GameObject seed = Instantiate(spawnPrefab, transform.position, transform.rotation, transform);

            SwipeScript seedSwipe = seed.GetComponent<SwipeScript>();

            seedSwipe.swipeArea = swipeArea;
            seedSwipe.SetFlowerPrefab(ReturnFlower());
        }

        numberOfSeeds--;

        if (numberOfSeeds <= 0)
        {
            if (OnSeedsDepleted != null)
            {
                OnSeedsDepleted.Invoke();
            }
        }

        isInstantiating = false;
    }

    private GameObject ReturnFlower()
    {
        int randomIndex = UnityEngine.Random.Range(0, usedFlowers.Count);

        return usedFlowers[randomIndex];
    }
}
