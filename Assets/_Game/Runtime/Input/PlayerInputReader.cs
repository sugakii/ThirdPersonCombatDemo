using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
/// <summary>
/// 项目的输入边界：从 Input System 读取设备状态，并向 Gameplay 层暴露输入意图。
/// 下游系统不需要知道输入来自键盘、鼠标还是其他控制设备。
/// </summary>
public class PlayerInputReader : MonoBehaviour
{
    // 只允许本组件写入，其他系统只能读取当前帧的输入快照。
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

        // 在初始化阶段缓存 Action，避免在每帧通过名称重复查询。
        lookAction = playerInput.actions["Look"];
        moveAction = playerInput.actions["Move"];
        sprintAction = playerInput.actions["Sprint"];
    }

    void Update()
    {
        // 每帧生成一次输入快照，供 CameraController 与 PlayerMotor 消费。
        LookInput = lookAction.ReadValue<Vector2>();
        MoveInput = moveAction.ReadValue<Vector2>();

        // IsPressed 同时支持按下阈值和不同控制设备，不把逻辑绑定到具体按键。
        SprintHeld = sprintAction.IsPressed();
    }
}
