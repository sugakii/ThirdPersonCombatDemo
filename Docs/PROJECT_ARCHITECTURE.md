# Project Architecture

> 最后复核：2026-09-10（Day 8 通过；HealthBarPresenter 与 WorldSpaceBillboard 已核对）
> 文档状态：目标架构基线  
> 重要说明：本文的 **Current Architecture** 来自实际工程扫描；**Target Architecture** 是已批准但尚未实现的设计。Planned 类型、接口和依赖不得当作已完成功能。

## 1. 项目上下文

本项目是用于鹰角游戏测试实习投递的第三人称战斗 Vertical Slice，同时用于展示 Unity 客户端工程能力。范围固定为：

- Player：红色 `Imp`。
- Enemy：3 个 `Puglin` 实例。
- 玩法：镜头空间移动、冲刺、三段普攻、Enemy AI、生命与血条、火焰突进技能、胜负与重开。
- 技术：Unity `6000.5.6f1`、URP、Input System、CharacterController、AI Navigation、Cinemachine 3.1.7、Unity Test Framework。
- 交付：Windows Build、源码、README、架构说明、测试用例、Bug Report 和演示视频。

## 2. 架构状态标记

| 标记 | 含义 |
|---|---|
| Current | 已从当前 Unity 工程、Packages 或 ProjectSettings 中扫描确认 |
| Planned | 计划实现，当前工程中尚不存在 |
| Validate in Unity | 静态文件审计无法确认，必须导入 Unity 后验证 |

## 3. Current Architecture（实际状态）

本节结合当前源码、场景 YAML、Unity 编辑态组件、Console 与用户 Day 4 全量手工测试结果。

```text
PlayerInput (InputSystem_Actions / Player map)
└─ PlayerInputReader [Player，同对象 GetComponent]
   ├─ LookInput → CameraController [Player] → CameraTarget.rotation → Cinemachine
   ├─ MoveInput ──┐
   └─ SprintHeld ─┴→ PlayerMotor [Player] → CharacterController.Move
                          镜头空间限幅移动 + 重力 + 转向 + 速度切换

PlayerMotor.CurrentMoveSpeed
└─ PlayerAnimatorDriver [Player / LateUpdate]
   └─ Animator.Speed → Idle / Walk / Jog / Sprint Blend Tree

Player
├─ CharacterController（编辑态与磁盘 center=(0,1,0)）
├─ Animator（编辑态与磁盘均已禁用）
├─ PlayerMotor（源码存在，编辑态与磁盘均已挂载）
├─ PlayerAnimatorDriver（将实际水平速度写入 Animator Speed）
├─ Imp（启用的 Animator + ImpAvatar + PlayerAnimator；Root Motion=false）
└─ CameraTarget
Ground_Blockout / Wall_Blockout
├─ Stairs_Blockout
├─ Slopes_Blockout（Slope_30 / Slope_50）
├─ Corner_Blockout
└─ Platform_Blockout（均已保存）

DamageInfo（当前只读 DamageAmount）
        ↓
IDamageable.TakeDamage(DamageInfo)
        ↓
Health
├─ MaxHealth / CurrentHealth
├─ HealthChanged(float)
└─ Died()
```

