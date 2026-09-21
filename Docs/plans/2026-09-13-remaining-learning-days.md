# Remaining Learning Days Implementation Plan

> **执行状态：** Day 17 后已由 `Docs/plans/2026-09-21-five-day-sprint.md` 接管；本文保留作为原始需求与 Day 18–27 映射依据。

> **For Claude:** REQUIRED SUB-SKILL: Use executing-plans to implement this plan task-by-task.

**Goal:** 在 2026-09-30 前完成可玩、可测试、可解释的第三人称战斗 Vertical Slice 及投递材料。

**Architecture:** 延续现有 InputReader → Runtime Controller → Animator/Hitbox/Health 的显式依赖链。每个学习日只完成一个可独立验收的切片；运行时状态不写入 ScriptableObject，Animation Event 只报告时机，伤害只通过 IDamageable 结算。

**Tech Stack:** Unity 6000.5.6f1、URP、Input System、CharacterController、Cinemachine、AI Navigation、Unity Test Framework、Git。

---

## 固定日程（每天 3–4 小时）

1. 15 分钟：读取 `Docs/PROJECT_STATUS.md` 和当日涉及代码。
2. 90–120 分钟：按学习规则实现当天唯一功能切片。
3. 45–60 分钟：功能、边界、异常测试；只登记真实复现的 Bug。
4. 30–45 分钟：修复/回归、更新 Docs、验收并提交 Git。

完整代码仍遵守教学披露规则：先给 API、职责和验收条件，由学习者独立实现；失败后再给最小提示或完整实现。

## Phase A：完成庭院主体、近战与 Enemy（Day 11–16）

### Day 11｜9/14：伤害窗口与 MeleeHitbox

**学习：** Trigger 生命周期、`HashSet<T>` 去重、接口调用、Animation Event 边界。

**文件：**
- Create: `Assets/_Game/Runtime/Combat/MeleeHitbox.cs`
- Modify: `Assets/_Game/Runtime/Combat/PlayerCombat.cs`
- Modify: `Assets/_Game/Runtime/Combat/PlayerCombatAnimationEvents.cs`
- Modify: `Assets/_Game/Animations/Source/UAL2_Standard.fbx.meta`

**开发：** 用动画事件打开/关闭攻击窗口；窗口开始时清空本次命中集合；命中时只调用 `IDamageable.TakeDamage(new DamageInfo(...))`。

**测试：** 窗口外不伤害、窗口内伤害、同目标多个 Collider 只扣一次、下一次攻击可再次伤害、多目标各扣一次。

**交付：** Player 的三段攻击可以对测试目标造成 10/15/20 点伤害，并完成窗口、去重、范围、Layer、方向和多目标手工回归。`MeleeHitbox` PlayMode 测试不再阻断 Day 11；反射配置难度超出当前学习阶段，后续只有在能用清晰测试夹具实现时才补。

### Day 12｜9/15：20×20m 中世纪庭院主体

**学习：** 模块化场景搭建、尺度与动线、模块吸附、静态碰撞和第三人称镜头空间。

**文件：**
- Modify: `Assets/_Game/Scenes/SampleScene.unity`
- Create/Modify: `Assets/_Game/Prefabs/Environment/` 中实际复用的场景 Prefab
- Modify: `Docs/ASSET_AUDIT.md`

**开发：** 用审计过的 `Floor_Brick`、`Wall_Plaster_Straight`、`Corner_Exterior_Wood`、门/门框、直楼梯/平台、木箱、马车和木栅栏等实际素材替换白盒外观，完成约 20×20m 的封闭庭院；白盒 Collider 可作为隐藏碰撞层保留，不制作屋顶和复杂装饰。

**测试：** 角色与门、墙、楼梯、平台的尺度；楼梯通行、墙角卡死、平台边缘、镜头遮挡、斜向移动；Console 无持续异常。

**交付：** 一个可供后续 NavMesh、3 Enemy 战斗和最终展示共同使用的庭院主体场景。

### Day 13｜9/16：Puglin 导入与 Enemy Prefab

**学习：** Humanoid Avatar、Prefab 引用、材质实例、Collider 与角色比例。

