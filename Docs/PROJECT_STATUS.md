# Current Project Status

> Last Updated：2026-09-22  
> Current Learning Day：Day 18 验收通过（五日冲刺第 1 天）  
> Current Phase：Phase B / Skill VFX、Cooldown UI 与 GameFlow  
> Next Checkpoint：Day 19——火焰表现、冷却 UI、胜负与重开  
> Source of Truth：当前 Unity 工程 + Git + Docs  
> Remaining Plan：`Docs/plans/2026-09-21-five-day-sprint.md`

## 验收结论

**Day 18：PASS。** `Sword_Dash` 已完成代码受控位移、碰撞阻挡、伤害窗口、多目标伤害和同目标多 Collider 去重；`BUG-004` 已通过受限空中控制关闭。EditMode `13/13 PASS`，PlayMode `4/4 PASS`，Console 0 Error。

## Implemented

- E 成功释放后播放 `Sword_Dash`；技能静态参数来自 `FireDash.asset`：Damage=50、Cooldown=3s、DashDistance=1.5m、DashDuration=0.5s。
- `SkillController` 负责释放准入、冷却、动画和伤害窗口启动，不直接修改 Player Transform。
- `PlayerMotor.TryStartDash()` 接收位移请求，并由 `CharacterController.Move()` 执行 Dash、重力和碰撞处理。
- Dash 最后一帧按剩余时间截断，避免不同帧率下累计位移超过配置距离。
- `PlayerMotor.DashEnded` 通知 `SkillHitDetector` 关闭伤害检测。
- `SkillHitDetector` 使用独立的 `SkillHitboxCenter`、Radius=1.2、Enemy LayerMask；按 `IDamageable` 去重，同一目标的多个 Collider 每次 Dash 只结算一次。
- 空中普通移动乘以 `airControlMultiplier=0.25`，不再保留完整地面水平控制，`BUG-004` Closed。
- Day 18 新增和修改代码已补充职责、边界与原因型注释；测试中由 `RequireComponent` 自动添加的依赖不再重复创建。

## Test Evidence

| 范围 | 结果 |
|---|---|
| Sword_Dash 动画与配置 | PASS：Motion 正确、Speed=1.2、Foot IK=false |
| 受控位移 | PASS：PlayerMotor + CharacterController，未直接修改 Transform |
| 贴墙 / 墙角 | PASS：受到碰撞阻挡，不穿墙 |
| 30° / 50°斜坡 | PASS |
| 平台边缘与空中控制 | PASS：空中水平控制降为地面的 25% |
| Dash 中重复 E / 冷却输入 | PASS：不会覆盖当前 Dash 或冷却 |
| 单目标与多目标伤害 | PASS |
| 同目标多 Collider 去重 | PASS：一次检测只调用一次 TakeDamage |
| EditMode | **13/13 PASS** |
| PlayMode | **4/4 PASS**：PlayerMotor 3 条、SkillHitDetector 1 条 |
| 编译 | Runtime、EditMode、PlayMode 程序集 0 Error / 0 Warning |
| 场景持久化 | PASS：独立 SkillHitboxCenter、Radius=1.2、Enemy Mask；临时 Cube 已删除，Scene dirty=false |
| Console | **0 Error** |

详细步骤与结果见 `Docs/TEST_REPORT/TEST_CASE_DAY18.md`。

## Current Architecture

```text
PlayerInputReader.SkillPressed
        ↓
SkillController
├─ SkillDefinition（静态伤害、冷却、距离、时长、动画名）
├─ RemainingCoolDown（运行时状态）
├─ PlayerMotor.TryStartDash(...)
└─ SkillHitDetector.BeginDetection(DamageInfo)
        ↓
PlayerMotor
├─ CharacterController.Move
├─ Dash 生命周期 / 碰撞 / 重力
└─ DashEnded ──► SkillHitDetector.EndDetection
                         ↓
               IDamageable.TakeDamage
```

- `PlayerMotor` 仍是 Player 唯一位移执行者。
- `SkillHitDetector` 依赖 `IDamageable`，不依赖 Enemy 或 Health 的具体实现。
- 普攻和技能分别使用 `MeleeHitboxCenter` 与 `SkillHitboxCenter`，可独立调整范围。
- 运行时 Dash、冷却和命中集合不写回 ScriptableObject。

## Files

- `Assets/_Game/Runtime/Player/PlayerMotor.cs`
- `Assets/_Game/Runtime/Skills/SkillController.cs`
- `Assets/_Game/Runtime/Skills/SkillHitDetector.cs`
- `Assets/_Game/Data/Skills/FireDash.asset`
- `Assets/_Game/Animations/Player/PlayerAnimator.controller`
- `Assets/_Game/Scenes/SampleScene.unity`
- `Assets/_Game/Tests/EditMode/SkillCooldownTests.cs`
- `Assets/_Game/Tests/PlayMode/PlayerMotorPlayModeTests.cs`
- `Assets/_Game/Tests/PlayMode/SkillHitDetectorPlayModeTests.cs`
- `Docs/TEST_REPORT/TEST_CASE_DAY18.md`

## Known Bugs / Risks

1. `Physics.OverlapSphere()` 在技能检测期间会分配数组；留到质量阶段用 Profiler 确认后再决定是否改为 NonAlloc，不提前优化。
2. `MeleeHitbox` PlayMode 测试仍延期，不能算作已有自动化证据。
3. `Assets/_Recovery/` 是恢复文件，不纳入正式项目提交。

## Git

- 当前分支：`main`；Day 18 已通过，可以创建 Dash 位移与伤害提交。
- 默认提交全部自有代码、对应 `.meta`、配置资产、Animator、Scene、测试与 Docs。
- 排除 Unity Assistant Settings、SceneTemplateSettings、`Assets/_Recovery/`、空 Debug/Combat 目录元文件及未经确认的 ProjectSettings 变化。
- Bestiary 原始 FBX/PNG 不进入公开仓库。

## Next Task

1. Day 19 上午：用 ParticleSystem + TrailRenderer 制作火焰 Dash 表现，并通过 Unity `ObjectPool<T>` 复用。
2. Day 19 上午：实现 `CooldownPresenter`，只读取技能冷却状态，不决定释放规则。
3. Day 19 下午：实现 Playing、Victory、GameOver 和 R 重开，补至少 1 条 GameFlow 自动化测试。

## Update Rules

- 未运行写 NOT RUN；测试名、步骤、断言和结果必须一致。
- 收到“验收”请求时，先扫描并补齐本次涉及代码的必要注释，再执行验证、更新 Docs，并在可提交时给出包含全部自有代码的 Git 指令。
- 注释解释职责、关键 API、边界和原因，不逐行复述代码。
- 每次验收结束后，根据当天实际内容提出 3–5 个理解题；回答情况用于安排后续教学，不篡改工程验收结果。
- Day 结束后同步工程、Git 和 Docs；已验收架构默认冻结。
