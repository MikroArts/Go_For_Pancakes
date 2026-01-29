using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.UI;

public class CursorUI : MonoBehaviour
{
    [SerializeField] private RectTransform canvasRectTransform;

    private VirtualMouseInput virtualMouseInput;

    private CanvasGroup canvasGroup;
    private bool isGamepadConnected = false;

    private void Awake()
    {
        virtualMouseInput = GetComponent<VirtualMouseInput>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }
    private void OnEnable()
    {
        InputSystem.onDeviceChange += OnDeviceChange;
        CheckForGamepad();
    }
    private void OnDisable()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
    }
    private void Update()
    {
        if (!isGamepadConnected) return;

        transform.localScale = 1f / canvasRectTransform.localScale.x * Vector3.one;
        transform.SetAsLastSibling();
    }

    private void LateUpdate()
    {
        if (!isGamepadConnected) return;

        Vector2 virtualMousePosition = virtualMouseInput.virtualMouse.position.value;
        virtualMousePosition.x = Mathf.Clamp(virtualMousePosition.x, 0, Screen.width);
        virtualMousePosition.y = Mathf.Clamp(virtualMousePosition.y, 0, Screen.height);

        InputState.Change(virtualMouseInput.virtualMouse.position, virtualMousePosition);
    }
    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        CheckForGamepad();
    }

    private void CheckForGamepad()
    {
        isGamepadConnected = Gamepad.current != null;

        canvasGroup.alpha = isGamepadConnected ? 1f : 0f;
        canvasGroup.blocksRaycasts = isGamepadConnected;
    }

    public void Hide()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
    }

    public void Show()
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }
}
