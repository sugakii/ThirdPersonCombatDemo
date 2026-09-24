using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
