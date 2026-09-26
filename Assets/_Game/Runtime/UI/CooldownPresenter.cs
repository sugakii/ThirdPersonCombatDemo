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

    private void Awake()
    {
        cooldownText.gameObject.SetActive(false);
    }

    private void Update()
    {
        // RemainingCoolDown 会持续变化，必须逐帧读取；总时长则用于换算填充比例。
        float remaining = skillController.RemainingCoolDown;
        float cooldownDuration = skillController.CooldownDuration;

        if(skillController.CanUse)
        {
            cooldownFill.fillAmount = 0;
            cooldownText.gameObject.SetActive(false);

            return;
        }

        cooldownText.gameObject.SetActive(true);
        cooldownFill.fillAmount = Mathf.Clamp01(remaining / cooldownDuration);

        int remainingCoolDown = Mathf.CeilToInt(remaining);

        cooldownText.text = remainingCoolDown.ToString();
    }
}
