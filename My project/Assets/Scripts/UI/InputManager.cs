using CodeMonkey.Utils;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.UI;
using UnityEngine.UIElements;



public class InputManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public bool usingVirtualMouse;
    PlayerInput input;
    public InputAction action;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] VirMouse mouse;

    void Awake()
    {
        input = GetComponent<PlayerInput>();
    }

    void Start()
    {
        Checkagain();
    }

    public void Checkagain()
    {
        if (input != null)
        {
            if (input.currentControlScheme == "Keyboard&Mouse")
            {
                usingVirtualMouse = false;
                mouse.gameObject.SetActive(false);
                UnityEngine.Cursor.visible = true;
                Mouse.current.WarpCursorPosition(new Vector2(Screen.width / 2, Screen.height / 2));
                
            }
            else
            {
                usingVirtualMouse = true;
                UnityEngine.Cursor.visible = false;
                mouse.gameObject.SetActive(true);

            }
        }
    }

   

}
