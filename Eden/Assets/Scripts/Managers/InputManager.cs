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

    [SerializeField] private float minimumSwipe = 10f;
    private Vector2 swipeDirection;

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
        playerControls = new PlayerControls();
        playerControls.Player.Enable();
        playerControls.Player.Touch.canceled += ProcessTouchComplete;
        playerControls.Player.Swipe.performed += ProcessSwipeDelta;
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

            if (Physics.Raycast(ray, out hit))
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
                }                
            }
        }
    }

    private void ProcessSwipeDelta(InputAction.CallbackContext context)
    {
        swipeDirection = context.ReadValue<Vector2>();
    }

    private void ProcessTouchComplete(InputAction.CallbackContext context)
    {
        Debug.Log("touch complete");
        if (Mathf.Abs(swipeDirection.magnitude) < minimumSwipe) return;
        Debug.Log("Swipe detected");

        var position = Vector3.zero;

        if(swipeDirection.x > 0)
        {
            Debug.Log("Swipe right");
            position.x = 1;
        }
        else if(swipeDirection.x < 0)
        {
            Debug.Log("Swipe left");
            position.x = -1;
        }

        if(swipeDirection.y > 0)
        {
            Debug.Log("Swipe up");
            position.y = 1;
        }
        else if(swipeDirection.y < 0)
        {
            Debug.Log("Swipe down");
            position.y = -1;
        }

        swipeDirection = Vector2.zero;
    }
}
