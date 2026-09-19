# Current Project Status

> Last Updated：2026-09-19
> Current Learning Day：Day 15 验收通过
> Current Phase：Phase A / Enemy Combat
> Next Checkpoint：Day 16 三个 Enemy 集成与 Phase A 验收
> Source of Truth：当前 Unity 工程 + Git + Docs
> Remaining Plan：`Docs/plans/2026-09-13-remaining-learning-days.md`

## 验收结论

**Day 15 复验：PASS（10/10）。** Enemy Attack、通过 `IDamageable` 伤害 Player、受击中断、Enemy Dead 终态和 Player 死亡后的停止行为均通过。`BUG-015` 已修复关闭，可以进入 Day 16。

## Implemented

- `AttackDefinition` 新增静态 `AttackRange`；Enemy 配置为 Damage=10、Range=1、State=`Sword_Attack`。
- `Health` 新增 `Damaged` 事件；无效伤害和死亡后的重复伤害不会触发。
- `EnemyStateMachine` 已包含 Idle、Chase、Attack、Hit、Dead 互斥状态，并成对订阅/退订自身 Health 事件。
- `EnemyCombat` 负责启动攻击、在动画命中帧复核距离，并仅通过 `IDamageable` 结算伤害。
- `Sword_Attack` 已配置 Hit 与 EndAttack 事件；Hit 流程使用 `LayToIdle` 的 EndHit 事件恢复决策。
- Puglin Dead 会停止 Agent、清除攻击状态并播放 Death01。
- EnemyStateMachine 缓存 Target Health，并通过成对的 Died 订阅在 Player 死亡时立即结束攻击和释放目标。
- Day 15 涉及脚本已补职责/原因型注释；重复诊断日志和重复条件已清理；动画事件桥接脚本名称已规范为 `EnemyCombatAnimationEvents`，原 `.meta` GUID 保持不变。

## Test Evidence

| 范围 | 结果 |
|---|---|
| 配置、引用、动画事件 | PASS |
| Enemy 进入攻击范围并攻击 | PASS |
| Enemy 每次命中造成 10 点伤害 | PASS |
| 攻击动画周期与 EndAttack | PASS |
| Enemy 攻击中受击清理攻击状态 | PASS |
| Hit 恢复链配置 | PASS |
| Enemy 致死后 Dead 终态 | PASS：HP=0、Death01、Agent 停止、IsAttacking=false |
| Player 死亡后停止攻击 | PASS：Target=null、IsAttacking=false、Agent 停止，两个攻击周期内未重启 |
| 编译与 Missing Script | PASS：Error=0，桥接组件引用保留 |
| Day 15 结论 | **PASS：10/10** |

详细用例见 `Docs/TEST_REPORT/TEST_CASE_DAY15.md`。

## Current Architecture

```text
EnemyStateMachine
├─ Idle / Chase ───────► NavMeshAgent
├─ Attack ─────────────► EnemyCombat
├─ Hit / Dead ◄──────── Health.Damaged / Health.Died
└─ 表现 ───────────────► Animator

EnemyCombat
├─ AttackDefinition（Damage / Range / State）
├─ Animation Event（Hit / EndAttack）
└─ IDamageable.TakeDamage(DamageInfo)
```

- Health 仍不引用 UI、Animator、Player 或 Enemy。
- Animation Event 只报告命中/结束时机，不决定 AI 状态。
- EnemyStateMachine 通过 Target Health 的 Died 事件停止攻击；不在 Update 热路径重复查找组件。

## Files

- `Assets/_Game/Runtime/Common/Health.cs`
- `Assets/_Game/Runtime/Combat/AttackDefinition.cs`
- `Assets/_Game/Runtime/Enemy/EnemyStateMachine.cs`
- `Assets/_Game/Runtime/Enemy/EnemyStateMachineAnimationEvents.cs`
- `Assets/_Game/Runtime/Enemy/Combat/EnemyCombat.cs`
- `Assets/_Game/Runtime/Enemy/Combat/EnemyCombatAnimationEvents.cs`
- `Assets/_Game/Data/Combat/Enemy/AttackDefinition.asset`
- `Assets/_Game/Animations/Enemy/PuglinTest.controller`
- `Assets/_Game/Animations/Source/UAL1_Standard.fbx.meta`
- `Assets/_Game/Scenes/SampleScene.unity`
- `Docs/TEST_REPORT/TEST_CASE_DAY15.md`

## Known Bugs / Risks

1. `BUG-004` Open：角色离地后仍保留完整水平控制速度；进入技能位移前处理。
2. `MeleeHitbox` PlayMode 测试延期且不计为自动化证据。
3. `Assets/_Recovery/` 是恢复文件，不纳入正式项目提交。

## Git

- 当前分支：`main`；提交前以 `git rev-parse --short HEAD` 复核实际 HEAD。
- Day 15 已通过，可以创建正式完成提交。
- 默认提交全部自有代码、对应 `.meta`、配置资产、Scene、测试与 Docs。
- 排除 Unity Assistant Settings、SceneTemplateSettings、`Assets/_Recovery/`、空 Debug 目录及未经确认的 ProjectSettings 变化。
- Bestiary 原始 FBX/PNG 不进入公开仓库。

## Next Task

1. 开始 Day 16：放置三个共享 Puglin Prefab 的 Enemy 实例。
2. 验证拥挤、同时受击、逐个死亡与 Player 死亡后全部停止。
3. 完成 Phase A 的 Health、血条、Combo、命中去重与 AI/Combat 回归。

## Update Rules

- 未运行写 NOT RUN；测试名、步骤、断言和结果必须一致。
- 收到“验收”请求时，先扫描并补齐本次涉及代码的必要注释，再执行验证、更新 Docs，并在可提交时给出包含全部自有代码的 Git 指令。
- 注释解释职责、关键 API、边界和原因，不逐行复述代码。
- 每次验收结束后，根据当天实际内容提出 3–5 个理解题；回答情况用于安排后续教学，不篡改工程验收结果。
- Day 结束后同步工程、Git 和 Docs；已验收架构默认冻结。
