using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.UI;

public class VirtualMouse : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private RectTransform rectTransform;
    private VirtualMouseInput virtualMouseInput;
    private float padding = 40f;
    void Start()
    {
        rectTransform = (RectTransform)transform.parent.transform;
        transform.position = new Vector2(Screen.width / 2, Screen.height / 2);
        virtualMouseInput = GetComponent<VirtualMouseInput>();
        InputState.Change(virtualMouseInput.virtualMouse.position, new Vector2(Screen.width / 2, Screen.height / 2));
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.localScale = Vector3.one * (1f / rectTransform.localScale.x);
        transform.SetAsLastSibling();
        Vector2 virtualMouseposition = virtualMouseInput.virtualMouse.position.value;
        virtualMouseposition.x = Mathf.Clamp(virtualMouseposition.x, padding, Screen.width - padding);
        virtualMouseposition.y = Mathf.Clamp(virtualMouseposition.y, padding, Screen.height - padding);
        InputState.Change(virtualMouseInput.virtualMouse.position, virtualMouseposition);
    }
}
