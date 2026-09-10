using System;
using UnityEngine;

/// <summary>
/// 管理生命值初始化、扣减、死亡与恢复，并通过事件向表现层报告状态变化。
/// 本组件不引用 UI、Animator、Player 或 Enemy。
/// </summary>
public class Health : MonoBehaviour, IDamageable
{
    /// <summary>本次初始化确定的生命上限。</summary>
    public float MaxHealth { get; private set; }

    /// <summary>当前生命值，始终保持在 0 到 MaxHealth 之间。</summary>
    public float CurrentHealth { get; private set; }

    // 参数是完成钳制后的最新生命值，UI 等监听者不需要再次计算。
    public event Action<float> HealthChanged;

    // 只在生命值首次从正数降到 0 时触发。
    public event Action Died;

    /// <summary>设置生命上限并以满血状态开始；负数按 0 处理。</summary>
    public void Initialize(float hp)
    {
        if(hp < 0)
        {
            hp = 0;
        }

        MaxHealth = hp;
        CurrentHealth = MaxHealth;
    }

    /// <summary>应用正伤害；无效伤害和已经死亡的对象不会产生事件。</summary>
    public void TakeDamage(DamageInfo damageInfo)
    {
        float damageAmount = damageInfo.DamageAmount;

        if(damageAmount <= 0)
        {
            return;
        }

        if(CurrentHealth <= 0)
        {
            return;
        }

        CurrentHealth -= damageAmount;

        if(CurrentHealth < 0f)
        {
            CurrentHealth = 0f;
        }

        // 先广播最终 HP，再广播死亡，保证 UI 能先显示 0。
        HealthChanged?.Invoke(CurrentHealth);

        if(CurrentHealth == 0f)
        {
            Died?.Invoke();
        }
    }

    /// <summary>恢复到初始化时的生命上限，并通知生命值监听者。</summary>
    public void ResetHealth()
    {
        CurrentHealth = MaxHealth;

        HealthChanged?.Invoke(CurrentHealth);
    }
}