**文件：**
- Create: `Assets/_Game/Prefabs/Enemy/Puglin.prefab`
- Create: `Assets/_Game/Materials/MI_Puglin.mat`
- Modify: `Assets/_Game/Scenes/SampleScene.unity`
- Modify: `Docs/ASSET_AUDIT.md`

**开发：** 只导入 Puglin 模型及必需纹理；配置 Animator、Health=50、Collider、世界空间血条；验证 Idle/Jog/Attack/Hit/Death 重定向。

**测试：** 无骨骼变形、无 Root Motion 漂移、材质正确、Player 攻击能扣血、死亡事件只触发一次。

**交付：** 一个可受伤、可死亡、可复用的 Puglin Prefab。

### Day 14｜9/17：NavMesh 与 Idle/Chase

**学习：** NavMeshSurface、NavMeshAgent、状态入口/退出、距离边界。

**文件：**
- Create: `Assets/_Game/Runtime/Enemy/EnemyStateMachine.cs`
- Create: `Assets/_Game/Runtime/Enemy/States/EnemyIdleState.cs`
- Create: `Assets/_Game/Runtime/Enemy/States/EnemyChaseState.cs`
- Modify: `Assets/_Game/Prefabs/Enemy/Puglin.prefab`
- Modify: `Assets/_Game/Scenes/SampleScene.unity`

**开发：** 烘焙最小 NavMesh；只实现 Idle ↔ Chase；由状态机统一切换 Animator 和 Agent。

**测试：** 仇恨距离内外、障碍绕行、NavMesh 边缘、Player 丢失/禁用时不报错。

**交付：** Puglin 能稳定发现并追逐 Player，不实现攻击。

### Day 15｜9/18：Enemy Attack/Hit/Dead

**学习：** 状态互斥、受击中断、死亡终态、事件订阅生命周期。

**文件：**
- Create: `Assets/_Game/Runtime/Enemy/States/EnemyAttackState.cs`
- Create: `Assets/_Game/Runtime/Enemy/States/EnemyHitState.cs`
- Create: `Assets/_Game/Runtime/Enemy/States/EnemyDeadState.cs`
- Create: `Assets/_Game/Runtime/Combat/EnemyCombat.cs`
- Modify: `Assets/_Game/Runtime/Enemy/EnemyStateMachine.cs`

**开发：** 补齐 Chase → Attack → Hit → Dead；Enemy 攻击也通过 IDamageable 伤害 Player；Dead 停止 Agent、攻击和重复迁移。

**测试：** 攻击距离边界、攻击冷却、受击中断、死亡瞬间命中、Player 死亡后停止攻击。

**交付：** Player 与单个 Puglin 可以互相攻击、受击和死亡。

### Day 16｜9/19：三个 Enemy 与 Phase A 验收

**学习：** 多实例共享配置、拥挤行为、回归测试与 Bug 分级。

**文件：**
- Modify: `Assets/_Game/Scenes/SampleScene.unity`
- Modify: `Docs/TEST_REPORT/`
- Modify: `Docs/BUG_REPORTS.md`
- Modify: `Docs/PROJECT_STATUS.md`

**开发：** 放置 3 个 Puglin Prefab 实例；只调整 Agent 半径/避让等必要参数，不增加新状态。

**测试：** 三敌追击、拥挤、同时受击、逐个死亡、Player 死亡后全部停止；回归 Health、血条、Combo 和命中去重。

**交付：** Phase A 战斗闭环；严重 Bug 为 0；累计测试用例目标 28+。

## Phase B：技能与游戏闭环（Day 17–22）

### Day 17｜9/20：SkillDefinition 与冷却规则

**学习：** ScriptableObject 静态配置、运行时冷却、可测试时间推进。

**文件：**
- Create: `Assets/_Game/Runtime/Skills/SkillDefinition.cs`
- Create: `Assets/_Game/Runtime/Skills/SkillController.cs`
- Create: `Assets/_Game/Tests/EditMode/SkillCooldownTests.cs`
- Modify: `Assets/_Game/Runtime/Input/PlayerInputReader.cs`

**开发：** 接入 E 意图；实现准入、开始冷却、剩余时间和重置，不实现位移/VFX。

**测试：** 首次可释放、冷却中拒绝、结束后恢复、重开清零；至少 2 条 EditMode 测试。

