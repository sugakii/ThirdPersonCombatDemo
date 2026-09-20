using UnityEngine;

[RequireComponent(typeof(PlayerInputReader))]
/// <summary>
/// 处理技能释放准入和运行时冷却，不负责 Day 18 才加入的位移与表现。
/// </summary>
public class SkillController : MonoBehaviour
{
    [SerializeField]
    private SkillDefinition skillDefinition;

    private PlayerInputReader inputReader;

    // 用于记录当前还剩多少秒冷却，是运行时状态。
    public float RemainingCoolDown { get; private set; }

    public bool CanUse => RemainingCoolDown <= 0f;

    public void Initialize(SkillDefinition definition)
    {
        skillDefinition = definition;
    }

    public bool TryUseSkill()
    {
        // 冷却未结束时不覆盖剩余时间，调用者可通过返回值判断释放是否成功。
        if(!CanUse)
        {
            Debug.Log("Skill use rejected: cooldown active.", this);
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

    private void Awake()
    {
        inputReader = GetComponent<PlayerInputReader>();
    }

    private void Update()
    {
        TickCooldown(Time.deltaTime);
        
        if(inputReader.SkillPressed)
        {
            TryUseSkill();
        }
    }
}
