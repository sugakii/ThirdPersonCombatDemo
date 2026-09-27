# Day 21 Test Report

> 日期：2026-09-27  
> 范围：运行时代码、场景/Prefab 引用、生命周期、物理查询分配与自动化回归  
> 实机复核：Unity MCP 连接当前工程

## 验收结论

Day 21 **PASS**。运行时代码和 36 个场景组件已完成审查；Puglin Prefab 的共享引用缺失已修复；普攻与技能检测已改为固定缓冲的 NonAlloc 查询，冷却文字不再逐帧创建字符串；EditMode `14/14 PASS`、PlayMode `5/5 PASS`，最终 Console `0 Error / 0 Warning`。P0/P1 缺陷为 0。

## 验收用例

| ID | 检查项 | 预期结果 | 实际结果 | 状态 |
|---|---|---|---|---|
| D21-01 | 扫描 Runtime 代码中的临时日志、TODO 与未实现代码 | 无遗留调试逻辑或未实现分支 | 删除 SkillController 两条临时日志；无 TODO/FIXME | PASS |
| D21-02 | 检查事件生命周期 | OnEnable/OnDisable 订阅退订成对 | Health、GameFlow、Enemy、Skill、UI 均成对 | PASS |
| D21-03 | 扫描 SampleScene 的自有运行时组件引用 | 必需序列化引用均有效 | 36 个组件，空引用 0 | PASS |
| D21-04 | 扫描 MainMenu 引用 | 必需序列化引用均有效 | 空引用 0 | PASS |
| D21-05 | 扫描 Puglin Prefab | 可复用配置保存在 Prefab | AttackDefinition 与 Animator 已写回 Prefab | PASS |
| D21-06 | 检查 Puglin 的 Player Target | 场景对象不写入 Prefab Asset | Prefab 保持空；3 个场景实例显式绑定 Player | PASS |
| D21-07 | 检查物理与冷却 UI 热路径 | 不反复分配查询数组或逐帧创建文字 | 检测使用固定缓冲；文字只在 3/2/1 变化时更新 | PASS |
| D21-08 | 检查缺失引用保护 | 缺失 Target/Animator/配置/Health 时明确失败 | EnemyStateMachine 与 SkillHitDetector 入口保护有效 | PASS |
| D21-09 | 核对自动化测试数量 | 历史稳定测试未丢失 | 恢复冷却结束测试；总计 19 条 | PASS |
| D21-10 | 运行 EditMode 测试 | 全部通过 | 14/14 PASS | PASS |
| D21-11 | 运行 PlayMode 测试 | 全部通过 | 5/5 PASS | PASS |
| D21-12 | 最终 Console 与场景状态 | 无 Error/Warning，场景已保存 | 0 Error / 0 Warning；SampleScene dirty=false | PASS |

## 修复记录

1. `Puglin.prefab` 原本依赖三个场景实例 Override 才能取得共享的 `AttackDefinition` 与 Animator；现已把可复用引用写回 Prefab。
2. `EnemyStateMachine` 增加 AttackDefinition 与 Target Health 的启动验证；Player Target 仍由场景实例绑定。
3. `MeleeHitbox`、`SkillHitDetector` 使用固定 Collider 缓冲区，移除攻击检测期间的数组返回查询。
4. `CooldownPresenter` 只在显示秒数变化时更新文本，避免冷却期间每帧 `ToString()`。
5. `SkillHitDetector` 的保护位于 `BeginDetection()`，兼容测试通过 `Initialize()` 注入配置的流程。
6. 恢复 `TickCooldown_WhenCooldownEnds_SkillCanBeUsedAgain`，EditMode 从 13 条恢复为已验收的 14 条。

## 延后项

- Windows x64 Build、Build 专项测试、README、架构图和 Release 属于 Day 22。
- `MeleeHitbox` 独立 PlayMode 自动化仍不强制补写；现有手工回归和 SkillHitDetector 测试不冒充该项证据。
