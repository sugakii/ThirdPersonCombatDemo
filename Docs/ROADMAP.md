# Unity 3D 求职项目 Roadmap

> 最后复核：2026-09-07（Learning Day 4 验收；实现完成，运行用例待执行）
> 项目周期：2026-09-02 至 2026-09-30  
> 学习日编号：2026-09-02 = Learning Day 1；之后按自然日连续递增，至 2026-09-30 = Learning Day 29
> 每日投入：3–4 小时  
> 固定节奏：约 70% 开发学习，30% 测试、复盘、文档和 Git  
> 项目目标：在 2026-09-30 前完成可玩、可测试、可解释的第三人称战斗 Vertical Slice

## 1. 最终交付范围

- Player：红色 `Imp`。
- Enemy：3 个 `Puglin` 实例。
- 场景：约 20×20m 中世纪庭院。
- 操作：WASD、鼠标镜头、Shift 冲刺、左键普攻、Q 火焰突进、R 重开。
- 系统：第三人称移动、三段普攻、Enemy AI、伤害与生命、血条、技能冷却/VFX、胜负与重开。
- 求职材料：Windows Build、README、架构说明、40–60 条测试用例、至少 5 份真实 Bug Report、6–10 个自动化测试、90–120 秒视频和简历描述。

不做：跳跃、Lock-On、Dodge、背包、任务、存档、联网、Boss、复杂 Shader、Addressables。

## 2. 当前工程基线

Day 4 实际复核：PlayerInputReader 已输出 SprintHeld；PlayerMotor 已实现镜头空间移动、转向、反向转向和 Sprint；Main Camera 引用及速度参数已保存；Idle/Walk/Jog/Sprint Loop Time 已开启。当前 Console 无编译错误，但 Day 4 键鼠运行用例、动作视觉预览、Blend Tree、Prefab、楼梯和自动化测试仍未完成。详细证据与风险以 PROJECT_STATUS.md 为准。

工程基线已具备 Unity 6000.5.6f1、URP、Input System 1.20.0、Cinemachine 3.1.7、AI Navigation、Test Framework、Git 和三份 asmdef，无需重复安装或重新建立输入层。

## 3. 每日工作制度

每个普通开发日：

1. 15 分钟：读取 `PROJECT_STATUS.md`，确认当天唯一任务与验收条件。
2. 2–2.5 小时：学习并实现一个可独立验证的功能切片。
3. 45–60 分钟：执行自动化/手工边界测试，登记真实 Bug。
4. 15–30 分钟：更新 Docs、检查 Git diff，并在通过相关测试后提交。

固定规则：

- 9/7、9/14、9/21、9/28 是周验收日，不增加功能。
- 第四周全周停止扩功能，只做修复、重构、测试、性能和作品集交付。
- 每个功能执行“需求 → 验收标准 → 失败测试/用例 → 实现 → 边界与异常测试 → Bug → 修复 → 回归”。
- 未通过本周验收门槛，不进入下一周系统。

### 教学代码披露规则

- 当助手判断某段代码在学习者当前能力范围内时，第一轮只说明职责、相关 API 的用途与用法、输入输出、约束和验收标准，不直接给出完整实现。
- 学习者根据这些信息独立写完其余代码并自行运行验证。
- 首次失败后，助手先根据实际代码和报错提供定位提示或最小修改建议；学习者再次尝试。
- 只有在学习者尝试后仍无法完成，或明确请求完整代码时，助手才提供完整实现，并要求学习者能够逐段解释。

## 4. Week 1：工程底座与角色控制（9/2–9/7）

### 开发

- 完成 Package Manager、Console 和脚本编译预检。
- 安装与 Unity 6000.5.6f1 兼容的 Cinemachine。
- 按 `ASSET_AUDIT.md` 只导入：
  - `Imp.fbx` 与红色外观所需纹理。
  - `UAL1_Standard.fbx`；UAL2 延到 Week 2 攻击任务前按需导入验证。
  - 庭院先使用基础体完成碰撞白盒；正式清单中必要的地面/墙/楼梯模型待控制验收后替换，12 件不是本周硬性数量门槛。
