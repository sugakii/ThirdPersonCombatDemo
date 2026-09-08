using System;
using UnityEngine;

[RequireComponent(typeof(PlayerInputReader))]
/// <summary>
/// 消费 PlayerInputReader 提供的视角输入，并旋转 Cinemachine 跟踪的 CameraTarget。
/// 本组件只决定观察方向，最终摄像机位置仍由 Cinemachine 计算。
/// </summary>
public class CameraController : MonoBehaviour
{
    // Player 层级内的镜头目标；CinemachineCamera 会跟踪它的旋转。
    [SerializeField]
    private Transform cameraTarget;

    // 将 Input System 的 Look 输入缩放为每帧旋转角度。
    [SerializeField]
    private float mouseSensitivity = 3f;

    private PlayerInputReader inputReader;

    // 分别保存上下俯仰角（Pitch）和水平偏航角（Yaw）。
    private float xRotation;
    private float yRotation;

    private void Awake()
    {
        // Inspector 引用无法由 RequireComponent 保证，因此启动时主动失败并停止 Update。
        if(cameraTarget == null)
        {
            Debug.LogError("CameraController: Camera Target is not assigned.", this);
            enabled = false;
            return;
        }

        inputReader = GetComponent<PlayerInputReader>();
    }

    void Update()
    {
        // CameraController 不直接读取鼠标，保持输入设备与镜头行为解耦。
        Vector2 lookinput = inputReader.LookInput;

        float mouseX = lookinput.x;
        float mouseY = lookinput.y;

        yRotation += mouseX * mouseSensitivity;
        xRotation -= mouseY * mouseSensitivity;

        // 限制俯仰角，避免镜头越过头顶后发生上下翻转。
        xRotation = Mathf.Clamp(
            xRotation,
            -40f,
            70f
        );

        // 写入目标的世界旋转；Cinemachine 随后根据目标姿态更新真实摄像机。
        cameraTarget.rotation =
        Quaternion.Euler(
            xRotation,
            yRotation,
            0
        );
    }
}
