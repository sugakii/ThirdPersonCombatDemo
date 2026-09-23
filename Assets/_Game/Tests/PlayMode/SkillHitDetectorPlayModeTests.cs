using UnityEngine;
using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;

/// <summary>
/// 仅记录受伤次数的测试替身，用于隔离 SkillHitDetector 与具体 Health 实现。
/// </summary>
public class TestDamageable : MonoBehaviour, IDamageable
{
    public int DamageCount;

    public void TakeDamage(DamageInfo damageInfo)
    {
        DamageCount += 1;
    }
}

public class SkillHitDetectorPlayModeTests
{
    private SkillHitDetector skillHitDetector;

    private GameObject target;

    private GameObject detectorObject;

    private TestDamageable testDamageable;

    private float radius;

    private LayerMask mask;

    private DamageInfo damageInfo;

    private float damageAmount = 1f;

    [SetUp]
    public void SetUp()
    {
        target = new GameObject("TestTarget");
        detectorObject = new GameObject("DetectorObject");
        target.AddComponent<BoxCollider>();
        target.AddComponent<BoxCollider>();
        testDamageable = target.AddComponent<TestDamageable>();
        skillHitDetector = detectorObject.AddComponent<SkillHitDetector>();
        radius = 1f;
        mask = 1 << target.layer;
        damageInfo = new DamageInfo(damageAmount);

        skillHitDetector.Initialize(detectorObject.transform, radius, mask);
    }

    [TearDown]
    public void TearDown()
    {
        GameObject.Destroy(target);
        GameObject.Destroy(detectorObject);
    }

    [UnityTest]
    public IEnumerator SingleDetection_MultipleCollidersOnSameTarget_DamagesOnlyOnce()
    {
        // 两个 Collider 指向同一个 IDamageable，本次检测应只结算一次。
        skillHitDetector.BeginDetection(damageInfo);

        yield return null;

        Assert.AreEqual(1, testDamageable.DamageCount);
    }
}
