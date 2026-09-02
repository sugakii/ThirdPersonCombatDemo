# Unity 3D 求职项目四周实施计划

> **归档说明（2026-09-02）**：这是最初的实施计划快照，不再作为当前状态来源。实际扫描后已确认 `ProjectVersion.txt` 存在、第一周从 9/2 重新排期。当前事实以 [`../PROJECT_STATUS.md`](../PROJECT_STATUS.md) 为准，当前时间表以 [`../ROADMAP.md`](../ROADMAP.md) 为准，素材结论以 [`../ASSET_AUDIT.md`](../ASSET_AUDIT.md) 为准。

**Goal:** 在 2026 年 9 月 30 日前完成一个可玩、可测试、可解释的第三人称战斗 Vertical Slice，用于鹰角游戏测试实习投递，同时展示 Unity 客户端工程能力。

**Architecture:** 项目采用职责分离的组件式结构：输入、移动、战斗、生命、AI、技能、UI 和游戏流程分别维护，通过接口和局部事件协作。游戏逻辑不依赖具体输入设备、角色类型或 UI 实现，并使用 ScriptableObject 保存静态配置。

**Tech Stack:** Unity 6000.5.6f1、URP、C#、Input System、CharacterController、Cinemachine、AI Navigation、Animator、Unity Test Framework、ParticleSystem、TrailRenderer、ObjectPool、Git。

---

## 一、项目目标

最终成品固定为：

- Player：`Imp.fbx`，默认红色 `T_Imp_BaseColor_1`。
- Enemy：`Puglin.fbx`，场景中放置 3 个实例。
- 场景：约 20×20 米中世纪庭院。
- 操作：WASD 移动、鼠标镜头、Shift 冲刺、左键普攻、Q 火焰突进技能、R 重开。
- 系统：第三人称移动、三段普攻、敌人 AI、伤害与生命、血条、技能冷却与 VFX、胜负与重开。
- 交付：Windows Build、README、架构说明、测试用例、Bug Report、演示视频、简历项目描述。

每天投入 3–4 小时：约 70% 开发学习、30% 测试与复盘。每周最后一天停止增加功能，只进行验收、回归和重构。

## 二、素材审计后的使用方案

本地六包共 1,213 个文件，但大量内容是 FBX、glTF 和 OBJ 的重复格式。正式工程只导入以下资源。

### 角色与纹理

- Player：`Imp.fbx`。
- Enemy：`Puglin.fbx`。
- 对应的 BaseColor、Normal 和 Emissive 纹理。
- Player 默认使用 `T_Imp_BaseColor_1`。
- Enemy 默认使用 `T_Puglin_BaseColor_1`。

### 动画

只导入非 Root Motion 文件：

- `UAL1_Standard.fbx`
- `UAL2_Standard.fbx`

Player 使用：

- `Idle_Loop`
- `Walk_Loop`
- `Jog_Fwd_Loop`
- `Sprint_Loop`
- `Sword_Regular_A_Rec`
- `Sword_Regular_B_Rec`
- `Sword_Regular_C`
- `Hit_Chest`
- `Death01`
- `Sword_Dash`

Enemy 使用：

- `Idle_Loop`
- `Jog_Fwd_Loop`
- `Sword_Attack`
- `Hit_Knockback`
- `Death01`

所有位移由代码控制：

```csharp
Animator.applyRootMotion = false;
```

不导入 `_RM`、glTF、GLB、OBJ 和未使用模型，避免重复资源和同名动画干扰。

### 场景资源

从 Medieval Village MegaKit 中只选择约 12 个 FBX：

- `Floor_Brick.fbx`
- `Wall_Plaster_Straight.fbx`
- `Corner_Exterior_Wood.fbx`
- `Wall_Plaster_Door_Round.fbx`
- `DoorFrame_Round_WoodDark.fbx`
- `Door_1_Round.fbx`
- `Stairs_Exterior_Straight.fbx`
- `Stairs_Exterior_Platform.fbx`
- `Prop_Crate.fbx`
- `Prop_Wagon.fbx`
- `Prop_WoodenFence_Single.fbx`
- `Prop_WoodenFence_Extension1.fbx`

场景模型需要自行建立共享 URP Material、Prefab 和 Collider。楼梯使用简化斜坡碰撞体，避免 CharacterController 卡住台阶。

### 素材边界

- 当前动画库没有侧移、后退或 Strafe 动画，本月不实现 Lock-On 八方向移动。
- 当前没有独立 VFX 包，火焰突进使用 Unity `ParticleSystem + TrailRenderer` 制作。
- Bestiary 使用 QAL v1.0，允许在游戏中使用，但禁止将原始素材作为资产重新分发。

## 三、分周安排

