# Day 6 测试用例

## 基本信息

- 记录日期：2026-09-08
- Unity：`6000.5.6f1`
- 场景：`Assets/_Game/Scenes/SampleScene.unity`
- 测试对象：Week 1 移动、镜头、动画、CharacterController 边界与 PlayMode 自动化
- 结果来源：当前工程扫描、Unity Console 与“游戏实习计划”中的用户实际测试记录

## 测试用例

| 用例编号 | 类型 | 用例标题 | 操作摘要 | 预期结果 | 实际结果 | 状态 |
|---|---|---|---|---|---|---|
| D6-01 | Manual | 单方向移动 | 分别按 W/A/S/D | 四个方向稳定移动，松键立即停止 | 用户测试通过 | PASS |
| D6-02 | Manual | 斜向限速 | 比较 W 与 W+D 移动速度 | 斜向速度不高于单方向速度 | 用户测试通过 | PASS |
| D6-03 | Manual | 相反输入抵消 | 同时按 W+S、A+D | 对向输入抵消，无残留移动 | 用户测试通过 | PASS |
| D6-04 | Manual | 镜头空间移动 | 旋转镜头后按 WASD | 移动方向跟随镜头水平朝向，输入强度不变 | 用户测试通过 | PASS |
| D6-05 | Manual | Sprint 切换与恢复 | 移动时按下并松开 Shift | 冲刺加速；松开后恢复普通速度 | 用户测试通过 | PASS |
| D6-06 | Manual | 楼梯通行 | 正反方向通过楼梯 | 稳定上下楼，不穿透、不异常悬空 | 用户测试通过 | PASS |
| D6-07 | Manual | 30°/50°斜坡 | 分别尝试通过两种坡度 | 按 CharacterController 坡度规则稳定处理 | 用户测试通过 | PASS |
| D6-08 | Manual | 墙角与沿墙滑动 | 正面撞墙、斜向贴墙、进入墙角 | 不穿墙；可合理停止或沿墙滑动；不永久卡死 | 用户测试通过 | PASS |
| D6-09 | Manual | 平台边缘掉落 | 从平台边缘走出 | 正常离地和下落，不悬空、不穿地 | 用户测试通过 | PASS |
| D6-10 | Manual | 动画与 Console 回归 | 测试 Idle/Jog/Sprint、撞墙和停止 | 动画跟随实际速度；无 Gameplay Error | 用户测试通过；当前 Console 0 Error | PASS |
| D6-AUT-01 | Automated | `DiagonalInput_DoesNotExceedMoveSpeed` | 虚拟键盘发送 W，再发送 W+D 并比较速度 | 前进速度 > 0；斜向速度 ≤ 前进速度 + 0.1 | 连续运行与冷启动运行均通过；源码断言覆盖目标 | STABLE PASS |
| D6-AUT-02 | Automated | `SprintRelease_RestoresNormalSpeed` | 当前源码仅发送 W 并断言普通速度 > 0 | 应发送 W+Shift、验证加速、释放 Shift 后验证恢复 | Test Runner 为 PASS，但缺少 Sprint/释放阶段与恢复断言 | INCOMPLETE |

## 自动化稳定性缺陷回归

| 项目 | 结果 |
|---|---|
| 原现象 | `normalSpeed` 偶发为 0，同一代码首次 FAIL、重跑 PASS |
| 根因范围 | `LoadScene()` 后只固定等待一帧，虚拟输入可能早于场景与 PlayerInput 稳定初始化 |
| 修复 | 改用 `LoadSceneAsync()`，等待 `AsyncOperation` 完成后再额外等待一帧 |
| 连续 Run All 5 次 | 2/2 绿灯 |
| Unity 冷启动首次 Run All | 2/2 绿灯 |
| 结论 | 初始化时序问题已关闭；不改变 D6-AUT-02 功能断言不完整的事实 |

## 结果汇总

| 结果 | 数量 |
|---|---:|
| PASS / STABLE PASS | 11 |
| INCOMPLETE | 1 |
| FAIL | 0 |
| NOT RUN | 0 |

结论：**CONDITIONAL PASS（11 项有效通过，1 项自动化覆盖未完成）**。手工回归和测试场景验收通过；补全 D6-AUT-02 的实际 Sprint 按下、释放与速度恢复断言后，才能关闭 Day 6 和 Week 1 自动化门槛。
