using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using System.Collections;
using System.Collections.Generic;

public class GameFlowPlayModeTests
{
    [UnityTest]
    public IEnumerator PlayerAndLastEnemyDieInSameFrame_GameOverTakesPriority()
    {
        // 使用正式场景验证事件订阅、帧末仲裁和结果 UI 的真实集成关系。
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync("SampleScene");

        yield return loadOperation;

        GameObject canvas = GameObject.Find("Canvas");

        GameObject gameOverPanel = canvas.transform.Find("GameOverPanel").gameObject;
        GameObject victoryPanel = canvas.transform.Find("VictoryPanel").gameObject;

        GameObject player = GameObject.Find("Player");

        EnemyStateMachine[] enemies = Object.FindObjectsByType<EnemyStateMachine>();

        List<Health> enemyHealths = new();

        Health playerHealth = player.GetComponent<Health>();

        foreach(EnemyStateMachine enemy in enemies)
        {
            Health enemyHealth = enemy.GetComponent<Health>();

            enemyHealths.Add(enemyHealth);
        }

        DamageInfo playerDamage = new DamageInfo(playerHealth.MaxHealth);
        playerHealth.TakeDamage(playerDamage);

        foreach(Health enemyHealth in enemyHealths)
        {
            DamageInfo enemyDamage = new DamageInfo(enemyHealth.MaxHealth);
            enemyHealth.TakeDamage(enemyDamage);
        }

        // GameFlowController 在帧末统一判定，因此必须等待一帧再检查结果。
        yield return null;

        Assert.IsTrue(gameOverPanel.activeSelf);
        Assert.IsFalse(victoryPanel.activeSelf);
    } 
}
