# Day 18 Test Case Report — Sword Dash Movement / Damage

> 计划日期：2026-09-21  
> 完成与验收日期：2026-09-22  
> Unity：6000.5.6f1  
> 结论：PASS

## 验收范围

- `Sword_Dash` 动画与代码受控位移。
- CharacterController 障碍、斜坡和平台边缘行为。
- Dash 重复输入、冷却和结束恢复。
- 单目标、多目标与同一目标多 Collider 去重。
- `BUG-004` 受限空中控制。
- EditMode / PlayMode 自动化、编译、场景持久化和 Console。

## 测试结果

| ID | 测试内容 | 预期结果 | 实际结果 | 状态 |
|---|---|---|---|---|
| D18-01 | 技能配置与动画 | E 成功释放后播放 Sword_Dash，参数来自 FireDash | Damage=50、CD=3s、Distance=1.5m、Duration=0.5s；动画 Motion 正确 | PASS |
| D18-02 | 代码受控位移 | SkillController 不直接修改 Transform，位移由 PlayerMotor 执行 | `TryStartDash` 请求由 CharacterController.Move 执行 | PASS |
| D18-03 | 帧率无关的结束距离 | 最后一帧不应因完整 deltaTime 超出配置水平距离 | 使用 `Min(deltaTime, remainingDashTime)` 截断最后一步 | PASS |
| D18-04 | 贴墙与墙角 Dash | CharacterController 阻挡角色，不穿过碰撞体 | 用户手工回归通过 | PASS |
| D18-05 | 30° / 50°斜坡 | Dash 不产生异常穿透、卡死或失控 | 用户手工回归通过 | PASS |
| D18-06 | 平台边缘 | 离开平台后继续受重力影响，普通空中操控受限 | 用户手工回归通过；airControlMultiplier=0.25 | PASS |
| D18-07 | Dash 中重复按 E | 当前 Dash 不被新请求覆盖 | 重复输入被拒绝 | PASS |
| D18-08 | 冷却期间按 E | 不重复释放，不覆盖剩余冷却 | 手工与自动化回归通过 | PASS |
| D18-09 | Dash 结束恢复 | Dash 和冷却结束后可以再次释放 | PlayMode 测试通过 | PASS |
| D18-10 | 单目标伤害 | 一次 Dash 对目标结算一次配置伤害 | 用户手工回归通过 | PASS |
| D18-11 | 同目标多 Collider | 多个 Collider 指向同一 IDamageable 时只伤害一次 | SkillHitDetector PlayMode 测试通过 | PASS |
| D18-12 | 多目标伤害 | 不同 IDamageable 各结算一次 | 用户手工回归通过 | PASS |
| D18-13 | 自动化总回归 | EditMode 与 PlayMode 全绿 | EditMode 13/13；PlayMode 4/4 | PASS |
| D18-14 | 场景与 Console | 正式出生位置、独立技能中心、无临时平台和 Error | SkillHitboxCenter 已绑定；临时 Cube 已删；Scene dirty=false；0 Error | PASS |

## 自动化证据

```text
EditMode
13 PASS / 0 FAIL / 0 SKIP

PlayMode
PlayerMotorPlayModeTests                 3 PASS
SkillHitDetectorPlayModeTests            1 PASS
Total                                    4 PASS / 0 FAIL / 0 SKIP
```

## 结论

- Day 18 合并完成五日冲刺第 1 天的 Dash 位移与伤害范围。
- `PlayerMotor` 保持唯一位移职责；伤害检测继续面向 `IDamageable`。
- 普攻与技能使用独立检测中心，范围可分别调试。
- Day 19 进入 VFX、Cooldown UI 与 GameFlow，不再扩展 Dash 规则。
