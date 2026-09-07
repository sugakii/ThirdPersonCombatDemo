using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]

public class PlayerInputReader : MonoBehaviour
{
    public Vector2 LookInput { get; private set; }
    public Vector2 MoveInput { get; private set; }
    public bool SprintHeld { get; private set; }


    private PlayerInput playerInput;
    private InputAction lookAction;
    private InputAction moveAction;
    private InputAction sprintAction;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        lookAction = playerInput.actions["Look"];
        moveAction = playerInput.actions["Move"];
        sprintAction = playerInput.actions["Sprint"];
    }
    void Update()
    {
        LookInput = lookAction.ReadValue<Vector2>();
        MoveInput = moveAction.ReadValue<Vector2>();

        SprintHeld = sprintAction.IsPressed();
    }
}
