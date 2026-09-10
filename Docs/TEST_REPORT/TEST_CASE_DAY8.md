# Day 8 测试用例

## 基本信息

- 记录日期：2026-09-10
- Unity：`6000.5.6f1`
- 测试类型：EditMode 规则回归、Play Mode 集成检查、场景静态检查
- 测试对象：`Health`、`HealthBarPresenter`、Player/Enemy Health Bar
- 当前结论：首轮验收未通过

## 测试用例

| 用例编号 | 验证目标 | 实际结果 | 状态 |
|---|---|---|---|
| D8-01 | `ResetHealth()` 改名后领域规则无回归 | 10 个 Health 测试方法全部通过 | PASS |
| D8-02 | Player/Enemy 从 Inspector 配置初始化 | Player=100，Enemy=50 | PASS |
| D8-03 | Player 血条首次显示当前值 | Health=100，Slider=100 | PASS |
| D8-04 | Enemy 血条首次显示当前值 | Health=50，Slider=50 | PASS |
| D8-05 | Player 扣血事件更新血条 | 100→75，Slider 同步为 75 | PASS |
| D8-06 | Enemy 扣血事件更新血条 | 50→40，Slider 同步为 40 | PASS |
| D8-07 | `ResetHealth()` 更新血条 | Player/Enemy 均恢复到 MaxHealth | PASS |
| D8-08 | 非交互 Slider 不保留 Handle | 两条血条的 `handleRect` 均为空 | PASS |
| D8-09 | Presenter 使用显式 Health/Slider 引用 | 两套引用均完整；无场景查找 | PASS |
| D8-10 | Presenter 禁用期间发生扣血后重新启用 | Health=90，Slider 仍为 100 | FAIL |
| D8-11 | Enemy 血条使用世界空间 Canvas | `RenderMode=WorldSpace` | PASS |
| D8-12 | Enemy 血条随镜头角度始终面向相机 | 镜头转向后 Canvas 不旋转，最近面夹角 78.4° | FAIL |

## 静态与环境检查

| 检查项 | 结果 |
|---|---|
| `Health.cs` Unity 静态验证 | 0 diagnostics |
| `HealthBarPresenter.cs` Unity 静态验证 | 0 diagnostics |
| 场景 Missing Script | 0 |
| Unity Gameplay Error | 0 |
| 编辑器服务 Warning | 2（Pipeline 自动化模式、AI Account 网络；与 Gameplay 无关） |

## 结果汇总

| 结果 | 数量 |
|---|---:|
| PASS | 10 |
| FAIL | 2 |
| NOT RUN | 0 |
| BLOCKED | 0 |

结论：**NOT PASS**。Player/Enemy 血条的初始化、事件更新和恢复路径成立，但 `BUG-005` 会让重新启用的 Presenter 显示旧状态，`BUG-006` 会让 Enemy 血条在镜头绕转后失去正面可读性。修复并回归 D8-10、D8-12 后再做 Day 8 最终验收。
