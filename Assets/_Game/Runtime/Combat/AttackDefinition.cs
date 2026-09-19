using UnityEngine;

/// <summary>
/// 保存一段攻击不会随运行过程改变的配置；Combo 序号和输入缓存不写入该资产。
/// </summary>
[CreateAssetMenu(
    fileName = "AttackDefinition",
    menuName = "Game/Combat/AttackDefinition"
)]
public class AttackDefinition : ScriptableObject
{
    // 由攻击结算系统读取，配置资产本身不负责寻找或伤害目标。
    [SerializeField]
    private float damage = 10f;

    // 必须与 Animator Controller 中对应 State 的名称完全一致。
    [SerializeField]
    private string attackStateName;

    // 未继续 Combo 时播放的收招 State；最后一段攻击可留空。
    [SerializeField]
    private string recoveryStateName;

    // Enemy 使用该距离切换 Attack，并在命中帧再次校验目标仍处于范围内。
    [SerializeField, Min(0f)]
    private float attackRange;

    public float Damage => damage;
    public string AttackStateName => attackStateName;
    public string RecoveryStateName => recoveryStateName;
    public float AttackRange => attackRange;
}
