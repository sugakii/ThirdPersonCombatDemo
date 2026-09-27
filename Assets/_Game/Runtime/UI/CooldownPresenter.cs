using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 只读取技能运行时冷却状态并更新 UI，不参与技能释放判定。
/// </summary>
public class CooldownPresenter : MonoBehaviour
{
    [SerializeField]
    private SkillController skillController;

    [SerializeField]
    private Image cooldownFill;

    [SerializeField]
    private TMP_Text cooldownText;

    private int displayedSeconds = -1;

    private bool cooldownVisible;

    private void Awake()
    {
        cooldownText.gameObject.SetActive(false);
        cooldownVisible = false;
    }

    private void Update()
    {
        // RemainingCoolDown 会持续变化，必须逐帧读取；总时长则用于换算填充比例。
        float remaining = skillController.RemainingCoolDown;
        float cooldownDuration = skillController.CooldownDuration;

        if(skillController.CanUse)
        {
            cooldownFill.fillAmount = 0;

            if(cooldownVisible)
            {
                cooldownText.gameObject.SetActive(false);
                cooldownVisible = false;
            }

            displayedSeconds = -1;

            return;
        }

        if(!cooldownVisible)
        {
            cooldownText.gameObject.SetActive(true);
            cooldownVisible = true;
        }

        cooldownFill.fillAmount = Mathf.Clamp01(remaining / cooldownDuration);

        int remainingCoolDown = Mathf.CeilToInt(remaining);

        // 秒数实际变化时才创建新文本，避免冷却期间每帧分配字符串。
        if(remainingCoolDown != displayedSeconds)
        {
            displayedSeconds = remainingCoolDown;
            cooldownText.text = displayedSeconds.ToString();
        }
    }
}
