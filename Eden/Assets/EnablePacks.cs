using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnablePacks : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> packs = new List<GameObject>();

    public void EnablePacksNow()
    {
        foreach (GameObject pack in packs)
        {
            pack.SetActive(true);

            pack.GetComponent<MoveToMiddle>().ResetToStartingPosition();
        }
    }
}
