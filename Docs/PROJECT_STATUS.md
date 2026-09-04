# Current Project Status

> Last Updated：2026-09-04
> Current Learning Day：Day 3（2026-09-04）
> Current Phase：Week 1 / Day 3 功能与持久化核对通过，保存后运行回归待确认
> 下一执行日：Day 4（9/5），先完成保存后最小回归再开始新功能
> Source of Truth：当前 Unity 工程 + Git + Docs

## 验收结论

本次核对磁盘代码、Unity Play Mode 只读快照、Console，以及用户在“游戏实习计划”提供的手工测试结果。
**保存阻断项已解除：保存后只读复查 dirty=false、isPlaying=false、isCompiling=false；磁盘已包含白盒、Motor 绑定、胶囊和动画配置。**
Day 3 功能阶段结合此前用户实测符合预期，持久化检查通过；尚未执行保存后的重新运行回归，最终收尾等待该项确认。

## 验收证据

| 项目 | 证据来源 | 结果 |
|---|---|---|
| PlayerMotor | 磁盘源码 + Play 中挂载 | 消费 MoveInput，世界 XZ 移动，ClampMagnitude 限幅，重力，CharacterController.Move；moveSpeed=5、gravity=-9.81 |
| 输入解耦 | 三份 Runtime 源码 | Motor 不直接读设备；InputReader 提供 Move/Look；CameraController 消费 Look |
| Idle 动画 | 磁盘 Controller + Unity 查询 | 默认状态 Armature\|Idle_Loop；循环=true；Imp Animator 有有效 Avatar、Controller，Root Motion=false；运行时 normalizedTime>13 |
| 动画视觉 | 用户手工测试 | 用户报告 Idle 循环与 Humanoid 重定向回归正常；本次未独立目视检查，未确认旧 Rig 错误根因 |
| 材质 | 磁盘 + Unity 查询 | Assets/_Game/Materials/MI_Imp.mat 已存在，运行时 Imp 的五个 Renderer 共用它 |
| 白盒 | Unity Play 快照 | Ground_Blockout=(0,-0.1,0)，scale=(20,0.2,20)；Wall_Blockout=(0,1.5,5)，scale=(10,3,0.5)，均有 BoxCollider |
| 胶囊 | Unity Play 快照 | center=(0,1,0)、height=2、radius=0.5；保存后磁盘与编辑态均一致 |
| Console | 当前 Console 查询 | 0 Error、1 Warning（Pipeline 非 automated 模式提示）；不是完整历史错误清零证明 |
| 场景持久化 | 磁盘与 Editor 对照 | 通过：磁盘包含 Ground/Wall、PlayerMotor GUID、胶囊 center=(0,1,0)；根 Animator 禁用，Imp Animator 绑定 Controller、关闭 Root Motion；Editor dirty=false |
| 自动化 / Build | 文件清单 | 只有测试 asmdef，无测试代码；未执行自动化或 Windows Build |

## 用户已执行的手工测试

以下 PASS 来自用户反馈，不是助手模拟输入的结果：

| ID | 操作 / 预期 | 结果 |
|---|---|---|
| D3-01 | Idle 持续循环，无明显重定向变形或 Root Motion 漂移 | 用户 PASS |
| D3-02 | 单方向输入正常移动 | 用户 PASS |
| D3-03 | W+D 斜向不比单方向更快 | 用户 PASS；源码限幅支持此预期，未测量位移 |
| D3-04 | 松开 W 后水平移动停止 | 用户 PASS |
| D3-05 | W+S 同时按住，输入抵消 | 用户 PASS |
| D3-06 | 正面撞墙停止 | 用户 PASS |
| D3-07 | 站立/移动不掉穿地面 | 用户 PASS |
| D3-08 | 停止后镜头轻微追随 | Observed / 用户接受；Damping 与输入更新顺序等原因尚未隔离 |

## Current Architecture

