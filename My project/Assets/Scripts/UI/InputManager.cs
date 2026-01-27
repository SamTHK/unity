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
    [SerializeField] private RectTransform rectTrans;
    [SerializeField] private GameObject virMouseObject;
    
    private GameObject virMouse;
    public bool usingVirtualMouse;
    PlayerInput input;
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    void Awake()
    {
        input = GetComponent<PlayerInput>();
  
        virMouse = Instantiate(virMouseObject, rectTrans, false);
        
        
        if (!input.currentControlScheme.Equals("Keyboard&Mouse"))
        {
            UnityEngine.Cursor.visible = false;
            virMouse.SetActive(true);
            usingVirtualMouse = true;
        }
        else
        {
            UnityEngine.Cursor.visible = true;
            virMouse.SetActive(false);
            usingVirtualMouse = false;
        }    


            input.onControlsChanged += Checkagain;
    }

    void Checkagain(PlayerInput input)
    {

        bool isMouse = input.currentControlScheme.Equals("Keyboard&Mouse");
        if (isMouse)
        {
            UnityEngine.Cursor.visible = true;
            virMouse.SetActive(false);
            usingVirtualMouse = false;
        }
        else
        {
            UnityEngine.Cursor.visible = false;
            virMouse.SetActive(true);
            usingVirtualMouse = true;
        }
    }
    

    
}
