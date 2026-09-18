using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyStateMachine : MonoBehaviour
{
    // 只保存互斥的运行状态，避免用多个布尔值组合出非法状态。
    private enum EnemyState
    {
        Idle,
        Chase
    }

    [SerializeField]
    private Transform target;

    [SerializeField, Min(0f)]
    private float chaseDistance = 6f;

    [SerializeField]
    private Animator animator;

    private NavMeshAgent agent;

    private EnemyState currentState;

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

    private void Awake()
    {
        // Inspector 引用缺失时启动即失败，避免 Update 每帧重复抛出空引用异常。
        if(target == null || animator == null)
        {
            Debug.LogError("EnemyStateMachine is missing the Target or Animator reference.", this);

            enabled = false;
            return;
        }

        agent = GetComponent<NavMeshAgent>();

        currentState = EnemyState.Idle;
    }

    private void Update()
    {
        if(target == null || !target.gameObject.activeInHierarchy)
        {
            currentState = EnemyState.Idle;
            HandleIdle();
            return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if(distanceToTarget <= chaseDistance)
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
        }
    }
}
