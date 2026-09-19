using UnityEngine;
/// <summary>
/// 将 LayToIdle 动画结束事件转发给父级 EnemyStateMachine。
/// </summary>
public class EnemyStateMachineAnimationEvents : MonoBehaviour
{
    private EnemyStateMachine enemyStateMachine;

    public void EndHit()
    {
        enemyStateMachine.EndHit();
    }

    private void Awake()
    {
        enemyStateMachine = GetComponentInParent<EnemyStateMachine>();
    }
}
