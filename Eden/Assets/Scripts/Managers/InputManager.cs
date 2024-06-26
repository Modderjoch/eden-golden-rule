// Copyright Oebe Rademaker All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Manager that is responsible for managing interactions
/// consequently also manages what needs to happen based on
/// the object that was interacted with
/// </summary>
public class InputManager : MonoBehaviour
{
    private static InputManager instance;
    private ScoreManager scoreManager;

    private PlayerControls playerControls;

    [SerializeField] private SeedManager seedManager;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private bool useSeed;

    public static InputManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<InputManager>();

                if (instance == null)
                {
                    GameObject obj = new GameObject("InputManager");
                    instance = obj.AddComponent<InputManager>();
                }
            }

            return instance;
        }
    }

    protected void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        scoreManager = ScoreManager.Instance;
    }

    protected void Start()
    {
        if (useSeed)
        {
            playerControls = new PlayerControls();
            playerControls.Player.Enable();
        }
    }

    protected void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame || Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            Ray ray;

            if (Touchscreen.current != null && Touchscreen.current.touches.Count > 0)
            {
                ray = Camera.main.ScreenPointToRay(Touchscreen.current.touches[0].position.ReadValue());
            }
            else
            {
                ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            }

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, layerMask))
            {
                GameObject hitObject = hit.collider.gameObject;

                if(hitObject.tag == "Interactable")
                {
                    Interactable interactable = hitObject.GetComponent<Interactable>();

                    if (interactable is Trash trash)
                    {
                        if (trash.PickUp())
                        {
                            scoreManager.AddScore(trash);
                        }
                    }
                    else if (interactable is Paper paper)
                    {
                        if (paper.PickUp())
                        {
                            scoreManager.AddScore(paper);
                        }
                    }
                    else if(interactable is Bracelet bracelet)
                    {
                        bracelet.CollectBracelet();
                    }
                    else if(interactable is Compass compass)
                    {
                        compass.CollectCompass();
                    }
                }                
            }
        }
    }
}
