# Day 3 测试用例

## 基本信息

- 测试日期：2026-09-04
- Unity：`6000.5.6f1`
- 场景：`Assets/_Game/Scenes/SampleScene.unity`
- 测试对象：Idle、基础移动、输入边界、碰撞、重力和场景持久化
- 结果来源：用户手工测试、工程静态检查和 Unity Console

## 测试用例

| 用例编号 | 测试模块 | 用例标题 | 前置条件 | 操作步骤 | 预期结果 | 实际结果 | 状态 |
|---|---|---|---|---|---|---|---|
| D3-01 | Animation | Idle 循环 | Imp Animator 已绑定有效 Avatar 和 PlayerAnimator；Root Motion=false | 进入 Play Mode，静置观察至少两个循环 | Idle 连续循环；角色无明显变形或位置漂移 | 用户观察符合预期 | PASS |
| D3-02 | Movement | 单方向移动 | Player 位于空旷地面 | 按住 W 约 2 秒 | Player 持续向前移动 | 用户测试符合预期 | PASS |
| D3-03 | Movement | 斜向输入限速 | Player 位于空旷地面 | 分别按 W 与 W+D，比较移动表现 | W+D 不应比 W 明显更快 | 未观察到斜向加速；未做数值测量 | PASS |
| D3-04 | Input | 松键立即停止 | Player 正在按 W 移动 | 松开 W | 水平移动立即停止，无残留滑行 | 用户测试符合预期 | PASS |
| D3-05 | Input | 相反方向抵消 | Player 位于空旷地面 | 同时按住 W+S | 前后输入互相抵消，Player 不移动 | 用户测试符合预期 | PASS |
| D3-06 | Collision | 正面撞墙 | Player 正对 Wall_Blockout | 持续按 W 走向墙体 | Player 在墙前停止，不穿墙 | 用户测试符合预期 | PASS |
| D3-07 | Collision | 地面承载 | Player 位于 Ground_Blockout | 静置并移动 | Player 不掉穿地面 | 用户测试符合预期 | PASS |
| D3-08 | Animation | Root Motion 禁用 | `Animator.applyRootMotion=false` | 播放 Idle 并观察 Player Transform | 动画不直接推动 Player 根对象 | 未观察到漂移，配置为 false | PASS |
| D3-09 | Diagnostics | Console 基础检查 | 工程完成编译 | 清空 Console 后进入 Play Mode | 无 Gameplay 编译错误或持续异常 | 0 Gameplay Error；工具警告不影响功能 | PASS |
| D3-10 | Persistence | 场景保存 | 白盒、胶囊、Motor 和 Animator 已配置 | 退出 Play Mode，保存并重新检查场景 | 配置完整保留，场景不处于 Dirty 状态 | 磁盘与编辑态一致 | PASS |
| D3-11 | Regression | 保存后运行回归 | D3-10 已完成 | 保存后重新进入 Play，复测 Idle、移动、停键、碰撞与 Console | 已通过功能仍然正常，无新增错误 | 后续 Day 4 全量运行测试未发现回归 | PASS |

## 结果汇总

| 结果 | 数量 |
|---|---:|
| PASS | 11 |
| FAIL | 0 |
| NOT RUN | 0 |
| BLOCKED | 0 |

结论：**PASS（11/11）**。