- 配置 Imp Humanoid Avatar、Bake Axis Conversion、非 RM 动画重定向和 `applyRootMotion=false`。
- 创建/整理 Player Actions：Move、Look、Sprint、Attack、Skill、Restart。
- 实现 `PlayerInputReader`。
- 实现 `PlayerMotor`、CharacterController 重力、镜头空间移动、转向和冲刺。
- 建立 Idle/Walk/Jog/Sprint 1D Blend Tree。
- 搭建约 20×20m 庭院 Blockout，设置墙、箱子、栅栏和楼梯简化碰撞。

### 工程

- 初始化 Git，创建 Unity `.gitignore`，建立可恢复的文档/空模板基线提交。
- 建立 `_Game/Runtime`、`Data`、`Animations`、`Materials`、`Prefabs`、`Scenes`、`Tests` 和 `ThirdParty` 目录。
- Runtime 与 EditMode/PlayMode Tests 分别建立 asmdef。
- `PlayerInputReader` 隔离 Input System；`PlayerMotor` 不读取具体键盘或鼠标按键。
- CharacterController 是 Player 位移的唯一执行者；Animator 只负责表现。
- 移动参数通过序列化配置暴露；运行时状态不写入 ScriptableObject。
- Player Prefab 固化 CharacterController、Animator、InputReader、Motor 的显式引用。
- `MaterialPackage/` 保持在 Unity Assets 外且不进入 Git；只复制使用中的文件。

### 测试

本周形成至少 10 条正式用例：

- 单方向与斜向速度一致性。
- 相反方向、多键冲突、快速切换和释放。
- Sprint 按下/松开、静止时 Sprint、恢复普通速度。
- 镜头旋转后 WASD 方向和输入强度。
- 镜头俯仰极限、近墙行为和 Camera 引用异常。
- 墙角、箱子、栅栏、楼梯、平台边缘碰撞。
- Animator 参数与实际速度同步。
- Root Motion 没有意外推动角色。

自动化目标：

- PlayMode：斜向输入不会超过配置移动速度。
- PlayMode：Sprint 切换后速度可恢复且位移稳定。

### 交付

- 可独立运行的角色控制 Vertical Slice。
- `Player.prefab`、庭院 Blockout Scene、Player Animator Controller。
- 10 条移动/相机测试用例及执行结果。
- 本周实际发现的 Bug 与回归记录。
- 更新后的 `ASSET_AUDIT.md`（填入 Unity Avatar/材质/动画验证结果）。
- 更新后的唯一当前 `PROJECT_STATUS.md`。

### 验收门槛

- Console 无编译错误或持续异常。
- WASD、鼠标镜头、Shift 冲刺均可用。
- 斜向移动不加速；镜头旋转不改变输入强度。
- Player 可稳定通过楼梯并与墙、箱子正确碰撞。
- Imp Avatar 有效，无明显骨骼变形。
- 动画与代码位移没有叠加。
- 两个自动化测试稳定通过。

未通过以上门槛，不开始战斗系统。

### 每日检查点

- **Learning Day 1｜9/2**：干净启动预检；Git、`.gitignore`、Docs 基线；目录与 asmdef；安装 Cinemachine。
- **Learning Day 2｜9/3（部分完成）**：Imp/UAL1 导入、Avatar、输入读取和镜头原型；欠动画播放验证、材质持久化、庭院 Blockout。
- **Learning Day 3｜9/4（功能阶段符合预期，持久化通过）**：Idle、独立材质、最小世界坐标 Motor 已实现；白盒与胶囊在 Play 内确认，用户已回归基本输入/碰撞。磁盘场景已同步；保存后运行回归待确认，暂不打最终通过标签。
- **Learning Day 4｜原计划 9/5，9/7 验收**：镜头空间移动、转向、反向转向、Sprint 和选定 locomotion Loop Time 已落地；静态配置与编译检查通过。运行输入测试和动作预览未执行，因此结论为“实现完成，运行验收待完成”。发现 `A_TPose` 被误开循环，登记为 `BUG-002`。
- **Learning Day 5｜下一学习日**：先用 45–60 分钟执行 Day 4 运行用例并修复 `BUG-002`；通过后实现 Idle/Walk/Jog/Sprint Blend Tree、Animator 速度参数和 Player Prefab；将 CameraController 移到 Runtime/Camera（保留 `.meta`）。时间不足时把依赖保护留到 Day 6，不压缩测试。
- **Learning Day 6｜Day 5 后一学习日**：停止新增玩法；补楼梯斜坡与最小障碍，执行至少 10 条移动/镜头/动画用例；完成 2 个最小 PlayMode 自动化测试，修复并回归。未满足 Week 1 门槛，不开始 Combat。

