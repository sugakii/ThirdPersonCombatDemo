using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
/// <summary>
/// 将 PlayerMotor 的实际水平速度同步到 Animator。
/// Animator 只负责表现，不参与 Player 的 Gameplay 位移。
/// </summary>
public class PlayerAnimatorDriver : MonoBehaviour
{
    // 指向 Player/Imp 上实际启用的 Animator。
    [SerializeField]
    private Animator animator;

    private PlayerMotor playerMotor;

    void Awake()
    {
        // Animator 位于子对象，无法使用 RequireComponent 自动保证引用。
        if(animator == null)
        {
            Debug.LogError("PlayerAnimatorDriver: Animator is not assigned.", this);
            enabled = false;
            return;
        }

        playerMotor = GetComponent<PlayerMotor>();
    }

    void LateUpdate()
    {
        // 在 Motor 完成当帧移动后再同步速度，使动画反映碰撞后的真实结果。
        animator.SetFloat("Speed", playerMotor.CurrentMoveSpeed);
    }
}
