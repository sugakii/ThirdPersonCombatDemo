# Current Project Status

> Last Updated：2026-09-15
> Current Learning Day：Day 11 已完成
> Current Phase：Phase A / MeleeHitbox 完成
> Next Checkpoint：Learning Day 12 — 20×20m 中世纪庭院主体
> Source of Truth：当前 Unity 工程 + Git + Docs
> Remaining Plan：`Docs/plans/2026-09-13-remaining-learning-days.md`

## 验收结论

**Day 11：PASS。** 伤害窗口、`MeleeHitbox`、单窗口命中去重、前半球过滤和伤害结算均已实现，手工功能/边界回归通过。`MeleeHitbox` PlayMode 测试经范围评估后延期：当前实现需要学习者尚未掌握的反射来模拟 Inspector 私有字段，不再作为 Day 11 阻断项；未完成的空测试骨架已删除，避免产生假通过。

## Implemented

- `PlayerCombat` 从当前 Combo 的 `AttackDefinition` 读取伤害，并把 `DamageInfo` 交给 `MeleeHitbox`。
- `PlayerCombatAnimationEvents` 只转发伤害窗口时机，不查询目标、不直接扣血。
- `MeleeHitbox` 只在窗口打开时执行 `Physics.OverlapSphere`，并使用 Enemy LayerMask 过滤候选目标。
- 候选目标再通过 `Vector3.Dot` 排除角色后方目标。
- `HashSet<IDamageable>` 在每次新窗口打开时清空，使同一目标一刀只结算一次、下一刀仍可再次命中。
- 命中只调用 `IDamageable.TakeDamage(DamageInfo)`，没有依赖 Enemy 具体类。
- A/B/C Clip 已保存 Open/Close Damage Window 事件；三段伤害分别为 10/15/20。
- `MeleeHitboxCenter`、0.75 半径和 Enemy LayerMask 已保存到 `SampleScene`。
- Day 11 涉及代码已补齐职责、边界和原因型注释。

## Test Evidence

| 范围 | 结果 |
|---|---|
| 当前脚本编译 | PASS：`Game.Runtime.dll` 晚于 Day 11 脚本，Editor.log 未检出编译错误 |
| 场景引用 | PASS：Center、Radius=0.75、Enemy Mask 已保存 |
| Animation Events | PASS：A/B/C 均保存伤害窗口事件 |
| 窗口内/外伤害 | PASS（用户实机） |
| 多帧与多 Collider 去重 | PASS（用户实机） |
| 下一次攻击重新命中 | PASS（用户实机） |
| 多目标、范围、Layer、后方过滤 | PASS（用户实机） |
| 三段 10/15/20 与超额伤害归零 | PASS（用户实机） |
| MeleeHitbox PlayMode 去重测试 | DEFERRED：当前不以反射测试作为学习门槛，且不计为自动化证据 |
| Day 11 结论 | PASS：11 项手工/静态验收通过；1 项异常测试未运行；自动化延期 |

详细用例见 `Docs/TEST_REPORT/TEST_CASE_DAY11.md`。

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

- `Assets/_Game/Runtime/Combat/AttackDefinition.cs`
- `Assets/_Game/Runtime/Combat/PlayerCombat.cs`
- `Assets/_Game/Runtime/Combat/PlayerCombatAnimationEvents.cs`
- `Assets/_Game/Runtime/Combat/MeleeHitbox.cs`
- `Assets/_Game/Runtime/Player/PlayerMotor.cs`
- `Assets/_Game/Animations/Player/PlayerAnimator.controller`
- `Assets/_Game/Animations/Source/UAL2_Standard.fbx.meta`
- `Assets/_Game/Data/Combat/Attack_01.asset`
- `Assets/_Game/Data/Combat/Attack_02.asset`
- `Assets/_Game/Data/Combat/Attack_03.asset`
- `Assets/_Game/Scenes/SampleScene.unity`
- `Docs/TEST_REPORT/TEST_CASE_DAY11.md`

## Known Bugs / Risks

1. `BUG-004` Open：角色离地后仍保留完整水平控制速度；进入技能位移前处理。
2. `MeleeHitbox` PlayMode 测试延期且不计为自动化证据；已有 Health EditMode 与 PlayerMotor PlayMode 测试继续提供自动化能力证明。
3. Day 11 新组件当前保存为 `SampleScene` 的 Player Prefab 实例覆盖；在 Phase A 集成前决定是否 Apply 到 Player Prefab。
4. `Physics.OverlapSphere` 在伤害窗口每帧分配数组；Day 24 通过 Profiler 决定是否需要改为 NonAlloc。
5. `Assets/_Recovery/` 是恢复文件，不纳入正式项目提交。

## Git

- 当前分支：`main`；当前已提交 HEAD：`fc77a55`。
- Day 11 已通过，可以创建正式完成提交。
- 默认提交全部自有代码、对应 `.meta`、配置资产、Scene、测试与 Docs。
- 排除 Unity Assistant Settings、SceneTemplateSettings、`Assets/_Recovery/`、空 Debug 目录及未经确认的 ProjectSettings 变化。
- Bestiary 原始 FBX/PNG 不进入公开仓库。

## Next Task

1. Learning Day 12 使用已审计的环境素材完成约 20×20m 中世纪庭院主体。
2. 优先完成地面、外围墙、墙角、门、楼梯与平台；必要时保留隐藏白盒 Collider。
3. 执行角色尺度、楼梯、墙角、平台边缘、镜头遮挡和基础碰撞回归。
4. 不在场景日增加新的玩法脚本。

## Update Rules

- 未运行写 NOT RUN；测试名、步骤、断言和结果必须一致。
- 收到“验收”请求时，先扫描并补齐本次涉及代码的必要注释，再执行验证、更新 Docs，并在可提交时给出包含全部自有代码的 Git 指令。
- 注释解释职责、关键 API、边界和原因，不逐行复述代码。
- 每次验收结束后，根据当天实际内容提出 3–5 个理解题；回答情况用于安排后续教学，不篡改工程验收结果。
- Day 结束后同步工程、Git 和 Docs；已验收架构默认冻结。