本次重排原则：学习日编号继续按任务顺序推进，不用日历日期冒充完成进度。保留 70% 开发学习、30% 测试，优先可玩白盒；环境美术替换与 UAL2 最终验证移到 Week 2。仅保留必要地面、墙和楼梯，纯装饰延后。
由于 9/7 尚未通过 Week 1 门槛，9/8 不直接开始 Health/Combat；先完成 Learning Day 5 和 Day 6。Week 2–4 日期均视为目标窗口，后续每日验收按实际进度顺延；优先删第三段 Combo 和装饰，不删测试。
若 Package Manager 受网络影响，最多排查 30 分钟；可先用静态 Camera 推进 Input/Motor，Cinemachine 最迟在 Week 1 验收前补齐。`com.unity.ai.assistant` 当前承担 Unity MCP Bridge，使用 MCP 期间保留；只有 `com.unity.pipeline` 再次确认阻断解析时才移除。

## 5. Week 2：生命、近战与 Enemy AI（9/8–9/14）

### 开发

- 测试先行实现 `DamageInfo`、`IDamageable`、通用 `Health`。
- 实现初始化、HP 钳制、`HealthChanged`、只触发一次的 `Died` 和重开恢复。
- 实现 Player 屏幕血条与 Enemy 世界空间血条。
- 创建三个 `AttackDefinition` 静态配置。
- 实现 `PlayerCombat`、三段普攻、输入缓存、攻击中断和动画伤害窗口。
- 实现 `MeleeHitbox` 每次攻击对同一目标的命中去重。
- 导入并配置 `Puglin.fbx`、纹理及 Idle/Run/Attack/Hit/Death 动画。
- 烘焙 NavMesh；实现 Idle、Chase、Attack、Hit、Dead 明确状态机。
- 先用单个 Enemy 验收，再放置 3 个共用 Prefab 的 Puglin 实例。

### 工程

- `Health` 不引用 UI、Player、Enemy 或 Animator。
- UI Presenter 订阅领域事件，不轮询具体角色类。
- Animation Event 只开关攻击窗口或报告动画节点，不查找目标、不直接扣血。
- `MeleeHitbox` 只通过 `IDamageable` 结算，不依赖 Enemy 具体类型。
- 当前 Combo、缓存输入和命中集合保存在 Runtime，不写入 `AttackDefinition`。
- Enemy 状态迁移经统一状态机入口；Dead 状态关闭移动、攻击和重复事件。
- Player 与 Enemy 复用同一个 Health/Damage 契约。

### 测试

新增约 18 条正式用例，使累计达到约 28 条：

- 0、1、最大、超额伤害和 HP 钳制。
- 死亡事件只触发一次；重开后完整恢复。
- 三段 Combo 顺序、疯狂连点、输入缓存时间边界。
- 攻击范围内外、伤害窗口前后。
- 单目标多个 Collider、多个目标同时命中。
- 攻击中受击、死亡瞬间再次命中。
- AI 仇恨/攻击距离边界、NavMesh 边缘、障碍绕行。
- 多 Enemy 拥挤；Player 死亡后 AI 是否停止。

新增自动化测试，使累计约 6 个：

- EditMode：Health 正确钳制。
- EditMode：`Died` 只触发一次。
- EditMode：Health Reset 恢复完整状态。
- PlayMode：一个攻击窗口对同一目标只结算一次。

### 交付

