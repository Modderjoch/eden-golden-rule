using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterTextureReplacing : MonoBehaviour
{
    public List<CharacterPose> poses = new List<CharacterPose>();
    public Material material;

    public void SetPose(string pose)
    {
        for (int i = 0; i < poses.Count; i++)
        {
            if(poses[i].name == pose)
            {
                material.mainTexture = poses[i].texture;
            }
        }
    }
}

[Serializable]
public class CharacterPose
{
    public string name;
    public Texture texture;
    [TextArea(3, 10)]
    public string description;
}