**交付：** 可解释、可测试的技能冷却核心。

### Day 18｜9/21：Sword_Dash 受控位移

**学习：** CharacterController 受控位移、时间归一化、碰撞阻挡。

**文件：**
- Modify: `Assets/_Game/Runtime/Player/PlayerMotor.cs`
- Modify: `Assets/_Game/Runtime/Skills/SkillController.cs`
- Modify: `Assets/_Game/Animations/Player/PlayerAnimator.controller`

**开发：** E 播放 Sword_Dash；SkillController 请求 PlayerMotor 执行 Dash，禁止直接修改 Transform。

**测试：** 距离、持续时间、正面/斜向/贴墙、斜坡、平台边缘、Dash 中重复输入。

**交付：** 不穿墙、可调参数的火焰突进位移骨架。

### Day 19｜9/22：Dash 伤害与多目标去重

**学习：** 复用命中规则、单次释放生命周期、多目标集合。

**文件：**
- Create: `Assets/_Game/Runtime/Skills/SkillHitDetector.cs`
- Create: `Assets/_Game/Tests/PlayMode/SkillHitDetectorPlayModeTests.cs`
- Modify: `Assets/_Game/Runtime/Skills/SkillController.cs`

**开发：** Dash 开始时清空命中集合；同一 Enemy 多 Collider 只伤害一次；复用 IDamageable，不复用 Player Melee 的运行时集合。

**测试：** 单目标、多 Collider、多目标、冷却中输入、技能中目标死亡；至少 1 条 PlayMode 去重测试。

**交付：** Dash 位移和伤害形成完整功能。

### Day 20｜9/23：火焰 VFX、对象池与冷却 UI

**学习：** ParticleSystem、TrailRenderer、`ObjectPool<T>`、复用对象重置。

**文件：**
- Create: `Assets/_Game/Runtime/Skills/SkillVfxPool.cs`
- Create: `Assets/_Game/Prefabs/VFX/FireDashVFX.prefab`
- Create: `Assets/_Game/Runtime/UI/CooldownPresenter.cs`
- Modify: `Assets/_Game/Runtime/Skills/SkillController.cs`
- Modify: `Assets/_Game/Scenes/SampleScene.unity`

**开发：** 自制一个粒子效果和一个 Trail；预热少量实例；Get/Release 时重置粒子、Trail、Transform 和计时；在现有 Canvas 增加最小 E 冷却显示，UI 只呈现状态，不决定技能能否释放。

**测试：** 连续释放、重开、池容量边界、Trail 残影、永久对象、冷却开始/结束、重复 E、Console 和 Hierarchy 数量。

**交付：** E 动画、位移、伤害、VFX、CD 与 UI 全部闭环。

### Day 21｜9/24：Victory/GameOver/Restart

**学习：** 游戏流程状态、同帧死亡优先级、事件退订、场景重载。

**文件：**
- Create: `Assets/_Game/Runtime/GameFlow/GameFlowController.cs`
- Modify: `Assets/_Game/Runtime/Input/PlayerInputReader.cs`
- Modify: `Assets/_Game/Scenes/SampleScene.unity`
- Create: `Assets/_Game/Tests/PlayMode/GameFlowPlayModeTests.cs`

**开发：** Playing/Victory/GameOver；Player 死亡优先 GameOver；流程结束冻结输入、Combat、Skill 和 AI；R 重载场景。

**测试：** 最后敌人与 Player 同帧死亡、结果后继续输入、连续 Restart、重复订阅和状态残留。

**交付：** 开始 → 战斗 → 胜/负 → 重开的完整循环。

### Day 22｜9/25：完整闭环压力回归

**学习：** 集成测试、失败隔离、回归矩阵。

**文件：**
- Modify: `Docs/TEST_REPORT/`
- Modify: `Docs/BUG_REPORTS.md`
- Modify: `Docs/PROJECT_STATUS.md`

**开发：** 不增加功能，只修复完整流程中的阻断/严重问题。

**测试：** 连续完成 5 次完整游戏循环；3 Enemy 压力、连续技能、胜负后输入、连续重开。

**交付：** Week 3 功能冻结；累计测试用例目标 43+，稳定自动化测试目标 6–10。

## Phase C：质量与投递（Day 23–27）