- Player 与 Puglin 可互相攻击、受击、死亡。
- 三段普攻、Player/Enemy 血条、Enemy 状态机。
- 至少 28 条累计测试用例。
- 至少 2 份由真实问题产生的规范 Bug Report。
- Health/Hitbox 自动化测试及执行结果。

### 验收门槛

- 一次攻击对同一对象只扣一次血。
- Combo 不因快速连点跳段、叠段或永久卡死。
- Health 不依赖 UI；死亡事件仅触发一次。
- Enemy 可完成 Idle → Chase → Attack → Hit → Dead。
- Player 死亡后 Enemy 不再攻击。
- 累计自动化测试全部稳定通过。

未通过以上门槛，不开始技能 VFX。

### 每日检查点

- **Learning Day 7｜9/8**：Health 失败测试、DamageInfo、IDamageable、Health。
- **Learning Day 8｜9/9**：Player/Enemy 血条与 Presenter。
- **Learning Day 9｜9/10**：UAL2 导入与攻击动作预检；AttackDefinition、攻击动画与 Combo 状态。
- **Learning Day 10｜9/11**：输入缓存、伤害窗口与命中去重。
- **Learning Day 11｜9/12**：必要庭院地面/墙/楼梯美术替换（保持白盒碰撞）；Puglin Prefab、NavMesh 与 Enemy 状态机。时间不足保留白盒，不阻塞 AI。
- **Learning Day 12｜9/13**：3 Enemy 集成、AI/Combat 边界测试。
- **Learning Day 13｜9/14**：停止扩功能；回归、修复、架构复核、Docs 与 Week 2 标签。

## 6. Week 3：火焰突进与完整游戏循环（9/15–9/21）

### 开发

- 实现 `SkillDefinition`、`SkillController` 和独立 Runtime 冷却状态。
- Q 触发 `Sword_Dash` 火焰突进。
- Skill 向 PlayerMotor 请求受控位移，不直接修改 Transform。
- 实现突进障碍阻挡、目标收集和单次释放命中去重。
- 使用 ParticleSystem、TrailRenderer、Unity `ObjectPool<T>` 完成火焰表现。
- 实现冷却 UI。
- 实现 Playing、Victory、GameOver 三个游戏流程状态。
- R 通过重新加载正式场景实现可靠重开。
- 完成 3 个 Puglin 的共同追击与完整战斗闭环。

### 工程

- PlayerMotor 仍是 CharacterController 所有位移的唯一入口。
- `SkillDefinition` 只保存伤害、冷却、距离、持续时间和表现资源。
- 剩余冷却、已命中目标和释放状态不写回 ScriptableObject。
- VFX 预热并复用；不在每次释放时反复 Instantiate/Destroy。
- GameFlow 订阅 Player/Enemy 死亡事件，并在帧末统一判定结果。
- 同帧 Player 与最后一个 Enemy 都死亡时，固定 GameOver 优先，避免结果依赖事件顺序。
- Victory/GameOver 后冻结移动、攻击、技能和 Enemy AI，只允许 Restart。

### 测试

新增约 15 条正式用例，使累计达到约 43 条：

- 冷却期间反复按 Q。
- 突进起点、终点、距离边界。
- 正面撞墙、斜向撞墙、贴墙释放。
- 同时命中多个 Enemy。
- 同一 Enemy 多 Collider 和重复接触。
- 技能过程中 Player 死亡或流程结束。
- Victory/GameOver 后继续输入。
- 最后一个 Enemy 与 Player 同帧死亡。
- 连续 Restart、重复订阅和陈旧运行时状态。

自动化测试累计达到目标 10 个：

- EditMode：冷却期间拒绝重复释放。
- EditMode：冷却结束/重开后恢复可用。
- PlayMode：一次 Dash 对同一目标只伤害一次。
- PlayMode：Restart 后生命、冷却和流程状态恢复。

Dash 撞墙保留为高优先级手工物理测试；若能稳定复现，再选择高价值场景自动化。

### 交付

- Q 火焰突进具有动画、代码位移、伤害、VFX、冷却和 UI。
- 3 个 Puglin 可共同参与战斗。
- 完整开始 → 战斗 → Victory/GameOver → Restart 流程。
- 累计约 43 条测试用例、目标 10 个自动化测试。
- 累计至少 4 份真实 Bug Report。

