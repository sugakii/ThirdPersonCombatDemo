using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 将 Health 的领域事件转换为 Slider 显示，不参与生命值计算。
/// 同一组件可供屏幕空间的 Player 血条和世界空间的 Enemy 血条复用。
/// </summary>
public class HealthBarPresenter : MonoBehaviour
{
    // 由场景或 Prefab 显式指定数据源，避免 Presenter 查找具体 Player/Enemy。
    [SerializeField]
    private Health health;

    // Presenter 只更新 Slider 数值，不控制血条美术层级。
    [SerializeField]
    private Slider slider;

    private void OnEnable()
    {
        // 组件启用期间监听变化，避免禁用后继续收到领域事件。
        health.HealthChanged += OnHealthChanged;
        SyncFromHealth();
    }

    private void OnDisable()
    {
        // 与 OnEnable 成对退订，防止重复订阅和对象生命周期泄漏。
        health.HealthChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(float currentHealth)
    {
        slider.value = currentHealth;
    }

    private void SyncFromHealth()
    {
        slider.minValue = 0;
        slider.maxValue = health.MaxHealth;
        slider.value = health.CurrentHealth;
    }

    private void Start()
    {
        // Health 已在 Awake 初始化；首次显示时同步范围和值，无需每帧轮询。
        SyncFromHealth();
    }
}
