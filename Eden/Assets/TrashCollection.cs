using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrashCollection : MonoBehaviour
{
    [SerializeField] private List<TrashItem> items = new List<TrashItem>();

    protected void Awake()
    {
        GameManager.Instance.SetTrashList(items);
    }
}

[Serializable]
public class TrashItem
{
    public Trash trash;
    public Sprite icon;
}
