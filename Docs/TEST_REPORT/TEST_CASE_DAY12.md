# Day 12 Test Case Report — Medieval Courtyard Blockout

> 计划日期：2026-09-15  
> 验收日期：2026-09-16  
> Unity：6000.5.6f1  
> 场景：`Assets/_Game/Scenes/SampleScene.unity`  
> 结论：PASS；庭院主体已持久化，功能、碰撞与镜头手工回归通过

## 验收范围

- 约 22×22m 中世纪庭院主体与第三人称战斗动线。
- 地面、四面墙、墙角、门洞/门框/门、楼梯与平台。
- 视觉模型与简化 Gameplay Collider 分离。
- CharacterController 通行、边界和 Cinemachine 镜头回归。

## 测试结果

| ID | 测试内容 | 预期结果 | 实际结果 | 状态 |
|---|---|---|---|---|
| D12-01 | 四面墙正面碰撞 | Player 被阻挡且不能穿墙 | 阻挡正常 | PASS |
| D12-02 | 贴着四个墙角移动 | 不穿透、不永久卡死 | 行为正常 | PASS |
| D12-03 | 庭院内外双向穿过门洞 | 门洞可通行，墙体和门框仍阻挡 | 双向通行正常 | PASS |
| D12-04 | Sprint 穿过门洞 | 不被隐藏空气墙阻挡 | 通行正常 | PASS |
| D12-05 | 普通移动上、下楼 | 沿隐藏斜坡稳定通行 | 无明显抖动或卡住 | PASS |
| D12-06 | Sprint 上楼 | 高速状态仍可稳定到达平台 | 通过 | PASS |
| D12-07 | 平台上移动与 Sprint | 不抖动、不下陷 | 行为正常 | PASS |
| D12-08 | 从侧面撞向平台 | 被平台 Collider 阻挡 | 无穿透 | PASS |
| D12-09 | 走到平台边缘 | CharacterController 边缘行为稳定 | 行为正常 | PASS |
| D12-10 | 镜头靠近墙和墙角 | 无阻断操作的严重穿墙或异常跳动 | 可接受 | PASS |
| D12-11 | 镜头靠近楼梯和平台 | 跟随与旋转保持可用 | 行为正常 | PASS |
| D12-12 | 全场斜向移动与基础碰撞回归 | 移动速度、碰撞和动画无回归 | 通过 | PASS |
| D12-13 | Play Mode Console | 无新增红色错误或持续异常 | 无新增红错 | PASS |

## 静态与持久化检查

- 场景保存 `Environment_Courtyard`，并包含 `NorthWall`、`SouthWall`、`EastWall`、`WestWall` 与 `RaisedArea` 等结构节点。
- 实际场景引用包括 121 个 `Floor_Brick`、43 个 `Wall_Plaster_Straight`、1 个门洞墙、1 个门框、1 个门、4 个外墙转角、楼梯和平台模块。
- 场景保存 15 个 Box Collider；楼梯使用隐藏斜坡碰撞，未给所有视觉 Mesh 添加复杂 Mesh Collider。
- 正式导入 8 个结构 FBX；未导入 OBJ/glTF 重复格式。
- 今日未修改玩法脚本，不需要新增代码注释或自动化测试。

## 范围说明

- Day 12 只验收可供战斗、NavMesh 和镜头测试使用的庭院主体。
- 木箱、马车、木栅栏、统一材质与灯光属于 Day 23 润色范围，不阻断本日验收。
- `MeleeHitboxPlayModeTests.cs` 仍按 Day 11 决定延期；无断言的空测试骨架已删除，不计入自动化证据。
