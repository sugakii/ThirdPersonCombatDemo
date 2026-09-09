/// <summary>
/// 描述一次伤害结算所需的数据。
/// 当前 Day 7 只携带伤害值；攻击来源、命中点和方向在战斗系统实际需要时再加入。
/// </summary>
public struct DamageInfo
{
    /// <summary>本次伤害的原始数值；创建后不可从外部修改。</summary>
    public float DamageAmount { get; }

    /// <summary>创建一个只读的伤害数据包。</summary>
    public DamageInfo(float damageAmount)
    {
        this.DamageAmount = damageAmount;
    }
}
