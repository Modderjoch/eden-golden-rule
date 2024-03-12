using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trash : MonoBehaviour
{
    public int Score => score;

    [SerializeField] private int score = 1;

    public bool PickUp()
    {
        Destroy(gameObject);

        return true;
    }
}
