using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputReader : MonoBehaviour
{
    public Vector2 LookInput { get; private set; }
    public Vector2 MoveInput { get; private set; }

    private PlayerInput playerInput;
    private InputAction lookAction;
    private InputAction moveAction;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        lookAction = playerInput.actions["Look"];
        moveAction = playerInput.actions["Move"];
    }
    void Update()
    {
        LookInput = lookAction.ReadValue<Vector2>();
        MoveInput = moveAction.ReadValue<Vector2>();
    }
}
