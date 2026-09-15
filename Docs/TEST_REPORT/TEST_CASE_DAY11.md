# Day 11 Test Case Report — Melee Damage Window

> 日期：2026-09-14  
> Unity：6000.5.6f1  
> 场景：`Assets/_Game/Scenes/SampleScene.unity`  
> 结论：PASS；功能与边界手工回归通过，反射 PlayMode 测试经范围评估后延期

## 验收范围

- Animation Event 伤害窗口。
- `MeleeHitbox` 物理查询、Layer 和方向过滤。
- 同一攻击窗口命中去重与跨窗口重置。
- `DamageInfo → IDamageable → Health` 伤害链。

## 测试结果

| ID | 测试内容 | 预期结果 | 实际结果 | 状态 |
|---|---|---|---|---|
| D11-01 | 窗口外目标位于范围内 | 不扣血 | 不扣血 | PASS |
| D11-02 | 窗口内目标位于范围内 | 按当前段配置扣血 | 正确扣血 | PASS |
| D11-03 | 同一刀跨多帧检测 | 同一目标只扣一次 | 只扣一次 | PASS |
| D11-04 | 同一目标挂载两个 Collider | 同一刀只扣一次 | 只扣一次 | PASS |
| D11-05 | 关闭窗口后开始下一刀 | 同一目标可再次受伤 | 再次正确扣血 | PASS |
| D11-06 | 一刀覆盖两个不同目标 | 两个目标各扣一次 | 两个目标均正确扣血 | PASS |
| D11-07 | Enemy 位于判定距离外 | 不扣血 | 不扣血 | PASS |
| D11-08 | 范围内对象不在 Enemy Layer | 不参与查询 | 不受伤且无异常 | PASS |
| D11-09 | Enemy 位于角色后方 | 前半球过滤后不扣血 | 不扣血 | PASS |
| D11-10 | Attack_01/02/03 | 分别造成 10/15/20 伤害 | 10/15/20 | PASS |
| D11-11 | 5 HP 承受 10 点伤害 | HP 钳制为 0，血条归零 | HP=0，血条归零 | PASS |
| D11-12 | `hitboxCenter` 未绑定 | 启动时报一次明确错误并禁用组件 | 代码具备保护，未提供实机执行证据 | NOT RUN |
| D11-13 | PlayMode：同窗口多 Collider 去重 | 自动断言只扣一次 | 当前阶段不实现反射测试，不计为自动化证据 | DEFERRED |
| D11-14 | PlayMode：新窗口允许再次命中 | 自动断言第二次扣血 | 当前阶段不实现反射测试，不计为自动化证据 | DEFERRED |

## 静态与持久化检查

- `Game.Runtime.dll` 的生成时间晚于 Day 11 相关脚本，当前 Editor.log 未检出 C# 编译错误。
- `MeleeHitboxCenter` 已保存于 Player 下，本地位置为 `(0, 1, 1.2)`。
- `MeleeHitbox` 已保存半径 `0.75` 与 Enemy LayerMask。
- `TestBody` 位于 Enemy Layer，两个 Collider 共享父级 `Health`，可用于多 Collider 去重验证。
- A/B/C 动画均保存 `OpenDamageWindow` 与 `CloseDamageWindow` 事件。

## 延期说明

未完成的测试骨架没有断言，会产生无意义的假通过，因此已删除。当前若保持生产代码封装，需要用反射模拟 Inspector 私有字段；这不是理解伤害窗口与命中去重的必要前置知识。以后只有在学习者掌握 PlayMode 测试夹具，或工程自然出现无需为测试暴露内部字段的配置入口时再补，不阻断 Day 11 和 Day 12。
