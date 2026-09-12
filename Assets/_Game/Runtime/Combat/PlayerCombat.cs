using UnityEngine;

/// <summary>
/// Player 战斗流程的运行时入口；Day 9 仅建立依赖骨架，Combo 状态留到下一学习日实现。
/// </summary>
[RequireComponent(typeof(PlayerInputReader))]
public class PlayerCombat : MonoBehaviour
{
    // Animator 位于 Player 的 Imp 子物体，因此通过 Inspector 显式绑定。
    [SerializeField]
    private Animator animator;

    // InputReader 与本组件位于同一 Player，由 RequireComponent 保证存在。
    private PlayerInputReader inputReader;

    private void Awake()
    {
        // 缓存同对象依赖，后续战斗逻辑不直接读取具体输入设备。
        inputReader = GetComponent<PlayerInputReader>();
    }
}
