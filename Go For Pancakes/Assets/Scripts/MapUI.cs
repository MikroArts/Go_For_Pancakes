using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] private GameObject mapUI;
    private PlayerInput playerInput;

    private void Awake()
    {
        playerInput = new PlayerInput();
        playerInput.Player.Enable();
        mapUI.SetActive(false);
    }

    private void Update()
    {
        mapUI.SetActive(playerInput.Player.Map.IsPressed());
    }

    private void OnDestroy()
    {
        playerInput.Player.Disable();
    }
}
