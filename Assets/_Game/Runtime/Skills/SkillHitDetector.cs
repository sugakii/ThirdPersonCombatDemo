using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 在技能位移期间检测可受伤目标，并保证同一次技能对同一目标只结算一次伤害。
/// </summary>
public class SkillHitDetector : MonoBehaviour
{
    [SerializeField]
    private Transform hitboxCenter;

    [SerializeField]
    private float hitboxRadius;

    [SerializeField]
    private LayerMask targetMask;

    private bool detectionActive;

    // 按 IDamageable 去重，而不是按 Collider 去重，避免多碰撞体目标重复受伤。
    private readonly HashSet<IDamageable> hitTargets = new();

    private DamageInfo currentDamageInfo;

    public void Initialize(Transform center, float radius, LayerMask mask)
    {
        hitboxCenter = center;
        hitboxRadius = radius;
        targetMask = mask;
    }

    public void BeginDetection(DamageInfo damageInfo)
    {
        currentDamageInfo = damageInfo;

        // 每次技能开始都是新的命中周期，上一轮目标不能影响本轮结算。
        hitTargets.Clear();

        detectionActive = true;
    }

    public void EndDetection()
    {
        detectionActive = false;
    }

    private void DetectTargets()
    {
        Collider[] hitColliders = Physics.OverlapSphere(
            hitboxCenter.position,
            hitboxRadius,
            targetMask
        );

        foreach(Collider hitCollider in hitColliders)
        {
            IDamageable hitTarget = hitCollider.GetComponentInParent<IDamageable>();

            if(hitTarget == null)
            {
                continue;
            }

            if(!hitTargets.Add(hitTarget))
            {
                continue;
            }

            hitTarget.TakeDamage(currentDamageInfo);
        }
    }

    private void Update()
    {
        if(!detectionActive)
        {
            return;
        }

        DetectTargets();
    }
}
