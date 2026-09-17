# Current Project Status

> Last Updated：2026-09-17
> Current Learning Day：Day 13 验收通过
> Current Phase：Phase A / Enemy AI
> Next Checkpoint：Day 14 最小 NavMesh 与 Enemy Idle ↔ Chase
> Source of Truth：当前 Unity 工程 + Git + Docs
> Remaining Plan：`Docs/plans/2026-09-13-remaining-learning-days.md`

## 验收结论

**Day 13 复验：PASS（15/15）。** Unity MCP 实时确认材质发光、Idle 默认状态、Enemy Prefab、正式场景实例和世界空间血条均已保存；Play Mode 中 Puglin 为 50/50 HP，Billboard 与 Main Camera 旋转一致，Console 无游戏 Error。可以进入 Day 14。

## Implemented

- 选择性导入 `Puglin.fbx` 与 BaseColor 1、Normal、Emissive、ORM，FBX/PNG 已加入公开仓库忽略规则。
- Puglin 保存 Humanoid、Create From This Model、Bake Axis Conversion=true，Idle/Jog/Attack/Hit/Death 手工预览通过。
- 创建外部 `MI_Puglin.mat` 并完成 FBX Material Remap。
- 创建 `Puglin.prefab`，根节点保存 Enemy Layer、Capsule Collider 与 Health=50。
- Prefab 内 HealthBarPresenter 的 Health/Slider 内部引用已保存；Animator Root Motion=false。
- 今日没有新增玩法代码；无需补充代码注释。

## Test Evidence

| 范围 | 结果 |
|---|---|
| 模型、Avatar 与五个动作 | PASS（配置静态检查 + 用户手工回归） |
| Root Motion | PASS：Prefab 保存 false |
| Layer / Collider / Health / 血条伤害 | PASS（配置静态检查 + 用户手工回归） |
| 材质 Remap、BaseColor、Normal | PASS |
| Emissive | PASS：白色乘数、Emissive 贴图与 `_EMISSION` 均已保存 |
| Animator 默认状态 | PASS：默认 State 为 Idle_Loop |
| Prefab 世界空间血条相机 | PASS：Play Mode 中 Billboard 启用且旋转与 Main Camera 一致 |
| 场景 Prefab 实例 | PASS：正式 Puglin Prefab GUID 已保存 |
| Console / Missing Script | PASS：未检出近期异常，Missing Script=0 |
| Day 13 结论 | PASS：15/15 |

详细用例见 `Docs/TEST_REPORT/TEST_CASE_DAY13.md`。

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

## Files

- `Assets/_Game/Art/Charactors/Enemy/Puglin/`
- `Assets/_Game/Materials/MI_Puglin.mat`
- `Assets/_Game/Animations/Enemy/PuglinTest.controller`
- `Assets/_Game/Prefabs/Enemy/Puglin.prefab`
- `Assets/_Game/Prefabs/Player/Player.prefab`（原 GUID 保持不变的目录移动）
- `Assets/_Game/Scenes/SampleScene.unity`
- `Docs/TEST_REPORT/TEST_CASE_DAY13.md`

## Known Bugs / Risks

1. `BUG-004` Open：角色离地后仍保留完整水平控制速度；进入技能位移前处理。
2. `MeleeHitbox` PlayMode 测试延期且不计为自动化证据；空骨架不得保留以免假通过。
3. `Assets/_Recovery/` 是恢复文件，不纳入正式项目提交。

## Git

- 当前分支：`main`；提交前以 `git rev-parse --short HEAD` 复核实际 HEAD。
- Day 13 已通过，可创建正式完成提交。
- 默认提交全部自有代码、对应 `.meta`、配置资产、Scene、测试与 Docs。
- 排除 Unity Assistant Settings、SceneTemplateSettings、`Assets/_Recovery/`、空 Debug 目录及未经确认的 ProjectSettings 变化。
- Bestiary 原始 FBX/PNG 不进入公开仓库。

## Next Task

1. 开始 Day 14：烘焙最小 NavMesh。
2. 实现 Enemy Idle ↔ Chase，并执行距离、路径与 NavMesh 边界测试。

## Update Rules

- 未运行写 NOT RUN；测试名、步骤、断言和结果必须一致。
- 收到“验收”请求时，先扫描并补齐本次涉及代码的必要注释，再执行验证、更新 Docs，并在可提交时给出包含全部自有代码的 Git 指令。
- 注释解释职责、关键 API、边界和原因，不逐行复述代码。
- 每次验收结束后，根据当天实际内容提出 3–5 个理解题；回答情况用于安排后续教学，不篡改工程验收结果。
- Day 结束后同步工程、Git 和 Docs；已验收架构默认冻结。
