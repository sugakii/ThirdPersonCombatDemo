using UnityEngine;

/// <summary>
/// 接收 Imp Animator 的 Animation Event，并转发给 PlayerCombat。
/// 本组件只负责动画与战斗逻辑之间的桥接，不保存 Combo 状态。
/// </summary>
public class PlayerCombatAnimationEvents : MonoBehaviour
{
    private PlayerCombat playerCombat;

    // Animation Event 入口，仅转发动画时机，不在此处理 Combo 规则。
    public void OpenComboAdvance()
    {
        playerCombat.OpenComboAdvance();
    }

    public void OpenComboInput()
    {
        playerCombat.OpenComboInput();
    }

    public void EnterRecovery()
    {
        playerCombat.EnterRecovery();
    }

    public void EndAttack()
    {
        playerCombat.EndAttack();
    }

    private void Awake()
    {
        // PlayerCombat 位于父级 Player，缓存父级依赖供 Animation Event 转发使用。
        playerCombat = GetComponentInParent<PlayerCombat>();
    }
}
