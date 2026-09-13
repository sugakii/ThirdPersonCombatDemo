# Current Project Status

> Last Updated：2026-09-13
> Current Learning Day：Day 10 已完成
> Current Phase：Week 2 / Combo Runtime 完成
> Next Checkpoint：Learning Day 11 — 伤害窗口与命中去重
> Source of Truth：当前 Unity 工程 + Git + Docs

## 验收结论

**Day 10：PASS。** 三段 Combo、输入缓存、Recovery Cancel 与逐段攻击朝向均已实现。工程静态检查、引用和持久化通过，用户完成 5 组实机回归；正式结果为 16/16 PASS，Gameplay Console 0 Error。

## Implemented

- `PlayerCombat` 已挂载到 Player，显式绑定 Imp Animator 与三个 AttackDefinition。
- Combo 索引、攻击状态、缓存输入、输入窗口和推进窗口均属于 PlayerCombat 运行时状态。
- `StartAttack()` 固定从第一段开始；`AdvanceCombo()` 消费缓存并保护数组边界。
- Attack_01/02 支持独立 Recovery；Recovery 阶段仍可取消并衔接下一段。
- Attack_03 完成后结束 Combo，不会出现第四段。
- `PlayerCombatAnimationEvents` 挂在 Imp，只把 Animation Event 转发给 PlayerCombat。
- A/B Clip 配置 OpenComboInput、OpenComboAdvance、EnterRecovery；两个 Recovery 和 C 配置 EndAttack。
- `PlayerMotor.FaceCameraForward()` 在第一段和后续段开始前按镜头水平朝向瞬间转向，并清除旧反向转身状态。
- AttackDefinition 保存 Damage、Attack State Name 和可选 Recovery State Name；三段 Damage 为 10/15/20。
- Day 10 涉及代码已补齐必要职责与原因型注释，无遗留诊断 Debug.Log。

## Test Evidence

| 范围 | 结果 |
|---|---|
| Day 10 脚本静态验证 | PASS：0 编译错误 |
| PlayerCombat 引用 | PASS：Animator 已绑定，Attack 数组长度 3 |
| Animation Event Bridge | PASS：存在并挂在 Imp |
| Attack/Recovery States | PASS：A/B/C 与 A_Rec/B_Rec 均存在 |
| 单击 A → Recovery → Locomotion | PASS（用户实机） |
| 提前输入缓存且不加速 | PASS（用户实机） |
| Recovery Cancel | PASS（用户实机） |
| A → B → C，无第四段 | PASS（用户实机） |
| 逐段镜头朝向 | PASS（用户实机） |
| 疯狂连点 | PASS（用户实机） |
| SampleScene 保存 | PASS：isDirty=false |
| Gameplay Console | 0 Error；1 条外部 Pipeline Warning |
| Day 10 总计 | 16 PASS / 0 FAIL / 0 NOT RUN |

详细用例见 `Docs/TEST_REPORT/TEST_CASE_DAY10.md`。

## Current Architecture

```text
PlayerInputReader.AttackPressed
        ↓
PlayerCombat
├─ currentComboIndex / bufferedAttack
├─ comboInputOpen / comboAdvanceOpen
├─ AttackDefinition[3]
├─ Animator.CrossFade
└─ PlayerMotor.FaceCameraForward
        ▲
        │ 只转发动画时机
PlayerCombatAnimationEvents（Imp）

MeleeHitbox / Damage Window
└─ Planned for Day 11
```

- Animator 和 Animation Event 提供表现与时机，PlayerCombat 持有 Combo 规则。
- Animation Event Bridge 不查找目标、不结算伤害。
- PlayerMotor 仍是 Player Gameplay 位移和朝向的唯一执行者。
- 当前没有 Hitbox；AttackDefinition.Damage 将在 Day 11 接入伤害结算。

## Files

- `Assets/_Game/Runtime/Combat/AttackDefinition.cs`
- `Assets/_Game/Runtime/Combat/PlayerCombat.cs`
- `Assets/_Game/Runtime/Combat/PlayerCombatAnimationEvents.cs`
- `Assets/_Game/Runtime/Player/PlayerMotor.cs`
- `Assets/_Game/Animations/Player/PlayerAnimator.controller`
- `Assets/_Game/Animations/Source/UAL2_Standard.fbx.meta`
- `Assets/_Game/Data/Combat/Attack_01.asset`
- `Assets/_Game/Data/Combat/Attack_02.asset`
- `Assets/_Game/Data/Combat/Attack_03.asset`
- `Assets/_Game/Scenes/SampleScene.unity`
- `Docs/TEST_REPORT/TEST_CASE_DAY10.md`

## Known Bugs / Risks

1. `BUG-004` Open：角色离地后仍保留完整水平控制速度；进入技能位移前处理。
2. `PlayerCombat` 与 Animation Event Bridge 的缺失引用异常测试尚未执行，安排在战斗集成回归。
3. `Assets/_Recovery/` 是恢复文件，不纳入正式项目提交。

## Git

- 当前分支：`main`；当前已提交 HEAD：`4c6835e`。
- Day 10 已通过，可以创建正式完成提交。
- 默认提交全部自有代码、对应 `.meta`、配置资产、Scene、测试与 Docs。
- 排除 Unity Assistant Settings、SceneTemplateSettings、`Assets/_Recovery/`、空 Debug 目录及未经确认的 ProjectSettings 变化。
- Bestiary 原始 FBX/PNG 不进入公开仓库。

## Next Task

1. Learning Day 11 实现最小 `MeleeHitbox`。
2. 用 Animation Event 打开/关闭伤害窗口；窗口打开时建立本次攻击独立命中集合。
3. 只通过 `IDamageable.TakeDamage(DamageInfo)` 结算，不依赖 Enemy 具体类型。
4. 增加一条 PlayMode 测试：同一攻击窗口内，同一目标多个 Collider 只扣一次血；下一次攻击可以再次扣血。

## Update Rules

- 未运行写 NOT RUN；测试名、步骤、断言和结果必须一致。
- 收到“验收”请求时，先扫描并补齐本次涉及代码的必要注释，再执行验证、更新 Docs，并在可提交时给出包含全部自有代码的 Git 指令。
- 注释解释职责、关键 API、边界和原因，不逐行复述代码。
- 每次验收结束后，根据当天实际内容提出 3–5 个理解题；回答情况用于安排后续教学，不篡改工程验收结果。
- Day 结束后同步工程、Git 和 Docs；已验收架构默认冻结。
