using UnityEngine;
using NUnit.Framework;
using UnityEditor;

public class SkillCooldownTests
{
    private GameObject testTarget;

    private SkillController skillController;

    private SkillDefinition skillDefinition;

    [SetUp]
    public void SetUp()
    {
        testTarget = new GameObject("TestTarget");

        skillController = testTarget.AddComponent<SkillController>();
        
        skillDefinition = AssetDatabase.LoadAssetAtPath<SkillDefinition>(
            "Assets/_Game/Data/Skills/FireDash.asset"
        );

        skillController.Initialize(skillDefinition);
    }

    [TearDown]
    public void TearDown()
    {
        GameObject.DestroyImmediate(testTarget);
    }

    [Test]
    public void NewSkillController_CanUseSkillImmediately()
    {
        Assert.IsTrue(skillController.CanUse);
    }

    [Test]
    public void TryUseSkill_DuringCooldown_ReturnsFalse()
    {
        bool firstResult = skillController.TryUseSkill();

        Assert.IsTrue(firstResult);

        bool secondResult = skillController.TryUseSkill();

        Assert.IsFalse(secondResult);

        Assert.AreEqual(
            skillDefinition.Cooldown,
            skillController.RemainingCoolDown
        );
    }

    [Test]
    public void TickCooldown_WhenCooldownEnds_SkillCanBeUsedAgain()
    {
        bool firstResult = skillController.TryUseSkill();

        Assert.IsTrue(firstResult);

        skillController.TickCooldown(3);

        Assert.AreEqual(
            0,
            skillController.RemainingCoolDown
        );

        Assert.IsTrue(skillController.CanUse);

        bool secondResult = skillController.TryUseSkill();

        Assert.IsTrue(secondResult);
    }

    [Test]
    public void ResetCooldown_DuringCooldown_MakesSkillUsableImmediately()
    {
        bool result = skillController.TryUseSkill();

        Assert.IsTrue(result);

        skillController.ResetCooldown();

        Assert.AreEqual(
            0,
            skillController.RemainingCoolDown
        );

        Assert.IsTrue(skillController.CanUse);
    }
}
