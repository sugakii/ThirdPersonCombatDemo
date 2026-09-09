/// <summary>
/// 统一 Player、Enemy 和其他可受伤对象的伤害入口，使攻击方不依赖具体目标类型。
/// </summary>
public interface IDamageable
{
    /// <summary>接收并处理一次伤害。</summary>
    void TakeDamage(DamageInfo damageInfo);
}
