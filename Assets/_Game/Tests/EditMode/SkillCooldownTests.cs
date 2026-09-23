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

        // RequireComponent 会同时补齐 PlayerMotor 与 SkillHitDetector，避免测试重复添加组件。
        skillController = testTarget.AddComponent<SkillController>();

        PlayerMotor playerMotor = testTarget.GetComponent<PlayerMotor>();

        skillController.InitializePlayerMotor(playerMotor);
        
        skillDefinition = AssetDatabase.LoadAssetAtPath<SkillDefinition>(
            "Assets/_Game/Data/Skills/FireDash.asset"
        );

        skillController.InitializeSkillDefinition(skillDefinition);

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
