using UnityEngine;

/// <summary>
/// 保存技能可复用的静态配置；剩余冷却等运行时状态由 SkillController 管理。
/// </summary>
[CreateAssetMenu(
    fileName = "SkillDefinition",
    menuName = "Game/Data/Skills/SkillDefinition"
    )]
public class SkillDefinition : ScriptableObject
{
    [SerializeField, Min(0f)]
    private float damage;

    [SerializeField, Min(0f)]
    private float cooldown;

    [SerializeField, Min(0f)]
    private float dashDistance;

    [SerializeField, Min(0f)]
    private float dashDuration;

    [SerializeField]
    private string skillStateName;

    public float Damage => damage;
    public float Cooldown => cooldown;
    public float DashDistance => dashDistance;
    public float DashDuration => dashDuration;
    public string SkillStateName => skillStateName;
}
