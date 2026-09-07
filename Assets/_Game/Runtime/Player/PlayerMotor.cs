using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 5f;

    [SerializeField]
    private float gravity = -9.81f;

    [SerializeField]
    private Transform cameraTransform;

    [SerializeField]
    private float rotationSpeed = 720f;

    [SerializeField]
    private float reverseTurnSpeed = 1440f;

    [SerializeField]
    private float sprintSpeed = 8f;

    private float verticalVelocity;
    private CharacterController controller;
    private PlayerInputReader inputReader;
    private Vector2 moveInput;
    private bool isReverseTurning = false;
    private Vector3 reverseTurnDirection;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        inputReader = GetComponent<PlayerInputReader>();
    }

    void Update()
    {
        float currentSpeed = moveSpeed;

        if(inputReader.SprintHeld)
        {
            currentSpeed = sprintSpeed;
        }

        Vector3 forward = cameraTransform.forward;

        forward.y = 0f;
        forward.Normalize();

        Vector3 right = cameraTransform.right;

        right.y = 0f;
        right.Normalize();

        moveInput = inputReader.MoveInput;

        if(controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        Vector3 moveDirection = forward * moveInput.y + right * moveInput.x;

        moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);

        if(moveDirection.sqrMagnitude > 0.001f)
        {
            float facingDot = Vector3.Dot(transform.forward, moveDirection.normalized);

            float temporarySpeed = rotationSpeed;

            Vector3 turnDirection = moveDirection.normalized;

            if(!isReverseTurning && facingDot <= -0.8f)
            {
                isReverseTurning = true;
                reverseTurnDirection = moveDirection.normalized;
            }

            if(isReverseTurning)
            {
                turnDirection = reverseTurnDirection;
                temporarySpeed = reverseTurnSpeed;
            }

            Quaternion targetRotation = Quaternion.LookRotation(turnDirection);

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                temporarySpeed * Time.deltaTime
            );

            float remainingAngle =  Quaternion.Angle(transform.rotation, targetRotation);

            if(isReverseTurning && remainingAngle <= 2f)
            {
                isReverseTurning = false;
            }
        }

        if(moveDirection.sqrMagnitude <= 0.001f)
        {
            isReverseTurning = false;
        }

        Vector3 velocity = moveDirection * currentSpeed;

        velocity.y = verticalVelocity;
        
        controller.Move(velocity * Time.deltaTime);
    }
}
