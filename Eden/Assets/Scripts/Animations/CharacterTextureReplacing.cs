// Copyright Oebe Rademaker All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterTextureReplacing : MonoBehaviour
{
    public List<CharacterPose> poses = new List<CharacterPose>();
    public Material material;

    private Dictionary<Texture, float> textures = new Dictionary<Texture, float>();
    private Coroutine changeTextureCoroutine;

    public void SetPose(string pose)
    {
        textures.Clear();

        for (int i = 0; i < poses.Count; i++)
        {
            if (poses[i].name == pose)
            {
                foreach (CharacterSubPose subPose in poses[i].subPoses)
                {
                    textures.Add(subPose.texture, subPose.delayToNextPose);
                }

                if (changeTextureCoroutine != null)
                {
                    CoroutineHandler.Instance.StopCoroutine(changeTextureCoroutine);
                }

                changeTextureCoroutine = CoroutineHandler.Instance.StartCoroutine(ChangeTextureOverTime());
            }
        }
    }

    private IEnumerator ChangeTextureOverTime()
    {
        foreach (var kvp in textures)
        {
            material.mainTexture = kvp.Key;
            yield return new WaitForSeconds(kvp.Value);
        }
    }
}

[Serializable]
public class CharacterPose
{
    public string name;
    public List<CharacterSubPose> subPoses;
    [TextArea(3, 10)]
    public string description;
}

[Serializable]
public class CharacterSubPose
{
    public string name;
    public float delayToNextPose;
    public Texture texture;
    [TextArea(3, 10)]
    public string description;
}
