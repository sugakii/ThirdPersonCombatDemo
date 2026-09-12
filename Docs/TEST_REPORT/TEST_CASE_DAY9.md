# Day 9 Test Cases — Attack Animation and Configuration Precheck

> 执行日期：2026-09-12  
> Unity：6000.5.6f1  
> 范围：UAL2、三段攻击动画、AttackDefinition、PlayerCombat 骨架与输入意图  
> 结论：复验通过；18 PASS / 0 FAIL

## Test Results

| ID | 检查项 | 预期结果 | 实际结果 | 结果 |
|---|---|---|---|---|
| D9-01 | UAL2 导入类型 | Humanoid / Create From This Model | 符合 | PASS |
| D9-02 | UAL2 Clip 完整性 | 包含 A、B、C 三段剑击 | 43 Clips，三段均存在 | PASS |
| D9-03 | 攻击 Clip 循环配置 | A、B、C 均不循环 | 均为非 Loop | PASS |
| D9-04 | Attack_01 视觉预检 | 无明显骨骼变形、异常位移或反转 | 用户实机确认正常 | PASS |
| D9-05 | Attack_02 视觉预检 | 无明显骨骼变形、异常位移或反转 | 用户实机确认正常 | PASS |
| D9-06 | Attack_03 视觉预检 | 无明显骨骼变形、异常位移或反转 | 用户临时预览确认正常 | PASS |
| D9-07 | Animator State 数量 | Attack_01/02/03 均存在 | 三个 State 均存在 | PASS |
| D9-08 | Attack_01 Motion | `Sword_Regular_A` | 已绑定 | PASS |
| D9-09 | Attack_02 Motion | `Sword_Regular_B` | 已绑定 | PASS |
| D9-10 | Attack_03 Motion | `Sword_Regular_C` | 已绑定 | PASS |
| D9-11 | 配置目录 | 三个配置位于 `Data/Combat` | 符合 | PASS |
| D9-12 | Attack_01 配置 | Damage=10，State=Attack_01 | 符合 | PASS |
| D9-13 | Attack_02 配置 | Damage 可序列化，State=Attack_02 | Damage=10，State=Attack_02 | PASS |
| D9-14 | Attack_03 配置 | Damage 可序列化，State=Attack_03 | Damage=10，State=Attack_03 | PASS |
| D9-15 | Attack 输入意图 | InputReader 用单帧按下语义输出 AttackPressed | `WasPressedThisFrame()` 已接入 | PASS |
| D9-16 | Day 9 脚本静态检查 | 无编译错误 | 0 Error；仅同对象 GetComponent 通用提示 | PASS |
| D9-17 | PlayerCombat 范围控制 | 仅保留骨架，不提前实现 Combo | 未挂载、未实现 Combo，符合 Day 9 边界 | PASS |
| D9-18 | Art 目录职责 | Player Art 保持在 `_Game/Art` | 已恢复；错误目录不存在 | PASS |

## Console

- Gameplay Error：0
- 外部编辑器服务 Warning：1（Unity Pipeline 非自动化模式，不属于 Gameplay 缺陷）

## Acceptance Decision

Day 9 复验通过。`BUG-007`、`BUG-008`、`BUG-009` 已关闭；Player Prefab 的 Imp、Animator、Avatar、Controller 引用有效，Missing Script=0。可以按 Roadmap 进入下一学习日。
