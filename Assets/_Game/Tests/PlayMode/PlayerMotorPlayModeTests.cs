using UnityEngine;
using UnityEngine.SceneManagement;
using NUnit.Framework;
using System.Collections;
using UnityEngine.TestTools;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

/// <summary>
/// 在真实 SampleScene 中验证 PlayerMotor 与 Input System 的关键移动规则。
/// </summary>
public class PlayerMotorPlayModeTests
{
    // 每个测试使用独立的虚拟键盘，不依赖测试机器上的真实设备输入。
    private Keyboard keyboard;

    [SetUp]
    public void SetUp()
    {
        // Arrange：为 Input System 注册本测试专用的输入设备。
        keyboard = InputSystem.AddDevice<Keyboard>();
    }

    [TearDown]
    public void TearDown()
    {
        // 防止虚拟设备泄漏到下一个测试，保证测试之间相互隔离。
        InputSystem.RemoveDevice(keyboard);
    }

    /// <summary>
    /// 验证组合输入经过限幅后，不会产生额外的斜向移动速度。
    /// </summary>
    [UnityTest]
    public IEnumerator DiagonalInput_DoesNotExceedMoveSpeed()
    {
        // Arrange：加载真实 Gameplay 场景，并取得被测 PlayerMotor。
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync("SampleScene");

        yield return loadOperation;

        yield return null;

        PlayerMotor playerMotor = Object.FindAnyObjectByType<PlayerMotor>();

        Assert.IsNotNull(playerMotor);

        PlayerInput playerInput = playerMotor.GetComponent<PlayerInput>();

        // 强制 PlayerInput 使用虚拟键盘，确保后续事件进入正确控制方案。
        playerInput.SwitchCurrentControlScheme(keyboard);

        // Act：先输入 W，记录普通前进速度作为比较基线。
        InputSystem.QueueStateEvent(
            keyboard,
            new KeyboardState(Key.W)
        );

        InputSystem.Update();

        yield return null;
        yield return null;

        float forwardSpeed = playerMotor.CurrentMoveSpeed;

        // Assert：先确认角色确实移动，避免“两个速度都是 0”造成假通过。
        Assert.Greater(forwardSpeed, 0.1f);

        // Act：改为 W+D，记录斜向速度。
        InputSystem.QueueStateEvent(
            keyboard,
            new KeyboardState(Key.W, Key.D)
        );

        InputSystem.Update();

        yield return null;
        yield return null;

        float diagonalSpeed = playerMotor.CurrentMoveSpeed;

        // Assert：允许少量帧时序误差，但斜向速度不能高于单方向速度。
        Assert.LessOrEqual(
            diagonalSpeed,
            forwardSpeed + 0.1f
        );
    }

    /// <summary>
    /// 目标是验证松开 Sprint 后恢复普通速度；当前实现仍缺少 Sprint 与释放阶段断言。
    /// </summary>
    [UnityTest]
    public IEnumerator SprintRelease_RestoresNormalSpeed()
    {
        // Arrange：加载场景、获取 Player，并绑定虚拟键盘。
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync("SampleScene");

        yield return loadOperation;
        
        yield return null;

        PlayerMotor playerMotor = Object.FindAnyObjectByType<PlayerMotor>();

        Assert.IsNotNull(playerMotor);

        PlayerInput playerInput = playerMotor.GetComponent<PlayerInput>();

        playerInput.SwitchCurrentControlScheme(keyboard);

        // Act：输入 W，取得普通移动速度基线。
        InputSystem.QueueStateEvent(
            keyboard,
            new KeyboardState(Key.W)
        );

        InputSystem.Update();

        yield return null;
        yield return null;

        float normalSpeed = playerMotor.CurrentMoveSpeed;

        // 当前断言只证明普通移动已经开始；Sprint 按下与松开恢复断言仍需补全。
        Assert.Greater(normalSpeed, 0.1f);
    }
}
