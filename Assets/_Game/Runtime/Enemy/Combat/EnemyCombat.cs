using UnityEngine;

/// <summary>
/// 负责单次 Enemy 攻击的动画启动、命中帧距离复核与伤害结算。
/// 不决定 AI 状态，也不依赖 Player 的具体实现。
/// </summary>
public class EnemyCombat : MonoBehaviour
{
    [SerializeField]
    private AttackDefinition attackDefinition;

    public bool IsAttacking { get; private set; }

    private Transform target;

    [SerializeField]
    private Animator animator;

    public void Hit()
    {
        // Animation Event 到达时目标可能已经离场，必须在结算前重新验证。
        if(target == null || !target.gameObject.activeInHierarchy)
        {
            return;
        }

        IDamageable hitTarget = target.GetComponent<IDamageable>();

        if(hitTarget == null)
        {
            return;
        }

        // 动画开始后目标仍可能离开攻击范围，因此不能只依赖状态机进入 Attack 时的距离。
        float distance = Vector3.Distance(transform.position, target.position);

        if(distance > attackDefinition.AttackRange)
        {
            return;
        }

        DamageInfo damageInfo = new DamageInfo(attackDefinition.Damage);

        hitTarget.TakeDamage(damageInfo);
    }

    public void TryAttack(Transform target)
    {
        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

        if(IsAttacking ||
        animator.IsInTransition(0) ||
        state.shortNameHash ==
            Animator.StringToHash(attackDefinition.AttackStateName))
        {
            return;
        }

        IsAttacking = true;
        this.target = target;
        animator.CrossFade(
            attackDefinition.AttackStateName,
            0.05f
        );

    }

    public void EndAttack()
    {
        IsAttacking = false;
        target = null;
    }
}
