# Current Project Status

> Last Updated：2026-09-08
> Current Learning Day：Day 6 条件通过
> Current Phase：Week 1 验收收尾
> Next Learning Day：补全 D6-AUT-02 后进入 Day 7
> Source of Truth：当前 Unity 工程 + Git + Docs

## 验收结论

**Day 6：CONDITIONAL PASS。** 楼梯、30°/50°斜坡、墙角、沿墙滑动、平台边缘和 Week 1 手工回归均通过；斜向限速自动化测试有效且稳定。场景加载时序导致的偶发假失败已修复，并通过连续 5 次和 Unity 冷启动首次运行回归。

当前唯一未完成门槛是 `SprintRelease_RestoresNormalSpeed`：Test Runner 显示 PASS，但源码只断言普通移动速度大于 0，没有发送 Shift、释放 Shift 或比较恢复后的速度，因此不能作为 Sprint 恢复功能的有效自动化证据。

## Implemented

- Week 1 Player 控制底座：InputReader、镜头控制、CharacterController 移动、重力、转向、Reverse Turn 与 Sprint。
- Idle/Walk/Jog/Sprint Blend Tree，并由实际水平速度驱动。
- Player Prefab 与 Inspector 引用保护。
- Day 6 物理测试白盒：楼梯、30°/50°斜坡、墙角和平台。
- PlayMode 测试程序集接入 Input System，并建立虚拟键盘输入测试。
- PlayMode 场景初始化改用 `LoadSceneAsync` 明确等待加载完成。

## Test Evidence

| 范围 | 结果 |
|---|---|
| Day 3 | 11/11 PASS |
| Day 4 | 16/16 PASS |
| Day 5 | 16/16 PASS |
| Day 6 手工回归 | 10/10 PASS |
| 楼梯、30°/50°斜坡、墙角/沿墙、平台边缘 | PASS |
| `DiagonalInput_DoesNotExceedMoveSpeed` | Stable PASS |
| `SprintRelease_RestoresNormalSpeed` | INCOMPLETE：测试名与实际断言不一致 |
| 自动化初始化稳定性 | 连续 5 次 + 冷启动首次运行，均 2/2 绿灯 |
| Unity Console | 0 Error；1 条 Pipeline 非自动化模式警告 |

## Current Architecture

```text
Input System / PlayerInput
        ↓
PlayerInputReader
├─ LookInput → CameraController → CameraTarget / Cinemachine
├─ MoveInput ──┐
└─ SprintHeld ─┴→ PlayerMotor → CharacterController.Move
                                      │
                                      └→ CurrentMoveSpeed
                                               ↓
                                  PlayerAnimatorDriver → Animator

PlayMode Tests
└─ Virtual Keyboard → PlayerInput → real SampleScene Player
```

- CharacterController 是 Player Gameplay 位移的唯一执行者；Animator Root Motion=false。
- `PlayerMotorPlayModeTests` 使用独立虚拟 Keyboard，并在每个测试后移除设备。
- Combat、Health、Enemy AI、Skill、UI、GameFlow 尚未实现。

## Files

- `Assets/_Game/Runtime/Input/PlayerInputReader.cs`
- `Assets/_Game/Runtime/Camera/CameraController.cs`
- `Assets/_Game/Runtime/Player/PlayerMotor.cs`
- `Assets/_Game/Runtime/Player/PlayerAnimatorDriver.cs`
- `Assets/_Game/Tests/PlayMode/PlayerMotorPlayModeTests.cs`
- `Assets/_Game/Tests/PlayMode/Game.Tests.PlayMode.asmdef`
- `Assets/_Game/Scenes/SampleScene.unity`
- `Docs/TEST_REPORT/TEST_CASE_DAY6.md`

## Known Bugs / Risks

1. `BUG-003` Closed：PlayMode 测试只固定等待一帧导致偶发假失败；改用 `LoadSceneAsync` 后稳定回归通过。
2. `BUG-004` Open：角色离地后仍保留完整水平控制速度；当前不阻断 Week 1 地面移动验收，进入技能位移前必须明确空中控制规则。
3. `SprintRelease_RestoresNormalSpeed` 尚未覆盖其命名目标，不能用绿灯替代测试设计验收。

## Git

- 当前分支：`main`；HEAD：`7e0182d`。
- Day 6 场景、测试、代码注释和 Docs 尚未提交。
- `ProjectSettings/Packages/com.unity.ai.assistant/Settings.json` 与 `ProjectSettings/SceneTemplateSettings.json` 属于本地工具/编辑器状态，本次不提交。
- 默认提交全部自有代码及对应 `.meta`、程序集配置、Scene 与 Docs。

## Next Task

### Day 6 收尾（先做）

- 补全 `SprintRelease_RestoresNormalSpeed`：记录普通速度 → W+Shift 验证速度提高 → 释放 Shift 保留 W → 验证恢复到普通速度容差内。
- 连续运行 5 次并执行一次 Unity 冷启动首次 Run All。
- 通过后把 Day 6 和 Week 1 改为 PASS，并创建 `week-01-movement` 标签。

### Learning Day 7（通过上项后）

- 从失败测试开始实现 `DamageInfo`、`IDamageable` 与通用 `Health`。
- 不在 Health 中引用 UI、Animator、Player 或 Enemy。

## Update Rules

- 未运行写 NOT RUN；测试名、步骤、断言和结果必须一致。
- 绿灯只证明现有断言通过，不自动证明用例标题描述的行为已覆盖。
- Day 结束后同步工程、Git 和 Docs；已验收架构默认冻结。
