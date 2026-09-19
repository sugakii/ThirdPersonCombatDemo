using UnityEngine;

/// <summary>
/// 将 Puglin 攻击动画上的 Hit/EndAttack 事件转发给父级 EnemyCombat。
/// </summary>
public class EnemyCombatAnimationEvents : MonoBehaviour
{
    private EnemyCombat enemyCombat;

    public void Hit()
    {
        enemyCombat.Hit();
    }

    public void EndAttack()
    {
        enemyCombat.EndAttack();
    }

    private void Awake()
    {
        // Animator 位于 Model 子物体，攻击运行时组件位于 Puglin 根节点。
        enemyCombat = GetComponentInParent<EnemyCombat>();
    }
}
