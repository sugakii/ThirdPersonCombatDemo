# Day 14 Test Case Report — NavMesh and Idle/Chase

> 计划日期：2026-09-17  
> 验收日期：2026-09-18  
> Unity：6000.5.6f1  
> 场景：`Assets/_Game/Scenes/SampleScene.unity`  
> 复验日期：2026-09-18  
> 结论：PASS（10/10）

## 验收范围

- Environment Layer 与最小 NavMesh 烘焙。
- Puglin `NavMeshAgent`、Idle/Chase 状态与 Animator 切换。
- 仇恨距离、路径清理、NavMesh 边界和目标失效保护。

## 测试结果

| ID | 测试内容 | 预期结果 | 实际结果 | 状态 |
|---|---|---|---|---|
| D14-01 | NavMesh 数据 | 场景存在可用烘焙数据 | 76 个顶点、30 个三角形，`NavMesh-Navigation` 已绑定 | PASS |
| D14-02 | Surface 配置 | 只收集 Environment Layer 的 Physics Colliders | LayerMask=64（Environment），Use Geometry=Physics Colliders | PASS |
| D14-03 | Agent 配置 | Puglin 位于 NavMesh 且参数已保存 | `isOnNavMesh=true`，Speed=3.5、Radius=0.28、Height=1.05 | PASS |
| D14-04 | 范围内追逐 | 6m 内生成有效路径并移动 | `hasPath=true`、Path Complete、Velocity=3.5 | PASS |
| D14-05 | 范围外 Idle | 超出 6m 后停止并清除旧路径 | `hasPath=false`、Velocity=0、RemainingDistance=0 | PASS |
| D14-06 | Animator 配置 | Idle 与 Jog 由 `IsChasing` 切换 | Bool 参数与双向无 Exit Time Transition 已保存 | PASS |
| D14-07 | Player 禁用 | 不报错 | Play Mode 禁用 Player 后 Error=0 | PASS |
| D14-08 | Player 运行中丢失 | 停止追逐且不持续报错 | Agent 停止、路径清除、Velocity=0、Error=0 | PASS |
| D14-09 | 障碍绕行 | 路径能够绕过庭院障碍 | 自动检索到 8 个拐点的 PathComplete 路线 | PASS |
| D14-10 | NavMesh 边缘 | Agent 不离开 NavMesh、不产生路径错误 | 场外点成功投影到边缘，至边缘路径为 PathComplete | PASS |

## 复验结论

- `BUG-014` 已修复：读取位置前检查 Target 是否存在且处于 Active Hierarchy；无效时切回 Idle、停止 Agent、清除路径并提前返回。
- 正常追逐、范围外 Idle、目标禁用、目标销毁、障碍路径、NavMesh 边缘和最终 Console 回归全部通过。
- Day 14 正式验收通过，可以进入 Day 15。

## Unity MCP 证据

- 场景与 NavMesh 数据已保存，场景非 Dirty。
- Puglin 的 Target=Player、Animator=Model、Chase Distance=6。
- 范围内 Agent 有完整路径；范围外 Agent 清除路径并停止。
- Player 禁用与销毁后 Agent 均为 `isStopped=true`、`hasPath=false`、Velocity=0，Console Error=0。
- 障碍路线包含 8 个拐点且为 PathComplete；场外点投影至 NavMesh 边缘后的路径同样为 PathComplete。
