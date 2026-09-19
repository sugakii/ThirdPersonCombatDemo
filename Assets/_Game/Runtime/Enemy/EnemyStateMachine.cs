using UnityEngine;
using UnityEngine.AI;

[RequireComponent(
    typeof(NavMeshAgent),
    typeof(EnemyCombat)
)]
[RequireComponent(typeof(Health))]
/// <summary>
/// 统一管理 Puglin 的 Idle、Chase、Attack、Hit 和 Dead 互斥状态。
/// 位移交给 NavMeshAgent，攻击时机交给 EnemyCombat，生命变化来自 Health 事件。
/// </summary>
public class EnemyStateMachine : MonoBehaviour
{
    // 只保存互斥的运行状态，避免用多个布尔值组合出非法状态。
    private enum EnemyState
    {
        Idle,
        Chase,
        Attack,
        Hit,
        Dead
    }

    [SerializeField]
    private Transform target;

    [SerializeField, Min(0f)]
    private float chaseDistance = 6f;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private AttackDefinition attackDefinition;

    private float attackDistance;

    private Health health;

    private NavMeshAgent agent;

    private EnemyState currentState;

    private EnemyCombat enemyCombat;

    // 缓存固定目标的生命组件，用死亡事件停止当前攻击，避免每帧重复查找组件。
    private Health targetHealth;

    private void HandleIdle()
    {
        agent.isStopped = true;

        // 离开 Chase 时清除旧路径，避免 Agent 继续沿上一条路径滑动。
        if(agent.hasPath)
        {
            agent.ResetPath();
        }

        animator.SetBool("IsChasing", false);
    }

    private void HandleChase()
    {
        agent.isStopped = false;

        // NavMeshAgent 统一负责寻路和位移，状态机只提供目标点。
        agent.SetDestination(target.position);

        animator.SetBool("IsChasing", true);
    }

    private void HandleAttack()
    {
        agent.isStopped = true;

        animator.SetBool("IsChasing", false);

        enemyCombat.TryAttack(target);
    }

    private void HandleHit()
    {
        currentState = EnemyState.Hit;

        // 受击优先级高于攻击；先清理攻击运行状态，避免旧动画事件继续命中目标。
        if(enemyCombat.IsAttacking)
        {
            enemyCombat.EndAttack();
        }

        agent.isStopped = true;

        animator.SetBool("IsChasing", false);

        animator.CrossFade(
            "Hit_Knockback",
            0.05f
        );
    }

    private void HandleDead()
    {
        currentState = EnemyState.Dead;

        agent.isStopped = true;

        animator.SetBool("IsChasing", false);

        animator.CrossFade(
            "Death01",
            0.05f
        );
    }

    public void EndHit()
    {
        if(currentState != EnemyState.Hit)
        {
            return;
        }

        currentState = EnemyState.Idle;
    }

    private void HandleTargetDied()
    {
        // Player 死亡后立即清理攻击状态并释放目标，下一帧会安全进入 Idle。
        enemyCombat.EndAttack();
        target = null;
    }

    private void OnEnable()
    {
        // 订阅与 OnDisable 成对，避免对象重复启用后累积回调。
        health.Died += HandleDead;
        targetHealth.Died += HandleTargetDied;
        health.Damaged += HandleHit;
    }

    private void OnDisable()
    {
        health.Died -= HandleDead;
        targetHealth.Died -= HandleTargetDied;
        health.Damaged -= HandleHit;
    }

    private void Awake()
    {
        // Inspector 引用缺失时启动即失败，避免 Update 每帧重复抛出空引用异常。
        if(target == null || animator == null)
        {
            Debug.LogError("EnemyStateMachine is missing the Target or Animator reference.", this);

            enabled = false;
            return;
        }

        targetHealth = target.GetComponent<Health>();

        agent = GetComponent<NavMeshAgent>();

        health = GetComponent<Health>();

        enemyCombat = GetComponent<EnemyCombat>();

        currentState = EnemyState.Idle;

        // 攻击距离来自静态配置，运行时只缓存读取结果，不修改 ScriptableObject。
        attackDistance = attackDefinition.AttackRange;
    }

    private void Update()
    {
        if(currentState == EnemyState.Dead)
        {
           return;
        }

        if(currentState == EnemyState.Hit)
        {
            return;
        }

        if(target == null || !target.gameObject.activeInHierarchy)
        {
            currentState = EnemyState.Idle;
            HandleIdle();
            return;
        }

        if(enemyCombat.IsAttacking)
        {
            return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if(distanceToTarget <= attackDistance)
        {
            currentState = EnemyState.Attack;
        }
        else if(distanceToTarget <= chaseDistance)
        {
            currentState = EnemyState.Chase;
        }
        else
        {
            currentState = EnemyState.Idle;
        }

        switch(currentState)
        {
            case EnemyState.Idle:
                HandleIdle();
                break;

            case EnemyState.Chase:
                HandleChase();
                break;

            case EnemyState.Attack:
                HandleAttack();
                break;
        }
    }
}