- Player：CharacterController、PlayerInput、PlayerInputReader、CameraController、PlayerMotor。
- Player 根 Animator 仍存在，编辑态与磁盘均已禁用；Imp 子 Animator 启用，负责 Idle 表现。
- Imp 的编辑态与磁盘 local rotation=(0,0,0)，不继续沿用旧聊天的 Y=180 作为当前事实。
- CameraTarget 为 Player 子对象；Main Camera 使用 CinemachineBrain。
- CameraController 仍在 Runtime/Input，Day 5 才迁往 Runtime/Camera，保留 .meta。
- 尚无镜头空间移动、角色转向、Sprint 读取/消费、Blend Tree、Player Prefab、楼梯、战斗或 AI。

## Files

- Assets/_Game/Runtime/Input/PlayerInputReader.cs
- Assets/_Game/Runtime/Input/CameraController.cs
- Assets/_Game/Runtime/Player/PlayerMotor.cs（及 .meta，当前未跟踪）
- Assets/_Game/Animations/Player/PlayerAnimator.controller
- Assets/_Game/Art/Charactors/Player/Model/Imp.fbx.meta
- Assets/_Game/Materials/MI_Imp.mat（及 .meta，当前未跟踪）
- Assets/_Game/Scenes/SampleScene.unity（用户已保存，磁盘配置核对通过）

## Known Issues / 风险

1. **保存后回归待确认。** 场景已经落盘且编辑态干净；下一次 Play 需复核 Idle、移动、停键、撞墙、落地与 Console。保存检查不等于运行回归。
2. Walk/Jog/Sprint 的 Loop 仍为 false，Day 4 逐条配置并预览。
3. InputReader、CameraController、PlayerMotor 均在 Update；采样顺序、禁用/启用、焦点切换尚未回归，不能保证严格零帧延迟。
4. 三个脚本缺少必需组件/引用的保护，非零相机初始角可能跳变；Day 5 做必要整理，不在验收时重写架构。
5. 根 Animator 已禁用并保存，Imp Animator 启用、Avatar 有效、Root Motion=false；保留当前已验证的单一动画驱动配置。
6. 平台边缘跌落恢复、楼梯、斜坡、移动动画和 Sprint 尚未测试；不属于本日已通过项。

## Git

- 当前分支 main；检查时 HEAD=d60549d，feat: checkpoint input and camera prototype (WIP)。
- 本次验收仅改 Docs，未改 Gameplay、未停止 Play、未保存场景、未暂存/提交/推送。
- Bestiary Imp FBX/PNG 已被 .gitignore 排除，check-ignore 验证成功；对应 .meta 与自有代码可提交。
- 当前自有新代码、材质、Controller/Imp 导入配置需随文档提交；场景已持久化，可一并纳入。
- AI Assistant Settings 与 SceneTemplateSettings 为额外本地变化，不作为本次验收成果。

## Next Task

### Day 3 最后确认

场景保存检查已通过。请重新进入 Play，确认 Idle、移动、停键、撞墙和落地仍正常，Console 无持续错误，再退出 Play；确认后即可将本日标记为正式通过。不需要重做已保存配置。

### Day 4｜9/5（总计 3–4 小时）

- 约 30 分钟：执行保存后最小回归与当日预检，持久化已完成。
- 约 60 分钟：镜头空间移动与角色转向；复用现有 InputReader 和 Motor。
- 约 40 分钟：Sprint 输入与速度切换。
- 约 30 分钟：Walk/Jog/Sprint 循环设置与重定向预览，不提前做 Blend Tree。
- 约 50–60 分钟：相机转向后 WASD、斜向限速、Sprint 按下/释放/静止、碰撞、镜头边界回归和 Docs/Git。
- 收尾超过 30 分钟时，优先保留镜头空间移动及测试；Sprint/动作预览顺延到 Day 5 并减少障碍内容，不压缩测试。

## Update Rules

- 只记录最新版事实；区分磁盘、编辑态、运行态、用户实测和助手实测。
- 教学遵循 ROADMAP：先 API 与验收条件，再独立练习、提示与必要的完整实现。
- 工程依赖实际变化才更新架构文档；Git 保存过程，不把本文件写成长日志。
