# Day 7 测试用例

## 基本信息

- 记录日期：2026-09-09
- Unity：`6000.5.6f1`
- 测试类型：EditMode
- 测试对象：`DamageInfo`、`IDamageable`、`Health`
- 测试文件：`Assets/_Game/Tests/EditMode/HealthTests.cs`
- 结果来源：当前 Unity 工程 Test Runner 生成的 `TestResults.xml`

## 测试用例

| 用例编号 | 自动化测试 | 验证目标 | 实际结果 | 状态 |
|---|---|---|---|---|
| D7-01 | `Initialize_SetsCurrentHealthToMaxHealth` | 初始化后 CurrentHealth 等于 MaxHealth | Passed | PASS |
| D7-02 | `Initialize_ClampsNegativeHealthToZero` | 负生命上限钳制为 0 | Passed | PASS |
| D7-03 | `TakeDamage_ReducesCurrentHealth` | 正常伤害正确扣减 | Passed | PASS |
| D7-04 | `TakeDamage_ClampsCurrentHealthAtZero` | 超额伤害不会使 HP 低于 0 | Passed | PASS |
| D7-05 | `TakeDamage_InvokesHealthChanged` | 扣血后事件报告最终 HP | Passed | PASS |
| D7-06 | `TakeDamage_InvokesDiedOnlyOnce` | 死亡后重复伤害不会重复触发 Died | Passed | PASS |
| D7-07 | `TakeDamage_NegativeDamageDoesNotChangeHealth` | 负伤害不改变 HP | Passed | PASS |
| D7-08 | `TakeDamage_ZeroDamageDoesNotChangeHealthOrInvokeHealthChanged` | 0 伤害不改变 HP，也不触发事件 | Passed | PASS |
| D7-09 | `Reset_RestoresHealthAndAllowsDyingAgain` | 恢复满血后可以再次死亡 | Passed | PASS |
| D7-10 | `Reset_InvokesHealthChangedWithMaxHealth` | 恢复时事件报告 MaxHealth | Passed | PASS |

## 静态与环境检查

| 检查项 | 结果 |
|---|---|
| 4 个新增 C# 文件 Unity 静态验证 | 0 diagnostics |
| `DamageInfo.DamageAmount` 外部只读 | PASS |
| Health 不依赖 UI/Animator/Player/Enemy | PASS |
| Test Runner 汇总 | 10 total / 10 passed / 0 failed / 0 skipped |
| Console 复查 | 0 Error / 0 Warning |

## 结果汇总

| 结果 | 数量 |
|---|---:|
| PASS | 10 |
| FAIL | 0 |
| NOT RUN | 0 |
| BLOCKED | 0 |

结论：**PASS（10/10）**。Day 7 的生命领域规则和事件行为满足当前验收范围。`Health.Reset()` 与 Unity 编辑器消息同名属于非阻断命名风险，计划在 Day 8 接入首个调用者前改名并回归。