- PlayerMotor 当前负责镜头空间移动、斜向限幅、重力、角色转向和 Sprint 速度切换；`cameraTransform` 指向 Prefab 内的 CameraTarget，普通速度 5、冲刺速度 10、转向速度 720°/s、反向转向速度 1440°/s。
- PlayerAnimator 当前使用 Speed 驱动的 1D Blend Tree：Idle=0、Walk=2.5、Jog=5、Sprint=10。PlayerAnimatorDriver 在 LateUpdate 写入 CharacterController 实际水平速度，撞墙降速测试通过。
- CameraController 已迁移到 `Runtime/Camera` 并保留 `.meta` GUID；Scene 与 Player Prefab 均无 Missing Script。
- Player Prefab 已创建，cameraTransform/cameraTarget 均指向内部 CameraTarget，animator 指向内部 Imp Animator，不依赖场景 Main Camera。
- PlayerInputReader、PlayerMotor、CameraController、PlayerAnimatorDriver 已声明必要 RequireComponent；三个 Inspector 引用缺失时会记录一次明确错误并禁用自身，异常测试通过。
- Imp 的编辑态与磁盘 local rotation=(0,0,0)，不是旧文档的 Y=180；根 Player 由 PlayerMotor 转向，视觉朝向的 Day 4 手工回归通过。
- Game.Runtime 引用 Input System；PlayMode Tests asmdef 已引用 Game.Runtime、Unity Test Runner 与 Unity.InputSystem。
- `PlayerMotorPlayModeTests` 使用虚拟 Keyboard 和真实 `SampleScene`；场景通过 `LoadSceneAsync` 完成初始化。斜向限速与 Sprint 按下/释放/速度恢复断言均有效，连续 5 轮 2/2 PASS。
- `DamageInfo`、`IDamageable` 与 `Health` 已实现；Health EditMode 测试 10/10 PASS。
- `HealthBarPresenter` 已实现并复用于 Player 屏幕空间血条和 Enemy 世界空间血条；它通过显式 Health/Slider 引用监听 `HealthChanged`，启用时主动同步当前状态，不轮询 Player 或 Enemy。
- `WorldSpaceBillboard` 在 LateUpdate 中同步显式 Main Camera 的旋转，使 Enemy 血条保持屏幕对齐；四个镜头方向回归通过。
- 无 PlayerCombat、Enemy AI、Skill、GameFlow 或 NavMesh。
- 下文 Target Architecture 仍是目标契约，不能作为已实现证据。

## 4. 功能需求

| ID | 功能 | 验收摘要 |
|---|---|---|
| FR-01 | 第三人称控制 | WASD 镜头空间移动、鼠标镜头、Shift 冲刺，斜向速度不增益 |
| FR-02 | 普通攻击 | 左键三段攻击、输入缓存、动画伤害窗口、单次攻击命中去重 |
| FR-03 | 通用伤害 | Player 与 Enemy 使用相同 `DamageInfo`、`IDamageable`、`Health` 契约 |
| FR-04 | Enemy AI | Puglin 明确实现 Idle、Chase、Attack、Hit、Dead 状态 |
| FR-05 | UI | Player/Enemy 血条和技能冷却显示由领域事件驱动 |
| FR-06 | 火焰突进 | Q 触发代码位移、障碍阻挡、多目标去重、伤害、VFX 和冷却 |
| FR-07 | 游戏流程 | Playing、Victory、GameOver；R 可可靠重开并清理运行时状态 |

## 5. 非功能需求

| 维度 | 约束 |
|---|---|
| 可维护性 | 类型职责单一；依赖方向明确；不使用全局 Singleton、自研事件总线或服务定位器 |
| 可测试性 | 领域逻辑尽量不依赖场景查找；Health、冷却和去重规则可被 EditMode/PlayMode 测试 |
| 可靠性 | 死亡事件只触发一次；流程结束后战斗停止；重开不残留订阅、冷却或命中集合 |
| 性能 | 目标设备 1080p 下核心玩法以 60 FPS 为目标；预热后核心循环无每帧 GC Alloc |
| 可诊断性 | Console 无持续异常；真实缺陷进入 Bug Report；关键状态可在 Inspector 或调试视图确认 |
| 许可合规 | Bestiary 原始 FBX/PNG 不进入公开仓库；README 记录来源、许可和复现步骤 |

## 6. Target Architecture（Planned）

```text
Input System
    │
    ▼
PlayerInputReader
    ├──────────────► CameraController ────► CameraTarget / Cinemachine
    ├──────────────► PlayerMotor ─────────► CharacterController
    ├──────────────► PlayerCombat ────────► MeleeHitbox
    ├──────────────► SkillController ─────► PlayerMotor / SkillHitDetector
    └──────────────► GameFlowController（仅 Restart 意图）

AttackDefinition / SkillDefinition（静态配置）
    │
    ▼
Combat / Skill Runtime
    │
    ▼
IDamageable.TakeDamage(DamageInfo)
    │
    ▼
Health（运行时生命状态）
    ├──────────────► HealthBarPresenter
    ├──────────────► EnemyStateMachine
    └──────────────► GameFlowController

EnemyStateMachine
    ├──────────────► NavMeshAgent
    ├──────────────► EnemyCombat
    └──────────────► Animator

GameFlowController
    └──────────────► Playing / Victory / GameOver / Restart
```

