# Day 16 Test Case Report — Three Enemies / Phase A Acceptance

> 计划日期：2026-09-19  
> 验收日期：2026-09-20  
> Unity：6000.5.6f1  
> 场景：`Assets/_Game/Scenes/SampleScene.unity`  
> 结论：PASS（12/12）

## 验收范围

- 3 个 Puglin Prefab 实例的共享配置与独立运行时状态。
- 多敌人追击、近身拥挤、同时受击、依次死亡和 Player 死亡后的停止行为。
- Health、世界空间血条、Enemy Attack/Hit/Dead 与场景持久化回归。

## 测试结果

| ID | 测试内容 | 预期结果 | 实际结果 | 状态 |
|---|---|---|---|---|
| D16-01 | 三敌实例与引用 | 场景包含 3 个 Puglin Prefab 实例，均引用 Player、Animator 与同一 Enemy AttackDefinition | Unity MCP 确认 `Puglin_01/02/03` 均来自同一 Prefab，引用完整 | PASS |
| D16-02 | 三敌共同追击 | 三敌均能生成完整路径并向 Player 接近 | 三个 Agent 均位于 NavMesh，追击时 `hasPath=true`、`PathComplete` | PASS |
| D16-03 | 近身拥挤 | 多敌人靠近同一目标时不穿插、不离开 NavMesh、不阻断状态机 | 最近两敌中心距离约 0.56m，与 0.28m Agent 半径之和一致；均保持在 NavMesh | PASS |
| D16-04 | 独立生命状态 | 每个实例拥有独立 Health，不共享运行时 HP | 三敌初始均为 50/50；分别结算后数值独立 | PASS |
| D16-05 | 同时受击 | 同一轮对三敌造成 10 点伤害时，三者均进入受击并各扣一次 | 三敌均由 50 降至 40，`IsAttacking=false`、Agent 停止 | PASS |
| D16-06 | 受击中断 | Enemy 正在攻击时受击应清除攻击状态 | 同时受击后三个 EnemyCombat 均为 `IsAttacking=false` | PASS |
| D16-07 | 逐个死亡 | 依次击杀三敌时，每个实例独立进入死亡终态 | 三敌依次 HP=0，均保持 `IsAttacking=false`、Agent 停止 | PASS |
| D16-08 | 死亡动画 | 每个死亡实例播放 Death01，不恢复追击/攻击 | 三敌运行态均为 `Death01=true` | PASS |
| D16-09 | Enemy 血条回归 | 三条世界空间血条各自绑定对应 Health 并显示最终 HP | 三个 Presenter 分别绑定 Puglin_01/02/03；死亡后均为 0/50 | PASS |
| D16-10 | Player 死亡后全部停止 | Player HP=0 后三敌全部结束攻击、清空目标并停止 Agent | 三敌均 `target=null`、`IsAttacking=false`、`isStopped=true`、`hasPath=false` | PASS |
| D16-11 | Console 与编译 | 游戏运行无持续异常，Runtime 程序集可编译 | Unity Console 0 Error / 0 Warning；`Game.Runtime` 构建 0 Error / 0 Warning | PASS |
| D16-12 | Play Mode 恢复 | 退出运行后不污染场景保存状态 | 场景恢复原位置，3 个实例均 Active，Scene `dirty=false` | PASS |

## 回归说明

- Day 16 没有新增 C# 运行时逻辑；复用 Day 8–15 已验收的 Health、血条、Combo、命中去重与 Enemy 状态职责。
- 本次通过三实例同时受击、逐个死亡和完整围攻，重点验证多实例不会共享 HP、命中集合或攻击运行状态。
- `PuglinTest.controller` 的相关 Exit Time 已保存为 1，完整播放后再退出状态。
- `MeleeHitbox` 反射 PlayMode 测试仍按既定决定延期，不计入本次自动化证据。

## 验收结论

- Day 16 正式通过，Phase A 的庭院、移动、生命/血条、三段 Combo、伤害窗口、单敌 AI/Combat 与三敌集成闭环成立。
- 严重/阻断 Bug 为 0，可以进入 Day 17 的 `SkillDefinition` 与冷却规则。

