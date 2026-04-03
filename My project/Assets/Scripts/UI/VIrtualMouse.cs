using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.UI;

public class VirMouse : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private RectTransform rectTransform;
    [SerializeField] private RectTransform mouseTransform;
    [SerializeField] private VirtualMouseInput virtualMouseInput;
    [SerializeField] private float padding = 80f;
    private void Awake()
    {
        rectTransform = (RectTransform)transform.parent.transform;

    }
    private void Start()
    {
        mouseTransform.position = new Vector2(Screen.width / 2, Screen.height / 2) * rectTransform.localScale.x;
        mouseTransform.localScale = rectTransform.localScale;
        InputState.Change(virtualMouseInput.virtualMouse.position, new Vector2(Screen.width / 2, Screen.height / 2));
    }
    private void OnEnable()
    {

        mouseTransform.position = new Vector2(Screen.width / 2, Screen.height / 2) * rectTransform.localScale.x;
        mouseTransform.localScale = rectTransform.localScale;
        if (virtualMouseInput.virtualMouse != null)
        {
            InputState.Change(virtualMouseInput.virtualMouse.position, new Vector2(Screen.width / 2, Screen.height / 2));
        }
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
