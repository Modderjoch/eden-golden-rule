using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetGrassOnStart : MonoBehaviour
{
    [SerializeField] private Material GrassMaterial;
    [SerializeField] private Material GrassClumpMaterial;

    protected void Start()
    {
        GrassMaterial.SetFloat("_GrassLerp", 1);
        GrassClumpMaterial.SetFloat("_GrassClump_Lerp", 1);
    }
}
