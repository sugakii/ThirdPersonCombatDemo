# Day 10 Test Cases — Combo Runtime

> 执行日期：2026-09-13  
> Unity：6000.5.6f1  
> 范围：三段 Combo、输入缓存、Recovery Cancel、攻击朝向  
> 结论：16 PASS / 0 FAIL / 0 NOT RUN

## Scope Note

Day 10 只验收 Combo Runtime。伤害窗口、`MeleeHitbox` 和命中去重顺延到 Day 11，不计入本日失败项。

## Test Results

| ID | 检查项 | 预期结果 | 实际结果 | 结果 |
|---|---|---|---|---|
| D10-01 | Day 10 脚本编译 | 无编译错误 | Unity 静态验证通过 | PASS |
| D10-02 | PlayerCombat 场景挂载 | 组件存在并启用 | 已挂载到 Player | PASS |
| D10-03 | Animator 引用 | 指向 Imp Animator | 已显式绑定 | PASS |
| D10-04 | AttackDefinition 顺序 | 01、02、03 共三个配置 | 数组长度为 3，顺序正确 | PASS |
| D10-05 | Combo Runtime 状态归属 | 索引、缓存和窗口保存在 PlayerCombat | 符合 | PASS |
| D10-06 | 动画事件桥接 | Imp 组件只转发，不保存 Combo 规则 | 符合 | PASS |
| D10-07 | 攻击 State | A/B/C Motion 均正确 | 3/3 正确 | PASS |
| D10-08 | Recovery State | A/B 收招 State 与 Motion 正确 | 2/2 正确 | PASS |
| D10-09 | Animation Event 配置 | A/B 控制输入、推进、收招；Recovery/C 结束攻击 | 配置符合 | PASS |
| D10-10 | 单击完整流程 | A → A Recovery → Locomotion，之后可再次攻击 | 用户实机确认 | PASS |
| D10-11 | 提前输入缓存 | 输入被缓存，到推进节点才进入 B | 用户实机确认 | PASS |
| D10-12 | Recovery Cancel | A Recovery 可接 B，B Recovery 可接 C | 用户实机确认 | PASS |
| D10-13 | 三段 Combo | A → B → C → Locomotion，无第四段 | 用户实机确认 | PASS |
| D10-14 | 疯狂连点 | 不加速、不跳段、不永久卡死 | 用户实机确认 | PASS |
| D10-15 | 攻击朝向 | 首段和后续段按当前镜头水平朝向瞬间转向 | 用户实机确认 | PASS |
| D10-16 | 场景与 Console | 场景已保存，无 Gameplay Error | isDirty=false；0 Error | PASS |

## Acceptance Decision

Day 10 正式通过。Combo 规则由 PlayerCombat 管理，Animator/Animation Event 只负责表现与时机。下一学习日再接入伤害窗口与命中去重。
