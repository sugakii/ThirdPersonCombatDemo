# Current Project Status

> Last Updated：2026-09-18
> Current Learning Day：Day 14 验收通过
> Current Phase：Phase A / Enemy AI
> Next Checkpoint：Day 15 Enemy Attack / Hit / Dead
> Source of Truth：当前 Unity 工程 + Git + Docs
> Remaining Plan：`Docs/plans/2026-09-13-remaining-learning-days.md`

## 验收结论

**Day 14 复验：PASS（10/10）。** 最小 NavMesh、Environment Layer、Puglin Agent、6m 仇恨距离及 Idle ↔ Chase 已完成。Unity MCP 运行态确认正常追逐、范围外停止、目标禁用/销毁保护、8 拐点障碍路径、NavMesh 边缘和 Console 全部通过。可以进入 Day 15。

## Implemented

- 新增 Environment Layer，并让 NavMeshSurface 只收集该层的 Physics Colliders。
- 烘焙 `NavMesh-Navigation`，当前数据为 76 个顶点、30 个三角形。
- Puglin Prefab 新增 `NavMeshAgent` 与 `EnemyStateMachine`。
- 实现 Idle/Chase 两个互斥状态、6m 仇恨距离、路径设置和离开追逐后的路径清理。
- Puglin Animator 新增 `IsChasing` Bool 与 Idle/Jog 双向切换。
- 已为 `EnemyStateMachine` 补充职责和原因型注释。

## Test Evidence

| 范围 | 结果 |
|---|---|
| NavMesh 数据与 Surface | PASS |
| Puglin Agent 在 NavMesh 上 | PASS |
| 6m 内 Chase | PASS：完整路径、Velocity=3.5 |
| 6m 外 Idle | PASS：路径清除、Velocity=0 |
| Player 禁用 | PASS：Error=0 |
| Player 运行中丢失 | PASS：Agent 停止、路径清除、Error=0 |
| 障碍绕行 / 完整边缘路线 | PASS：8 拐点 PathComplete；边缘 PathComplete |
| Day 14 结论 | PASS：10/10 |

详细用例见 `Docs/TEST_REPORT/TEST_CASE_DAY14.md`。

## Current Architecture

```text
UAL2 Animation Events
        ↓
PlayerCombatAnimationEvents（只转发）
        ↓
PlayerCombat（读取当前 AttackDefinition.Damage）
        ↓ DamageInfo
MeleeHitbox
├─ OverlapSphere + Enemy LayerMask
├─ Vector3.Dot 前半球过滤
├─ HashSet<IDamageable> 单窗口去重
└─ IDamageable.TakeDamage
        ↓
Health
```

- Animator 只提供伤害窗口时机；命中检测、伤害配置和生命规则仍彼此分离。
- PlayerCombat 不依赖具体 Enemy；MeleeHitbox 只面向 `IDamageable`。
- 当前物理查询使用 `OverlapSphere`，短窗口内会分配数组；是否改 NonAlloc 留到 Profiler 日依据数据决定。
- EnemyStateMachine 当前以 enum + switch 管理 Idle/Chase；Agent 负责寻路和位移，Animator 只表现状态。

## Files

- `Assets/_Game/Art/Charactors/Enemy/Puglin/`
- `Assets/_Game/Materials/MI_Puglin.mat`
- `Assets/_Game/Animations/Enemy/PuglinTest.controller`
- `Assets/_Game/Prefabs/Enemy/Puglin.prefab`
- `Assets/_Game/Prefabs/Player/Player.prefab`（原 GUID 保持不变的目录移动）
- `Assets/_Game/Scenes/SampleScene.unity`
- `Assets/_Game/Scenes/SampleScene/NavMesh-Navigation.asset`
- `Assets/_Game/Runtime/Enemy/EnemyStateMachine.cs`
- `Docs/TEST_REPORT/TEST_CASE_DAY14.md`

## Known Bugs / Risks

1. `BUG-004` Open：角色离地后仍保留完整水平控制速度；进入技能位移前处理。
2. `MeleeHitbox` PlayMode 测试延期且不计为自动化证据；空骨架不得保留以免假通过。
3. `Assets/_Recovery/` 是恢复文件，不纳入正式项目提交。

## Git

- 当前分支：`main`；提交前以 `git rev-parse --short HEAD` 复核实际 HEAD。
- Day 14 已通过，可创建正式完成提交。
- 默认提交全部自有代码、对应 `.meta`、配置资产、Scene、测试与 Docs。
- 排除 Unity Assistant Settings、SceneTemplateSettings、`Assets/_Recovery/`、空 Debug 目录及未经确认的 ProjectSettings 变化。
- Bestiary 原始 FBX/PNG 不进入公开仓库。

## Next Task

1. 开始 Day 15：补齐 Enemy Attack / Hit / Dead 状态。
2. 让 Enemy 通过 `IDamageable` 伤害 Player，并验证死亡终态与事件生命周期。

## Update Rules

- 未运行写 NOT RUN；测试名、步骤、断言和结果必须一致。
- 收到“验收”请求时，先扫描并补齐本次涉及代码的必要注释，再执行验证、更新 Docs，并在可提交时给出包含全部自有代码的 Git 指令。
- 注释解释职责、关键 API、边界和原因，不逐行复述代码。
- 每次验收结束后，根据当天实际内容提出 3–5 个理解题；回答情况用于安排后续教学，不篡改工程验收结果。
- Day 结束后同步工程、Git 和 Docs；已验收架构默认冻结。
