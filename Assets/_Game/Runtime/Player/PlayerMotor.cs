using UnityEngine;

[RequireComponent(
    typeof(CharacterController),
    typeof(PlayerInputReader)
    )]
/// <summary>
/// Player 的唯一 Gameplay 位移执行者。
/// 负责镜头空间移动、重力、角色朝向、反向转身和 Sprint。
/// </summary>
public class PlayerMotor : MonoBehaviour
{
    /// <summary>
    /// CharacterController 完成碰撞处理后的实际水平速度，供动画系统读取。
    /// </summary>
    public float CurrentMoveSpeed { get; private set; }

    // 普通移动速度（米/秒）。
    [SerializeField]
    private float moveSpeed = 5f;

    // 垂直加速度；负值表示向下。
    [SerializeField]
    private float gravity = -9.81f;

    // 当前使用 Player 内部 CameraTarget，提供镜头空间的 forward/right 基准。
    [SerializeField]
    private Transform cameraTransform;

    // 普通转向与接近 180°反向转身的角速度（度/秒）。
    [SerializeField]
    private float rotationSpeed = 720f;

    [SerializeField]
    private float reverseTurnSpeed = 1440f;

    // 按住 Sprint 时使用的移动速度（米/秒）。
    [SerializeField]
    private float sprintSpeed = 10f;

    // CharacterController 不自动应用重力，因此需要保留跨帧垂直速度。
    private float verticalVelocity;
    private CharacterController controller;
    private PlayerInputReader inputReader;
    private Vector2 moveInput;

    // 反向转身期间锁定初始目标方向，避免在临界角度反复切换转向策略。
    private bool isReverseTurning = false;
    private Vector3 reverseTurnDirection;

    void Awake()
    {
        // cameraTransform 是 Inspector 引用；缺失时尽早停止，避免每帧刷空引用异常。
        if(cameraTransform == null)
        {
            Debug.LogError("PlayerMotor: Camera Transform is not assigned.", this);
            enabled = false;
            return;
        }

        controller = GetComponent<CharacterController>();
        inputReader = GetComponent<PlayerInputReader>();
    }

    void Update()
    {
        // Sprint 只切换速度，不改变输入向量，也不会在静止时主动产生位移。
        float currentSpeed = moveSpeed;

        if(inputReader.SprintHeld)
        {
            currentSpeed = sprintSpeed;
        }

        // 去掉镜头的垂直分量，只在地面 XZ 平面上计算移动方向。
        Vector3 forward = cameraTransform.forward;

        forward.y = 0f;
        forward.Normalize();

        Vector3 right = cameraTransform.right;

        right.y = 0f;
        right.Normalize();

        moveInput = inputReader.MoveInput;

        // 接地时施加轻微向下速度，使 CharacterController 稳定贴住地面。
        if(controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        // 将二维输入转换成相对镜头的三维世界方向。
        Vector3 moveDirection = forward * moveInput.y + right * moveInput.x;

        // 防止 W+D 等斜向输入的长度大于 1，造成斜向速度增益。
        moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);

        if(moveDirection.sqrMagnitude > 0.001f)
        {
            // 点积接近 -1 表示目标方向位于角色正后方。
            float facingDot = Vector3.Dot(transform.forward, moveDirection.normalized);

            float temporarySpeed = rotationSpeed;

            Vector3 turnDirection = moveDirection.normalized;

            if(!isReverseTurning && facingDot <= -0.8f)
            {
                // 进入反向转身时记录目标方向，直到转身完成再恢复普通追踪。
                isReverseTurning = true;
                reverseTurnDirection = moveDirection.normalized;
            }

            if(isReverseTurning)
            {
                turnDirection = reverseTurnDirection;
                temporarySpeed = reverseTurnSpeed;
            }

            Quaternion targetRotation = Quaternion.LookRotation(turnDirection);

            // RotateTowards 使用角速度限制转向，避免瞬间改变朝向。
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

        // 所有水平移动和重力最终只通过 CharacterController 执行一次。
        controller.Move(velocity * Time.deltaTime);

        // 使用碰撞处理后的实际速度驱动动画，撞墙时动画能够随之降速。
        Vector3 actualVelocity = controller.velocity;

        actualVelocity.y = 0f;

        CurrentMoveSpeed = actualVelocity.magnitude;
    }
}