### 6.1 依赖方向

```text
Presentation（UI / Animator / VFX）
            ↓ 订阅或消费
Application（Player、Enemy、Combat、Skill、GameFlow）
            ↓ 依赖
Domain（DamageInfo、IDamageable、Health 规则、静态定义）
            ↓
Unity Framework（Input System、CharacterController、NavMesh、ObjectPool）
```

约束：Domain 不反向引用 UI；Runtime 不引用 Tests 或 Editor；攻击方不依赖具体 Enemy 类型。

## 7. 核心类型职责（Current / Planned）

`DamageInfo`、`IDamageable`、`Health` 为 Current；其余尚未实现的类型为 Planned。

| 类型 | 唯一职责 | 允许依赖 | 禁止事项 |
|---|---|---|---|
| `PlayerInputReader` | 将 Input System 转换为 Move、Look、Sprint、Attack、Skill、Restart 意图 | Input Actions | 直接移动角色、扣血或控制 UI |
| `CameraController` | 消费 Look 意图并旋转 `CameraTarget`，让 Cinemachine 计算最终机位 | PlayerInputReader、CameraTarget | 直接读取具体输入设备；直接写 CinemachineCamera Transform |
| `PlayerMotor` | CharacterController 位移、重力、面向和所有受控突进位移的唯一入口 | CharacterController、相机朝向、移动配置 | 读取具体键盘按键；直接处理攻击 |
| `DamageInfo` | 当前携带只读伤害值；来源、命中点和方向在 Combat 确实需要时扩展 | 值类型/Unity 基础类型 | 持有目标行为或产生副作用 |
| `IDamageable` | 为命中系统提供统一伤害入口 | `DamageInfo` | 暴露具体 Player/Enemy 实现 |
| `Health` | HP 钳制、初始化、`HealthChanged`、只触发一次的 `Died` | 生命配置 | 引用 UI、Animator、Player 或 Enemy |
| `PlayerCombat` | Combo 状态、输入缓存、攻击切换与中断 | InputReader、Animator、AttackDefinition、MeleeHitbox | 在动画事件中查找目标或直接依赖 Enemy |
| `MeleeHitbox` | 攻击窗口、目标收集、每次攻击命中去重 | `IDamageable`、`DamageInfo` | 保存跨攻击的陈旧命中集合 |
| `AttackDefinition` | 保存伤害、窗口、移动限制等静态攻击配置 | ScriptableObject | 保存当前 Combo、缓存输入或命中目标 |
| `SkillDefinition` | 保存伤害、冷却、距离、持续时间和表现资源 | ScriptableObject | 保存剩余冷却或当前释放状态 |
| `SkillController` | 技能准入、冷却、释放流程和命中去重 | SkillDefinition、PlayerMotor、ObjectPool | 直接写 Transform；修改配置资产 |
| `EnemyStateMachine` | 统一管理 Idle/Chase/Attack/Hit/Dead 迁移 | NavMeshAgent、Animator、EnemyCombat、Health | 用互相冲突的布尔变量替代状态 |
| `HealthBarPresenter`（Current） | 订阅 Health 事件并更新 Player/Enemy 血条 | Health、Unity UI Slider | 轮询具体 Player/Enemy 类；持有生命规则 |
| `WorldSpaceBillboard`（Current） | 让世界空间 UI 与 Gameplay Camera 保持同旋转 | Main Camera Transform | 查找 Player/Enemy；修改 Health 或 Slider |
| `CooldownPresenter` | 订阅技能冷却状态并更新 UI | SkillController、UI | 驱动技能逻辑 |
| `GameFlowController` | 维护 Playing/Victory/GameOver、冻结战斗并重开 | Player/Enemy 死亡事件、场景加载 | 持有攻击或 AI 的业务细节 |

