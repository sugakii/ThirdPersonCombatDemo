using UnityEngine;
using System.Collections.Generic;

/// <summary>
///  负责近战伤害窗口中的目标检测、单次攻击命中去重和伤害转发。
///  不负责 Combo 流程、动画切换或目标生命值规则。 
/// </summary>
public class MeleeHitbox : MonoBehaviour
{
    // 判定中心独立于 Player 根节点，便于根据武器和动作调整攻击范围。
   [SerializeField]
   private Transform hitboxCenter;

   [SerializeField, Min(0f)]
   private float hitboxRadius;

   [SerializeField]
   private LayerMask targetMask;

   // 只有动画窗口打开时才查询，避免待机和移动阶段持续执行物理检测。
   private bool damageWindowOpen;

    // 记录当前攻击窗口已经命中的目标，防止多 Collider 或多帧检测造成重复伤害。
   private readonly HashSet<IDamageable> hitTargets = new();

    // 缓存当前伤害窗口的数据，使 Hitbox 不需要依赖 Combo 或 AttackDefinition。
   private DamageInfo currentDamageInfo;

   public void OpenDamageWindow(DamageInfo damageInfo)
    {
        currentDamageInfo = damageInfo;

        // 每次新攻击窗口都清空上一击的命中记录，允许下一段攻击再次命中同一目标。
        hitTargets.Clear();

        damageWindowOpen = true;
    }

    public void CloseDamageWindow()
    {
        damageWindowOpen = false;
    }

    private void DetectTargets()
    {
        Collider[] hitColliders = Physics.OverlapSphere(
            hitboxCenter.position,
            hitboxRadius,
            targetMask
        );

        // 物理查询可能返回同一目标的多个 Collider，后续按 IDamageable 去重。
        foreach(Collider hitCollider in hitColliders)
        {
            Vector3 directionToTarget = hitCollider.bounds.center - hitboxCenter.position;

            // 球形查询只负责距离筛选，这里再过滤攻击方向后的目标。
            if(Vector3.Dot(transform.forward, directionToTarget) < 0f)
            {
                continue;
            }

            IDamageable target = hitCollider.GetComponentInParent<IDamageable>();

            if(target == null)
            {
                continue;
            }

            // HashSet.Add 只有首次加入目标时返回 true，同一攻击窗口内重复 Collider 不再造成伤害。
            if(!hitTargets.Add(target))
            {
                continue;
            }

            target.TakeDamage(currentDamageInfo);
        }
    }

    // 在 Scene 中显示实际物理查询范围，便于调整攻击中心和半径。
    private void OnDrawGizmosSelected()
    {
        if(hitboxCenter == null)
        {
            return;
        }

        Gizmos.DrawWireSphere(
            hitboxCenter.position,
            hitboxRadius
        );

        // 标出方向过滤使用的正前方，避免把完整球形 Gizmo 误认为实际有效范围。
        Gizmos.DrawLine(
            hitboxCenter.position,
            hitboxCenter.position + transform.forward * hitboxRadius
        );
    }

    private void Awake()
    {
        if(hitboxCenter == null)
        {
            Debug.LogError(
                "MeleeHitbox: Hitbox Center is not assigned.",
                this
            );

            enabled = false;
        }
    }

    private void Update()
    {
        // 只在动画定义的伤害窗口内进行物理查询，避免非攻击阶段产生命中。
        if(!damageWindowOpen)
        {
            return;
        }

        DetectTargets();
    }
}
