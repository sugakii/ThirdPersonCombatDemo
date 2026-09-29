# Current Project Status

> Last Updated：2026-09-27  
> Current Learning Day：Day 21 验收通过  
> Current Phase：Phase C / 功能冻结与交付质量  
> Next Checkpoint：Day 22——Windows Build、README、架构图与 Release  
> Source of Truth：当前 Unity 工程 + Git + Docs  

## 验收结论

**Day 21：PASS。** 完成运行时代码、场景/Prefab 引用、事件生命周期和物理查询热路径审查；修复 Puglin Prefab 共享引用缺失，普攻/技能检测改用 NonAlloc 固定缓冲；EditMode `14/14`、PlayMode `5/5` 全部通过，Console `0 Error / 0 Warning`，P0/P1 为 0。

## Implemented

- `MainMenu` 场景已加入 Build Index 0，`SampleScene` 为 Build Index 1。
- `MainMenuController` 提供开始游戏与退出 Build。
- `PauseMenuController` 通过 Esc 切换暂停，提供继续、重开和返回主菜单，并协调 `Time.timeScale`、鼠标和 Gameplay 输入消费者。
- Pause 期间发生 GameOver 时，GameFlow 正确接管结果面板并恢复 `timeScale=1`。
- 庭院已导入并放置 Wagon/Crate；13/13 实例使用简化 BoxCollider，战斗中心保持开阔。
- `PlayerMotor.OnDisable()` 在组件禁用时清零移动速度，避免终局继续保留陈旧 locomotion 参数。
- `Puglin.prefab` 已保存共享的 AttackDefinition 与 Animator；Player Target 作为场景对象继续由三个实例显式绑定。
- `MeleeHitbox` 与 `SkillHitDetector` 使用固定 Collider 缓冲和 `OverlapSphereNonAlloc`，攻击检测热路径不再返回新数组。
- `CooldownPresenter` 只在倒计时整数变化时更新文本，冷却期间不再逐帧创建字符串。
- `EnemyStateMachine` 补充 AttackDefinition 与 Target Health 引用验证；技能临时调试日志已移除。
- 冷却结束自动化测试已恢复，当前稳定自动化总数为 19 条。

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
| EditMode 自动化 | **14/14 PASS**：Health 10、SkillCooldown 4（Day 21 Unity MCP） |
| PlayMode 自动化 | **5/5 PASS**：GameFlow 1、PlayerMotor 3、SkillHitDetector 1（Day 21 Unity MCP） |
| Playing 初始状态 | PASS：Victory/GameOver 面板均隐藏（Unity MCP） |
| 三个 Enemy 全部死亡 | PASS：只显示 VictoryPanel（Unity MCP） |
| Player 死亡 | PASS：只显示 GameOverPanel（Unity MCP） |
| 同帧双方死亡 | PASS：GameOver 优先（PlayMode 自动化） |
| Restart Button / 场景重载 | PASS：重载后两个面板隐藏、Console 0 Error（Unity MCP） |
| 场景引用 | PASS：GameFlow 的 Player、Combat、Skill、Camera 与两个 Panel 引用均已持久化 |
| Scene 状态 | PASS：`SampleScene` 已保存，`isDirty=false` |
| Gameplay Console | **0 Error** |
| MainMenu / PauseMenu | PASS：开始、退出、继续、重开、返回主菜单绑定正确 |
| Pause × GameOver | PASS：GameOver 接管并恢复 timeScale |
| Prop 碰撞 | PASS：13/13 BoxCollider 有效，Dash 未穿过 Wagon |
| NavMesh | PASS：3/3 Puglin 到 Player 路径完整 |
| 三轮完整流程 | PASS：Victory→Restart、GameOver→MainMenu、Pause/Resume→Victory→MainMenu |

Day 21 详细结果见 `Docs/TEST_REPORT/TEST_CASE_DAY21.md`。

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
- `Assets/_Game/Prefabs/Enemy/Puglin.prefab`
- `Assets/_Game/Runtime/Combat/MeleeHitbox.cs`
- `Assets/_Game/Runtime/Enemy/EnemyStateMachine.cs`
- `Assets/_Game/Runtime/Skills/SkillHitDetector.cs`
- `Assets/_Game/Runtime/UI/CooldownPresenter.cs`
- `Assets/_Game/Tests/EditMode/SkillCooldownTests.cs`
- `Docs/TEST_REPORT/TEST_CASE_DAY21.md`

## Known Bugs / Risks

1. `MeleeHitbox` 独立 PlayMode 测试仍延期，不能算作已有自动化证据。
2. 独立 Windows Build 尚未生成和测试。

## Git

- 当前分支：`main`；Day 20/21 自有代码、场景、Prefab、测试和文档可提交。
- 本次提交包含已验收的菜单/场景内容、Day 21 性能修正、Prefab 配置与 Docs。
- 排除 Render Pipeline/ProjectSettings 自动变化、Unity Assistant Settings、SceneTemplateSettings、`Assets/_Recovery/`、空 Debug 元文件和异常临时文件。

## Next Task

1. 不再新增玩法或场景内容。
2. 生成 Windows x64 Build 并完成独立运行专项测试。
3. 完成 README、架构图、Build Test Report 与素材许可说明。
4. 整理演示视频和 `v1.0.0` Release 前材料。

## Update Rules

- 未运行写 NOT RUN；测试名、步骤、断言和结果必须一致。
- 收到“验收”请求时，先扫描并补齐本次涉及代码的必要注释，再执行验证、更新 Docs，并在可提交时给出包含全部自有代码的 Git 指令。
- 注释解释职责、关键 API、边界和原因，不逐行复述代码。
- 每次验收结束后，根据当天实际内容提出 3–5 个理解题；回答情况用于安排后续教学，不篡改工程验收结果。
- Day 结束后同步工程、Git 和 Docs；已验收架构默认冻结。
