using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

/// <summary>
/// 监听玩家与敌人的死亡事件，在帧末统一决定胜负，并冻结已结束的战斗。
/// </summary>
public class GameFlowController : MonoBehaviour
{
    private enum GameState
    {
        Playing,
        Victory,
        GameOver
    }

    [SerializeField]
    private Health playerHealth;

    private List<Health> enemyHealths = new();

    private List<EnemyStateMachine> enemyStateMachines = new();

    private List<EnemyCombat> enemyCombats = new();

    private int remainingEnemies;

    private GameState currentState;

    private bool playerDiedThisFrame;

    private bool allEnemiesDiedThisFrame;

    private bool resolveScheduled;

    [SerializeField]
    private PlayerMotor playerMotor;

    [SerializeField]
    private PlayerCombat playerCombat;

    [SerializeField]
    private MeleeHitbox meleeHitbox;

    [SerializeField]
    private SkillController skillController;

    [SerializeField]
    private SkillHitDetector skillHitDetector;

    [SerializeField]
    private CameraController cameraController;

    [SerializeField]
    private GameObject victoryPanel;

    [SerializeField]
    private GameObject gameOverPanel;

    public void RestartGame()
    {
        // 重新加载场景可一次性清理生命、冷却、命中集合和事件订阅等运行时状态。
        SceneManager.LoadScene("SampleScene");
    }

    private void HandlePlayerDied()
    {
        playerDiedThisFrame = true;

        if(resolveScheduled)
        {
            return;
        }

        StartCoroutine(ResolveAtEndOfFrame());
    }

    private void HandleEnemyDied()
    {
        remainingEnemies -= 1;

        if(remainingEnemies <= 0)
        {
            allEnemiesDiedThisFrame = true;
        }

        if(resolveScheduled)
        {
            return;
        }

        StartCoroutine(ResolveAtEndOfFrame());
    }

    private void ResolveGameResult()
    {
        if(currentState != GameState.Playing)
        {
            return;
        }

        if(playerDiedThisFrame)
        {
            currentState = GameState.GameOver;
            FreezeGameplay();
            gameOverPanel.SetActive(true);
            return;
        }

        if(allEnemiesDiedThisFrame)
        {
            currentState = GameState.Victory;
            FreezeGameplay();
            victoryPanel.SetActive(true); 
            return;
        }
    }

    private void ClearFrameDeathFlags()
    {
        playerDiedThisFrame = false;
        allEnemiesDiedThisFrame = false;
        resolveScheduled = false;
    }

    private IEnumerator ResolveAtEndOfFrame()
    {
        resolveScheduled = true;

        // 延迟到帧末收集同一帧内的全部死亡事件，使结果不依赖事件触发顺序。
        yield return new WaitForEndOfFrame();

        ResolveGameResult();
        ClearFrameDeathFlags();
    }

    private void FreezeGameplay()
    {
        // 结果界面出现后停止所有能继续改变战局的 Player 行为。
        playerMotor.enabled = false;
        playerCombat.enabled = false;
        skillController.enabled = false;

        meleeHitbox.CloseDamageWindow();
        meleeHitbox.enabled = false;

        skillHitDetector.EndDetection();
        skillHitDetector.enabled = false;

        cameraController.enabled = false;

        foreach(EnemyStateMachine enemyStateMachine in enemyStateMachines)
        {
            enemyStateMachine.enabled = false;

            NavMeshAgent agent = enemyStateMachine.GetComponent<NavMeshAgent>();
            agent.isStopped = true;
            agent.ResetPath();
        }

        foreach(EnemyCombat enemyCombat in enemyCombats)
        {
            enemyCombat.EndAttack();
        }

    }

    private void OnEnable()
    {
        // OnEnable/OnDisable 成对管理订阅，避免场景重载或组件重启后重复响应死亡事件。
        playerHealth.Died += HandlePlayerDied;

        foreach(Health enemyHealth in enemyHealths)
        {
            enemyHealth.Died += HandleEnemyDied;
        }        
    }

    private void OnDisable()
    {
        playerHealth.Died -= HandlePlayerDied;

        foreach(Health enemyHealth in enemyHealths)
        {
            enemyHealth.Died -= HandleEnemyDied;
        }    
    }

    private void Awake()
    {
        victoryPanel.SetActive(false);
        gameOverPanel.SetActive(false);

        playerDiedThisFrame = false;
        allEnemiesDiedThisFrame = false;

        EnemyStateMachine[] enemies = FindObjectsByType<EnemyStateMachine>();

        foreach(EnemyStateMachine enemy in enemies)
        {
            enemyStateMachines.Add(enemy);

            Health enemyHealth = enemy.GetComponent<Health>();
            EnemyCombat enemyCombat = enemy.GetComponent<EnemyCombat>();

            if(enemyHealth != null)
            {
                enemyHealths.Add(enemyHealth);
            }

            if(enemyCombat != null)
            {
                enemyCombats.Add(enemyCombat);
            }
        }

        remainingEnemies = enemyHealths.Count;

        currentState = GameState.Playing;
    }
}