## 8. 运行时规则

### 输入

- Gameplay 代码不使用 `Keyboard.current` 或 `Mouse.current` 读取具体按键。
- `PlayerInputReader` 是唯一 Input System 边界；下游只消费意图和值。
- `CameraController` 属于 Camera 模块，当前位于 `Runtime/Camera`；Input 模块只保留输入读取职责。
- 默认 Input Asset 已有 Move、Look、Attack、Sprint；PlayerInputReader 当前输出 Move、Look、Sprint。Skill(Q) 与 Restart(R) 尚待添加。

### 位移与动画

- CharacterController 是 Player 位移的唯一执行者。
- 所有选用动画来自非 RM 文件；`Animator.applyRootMotion = false`。
- Animator 负责表现，不拥有 Gameplay 位置。
- Dash 通过 PlayerMotor 执行，并使用碰撞检测限制位移，不能直接修改 Transform。

### 配置与状态

- ScriptableObject 只保存静态配置。
- 当前 HP、Combo、输入缓存、冷却剩余时间、当次已命中集合保存在运行时组件中。
- 所有可变运行时集合在攻击结束、技能结束、死亡和重开时有明确清理点。

### 事件与引用

- 使用局部 C# 事件连接 Health、UI 和 GameFlow。
- 订阅与退订必须成对，优先放在 `OnEnable` / `OnDisable` 或明确生命周期方法中。
- Prefab 依赖使用 `[SerializeField] private` 显式注入；Gameplay 热路径不使用 `Find*`。

### VFX

- ParticleSystem 与 TrailRenderer 使用 Unity `ObjectPool<T>`。
- 对象归还池前重置粒子、Trail、Transform 和计时状态。
- 不为这个月的范围自研通用对象池或事件总线。

## 9. 目标目录与程序集（Planned）

```text
Assets/
├─ _Game/
│  ├─ Runtime/
│  │  ├─ Common/
│  │  ├─ Camera/
│  │  ├─ Input/
│  │  ├─ Player/
│  │  ├─ Combat/
│  │  ├─ Enemy/
│  │  ├─ Skills/
│  │  ├─ UI/
│  │  └─ GameFlow/
│  ├─ Data/
│  ├─ Animations/
│  ├─ Materials/
│  ├─ Prefabs/
│  ├─ Scenes/
│  └─ Tests/
│     ├─ EditMode/
│     └─ PlayMode/
└─ ThirdParty/
   └─ Quaternius/

Docs/
├─ ASSET_AUDIT.md
├─ PROJECT_ARCHITECTURE.md
├─ ROADMAP.md
└─ PROJECT_STATUS.md
```

计划程序集：

- `Game.Runtime.asmdef`：所有 Gameplay Runtime 类型。
- `Game.Tests.EditMode.asmdef`：引用 `Game.Runtime`，验证纯规则与配置。
- `Game.Tests.PlayMode.asmdef`：引用 `Game.Runtime`，验证 CharacterController、Hitbox、流程和重开集成。

## 10. 架构决策记录

### ADR-001：CharacterController + In-Place 动画

- **状态**：Accepted / 基础 Motor、镜头空间移动、转向、Sprint 与 locomotion 视觉回归通过。
- **决定**：Player 使用 CharacterController；只导入非 RM 动画；Gameplay 位移由代码控制。
- **原因**：素材缺少完整八方向动画；代码位移更容易测试速度、碰撞、Dash 和异常状态。
- **收益**：单一位移所有者、可预测、便于自动化测试。
- **代价**：需要人工校准动画速度，动作驱动感弱于完整 Root Motion 方案。

### ADR-002：局部 C# 事件，不建全局事件总线

- **状态**：Accepted / Health → HealthBarPresenter 已实现；GameFlow 尚未实现。
- **决定**：Health、UI 和 GameFlow 通过显式引用与局部事件连接。
- **原因**：当前规模小，显式依赖更容易追踪和调试。
- **收益**：调用链清楚，测试替身简单。
- **代价**：Prefab/场景需要手工连接引用和管理订阅生命周期。

