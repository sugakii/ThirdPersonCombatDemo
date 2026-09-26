# Five-Day Demo Completion Sprint Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use executing-plans to implement this plan task-by-task.

**Goal:** 从 Learning Day 18 起连续五个学习日完成原 Day 18–27 的核心内容，交付可玩、可测试、可构建、可展示的第三人称战斗 Demo。对外始终使用 Learning Day 18–22 编号。

**Architecture:** 保持现有 InputReader → Gameplay Controller → PlayerMotor/Combat/Health 的依赖方向。前三天完成技能、游戏流程、菜单和场景冻结；后两天只做质量、Build、文档、视频与 Release，不再扩展玩法。

**Tech Stack:** Unity 6000.5.6f1、URP、Input System、CharacterController、Cinemachine、AI Navigation、Unity Test Framework、Git。

---

## 执行约束

- 每天投入约 6–8 小时，分成两个 3 小时开发块和一个 1–2 小时测试/文档块。
- 每完成一个独立切片立即回归并提交，禁止把五天改动堆到最后一次提交。
- 第三天结束后功能冻结；第四、五天只修复已经复现的阻断/严重问题。
- 教学继续遵守 `Docs/PROJECT_STATUS.md` 的 Update Rules：先提供职责、API 和验收标准，由学习者尝试实现。
- 不新增 Lock-On、Dodge、跳跃、任务、存档、Boss、复杂 Shader 或 Addressables。

## Learning Day 18｜完成：Dash 位移 + 伤害（冲刺第 1 天）

### 上午：Sword_Dash 受控位移

**Files**

- Modify: `Assets/_Game/Runtime/Player/PlayerMotor.cs`
- Modify: `Assets/_Game/Runtime/Skills/SkillController.cs`
- Modify: `Assets/_Game/Data/Skills/FireDash.asset`
- Modify: `Assets/_Game/Animations/Player/PlayerAnimator.controller`

**开发**

- E 成功释放后播放 `Sword_Dash`。
- `SkillController` 只申请 Dash；所有位移仍由 `PlayerMotor` 和 `CharacterController.Move` 执行。
- 使用配置中的距离和持续时间计算速度，不直接修改 Transform。
- Dash 期间拒绝重复技能输入；结束时清理 Dash 状态。
- 同时处理与 Dash 强相关的 `BUG-004` 空中水平控制问题，至少避免离地后保持完整地面操控。

**测试**

- 3m/5m 距离、不同帧率下的最终距离与持续时间。
- 正面、斜向、贴墙、墙角、30°/50°斜坡和平台边缘。
- Dash 中重复按 E、冷却中按 E、Dash 结束恢复普通移动。

### 下午：Dash 伤害与去重

**Files**

- Create: `Assets/_Game/Runtime/Skills/SkillHitDetector.cs`
- Modify: `Assets/_Game/Runtime/Skills/SkillController.cs`
- Create: `Assets/_Game/Tests/PlayMode/SkillHitDetectorPlayModeTests.cs`

**开发**

- 每次 Dash 开始时清空本次命中集合。
- 使用 `IDamageable.TakeDamage(DamageInfo)`；不依赖具体 Enemy 类型。
- 同一 Enemy 的多个 Collider 在一次 Dash 中只结算一次；不同 Enemy 可以各结算一次。

**当日交付门槛**

- E 能完成动画、受控位移和伤害。
- 不穿墙，不重复伤害，冷却期间不能释放。
- 至少完成 8 条手工用例和 1 条去重自动化测试；Console 0 Error。

## Learning Day 19：VFX/CD UI + GameFlow 核心（冲刺第 2 天）

### 上午：火焰表现与冷却 UI

**Files**

- Create: `Assets/_Game/Prefabs/VFX/FireDashVFX.prefab`
- Create: `Assets/_Game/Runtime/Skills/SkillVfxPool.cs`
- Create: `Assets/_Game/Runtime/UI/CooldownPresenter.cs`
- Modify: `Assets/_Game/Runtime/Skills/SkillController.cs`
- Modify: `Assets/_Game/Scenes/SampleScene.unity`

**开发**

- 使用一个 ParticleSystem + TrailRenderer 自制火焰 Dash 表现。
- 使用 Unity `ObjectPool<T>`；Get/Release 时重置 Transform、粒子、Trail 和计时。
- 冷却 UI 只读取 SkillController 状态，不决定能否释放。

**测试**

- 连续释放、冷却开始/结束、Trail 残影、池中对象数量、重开后状态。
- UI 显示与真实 RemainingCoolDown 一致。

### 下午：胜负与重开核心

**Files**

- Create: `Assets/_Game/Runtime/GameFlow/GameFlowController.cs`
- Modify: `Assets/_Game/Runtime/Input/PlayerInputReader.cs`
- Modify: `Assets/InputSystem_Actions.inputactions`
- Modify: `Assets/_Game/Scenes/SampleScene.unity`
- Create: `Assets/_Game/Tests/PlayMode/GameFlowPlayModeTests.cs`

**开发**

- 实现 Playing、Victory、GameOver。
- Player 死亡优先 GameOver；3 个 Enemy 全部死亡进入 Victory。
- 结果状态冻结 Player 输入、Combat、Skill 和 Enemy AI。
- Victory/GameOver 面板按钮重新加载当前游戏场景。

**当日交付门槛**

