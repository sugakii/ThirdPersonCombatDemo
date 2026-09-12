# Current Project Status

> Last Updated：2026-09-12
> Current Learning Day：Day 9 已完成
> Current Phase：Week 2 / 攻击动画与静态配置完成，准备 Combo Runtime
> Next Checkpoint：Learning Day 10
> Source of Truth：当前 Unity 工程 + Git + Docs

## 验收结论

**Day 9：PASS。** UAL2、三段攻击 Motion、三个 AttackDefinition、Attack 输入意图与 PlayerCombat 骨架均已持久化。复验 18/18 PASS，Player Prefab 引用完整，Gameplay Console 0 Error；BUG-007/008/009 已关闭。

## Implemented

- UAL2 以 Humanoid / Create From This Model 导入，包含 43 个 Clips；剑击 A/B/C 均存在且不循环。
- 用户已分别预览 `Sword_Regular_A/B/C`，三段均无明显骨骼变形、异常位移或反转。
- PlayerAnimator 已建立 Attack_01、Attack_02、Attack_03 State，分别绑定 Sword_Regular_A/B/C。
- `AttackDefinition` 使用 ScriptableObject 保存 Damage 与 Animator State Name 静态配置。
- `Attack_01/02/03.asset` 已建立在 `Assets/_Game/Data/Combat/`。
- `PlayerInputReader.AttackPressed` 使用 `WasPressedThisFrame()` 输出单帧攻击意图。
- `PlayerCombat` 位于 Runtime/Combat；当前只保留依赖骨架，尚未挂载或实现 Combo，符合 Day 9 范围。
- Day 9 涉及代码已补充职责、配置边界和输入语义注释。

## Test Evidence

| 范围 | 结果 |
|---|---|
| UAL2 Importer | PASS：Human / Create From This Model / 43 Clips |
| A/B/C Clip 与 Loop | PASS：均存在且不循环 |
| 三段视觉预检 | PASS：用户实机确认 |
| Animator State 存在性 | PASS：3/3 |
| Attack_01 Motion | PASS：Sword_Regular_A |
| Attack_02 Motion | PASS：Sword_Regular_B |
| Attack_03 Motion | PASS：Sword_Regular_C |
| Attack_01 配置 | PASS：10 / Attack_01 |
| Attack_02 配置 | PASS：10 / Attack_02 |
| Attack_03 配置 | PASS：10 / Attack_03 |
| Day 9 脚本静态检查 | PASS：0 编译错误 |
| Player Art 目录 | PASS：已恢复到 `_Game/Art` |
| Player Prefab 引用 | PASS：Imp/Animator/Avatar/Controller 有效，Missing Script=0 |
| Gameplay Console | 0 Error；1 条外部 Pipeline Warning |
| Day 9 总计 | 18 PASS / 0 FAIL |

详细步骤见 `Docs/TEST_REPORT/TEST_CASE_DAY9.md`。

## Current Architecture

```text
Input System Attack
        ↓ WasPressedThisFrame
PlayerInputReader.AttackPressed
        ↓（Day 10+）
PlayerCombat（当前仅骨架）
        ├─ Animator
        └─ AttackDefinition[]（静态配置）
```

- `AttackDefinition` 只保存静态数据，不保存当前 Combo 段数、缓存输入或命中目标。
- `PlayerCombat` 通过同对象 `GetComponent` 获取 PlayerInputReader；子物体 Imp 的 Animator 保留显式 Inspector 引用。
- Animator 只负责表现，Gameplay 位移仍由 CharacterController / PlayerMotor 控制。
- `_Rec` Clips 已确认是收招，不作为 Attack_01/02 主攻击动画。

## Files

- `Assets/_Game/Animations/Source/UAL2_Standard.fbx`
- `Assets/_Game/Animations/Player/PlayerAnimator.controller`
- `Assets/_Game/Runtime/Combat/AttackDefinition.cs`
- `Assets/_Game/Runtime/Combat/PlayerCombat.cs`
- `Assets/_Game/Runtime/Input/PlayerInputReader.cs`
- `Assets/_Game/Data/Combat/Attack_01.asset`
- `Assets/_Game/Data/Combat/Attack_02.asset`
- `Assets/_Game/Data/Combat/Attack_03.asset`
- `Docs/TEST_REPORT/TEST_CASE_DAY9.md`

## Known Bugs / Risks

1. `BUG-004` Open：角色离地后仍保留完整水平控制速度；进入技能位移前处理。
2. `BUG-007` Closed：Attack_01/02/03 状态名分别正确；Damage=10 为合法占位值。
3. `BUG-008` Closed：Attack_03 已绑定 Sword_Regular_C。
4. `BUG-009` Closed：Player Art 已恢复，Prefab 引用回归通过。
5. `Assets/_Recovery/0.unity` 是恢复文件，不纳入正式项目提交。

## Git

- 当前分支：`main`；当前已提交 HEAD：`17fc48a`。
- Day 9 代码、资源配置与文档尚未提交。
- Day 9 已通过，可以创建完成提交。
- 默认提交全部自有代码、对应 `.meta`、配置资产、测试、Scene 与 Docs。
- 继续排除 Unity Assistant Settings、SceneTemplateSettings、`Assets/_Recovery/` 和未经确认的 ProjectSettings 变化。
- Bestiary 原始 FBX/PNG 不进入公开仓库。

## Next Task

1. 进入 Learning Day 10，实现最小 Combo Runtime 与输入缓存。
2. `PlayerCombat` 消费 `AttackPressed`，根据当前段读取对应 AttackDefinition。
3. 明确攻击衔接窗口；伤害窗口与 MeleeHitbox 去重按独立步骤实现和测试。

## Update Rules

- 未运行写 NOT RUN；测试名、步骤、断言和结果必须一致。
- 收到“验收”请求时，先扫描并补齐本次涉及代码的必要注释，再执行验证、更新 Docs，并在可提交时给出包含全部自有代码的 Git 指令。
- 注释解释职责、关键 API、边界和原因，不逐行复述代码。
- 每次验收结束后，根据当天实际内容提出 3–5 个理解题；回答情况用于安排后续教学，不篡改工程验收结果。
- Day 结束后同步工程、Git 和 Docs；已验收架构默认冻结。