### ADR-003：ScriptableObject 只保存静态配置

- **状态**：Accepted / 尚未实现。
- **决定**：AttackDefinition、SkillDefinition 等资产不保存运行时可变状态。
- **原因**：避免多个实例共享状态、停止 Play 后污染资产及重开状态泄漏。
- **收益**：配置可复用，运行时状态归属明确。
- **代价**：需要独立 Runtime state 字段或小型状态对象。

### ADR-004：公开仓库排除 Bestiary 原始资产

- **状态**：Accepted。
- **决定**：公开 Git 不提交 Bestiary 的原始 FBX/PNG；提交代码、配置、许可说明和素材放置步骤。
- **原因**：本地 `License_Standard.txt` 允许用于游戏，但禁止重新分发原始资产。
- **收益**：公开作品集保持许可合规。
- **代价**：他人克隆源码后需要按 README 自行下载和放置素材。

## 11. 风险与失败模式

| 风险 | 触发信号 | 预防/处理 |
|---|---|---|
| Imp/Puglin Avatar 不兼容 | Unity Avatar 显示 Invalid、骨骼扭曲 | 先完成 Humanoid 导入预检；保留导入截图和结论；失败时在进入 Gameplay 前调整映射 |
| 模型朝向错误 | 动画播放时横移或背向 +Z | 开启 Bake Axis Conversion；在 Model Importer 和场景中验证，不在静态审计中猜源轴 |
| Root Motion 重复位移 | 动画和代码同时推动角色 | 只导入非 RM FBX；`applyRootMotion=false`；增加专项测试 |
| NavMesh 不可用 | Agent 不追击或反复报路径错误 | 先烘焙最小地面；验证边缘、楼梯、障碍和重开后的 Agent 状态 |
| 事件重复订阅 | 重开后一次死亡触发多次流程 | 成对订阅/退订；增加连续重开 PlayMode 测试 |
| ScriptableObject 被运行时修改 | 多敌人共享 HP/CD、停止 Play 后值改变 | 代码审查禁止写配置；测试多个实例隔离性 |
| Dash 穿墙或重复伤害 | 高速位移越过墙体、一个目标多次扣血 | 分段受控位移/碰撞检测；每次释放独立 HashSet 去重 |
| 池化 VFX 状态污染 | Trail 残影、粒子永久存在 | Get/Release 时显式重置；连续释放测试 |
| Package 解析复发 | Unity 启动时持续 UPM 错误 | 先验证核心包；Cinemachine 失败时短期用静态相机推进 Input/Motor；只移除确认阻断的非核心包 |
| 输入采样晚一帧 | InputReader 与消费者同在 Update，执行顺序未固定 | 先用运行测试判断是否可感知；只有实际出现问题时再统一采样时机或设置执行顺序 |
| 反向转向与输入改变不同步 | 180° 转向期间快速改变方向时视觉朝向偏离位移 | 用边界用例确认；仅在可复现时修改反向转向状态逻辑 |
| 空中控制沿用地面速度 | Player 离开平台后仍可完整改变水平速度与方向 | Week 1 保留为 BUG-004；实现 Dash 前确定受限空中控制规则并回归 |
| PlayMode 用例标题与断言不一致 | Test Runner 绿灯，但目标行为未真正执行或比较 | 验收时同时审查 Arrange/Act/Assert；Day 6 已补全 Sprint 按下、释放与恢复断言 |
| Health 恢复 API 与 Unity 消息同名 | `Health.Reset()` 可能被误认为或触发为编辑器 `MonoBehaviour.Reset()` | Day 8 首个消费者接入前改名为 `ResetHealth()` 并回归现有 10 条测试 |

## 12. 变更规则

- 只有接口、职责、依赖方向、目录边界或已接受 ADR 发生变化时才更新本文。
- 实现进度只写入 `PROJECT_STATUS.md`，不在本文维护进度日记。
- 新类型第一次落地后，将其状态由 Planned 改为 Implemented，并核对代码路径和测试。
- 架构图必须反映工程真实依赖；聊天中的建议不能直接覆盖本文件。