### 验收门槛

- Dash 不穿墙，不因多 Collider 重复伤害。
- 冷却期间无法重复释放。
- VFX 正确回池，不残留永久对象或 Trail 残影。
- Victory/GameOver 后所有战斗行为停止。
- 连续重开无重复监听、冷却或命中状态残留。
- 目标自动化测试稳定通过。

### 每日检查点

- **Learning Day 14｜9/15**：SkillDefinition、冷却状态与失败测试。
- **Learning Day 15｜9/16**：由 PlayerMotor 执行 Dash 位移与障碍处理。
- **Learning Day 16｜9/17**：技能伤害、多目标与单次释放去重。
- **Learning Day 17｜9/18**：ParticleSystem、TrailRenderer、对象池、冷却 UI。
- **Learning Day 18｜9/19**：GameFlow、胜负、重开、同帧死亡规则。
- **Learning Day 19｜9/20**：3 Enemy 压力测试与完整闭环。
- **Learning Day 20｜9/21**：停止扩功能；回归、修复、Docs 与 Week 3 标签。

## 7. Week 4：质量、作品集与发布（9/22–9/28）

### 开发

本周不增加玩法，只修复已登记问题：

- 审查依赖方向、空引用、事件订阅和对象生命周期。
- 清理无用脚本、重复配置、未使用输入和临时调试对象。
- 检查 Animator 状态退出、死亡状态和场景重载。
- 使用 Profiler 检查 Update、物理、动画、VFX 和 GC。
- 统一共享材质；静态环境正确标记 Static。
- 完成 Windows Build、README、架构图、测试材料和演示视频。

### 工程

- Runtime 不反向依赖 Tests、Editor 或具体 UI。
- 不引入全局 Singleton、事件总线、服务定位器或第三方战斗框架。
- 避免核心 Update 中产生集合、闭包、LINQ 或字符串拼接。
- 场景/Prefab 必需引用在进入 Play Mode 前可验证。
- Bestiary 原始 FBX/PNG 不进入公开 Git 历史；README 写明下载和固定放置路径。
- CC0 素材也只保留实际使用文件和许可证，不提交整包。

### 测试

- 最终整理 40–60 条正式用例，目标约 50 条。
- 保留 6–10 个稳定自动化测试，目标 10 个，不为追数量编写脆弱测试。
- 至少完成 5 份来源于真实缺陷的 Bug Report，不伪造 Bug。
- 执行功能、边界、异常、状态迁移和完整回归矩阵。
- P0/P1 清零；未修复低级问题记录影响、规避和延期理由。
- Profiler 预热后，Idle、移动、普攻、技能核心循环无每帧 GC Alloc。
- 在 Windows Build 中测试分辨率、焦点丢失、输入和 Restart。

### 交付

- Windows 独立 Build。
- README、架构说明和核心数据流图。
- 40–60 条测试用例及结果。
- 至少 5 份 Bug Report 和回归记录。
- 6–10 个自动化测试及执行结果。
- 90–120 秒 Demo 视频。
- 简历项目描述、源码讲解提纲和模拟面试题。

### 验收门槛

- Editor 和 Windows Build 无编译错误或持续异常。
- P0/P1 为 0。
- 核心循环无持续 GC Alloc。
- Build 可连续完成完整流程。
- README 包含控制、架构、测试成果、素材许可、安装与复现步骤。
- 视频清晰展示移动、Combat、AI、血条、技能、胜负和重开。

### 每日检查点

- **Learning Day 21｜9/22**：架构、依赖、空引用、生命周期审查。
- **Learning Day 22｜9/23**：完整测试会与 Bug 分级。
- **Learning Day 23｜9/24**：P0/P1 修复、Profiler、资源优化。
- **Learning Day 24｜9/25**：Windows Build 与 Build 内专项测试。
- **Learning Day 25｜9/26**：README、架构图、测试报告、许可说明。
- **Learning Day 26｜9/27**：演示视频、简历描述、源码讲解。
- **Learning Day 27｜9/28**：停止所有新增内容；最终回归、Docs 同步、RC 标签。

