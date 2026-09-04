using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 5f;

    [SerializeField]
    private float gravity = -9.81f;

    private float verticalVelocity;
    private CharacterController controller;
    private PlayerInputReader inputReader;
    private Vector2 MoveInput;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        inputReader = GetComponent<PlayerInputReader>();
    }

    void Update()
    {
        MoveInput = inputReader.MoveInput;

        if(controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        Vector3 moveDirection = new Vector3
        (
            MoveInput.x,
            0f,
            MoveInput.y
        );

        moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);

        Vector3 velocity = moveDirection * moveSpeed;

        velocity.y = verticalVelocity;
        
        controller.Move(velocity * Time.deltaTime);
    }
}
