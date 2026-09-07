# Day 5 测试用例

## 基本信息

- 记录日期：2026-09-07
- Unity：`6000.5.6f1`
- 场景：`Assets/_Game/Scenes/SampleScene.unity`
- Prefab：`Assets/_Game/Prefabs/Player.prefab`
- 测试对象：Blend Tree、Animator Driver、Player Prefab、脚本迁移和引用保护
- 结果来源：当前工程扫描及“游戏实习计划”中的用户手工测试记录

## 测试用例

| 用例编号 | 测试模块 | 用例标题 | 前置条件 | 操作步骤 | 预期结果 | 实际结果 | 状态 |
|---|---|---|---|---|---|---|---|
| D5-01 | Diagnostics | Day 5 编译检查 | Day 5 脚本和 Animator 已保存 | 等待 Unity 编译完成并检查 Console | 无编译错误和 Missing Script | 用户报告无错误 | PASS |
| D5-02 | Animation | Blend Tree 配置 | PlayerAnimator 已包含 Speed 参数 | 检查 Blend Tree 的 Motion、Threshold 和参数 | Idle=0、Walk=2.5、Jog=5、Sprint=10；参数为 Speed | 当前 Controller 静态配置符合预期 | PASS |
| D5-03 | Animation | 静止播放 Idle | Animator 和 Driver 引用有效 | 进入 Play Mode，不输入移动 | Speed 接近 0，播放 Idle | 用户测试符合预期 | PASS |
| D5-04 | Animation | 普通移动播放 Jog | Player 位于空旷地面 | 按住 WASD 普通移动 | Speed 随实际速度变化，主要播放 Jog | 用户测试符合预期 | PASS |
| D5-05 | Animation | 冲刺播放 Sprint | Player 正在普通移动 | 按住 Shift | Blend Tree 平滑过渡到 Sprint | 用户测试符合预期 | PASS |
| D5-06 | Animation | 松键返回 Idle | Player 正在移动或冲刺 | 松开全部移动键 | 实际速度归零，Blend Tree 返回 Idle | 用户测试符合预期 | PASS |
| D5-07 | Animation | 撞墙后动画降速 | Player 正对墙体 | 按住 W 走向墙并持续输入 | CharacterController 实际水平速度下降，动画回落而非原地高速跑 | 用户测试符合预期 | PASS |
| D5-08 | Prefab | Player Prefab 内部引用 | Player.prefab 已创建 | 打开 Prefab，检查 CameraTarget、Animator 和组件引用 | cameraTransform/cameraTarget 指向 Prefab 内 CameraTarget；animator 指向 Imp Animator | 当前 Prefab 静态引用符合预期 | PASS |
| D5-09 | Prefab | Prefab 实例运行回归 | 场景 Player 为 Prefab Instance | 进入 Play，测试 WASD、镜头、转向、Sprint 和 Blend Tree | 创建 Prefab 后所有既有功能不退化 | 用户测试符合预期 | PASS |
| D5-10 | Architecture | CameraController 迁移 | CameraController 已从 Runtime/Input 移动 | 等待编译，检查 Scene 与 Prefab 组件 | `.meta`/GUID 保留；无 Missing Script 或编译错误 | 用户报告无异常；当前新路径文件及 `.meta` 存在 | PASS |
| D5-11 | Dependency | RequireComponent 声明 | 三个脚本完成编译 | 检查 PlayerInputReader、PlayerMotor、CameraController、PlayerAnimatorDriver | 同对象强依赖通过 RequireComponent 明确声明 | 当前源码符合预期 | PASS |
| D5-12 | Exception | CameraTarget 缺失保护 | 正常引用已记录，处于 Edit Mode | 临时清空 CameraController.Camera Target 后进入 Play | 只记录一次明确错误；CameraController 禁用；不持续刷 NullReferenceException | 用户截图验证符合预期 | PASS |
| D5-13 | Exception | Animator 缺失保护 | 正常引用已记录，处于 Edit Mode | 临时清空 PlayerAnimatorDriver.Animator 后进入 Play | 只记录一次明确错误；Driver 禁用；不持续刷 NullReferenceException | 用户截图验证符合预期 | PASS |
| D5-14 | Exception | Camera Transform 缺失保护 | 正常引用已记录，处于 Edit Mode | 临时清空 PlayerMotor.Camera Transform 后进入 Play | 只记录一次明确错误；PlayerMotor 禁用；不持续刷 NullReferenceException | 用户截图验证符合预期 | PASS |
| D5-15 | Persistence | 恢复引用并保存 | D5-12～14 已完成 | 退出 Play，在 Edit Mode 恢复三个引用并保存 Scene/Prefab | Scene 与 Prefab 均保存正确引用 | 用户确认完成；磁盘 Prefab 的三个内部引用均有效，Scene 已更新 | PASS |
| D5-16 | Regression | Day 5 最终最小回归 | D5-15 已通过，Console 已清空 | 依次测试 WASD、镜头、转向、Reverse Turn、Sprint、Idle/Jog/Sprint 和撞墙 | 全部既有功能正常，Console Gameplay 0 Error | 用户完成测试并报告全部通过 | PASS |

## 结果汇总

| 结果 | 数量 |
|---|---:|
| PASS | 16 |
| FAIL | 0 |
| NOT RUN | 0 |
| BLOCKED | 0 |

结论：**PASS（16/16）**。Day 5 的 Blend Tree、Prefab、脚本迁移、依赖声明、引用保护和最终回归均通过。