## 8. 最终交付（9/29–9/30）

### Learning Day 28｜9/29

- 在 Windows Build 中连续完成 3 次完整游戏循环。
- 复核全部测试用例和已修复 Bug 的 Regression。
- 检查公开 Git 不包含 QAL 原始资产或完整素材包。
- 只允许修复阻断投递的 P0/P1，不再调整表现或范围。
- 生成最终 Release Candidate。

### Learning Day 29｜9/30

- 完成简历项目描述、作品链接、视频链接和代码仓库说明。
- 进行一次 U3D 架构讲解与游戏测试模拟面试。
- 最终同步 `PROJECT_STATUS.md`、README 和测试结果。
- 创建最终 `v1.0.0` 标签并冻结项目。
- 提交实习简历。

最终门槛：

- 连续 3 次完整流程无阻断问题。
- Build、视频、README、代码、架构、测试用例和 Bug Report 均可访问。
- 能解释各核心类职责、依赖方向、命中去重、状态机、冷却、事件生命周期和测试方法。

## 9. 范围砍减顺序

只在周验收未通过或累计落后超过一个完整开发日时执行：

1. 删除屋顶、门窗和纯装饰场景件，只保留地面、墙、楼梯和必要碰撞障碍。
2. 删除颜色变体和额外材质，只保留红色 Imp 与一种 Puglin 外观。
3. 删除第三段 Combo，保留稳定的两段普攻。
4. Enemy 不制作个体差异，统一一个 Puglin Prefab 和配置，但仍保留 3 个实例。
5. 将火焰表现简化为一个 ParticleSystem + 一个 TrailRenderer，但保留技能伤害、位移和冷却。
6. 减少视频镜头和装饰性剪辑，但不删除测试材料、Build 和 README。

任何情况下都不删除：

- 镜头空间移动与冲刺。
- 基础攻击。
- Enemy AI。
- Health 与血条。
- 火焰技能功能闭环。
- Victory、GameOver、Restart。
- 测试文档、Bug Report 和必要自动化测试。

## 10. Git 与 Docs 节奏

### Git

- 默认提交范围包含全部自有代码及对应 .meta、程序集配置、相关场景和文档，不再只给 Docs 提交指令；仍排除受限原始素材、凭据和生成文件。未验收的代码可作为明确标注的 WIP 保存，不等于功能通过。提供指令不等于自动执行 commit/push。
- 9/2 初始化 Git，并创建 Unity `.gitignore`；现有 `ignore.conf` 不能代替。
- 忽略 `Library/`、`Temp/`、`Logs/`、`UserSettings/`、Build 输出、IDE 文件、根目录 `MaterialPackage/` 和 Bestiary 原始模型/纹理。
- 素材导入、代码功能、测试、Docs 分成可解释的小提交。
- 每个功能切片通过相关测试后提交，不把整周工作堆在一个提交中。
- 提交前检查 Git diff、测试结果和 Console。
- 建议提交前缀：`chore:`、`feat:`、`test:`、`fix:`、`docs:`。
- 每周验收后建议标签：`week-01-movement`、`week-02-combat`、`week-03-loop`、`week-04-rc`。
- Git 历史承担过程记录；`PROJECT_STATUS.md` 不写成长日志。

### Docs

每天开始：

1. 读取 `PROJECT_STATUS.md`。
2. 核对 Current Learning Day、Known Bugs 和 Next Task 是否符合工程事实。

每天结束：

1. 用工程事实覆盖更新 `PROJECT_STATUS.md`，只保留最新版。
2. 实际发现新素材属性或导入问题时，更新 `ASSET_AUDIT.md`。
3. 接口、依赖方向或数据流实际变化时，更新 `PROJECT_ARCHITECTURE.md`。
4. 只有日期、范围、验收门槛或砍减决策变化时，更新 `ROADMAP.md`。
5. Docs 与当天代码、测试一起进入 Git，保证提交后的工程和文档一致。

唯一真实状态固定为：

```text
Unity 工程 + Git + Docs
```

聊天只用于教学、解释、架构 Review 和测试训练，不能代替工程内状态。
