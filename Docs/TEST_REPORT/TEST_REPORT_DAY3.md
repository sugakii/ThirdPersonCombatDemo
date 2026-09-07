# Day 3–4 Test Report

> 本文件保留 Day 3 已执行结果，并追加 Day 4 验收。文件名暂不改动，以维持现有六份文档；后续可在周验收时统一重命名为 `TEST_REPORT_WEEK1.md`。

## 1. 基本信息

- Day 3 测试日期：2026-09-04
- Day 4 验收日期：2026-09-07
- 测试版本：`b051871` 之后的 Day 4 工作树
- Unity：`6000.5.6f1`
- 场景：`Assets/_Game/Scenes/SampleScene.unity`
- 测试类型：功能、边界、回归、静态配置检查

## 2. Day 3 已执行结果

| ID | 测试项 | 实际结果 | 结果 |
|---|---|---|---|
| D3-01 | Idle 循环 | 用户报告连续循环，无明显变形或漂移 | PASS |
| D3-02 | 单方向移动 | 用户报告正常 | PASS |
| D3-03 | W+D 斜向限速 | 未观察到明显加速；未测量数值 | PASS |
| D3-04 | 松键停止 | 松开 W 后立即停止 | PASS |
| D3-05 | W+S 相反输入 | 角色不移动 | PASS |
| D3-06 | 墙体碰撞 | 角色停止且不穿墙 | PASS |
| D3-07 | 地面碰撞 | 不掉穿地面 | PASS |
| D3-08 | Root Motion | 未观察到漂移；配置为 false | PASS |
| D3-09 | Console | 0 Gameplay Error；Pipeline 工具警告不影响功能 | PASS |
| D3-10 | 场景持久化 | 磁盘与编辑态一致，dirty=false | PASS |
| D3-11 | 保存后运行回归 | 用户在保存后的当前工程完成 Day 4 全量运行测试，未报告回归异常 | PASS |

Day 3 结论：**PASS**（11 PASS / 0 FAIL）。

## 3. Day 4 范围

包含：

- 镜头空间移动与斜向限速。
- 角色朝移动方向转向与 180°反向转向。
- Sprint 按下、松开和静止输入。
- Main Camera 引用与移动参数。
- Idle/Walk/Jog/Sprint Loop 配置。
- Console 与场景静态检查。

不包含：Blend Tree、Player Prefab、楼梯、自动化测试、Windows Build。

## 4. Day 4 执行结果

| ID | 测试项 | 操作 / 检查 | 预期结果 | 实际结果 | 结果 |
|---|---|---|---|---|---|
| D4-01 | 编译与 Console | 打开工程并检查 Console | 无 Gameplay 编译错误或持续异常 | 0 Error；工具警告/Info 不影响 Gameplay | PASS |
| D4-02 | Camera 引用 | 检查 PlayerMotor 序列化引用 | 指向 Main Camera Transform | 已绑定 Main Camera | PASS |
| D4-03 | Sprint 输入链 | 检查 Input Action 与 InputReader | Sprint Action 可被读取并输出意图 | Sprint 存在；SprintHeld 使用 IsPressed | PASS（静态） |
| D4-04 | Locomotion Loop | 检查 UAL1 Clip 设置 | Idle/Walk/Jog/Sprint Loop Time=true；A_TPose 不循环 | 五项磁盘配置均正确；运行视觉回归归入 D4-16 | PASS |
| D4-05 | 镜头朝向后 W | 旋转镜头后按 W | 朝镜头前方的水平投影移动 | 用户测试符合预期 | PASS |
| D4-06 | 镜头朝向后 A/D/S | 多角度分别输入 | 相对镜头方向正确 | 用户测试符合预期 | PASS |
| D4-07 | 斜向速度 | 对比 W 与 W+D 位移 | 斜向不快于单方向 | 用户测试符合预期 | PASS |
| D4-08 | 普通转向 | 输入连续改变方向 | Player 平滑朝移动方向 | 用户测试符合预期 | PASS |
| D4-09 | 180°换向 | W 移动时快速切 S，再改变方向 | 快速转身，无持续视觉/位移分离 | 用户测试符合预期 | PASS |
| D4-10 | Sprint 加速 | 移动中按住 Shift | 速度由 5 切到 10 | 用户测试符合预期 | PASS |
| D4-11 | Sprint 恢复 | 松开 Shift 继续移动 | 恢复速度 5，无残留 | 用户测试符合预期 | PASS |
| D4-12 | 静止 Sprint | 静止按住/松开 Shift | 不产生位移或错误 | 用户测试符合预期 | PASS |
| D4-13 | Sprint 碰撞 | 冲刺撞墙及贴墙斜移 | 不穿墙、不抖动失控 | 用户测试符合预期 | PASS |
| D4-14 | 镜头边界 | 旋转至俯仰极限并移动 | 无翻转；移动方向稳定 | 用户测试符合预期 | PASS |
| D4-15 | 焦点恢复 | Alt-Tab 后返回并释放输入 | 无粘键或持续冲刺 | 用户测试符合预期 | PASS |
| D4-16 | 动作视觉预览 | 逐条预览 Idle/Walk/Jog/Sprint | 无明显扭曲、错误朝向、严重滑步或循环跳变 | 用户测试符合预期 | PASS |

## 5. Day 4 统计

| 结果 | 数量 |
|---|---:|
| PASS | 16 |
| FAIL | 0 |
| NOT RUN | 0 |
| BLOCKED | 0 |

Day 4 执行率与通过率均为 `16/16 = 100%`，总体验收状态为 **PASS**。D4-01～04 包含工程静态证据，D4-05～16 的运行结果来自用户手工测试。

## 6. 缺陷与观察

- `BUG-001`：Imp Avatar/Rig 旧问题；Idle/Walk/Jog/Sprint 视觉回归及 Console 检查通过，Closed。
- `BUG-002`：A_TPose 被误开 Loop Time/Loop Pose，配置修复与用户 Play Mode 回归均通过，Closed。
- `OBS-001`：停止移动时镜头轻微追随；用户接受，暂不作为 Bug。

## 7. 验收结论

Day 4 全部 16 条用例通过，可以进入 Learning Day 5。自动化测试、Blend Tree、Player Prefab 与楼梯仍属于后续任务，不因本次 Day 4 通过而视为完成。
