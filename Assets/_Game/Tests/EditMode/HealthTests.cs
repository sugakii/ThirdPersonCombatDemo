using UnityEngine;
using NUnit.Framework;

/// <summary>
/// 独立验证 Health 的生命钳制、事件顺序和死亡/恢复规则。
/// 每个用例都创建自己的 GameObject，避免组件状态在测试之间泄漏。
/// </summary>
public class HealthTests
{
    private GameObject testHealth;
    private Health health;
    private float reportedHealth;
    private int deathCount;

    [SetUp]
    public void SetUp()
    {
        // Arrange：为每个测试创建隔离的 Health 实例和事件记录状态。
        testHealth = new GameObject("TestHealth");

        health = testHealth.AddComponent<Health>();

        reportedHealth = -1f;

        deathCount = 0;
    }

    [TearDown]
    public void TearDown()
    {
        // EditMode 中立即销毁临时对象，避免污染场景和后续测试。
        GameObject.DestroyImmediate(testHealth);
    }

    [Test]
    public void Initialize_SetsCurrentHealthToMaxHealth()
    {
        health.Initialize(100);

        float initialHealth = health.CurrentHealth;

        Assert.AreEqual(
            health.MaxHealth,
            initialHealth
        );
    }

    [Test]
    public void TakeDamage_ReducesCurrentHealth()
    {
        health.Initialize(100);

        DamageInfo damageInfo = new DamageInfo(30);

        health.TakeDamage(damageInfo);

        float currentHealth = health.CurrentHealth;

        Assert.AreEqual(
            70,
            currentHealth
        );
    }

    [Test]
    public void TakeDamage_ClampsCurrentHealthAtZero()
    {
        health.Initialize(20);

        DamageInfo damageInfo = new DamageInfo(30);

        health.TakeDamage(damageInfo);

        float currentHealth = health.CurrentHealth;

        Assert.AreEqual(
            0,
            currentHealth
        );
    }

    [Test]
    public void TakeDamage_InvokesHealthChanged()
    {
        health.Initialize(100);

        health.HealthChanged += OnHealthChanged;

        DamageInfo damageInfo = new DamageInfo(30);

        health.TakeDamage(damageInfo);

        Assert.AreEqual(
            70,
            reportedHealth
        );
    }
    
    [Test]
    public void TakeDamage_InvokesDiedOnlyOnce()
    {
        health.Initialize(20);

        health.Died += OnDied;

        DamageInfo damageInfo = new DamageInfo(30);

        health.TakeDamage(damageInfo);

        health.TakeDamage(damageInfo);

        Assert.AreEqual(
            1,
            deathCount
        );
    }

    [Test]
    public void Reset_RestoresHealthAndAllowsDyingAgain()
    {
        health.Initialize(20);

        health.Died += OnDied;

        DamageInfo damageInfo = new DamageInfo(30);

        health.TakeDamage(damageInfo);

        health.Reset();

        Assert.AreEqual(
            health.MaxHealth,
            health.CurrentHealth
        );

        health.TakeDamage(damageInfo);

        Assert.AreEqual(
            2,
            deathCount
        );
    }

    [Test]
    public void Reset_InvokesHealthChangedWithMaxHealth()
    {
        health.Initialize(20);

        health.HealthChanged += OnHealthChanged;

        DamageInfo damageInfo = new DamageInfo(30);

        health.TakeDamage(damageInfo);

        reportedHealth = -1f;

        health.Reset();

        Assert.AreEqual(
            20,
            reportedHealth
        );
    }

    [Test]
    public void Initialize_ClampsNegativeHealthToZero()
    {
        health.Initialize(-20);

        Assert.AreEqual(
            0,
            health.MaxHealth
        );

        Assert.AreEqual(
            0,
            health.CurrentHealth
        );
    }
    
    [Test]
    public void TakeDamage_NegativeDamageDoesNotChangeHealth()
    {
        health.Initialize(100);

        DamageInfo damageInfo = new DamageInfo(-30);

        health.TakeDamage(damageInfo);

        Assert.AreEqual(
            100,
            health.CurrentHealth
        );
    }

    [Test]
    public void TakeDamage_ZeroDamageDoesNotChangeHealthOrInvokeHealthChanged()
    {
        health.Initialize(100);

        DamageInfo damageInfo = new DamageInfo(0);

        health.HealthChanged += OnHealthChanged;

        health.TakeDamage(damageInfo);

        Assert.AreEqual(
            100,
            health.CurrentHealth
        );

        Assert.AreEqual(
            -1,
            reportedHealth
        );
    }

    private void OnHealthChanged(float newHealth)
    {
        // 保存最后一次事件值，供测试断言事件是否触发及参数是否正确。
        reportedHealth = newHealth;
    }

    private void OnDied()
    {
        // 计数比 bool 更容易发现 Died 被重复触发的问题。
        deathCount++;
    }
}
