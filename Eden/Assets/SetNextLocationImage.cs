using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetNextLocationImage : MonoBehaviour
{
    [SerializeField] private Image nextLocationImage;

    public void SetNextImage(Sprite sprite)
    {
        nextLocationImage.sprite = sprite;
    }
}