| 阶段 | 学习与实现重点 | 每周交付和验收 |
|---|---|---|
| **第一周：9/1–9/7** | 工程基础、素材导入、Input System、CharacterController、Cinemachine、镜头空间移动、重力、转向、冲刺、Humanoid 重定向、Idle/Walk/Jog/Sprint Blend Tree；搭建 20×20m 庭院及基础碰撞 | Imp 能稳定移动和冲刺；镜头可旋转且不干扰移动方向；动画无明显滑步或变形；楼梯、墙角、斜向移动通过测试；形成第一批移动/镜头测试用例 |
| **第二周：9/8–9/14** | `DamageInfo`、`IDamageable`、通用 `Health`、事件驱动血条；三段普攻、输入缓存、动画伤害窗口、单次攻击去重；Puglin NavMesh AI 与 Idle/Chase/Attack/Hit/Dead 状态机 | Player 与 Enemy 能互相攻击、受击、死亡；一次攻击对同一目标只结算一次；三段攻击可正常衔接和中断；Player/Enemy 血条正确；Health 自动化测试通过 |
| **第三周：9/15–9/21** | `SkillDefinition`、`SkillController`、冷却系统；`Sword_Dash` 火焰突进、代码位移、碰撞阻挡、多目标去重；ParticleSystem、TrailRenderer、对象池；Victory/Game Over/Restart | Q 技能有动画、位移、伤害、VFX 和 CD；突进不会穿墙或重复伤害；3 个 Puglin 能共同作战；游戏具备开始→战斗→胜利/失败→重开的完整闭环 |
| **第四周：9/22–9/28** | 停止扩功能；代码审查、依赖整理、空引用和生命周期检查；完整功能、边界、异常、状态迁移测试；Profiler、GC、共享材质、静态批处理；Build 和作品集整理 | 40–60 条正式测试用例、至少 5 份规范 Bug Report、6–10 个 EditMode/PlayMode 测试；严重 Bug 清零；核心玩法无每帧 GC；完成 Windows Build、README、架构图和 90–120 秒视频 |
| **交付：9/29–9/30** | 最终回归、简历描述、源码讲解、游戏测试模拟面试 | 连续完成 3 次完整流程无阻断问题；整理投递材料，不再增加功能 |

## 四、第一周特别预检

当前工程是 Unity `6000.5.6f1` URP 空模板，已安装 Input System、AI Navigation 和 Test Framework，但首次创建时发生过 Package Manager 网络中断，且此前未生成 `ProjectVersion.txt`。

第一周开始时依次完成：

1. 确认工程可以完成包解析并正常编译。
2. 若 `com.unity.ai.assistant`、`com.unity.pipeline` 持续阻断启动，移除这两个非核心包。
3. 添加与当前 Unity 版本兼容的 Cinemachine。
4. 保存工程并确认 `ProjectVersion.txt` 正常生成。
5. 初始化 Git，并建立 Unity `.gitignore`。
6. 建立 `_Game/Runtime`、`Data`、`Prefabs`、`Scenes`、`Tests`、`Docs` 与 `ThirdParty` 结构。

建议工程目录：

```text
Assets/
├─ _Game/
│  ├─ Runtime/
│  │  ├─ Core/
│  │  ├─ Input/
│  │  ├─ Character/
│  │  ├─ Combat/
│  │  ├─ AI/
│  │  ├─ Skills/
│  │  ├─ UI/
│  │  ├─ VFX/
│  │  └─ GameFlow/
│  ├─ Data/
│  ├─ Animations/
│  ├─ Materials/
│  ├─ Prefabs/
│  ├─ Scenes/
│  ├─ Tests/
│  │  ├─ EditMode/
│  │  └─ PlayMode/
│  └─ Docs/
└─ ThirdParty/
   └─ Quaternius/
```

## 五、核心工程接口与依赖

### 输入和移动

- `PlayerInputReader`：隔离 Input System，只输出 Move、Look、Sprint、Attack、Skill 和 Restart 意图。
- `PlayerMotor`：唯一负责 CharacterController 移动、重力和转向，不直接读取键盘或鼠标。
- 第三人称移动始终朝当前移动方向旋转，不实现锁敌状态下的 Strafe。

### 战斗和生命

- `DamageInfo`：携带伤害值、攻击来源、命中位置和命中方向。
- `IDamageable.TakeDamage(DamageInfo)`：攻击系统不依赖具体 Enemy 类。
- `Health`：只负责生命值钳制、`HealthChanged` 和只触发一次的 `Died` 事件，不引用 UI。
- `PlayerCombat`：管理 Combo 状态、输入缓存和攻击切换。
- `MeleeHitbox`：管理动画伤害窗口，并确保一次攻击对同一目标只结算一次。

### 配置和状态