- 技能已有完整反馈和 CD UI。
- 战斗能进入 Victory/GameOver 并可靠重开。
- 至少 1 条 GameFlow 自动化测试；Console 0 Error。

## Learning Day 20：开始/暂停菜单 + 场景润色 + 完整回归（冲刺第 3 天）

### 上午：最小完整菜单

**Files**

- Create: `Assets/_Game/Scenes/MainMenu.unity`
- Create: `Assets/_Game/Runtime/UI/MainMenuController.cs`
- Create: `Assets/_Game/Runtime/UI/PauseMenuController.cs`
- Modify: `Assets/_Game/Scenes/SampleScene.unity`
- Modify: `Assets/InputSystem_Actions.inputactions`

**开发**

- MainMenu 只提供开始游戏和退出游戏。
- Esc 打开/关闭暂停菜单；提供继续、重新开始、返回主菜单。
- 暂停时冻结 Gameplay 时间并显示鼠标；继续时恢复时间和输入状态。
- 不制作设置、存档、关卡选择或复杂转场。

### 下午：庭院第二遍制作与功能冻结

**Files**

- Modify: `Assets/_Game/Scenes/SampleScene.unity`
- Modify: only environment assets actually used by the scene
- Modify: `Docs/TEST_REPORT/`
- Modify: `Docs/BUG_REPORTS.md`

**开发**

- 只处理构图、基础灯光和少量木箱/栅栏/马车；不做屋顶和复杂材质变体。
- 移除可见测试方块，保留必要的隐藏简化 Collider。
- 结束后正式冻结玩法和场景范围。

**完整回归**

- 连续完成 3 次：MainMenu → 战斗 → Victory/GameOver → Restart/MainMenu。
- 回归移动、Combo、三敌 AI、血条、Dash、CD、暂停和重开。
- 检查镜头遮挡、NavMesh、Dash 碰撞、装饰碰撞和 Console。

**当日交付门槛**

- Demo 已经可以从开始界面进入并完整玩到胜负、暂停和重开。
- P0/P1=0，累计正式测试用例不少于 43 条。
- 当天结束后禁止新增功能。

## Learning Day 21：代码审查 + Profiler + Windows Build + 文档（冲刺第 4 天）

### 上午：质量与 Build

**Files**

- Modify: only files with reproduced defects
- Modify: `Docs/PROJECT_ARCHITECTURE.md`
- Modify: `Docs/BUG_REPORTS.md`
- Create: `Docs/TEST_REPORT/BUILD_TEST_REPORT.md`

**工作**

- 检查所有 SerializeField、订阅/退订、死亡/暂停/重开清理和生命周期。
- 删除临时 Debug.Log、未使用 using 和未使用资源。
- Profiler 只检查 Idle、移动、Combo、三敌战斗和 Dash；确认核心循环无持续每帧 GC。
- 生成 Windows x64 Build；Build 文件不提交源码 Git。

### 下午：README 与测试材料

**Files**

- Create/Modify: `README.md`
- Modify: `Docs/PROJECT_ARCHITECTURE.md`
- Modify: `Docs/ASSET_AUDIT.md`
- Modify: `Docs/TEST_REPORT/`
- Modify: `Docs/BUG_REPORTS.md`

**工作**

- README 写清控制方式、安装/素材放置、架构、测试统计、已知问题和素材许可。
- 确认公开仓库不含 Bestiary 原始 FBX/PNG。
- 在独立 Build 中执行分辨率、输入、失焦恢复、暂停、胜负和连续重开。

**当日交付门槛**

- Windows Build 可独立运行，P0/P1=0。
- README 和 Docs 能让不了解聊天历史的人理解、运行和测试项目。

## Learning Day 22：视频 + 简历 + 最终 Release（冲刺第 5 天）

### 上午：作品展示

**Files**

- Create: `Docs/PORTFOLIO_NOTES.md`
- Create: video outside Git unless体积足够小

**工作**

- 录制 90–120 秒：开始界面 → 移动/Combo → 三敌 AI/血条 → Dash/VFX/CD → 暂停 → 胜负/重开。
- 写 3–4 条量化简历描述。
- 准备 InputReader、Motor、Combat、Health、AI、Skill、GameFlow 的源码讲解提纲。

### 下午：Release Candidate

**Files**

- Modify: `Docs/PROJECT_STATUS.md`
- Modify: `README.md` only for blocking corrections

**工作**

- Windows Build 连续完成 3 次完整流程。
- 复核关键 Bug 回归、下载链接、素材许可、Git 状态和 Build 文件。
- 只修复阻断投递的 P0/P1；创建 `v1.0.0` 标签。

**最终交付门槛**

- 源码、Windows Build、README、架构、测试用例、Bug Report、视频、简历描述全部完成。
- Demo 连续 3 次完整流程无阻断问题。

## 进度落后时的删除顺序

1. 减少场景 Props 和灯光微调，不删除庭院主体与必要碰撞。
2. 减少视频额外镜头，只保留一条 90 秒完整流程。
3. 对象池若成为 Day 2 唯一阻断，可暂用单个持久 VFX 实例；作品集说明其单玩家/冷却约束，后续再升级。
4. 减少新增自动化数量，但保留 Health、Cooldown、移动以及至少一条 Skill/GameFlow 自动化证据。

不得删除：Dash 位移/伤害、Enemy AI、Health/血条、Victory/GameOver/Restart、开始/暂停菜单、正式测试文档、Windows Build 和 README。
