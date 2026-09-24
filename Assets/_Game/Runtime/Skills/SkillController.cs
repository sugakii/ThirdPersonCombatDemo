using UnityEngine;

[RequireComponent(
    typeof(PlayerMotor),
    typeof(SkillHitDetector),
    typeof(SkillVfxPool)
    )]
/// <summary>
/// 处理技能释放准入和运行时冷却。
/// </summary>
public class SkillController : MonoBehaviour
{
    [SerializeField]
    private SkillDefinition skillDefinition;

    [SerializeField]
    private Animator animator;

    public float CooldownDuration {get; private set; }

    private SkillHitDetector skillHitDetector;

    private PlayerInputReader inputReader;

    private PlayerMotor playerMotor;

    // 用于记录当前还剩多少秒冷却，是运行时状态。
    public float RemainingCoolDown { get; private set; }

    public bool CanUse => RemainingCoolDown <= 0f;

    private DamageInfo damageInfo;

    private SkillVfxPool vfx;

    public void InitializeSkillDefinition(SkillDefinition definition)
    {
        skillDefinition = definition;
    }

    public void InitializePlayerMotor(PlayerMotor playerMotor)
    {
        this.playerMotor = playerMotor;
    }

    public bool TryUseSkill()
    {
        // 冷却未结束时不覆盖剩余时间，调用者可通过返回值判断释放是否成功。
        if(!CanUse)
        {
            Debug.Log("Skill use rejected: cooldown active.", this);
            return false;
        }

        Vector3 forward = transform.forward;
        forward.y = 0;

        if(
            !playerMotor.TryStartDash(
                forward,
                skillDefinition.DashDistance,
                skillDefinition.DashDuration
            ))
        {
            return false;
        }

        RemainingCoolDown = skillDefinition.Cooldown;

        Debug.Log("Skill used successfully.", this);
        return true;
    }

    public void TickCooldown(float deltaTime)
    {
        if(CanUse)
        {
            return;
        }

        // 钳制到 0，避免浮点步进让冷却出现负数。
        RemainingCoolDown = Mathf.Max(0, RemainingCoolDown - deltaTime);
    }

    public void ResetCooldown()
    {
        RemainingCoolDown = 0;
    }

    private void OnEnable()
    {
        // 伤害检测窗口与实际 Dash 生命周期绑定，不依赖动画长度猜测结束时机。
        playerMotor.DashEnded += skillHitDetector.EndDetection;
    }

    private void OnDisable()
    {
        playerMotor.DashEnded -= skillHitDetector.EndDetection;
    }

    private void Awake()
    {
        inputReader = GetComponent<PlayerInputReader>();
        playerMotor = GetComponent<PlayerMotor>();
        skillHitDetector = GetComponent<SkillHitDetector>();
        vfx = GetComponent<SkillVfxPool>();
        CooldownDuration = skillDefinition.Cooldown;
    }

    private void Update()
    {
        TickCooldown(Time.deltaTime);
        
        if(inputReader.SkillPressed)
        {
            if(!TryUseSkill())
            {
                return;
            }

            damageInfo = new DamageInfo(skillDefinition.Damage);

            skillHitDetector.BeginDetection(damageInfo);

            animator.CrossFade(
                skillDefinition.SkillStateName,
                0.05f
            );

            vfx.Play(transform, skillDefinition.DashDuration);
        }
    }
}
