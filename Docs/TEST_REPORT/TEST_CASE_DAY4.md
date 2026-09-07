# Day 4 测试用例

## 基本信息

- 验收日期：2026-09-07
- Unity：`6000.5.6f1`
- 场景：`Assets/_Game/Scenes/SampleScene.unity`
- 测试对象：镜头空间移动、转向、Sprint、动画循环与异常输入
- 结果来源：工程静态检查和用户手工测试

## 测试用例

| 用例编号 | 测试模块 | 用例标题 | 前置条件 | 操作步骤 | 预期结果 | 实际结果 | 状态 |
|---|---|---|---|---|---|---|---|
| D4-01 | Diagnostics | 编译与 Console | Day 4 代码和场景已保存 | 等待 Unity 编译完成，清空 Console 后进入 Play | 无 Gameplay 编译错误或持续异常 | 0 Gameplay Error；工具提示不影响功能 | PASS |
| D4-02 | Configuration | Camera 引用 | PlayerMotor 已挂载 | 检查 PlayerMotor 的 Camera Transform | 引用有效，运行时不会因缺失引用失败 | 引用有效 | PASS |
| D4-03 | Input | Sprint 输入链 | Input Action Asset 含 Sprint | 检查 Sprint Action 和 PlayerInputReader，再进入 Play 按 Shift | SprintHeld 能反映 Shift 按下/释放 | 用户运行测试符合预期 | PASS |
| D4-04 | Animation | Locomotion 循环配置 | UAL1 已导入 | 检查 Idle/Walk/Jog/Sprint 与 A_TPose 的导入设置 | 四个 locomotion 开启 Loop Time；A_TPose 不循环 | 配置符合预期；BUG-002 已关闭 | PASS |
| D4-05 | Movement | 镜头朝向后按 W | Player 位于空旷地面 | 水平旋转镜头后按 W | Player 朝镜头水平前方移动 | 用户测试符合预期 | PASS |
| D4-06 | Movement | 镜头朝向后 A/D/S | Player 位于空旷地面 | 在多个镜头角度分别按 A、D、S | 移动方向始终相对镜头正确 | 用户测试符合预期 | PASS |
| D4-07 | Movement | 镜头空间斜向限速 | Player 位于空旷地面 | 比较按 W 与 W+D 的移动表现 | 斜向速度不高于单方向速度 | 用户测试符合预期 | PASS |
| D4-08 | Rotation | 普通方向转向 | Player 正在移动 | 连续改变 WASD 输入方向 | Player 平滑朝当前移动方向转向 | 用户测试符合预期 | PASS |
| D4-09 | Rotation | 180°快速换向 | Player 正在向前移动 | 保持 W 后快速切换为 S，再改变方向 | 快速完成反向转身，无持续朝向与位移分离 | 用户测试符合预期 | PASS |
| D4-10 | Sprint | 移动中开始冲刺 | Player 正在普通移动 | 按住 Shift | 移动速度从 5 提升到 10 | 用户测试符合预期 | PASS |
| D4-11 | Sprint | 松开 Shift 恢复速度 | Player 正在冲刺 | 松开 Shift，继续保持移动输入 | 立即恢复普通速度 5，无冲刺残留 | 用户测试符合预期 | PASS |
| D4-12 | Sprint | 静止时按 Shift | Player 无移动输入 | 按住并松开 Shift | Player 不产生位移，Console 无异常 | 用户测试符合预期 | PASS |
| D4-13 | Collision | 冲刺撞墙 | Player 正对墙或贴近墙体 | 按 W+Shift 冲向墙，并尝试贴墙斜移 | 不穿墙，不发生失控抖动 | 用户测试符合预期 | PASS |
| D4-14 | Camera | 镜头俯仰边界 | CameraController 引用有效 | 将镜头转至上下极限并移动 | 镜头不翻转，移动方向保持稳定 | 用户测试符合预期 | PASS |
| D4-15 | Input | 窗口焦点恢复 | 游戏处于 Play Mode | 按住移动/冲刺输入，Alt-Tab，松键后返回 Unity | 不出现粘键、持续移动或持续冲刺 | 用户测试符合预期 | PASS |
| D4-16 | Animation | Locomotion 视觉预览 | Idle/Walk/Jog/Sprint 循环已配置 | 分别预览四个 Clip | 无明显骨骼扭曲、错误朝向、严重滑步或循环跳变 | 用户测试符合预期；BUG-001、BUG-002 已关闭 | PASS |

## 结果汇总

| 结果 | 数量 |
|---|---:|
| PASS | 16 |
| FAIL | 0 |
| NOT RUN | 0 |
| BLOCKED | 0 |

结论：**PASS（16/16）**。
