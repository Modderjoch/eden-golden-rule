using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    public Slider ScaleSlider => scaleSlider;
    public Slider OffsetSlider => offsetSlider;
    public GameObject UIParent => uiParent;
    public Text ObjectName => objectName;

    private static InputManager instance;
    [SerializeField] private Slider scaleSlider;
    [SerializeField] private Slider offsetSlider;
    [SerializeField] private GameObject uiParent;
    [SerializeField] private Text objectName;

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

    private void Awake()
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
    }

    protected void Update()
    {
        // Check for mouse click or touch using the new Input System
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

            // Perform the raycast
            if (Physics.Raycast(ray, out hit))
            {
                // Check if the ray hit the collider of the 3D object with the "Test" tag
                if (hit.collider != null && hit.collider.gameObject.CompareTag("Test"))
                {
                    TestPanel testPanel = hit.collider.gameObject.GetComponent<TestPanel>();

                    testPanel.ToggleUI();

                    // The 3D object is clicked
                    Debug.Log("Clicked on 3D object!");
                }
            }
        }
    }}
