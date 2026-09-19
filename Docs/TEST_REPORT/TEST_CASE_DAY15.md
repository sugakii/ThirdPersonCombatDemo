# Day 15 Test Case Report — Enemy Attack / Hit / Dead

> 计划日期：2026-09-18  
> 验收日期：2026-09-19  
> Unity：6000.5.6f1  
> 场景：`Assets/_Game/Scenes/SampleScene.unity`  
> 复验日期：2026-09-19  
> 结论：PASS（10/10）

## 验收范围

- Puglin Attack、Hit、Dead 互斥状态。
- Enemy 通过 `IDamageable` 对 Player 造成伤害。
- 受击中断、死亡终态、Animation Event 与事件订阅生命周期。
- Player 死亡后的 Enemy 停止行为。

## 测试结果

| ID | 测试内容 | 预期结果 | 实际结果 | 状态 |
|---|---|---|---|---|
| D15-01 | Enemy 配置与引用 | StateMachine、EnemyCombat、Animator、Health、AttackDefinition 引用完整 | Unity MCP 确认引用完整；Enemy 配置为 Damage=10、Range=1、State=`Sword_Attack` | PASS |
| D15-02 | 攻击动画事件 | 命中帧调用 Hit，结束帧调用 EndAttack | `Sword_Attack` 保存 `Hit@0.383`、`EndAttack@1.481` | PASS |
| D15-03 | 进入攻击距离 | 目标进入 1m 范围后 Enemy 停止 Agent 并开始攻击 | 运行态 `Agent.isStopped=true`，Enemy 进入攻击循环 | PASS |
| D15-04 | Enemy 伤害 Player | 每个有效命中造成 10 点伤害 | Player HP 从 100 依次降至 60，伤害结算通过 `IDamageable` | PASS |
| D15-05 | 攻击节奏与结束 | 一次攻击结束后才能开始下一次，不在同一帧重复开启 | `IsAttacking` 在动画事件后复位，攻击按动画周期重新开始 | PASS |
| D15-06 | 攻击中受击 | 受击立即清理攻击状态并进入 Hit 流程 | 运行态受击后 HP 50→40，`IsAttacking` 立即变为 false | PASS |
| D15-07 | Hit 恢复配置 | Hit 后经 LayToIdle 返回可决策状态 | Hit→LayToIdle→Idle Transition 与 `EndHit` Animation Event 均已保存 | PASS |
| D15-08 | Enemy 死亡终态 | HP=0 后停止 Agent、停止攻击并保持 Death | 运行态 HP=0、Death01=true、`IsAttacking=false`、`Agent.isStopped=true` | PASS |
| D15-09 | 事件生命周期 | Health 订阅与退订成对，不累积回调 | `OnEnable`/`OnDisable` 成对管理 `Damaged` 与 `Died` | PASS |
| D15-10 | Player 死亡后停止攻击 | Player HP=0 后 Enemy 不再开始新攻击 | Died 回调结束攻击并清空 Target；超过两个原攻击周期后仍为 `IsAttacking=false`、`Agent.isStopped=true`、`SwordAttack=false` | PASS |

## 缺陷回归

- `BUG-015` 已关闭：EnemyStateMachine 缓存 Target Health，并通过成对的 `Died` 订阅/退订在 Player 死亡当帧结束攻击、清空目标。
- 复验同时覆盖状态机禁用/重新启用、Enemy 受击与 Enemy 致死，未观察到重复回调。

## 验收结论

- Enemy 攻击、伤害、受击中断、自身死亡终态和 Player 死亡后的停止行为均已建立。
- D15-03～D15-10 回归通过，Console 0 Error / 0 Warning。
- Day 15 正式验收通过，可以进入 Day 16 的三个 Enemy 集成。

## Unity MCP 证据

- 编辑态场景非 Dirty，Player/Puglin 均保持 Active，运行态测试停止后位置已恢复。
- `PuglinTest.controller` 包含 Idle、Jog、Sword_Attack、Hit_Knockback、LayToIdle、Death01。
- Enemy AttackDefinition：Damage=10、AttackRange=1、AttackStateName=`Sword_Attack`。
- 致死测试：HP=0、Death01=true、EnemyCombat.IsAttacking=false、NavMeshAgent.isStopped=true。
- Player HP=0 后 Target=null、`IsAttacking=false`、Agent 停止，超过两个原攻击周期后仍未重新进入 SwordAttack。
- 状态机禁用/重新启用后，Enemy 受击 HP 50→40；致死后 HP=0、Death01=true、Agent 停止。
