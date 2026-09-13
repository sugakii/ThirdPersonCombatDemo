using UnityEngine;

/// <summary>
/// 负责 Player 的普通攻击流程、 Combo 状态和输入缓存。
/// 不负责命中检测和生命结算。
/// </summary>
[RequireComponent(
    typeof(PlayerInputReader), typeof(PlayerMotor)
    )]
public class PlayerCombat : MonoBehaviour
{
    // Animator 位于 Player 的 Imp 子物体，因此通过 Inspector 显式绑定。
    [SerializeField]
    private Animator animator;

    // 数组顺序就是 Combo 顺序：0=Attack_01,1=Attack_02，2=Attack_03。
    [SerializeField]
    private AttackDefinition[] attacks;

    // InputReader 与本组件位于同一 Player，由 RequireComponent 保证存在。
    private PlayerInputReader inputReader;

    // PlayerMotor 与本组件位于同一 Player，由 RequireComponent 保证存在。
    private PlayerMotor playerMotor;

    // 当前 Combo 对应的攻击配置索引：0=第一段、1=第二段、2=第三段。
    private int currentComboIndex;

    // 标记当前是否处于一次攻击流程中。
    private bool isAttacking;

    // 玩家提前输入下一次攻击时先缓存，等待 Combo 窗口消费。
    private bool bufferedAttack;

    // 当前是否允许记录下一段攻击。
    private bool comboInputOpen;

    // 当前是否已经到达允许真正切换下一段攻击的时机。
    private bool comboAdvanceOpen;

    private void Awake()
    {
        // 缓存同对象依赖，后续战斗逻辑不直接读取具体输入设备。
        inputReader = GetComponent<PlayerInputReader>();
        playerMotor = GetComponent<PlayerMotor>();
    }

    private void StartAttack()
    {
        currentComboIndex = 0;
        isAttacking = true;
        bufferedAttack = false;
        comboAdvanceOpen = false;
        comboInputOpen = false;

        playerMotor.FaceCameraForward();

        // 从当前 Animator State 平滑切换到攻击第一段。
        animator.CrossFade(
            attacks[currentComboIndex].AttackStateName,
            0.05f
        );
    }

    // 进入允许衔接下一段攻击的动画时间区间。
    public void OpenComboAdvance()
    {
        comboAdvanceOpen = true;

        // 提前记录的攻击输入在固定衔接节点到达后统一消费。
        if(bufferedAttack)
        {
            AdvanceCombo();
        }
    }

    // 打开接收玩家攻击输入的窗口。
    public void OpenComboInput()
    {
        comboInputOpen = true;
    }

    // 消费一次有效的衔接请求，并将 Combo 推进到下一段攻击。
    private void AdvanceCombo()
    {
        // 本次缓存输入已被处理，不能遗留到下一套 Combo。
        bufferedAttack = false;

        // 最后一段没有后续攻击，避免访问 attacks 数组范围之外。
        if(currentComboIndex >= attacks.Length - 1)
        {
            return;
        }

        currentComboIndex++;

        // 下一段必须重新等待自己的输入窗口和固定衔接节点。
        comboAdvanceOpen = false;
        comboInputOpen = false;

        playerMotor.FaceCameraForward();

        animator.CrossFade(
            attacks[currentComboIndex].AttackStateName,
            0.05f
        );
    }

    // 当前这段攻击没有成功衔接下一段时，播放它对应的收招动画。
    private void PlayRecovery()
    {
        // 没有独立 Recovery State 时不切换动画，由当前攻击自身完成收招。
        if(string.IsNullOrEmpty(
            attacks[currentComboIndex].RecoveryStateName
            ))
        {
            return;
        }

        animator.CrossFade(
            attacks[currentComboIndex].RecoveryStateName,
            0.05f
        );
    }

    // 当前出招阶段结束后进入收招，但保持 Combo 窗口开放。
    public void EnterRecovery()
    {
        // 成功衔接下一段后，两个窗口都会关闭
        // 因此忽略上一段动画可能晚到的 Recovery Event。
        if(!comboAdvanceOpen && !comboInputOpen)
        {
            return;
        }

        PlayRecovery();
    }

    // 结束一套 Combo 之后，恢复到空闲状态。
    public void EndAttack()
    {
        isAttacking = false;
        currentComboIndex = 0;
        bufferedAttack = false;
        comboAdvanceOpen = false;
        comboInputOpen = false;
    }

    private void Update()
    {
        if(!inputReader.AttackPressed)
        {
            return;
        }

        // 只有在空闲状态下的新攻击输入，才允许开启一套新的 Combo。
        if(!isAttacking)
        {
            StartAttack();
            return;
        }

        if(!comboInputOpen)
        {
            return;
        }

        if(comboAdvanceOpen)
        {
            AdvanceCombo();
            return;
        }

        // 输入有效，但尚未到达固定衔接节点，先缓存。
        bufferedAttack = true;
    }
}