- `AttackDefinition`：用 ScriptableObject 保存攻击伤害、硬直等静态数据。
- `SkillDefinition`：保存技能伤害、冷却、突进距离、持续时间和 VFX Prefab。
- ScriptableObject 只保存配置，不保存运行时剩余冷却、当前 Combo 等可变状态。
- `EnemyStateMachine`：明确实现 Idle、Chase、Attack、Hit 和 Dead，避免无限堆叠布尔变量。

### UI、VFX 和流程

- `HealthBarPresenter`：监听 Health 事件并更新血条。
- `CooldownPresenter`：读取技能冷却状态并更新 UI。
- `GameFlowController`：维护 Playing、Victory 和 GameOver，负责停止战斗与重开。
- VFX 使用 Unity 自带 `ObjectPool<T>`，不重复进行高频 Instantiate/Destroy。
- 不自研全局事件总线、不使用全局 Singleton、不引入第三方战斗框架。

## 六、测试流程与重点场景

每个功能都按以下流程执行：

```text
需求
↓
验收标准
↓
实现
↓
功能 / 边界 / 异常测试
↓
Bug 记录
↓
修复
↓
Regression Test
```

### 移动与相机

- 斜向输入是否比单方向更快。
- 墙角、木箱和马车旁是否卡死。
- 上下楼梯是否抖动或悬空。
- 平台边缘是否出现异常悬浮。
- 相机旋转后 WASD 方向是否仍以镜头为基准。
- 靠墙和大型障碍物旁是否穿模。

### 战斗

- 攻击范围边界内外是否正确命中。
- 疯狂连点是否破坏 Combo 顺序。
- 攻击动画被受击打断后状态是否恢复。
- 一次攻击同时命中多个目标时是否正确结算。
- 同一攻击窗口内是否对同一目标重复扣血。
- 目标死亡瞬间再次命中是否重复触发死亡。

### Health

- 当前 HP 为 0、1、最大值时的表现。
- 伤害为 0、1、等于当前 HP、远大于当前 HP 时的结果。
- HP 是否被正确钳制在 0 到 MaxHealth。
- `Died` 是否只触发一次。
- Restart 后生命值和死亡状态是否完全初始化。

### Enemy AI

- 仇恨范围和攻击范围的边界值。
- NavMesh 边缘、楼梯和障碍物绕行。
- 多个 Enemy 同时追击时是否拥挤或互相阻挡。
- Player 死亡后 AI 是否继续移动或攻击。
- Enemy 受击、死亡时 NavMeshAgent 是否正确停止。

### 技能

- 冷却期间反复输入是否重复释放。
- 突进撞墙是否穿透。
- 同时命中多个目标是否正确结算。
- 同一目标是否在单次突进中重复受伤。
- 技能执行过程中 Player 死亡时是否正确中断。

### 游戏流程

- 最后一个 Enemy 与 Player 同时死亡时的结算优先级。
- Victory 后是否仍能移动、攻击或释放技能。
- Game Over 后 Enemy AI 是否停止。
- 连续 Restart 是否产生重复事件订阅或错误状态。

## 七、最终验收标准

- Console 无编译错误和持续异常。
- Windows Build 可以脱离 Unity Editor 独立运行。
- 核心代码能够逐类解释职责、依赖方向和扩展方式。
- 严重和阻断 Bug 为零，其余问题均有清晰记录。
- 完成 40–60 条正式测试用例。
- 完成至少 5 份规范 Bug Report。
- 完成 6–10 个 EditMode/PlayMode 自动化测试。
- 核心玩法无每帧 GC Alloc。
- 连续完成 3 次完整游戏循环，无阻断问题。
- README 包含控制方式、架构、测试成果、素材来源和复现步骤。

## 八、范围控制与默认决策

- 本月不做跳跃、Lock-On、Dodge、背包、任务、存档、联网、Boss、复杂 Shader 或 Addressables。
- 如果进度落后，依次删除屋顶/门窗装饰、换色和第三段 Combo。
- 不删除移动、基础攻击、AI、Health/UI、技能 VFX、测试文档和完整游戏循环。
- Bestiary 原始 FBX 和 PNG 不提交到公开仓库；公开代码、配置、素材下载与放置说明。
- 完成的 Build、截图和视频可作为作品展示。
- 其他五包虽然是 CC0，也只导入实际使用的资源，避免让仓库成为素材备份。

## 九、最终交付物清单

- Windows 可执行 Build。
- 90–120 秒 Demo 视频。
- Git 仓库与规范提交记录。
- 项目 README。
- 系统架构图和核心数据流说明。
- 测试需求分析与测试点清单。
- 40–60 条测试用例及执行结果。
- 至少 5 份 Bug Report 与回归记录。
- 自动化测试代码与结果。
- 简历项目描述。
- 项目源码讲解与游戏测试模拟面试题。
