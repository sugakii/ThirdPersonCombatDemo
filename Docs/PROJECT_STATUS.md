# Current Project Status

> Last Updated：2026-09-26  
> Current Learning Day：Day 19 验收通过  
> Current Phase：Phase B / 最小菜单、暂停与功能冻结  
> Next Checkpoint：Day 20——开始界面、暂停菜单与庭院有限润色  
> Source of Truth：当前 Unity 工程 + Git + Docs  
> Remaining Plan：`Docs/plans/2026-09-21-five-day-sprint.md`

## 验收结论

**Day 19：PASS。** 火焰突进 VFX、对象池、冷却 UI、Playing/Victory/GameOver、帧末结果仲裁、战斗冻结、结果面板和按钮重开均已落地。用户截图确认 PlayMode `5/5 PASS`；Unity MCP 实测 Victory、GameOver 与场景重载通过，Gameplay Console `0 Error`。可靠重开由结果面板按钮完成，不再要求额外的 R 键入口。

## Implemented

- `SkillVfxPool` 使用 Unity `ObjectPool<GameObject>` 复用 `FireDashVFX`，归还前清理 ParticleSystem、TrailRenderer 与父子关系。
- 技能成功释放后让粒子从 Imp 武器的 `SkinnedMeshRenderer` 表面发射；当前效果定位为功能版占位，不在 Demo 完成前继续扩展 VFX。
- `CooldownPresenter` 只读取 `SkillController` 的冷却状态，更新径向填充和向上取整的 `3/2/1` 文本，不参与释放判定。
- `GameFlowController` 订阅 Player 与三个 Enemy 的 `Health.Died`，在帧末统一判定结果。
- 同帧 Player 与最后一个 Enemy 死亡时固定 GameOver 优先，不依赖事件触发顺序。
- Victory/GameOver 后关闭 Player Motor、Combat、Skill、HitDetector 与 CameraController，停止 Enemy StateMachine、NavMeshAgent 和 EnemyCombat。
- `VictoryPanel`、`GameOverPanel` 默认在运行时隐藏，结果确定后只显示对应面板。
- 两个结果面板的 Restart Button 均调用 `GameFlowController.RestartGame()` 重载 `SampleScene`。
- 新增 `GameFlowPlayModeTests.PlayerAndLastEnemyDieInSameFrame_GameOverTakesPriority`。
- Day 19 涉及的 GameFlow、测试、Cooldown UI 与 VFX Pool 已补充职责和原因型注释。

## Test Evidence

| 范围 | 结果 |
|---|---|
| PlayMode 自动化 | **5/5 PASS**：GameFlow 1、PlayerMotor 3、SkillHitDetector 1（用户截图） |
| Playing 初始状态 | PASS：Victory/GameOver 面板均隐藏（Unity MCP） |
| 三个 Enemy 全部死亡 | PASS：只显示 VictoryPanel（Unity MCP） |
| Player 死亡 | PASS：只显示 GameOverPanel（Unity MCP） |
| 同帧双方死亡 | PASS：GameOver 优先（PlayMode 自动化） |
| Restart Button / 场景重载 | PASS：重载后两个面板隐藏、Console 0 Error（Unity MCP） |
| 场景引用 | PASS：GameFlow 的 Player、Combat、Skill、Camera 与两个 Panel 引用均已持久化 |
| Scene 状态 | PASS：`SampleScene` 已保存，`isDirty=false` |
| Gameplay Console | **0 Error** |

详细结果见 `Docs/TEST_REPORT/TEST_CASE_DAY19.md`。

## Current Architecture

```text
Health.Died（Player / 3 Enemies）
                ↓
        GameFlowController
        ├─ 帧末收集死亡结果
        ├─ Player 死亡优先 → GameOver
        ├─ Enemy 全灭 → Victory
        ├─ FreezeGameplay
        └─ RestartGame → Reload SampleScene
```

- `GameFlowController` 只协调结果、冻结和重开，不持有伤害或 AI 决策规则。
- Health 仍不依赖 GameFlow；连接通过局部 `Died` 事件完成。
- 场景重载作为最小可靠重置方案，一次性清除生命、冷却、命中集合和事件订阅等运行时状态。
- `CooldownPresenter` 属于表现层，只读取 SkillController 的公开状态。

## Files

- `Assets/_Game/Runtime/GameFlow/GameFlowController.cs`
- `Assets/_Game/Runtime/Skills/SkillVfxPool.cs`
- `Assets/_Game/Runtime/Skills/SkillController.cs`
- `Assets/_Game/Runtime/UI/CooldownPresenter.cs`
- `Assets/_Game/Tests/PlayMode/GameFlowPlayModeTests.cs`
- `Assets/_Game/Prefabs/VFX/FireDashVFX.prefab`
- `Assets/_Game/Scenes/SampleScene.unity`
- `Docs/TEST_REPORT/TEST_CASE_DAY19.md`

## Known Bugs / Risks

1. `RestartGame()` 当前使用场景名 `SampleScene`；Day 20 增加 MainMenu 后复核 Build Settings 与场景切换关系。
2. `Physics.OverlapSphere()` 仍会分配数组；留到质量阶段用 Profiler 决定是否需要 NonAlloc，不提前优化。
3. `MeleeHitbox` PlayMode 测试仍延期，不能算作已有自动化证据。
4. `Assets/_Recovery/` 与 Unity/MCP 自动产生的 ProjectSettings 变化不进入本次提交。

## Git

- 当前分支：`main`；Day 19 GameFlow 与文档可单独提交。
- 本次提交只包含 GameFlow、自有 VFX 修正、Scene、PlayMode 测试和 Docs。
- 排除 Render Pipeline/ProjectSettings 自动变化、Unity Assistant Settings、SceneTemplateSettings、`Assets/_Recovery/`、空 Debug 元文件和异常临时文件。

## Next Task

1. 创建 `MainMenu` 场景，只提供开始游戏和退出游戏。
2. 实现 Esc 暂停菜单：继续、重新开始、返回主菜单，并正确恢复 `Time.timeScale` 与鼠标状态。
3. 只做少量庭院 Props、构图和基础灯光整理；Day 20 结束后冻结功能。

## Update Rules

- 未运行写 NOT RUN；测试名、步骤、断言和结果必须一致。
- 收到“验收”请求时，先扫描并补齐本次涉及代码的必要注释，再执行验证、更新 Docs，并在可提交时给出包含全部自有代码的 Git 指令。
- 注释解释职责、关键 API、边界和原因，不逐行复述代码。
- 每次验收结束后，根据当天实际内容提出 3–5 个理解题；回答情况用于安排后续教学，不篡改工程验收结果。
- Day 结束后同步工程、Git 和 Docs；已验收架构默认冻结。