### Day 23｜9/26：庭院美术、灯光与最终场景回归

**学习：** 场景构图、引导视线、基础 URP 灯光、静态标记以及美术表现与碰撞层分离。

**文件：**
- Modify: `Assets/_Game/Scenes/SampleScene.unity`
- Modify: only environment Prefabs/materials actually used by the scene

**开发：** 在 Day 12 主体上完成第二遍场景制作：调整入口、战斗区和楼梯平台构图，补充必要木箱/马车/栅栏，统一材质与基础灯光；删除可见测试方块，隐藏碰撞体可保留；不增加玩法或复杂 Shader。

**测试：** 全战斗动线、镜头遮挡、敌人路径、NavMesh、碰撞、阴影和可读性；确认装饰不阻断移动、攻击或 Dash。

**交付：** 可录制视频、可构建且不影响玩法测试的最终庭院场景。

### Day 24｜9/27：代码审查、Profiler 与 Windows Build

**学习：** 生命周期、空引用保护、Profiler/GC、依赖方向审查。

**文件：**
- Modify: only files with reproduced defects
- Modify: `Docs/PROJECT_ARCHITECTURE.md`
- Modify: `Docs/BUG_REPORTS.md`
- Create: `Docs/TEST_REPORT/BUILD_TEST_REPORT.md`

**开发：** 检查序列化引用、事件订阅、死亡/重开清理、Profiler 与 GC；删除临时 Debug 和未使用资源；只修复已复现的 P0/P1；生成 Windows x64 Release Build，Build 输出不进入源码 Git。

**测试：** 故意清空关键引用；Profiler 预热后检查 Idle/移动/普攻/Dash；Build 中验证分辨率、失焦恢复、全部输入、胜负和连续重开。

**交付：** P0/P1=0、核心玩法无每帧 GC、架构与代码一致，并产出可独立运行的 Windows Build。

### Day 25｜9/28：README、架构图与测试材料

**学习：** 技术项目表达、复现说明、许可合规、测试证据组织。

**文件：**
- Create/Modify: `README.md`
- Modify: `Docs/PROJECT_ARCHITECTURE.md`
- Modify: `Docs/ASSET_AUDIT.md`
- Modify: `Docs/TEST_REPORT/`
- Modify: `Docs/BUG_REPORTS.md`

**开发：** 停止功能开发；整理控制、架构、安装、素材来源、测试统计、已知问题和复现步骤。

**测试：** 按 README 在干净目录检查克隆/素材放置/打开流程；公开 Git 不含 Bestiary 原始 FBX/PNG。

**交付：** 面试官无需聊天上下文即可理解并运行项目。

### Day 26｜9/29：演示视频、简历描述与讲解

**学习：** 作品展示取舍、STAR 项目描述、架构口述。

**文件：**
- Create: `Docs/PORTFOLIO_NOTES.md`
- Create: video source/output outside Git unless size合适

**开发：** 录制 90–120 秒视频：移动 → Combo → Enemy AI → 血条 → Dash → 胜负/重开；整理 3–4 条简历描述。

**测试：** 视频画面/声音/分辨率；逐类讲解 InputReader、Motor、Combat、Health、AI、Skill、GameFlow。

**交付：** 视频、简历描述、源码讲解提纲和模拟面试问题。

### Day 27｜9/30：最终回归、Release 与投递

**学习：** Release Candidate 冻结、风险接受、最终交付核对。

**文件：**
- Modify: `Docs/PROJECT_STATUS.md`
- Modify: `README.md` only for blocking corrections

**开发：** 禁止新增功能；只修复阻断投递的 P0/P1。

**测试：** Windows Build 连续完成 3 次完整流程；复核所有关键 Bug Regression、仓库许可、链接和下载。

**交付：** 创建 `v1.0.0` 标签，冻结源码和 Build，提交简历与作品材料。

## 最低保留范围

进度落后时依次删除屋顶/门窗装饰、颜色变体、第三段 Combo 和额外视频镜头。Day 12 的庭院主体与必要碰撞不可删除；Day 23 的美术润色可缩减为灯光、构图和少量 Props。不得删除基础攻击、Enemy AI、Health/血条、火焰技能闭环、Victory/GameOver/Restart、测试文档和可运行 Build。
