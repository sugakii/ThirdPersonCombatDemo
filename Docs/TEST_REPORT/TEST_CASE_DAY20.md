# Day 20 Test Report

> 日期：2026-09-27  
> 范围：MainMenu、PauseMenu、GameFlow 冲突、场景切换、庭院 Props 与 NavMesh  
> 实机复核：Unity MCP 连接当前工程

## 验收结论

Day 20 **PASS**。首轮发现的 Prop 碰撞缺失和 `OnDisable` 拼写错误均已修复并复验；MainMenu、Pause/Resume、Restart、ReturnToMainMenu、Pause×GameOver、3/3 Enemy NavMesh、Dash 撞 Wagon 和三轮完整流程全部通过。Gameplay Console `0 Error`。

## 功能与集成用例

| ID | 测试步骤 | 预期结果 | 实际结果 | 状态 |
|---|---|---|---|---|
| D20-01 | 读取 Build Settings | MainMenu=0，SampleScene=1，均启用 | 配置正确 | PASS |
| D20-02 | 检查 MainMenu 按钮 | Start→StartGame，Quit→QuitGame | 两个 Persistent Listener 正确 | PASS |
| D20-03 | 从 MainMenu 调用 StartGame | 进入 SampleScene | Active Scene 变为 SampleScene | PASS |
| D20-04 | Playing 状态调用 PauseGame | timeScale=0，PausePanel 显示，Camera/Combat/Skill 停止 | 状态全部符合 | PASS |
| D20-05 | 调用 ResumeGame | timeScale=1，PausePanel 隐藏，三个控制组件恢复 | 状态全部符合 | PASS |
| D20-06 | 暂停期间令 Player 死亡 | GameOver 接管，暂停面板关闭，timeScale 恢复 | GameOver 显示且 PausePanel 隐藏 | PASS |
| D20-07 | 检查 Gameplay 菜单按钮 | Resume、Restart、ReturnToMainMenu 均有正确回调 | 7 个按钮绑定均有效 | PASS |
| D20-08 | 暂停后返回主菜单 | 恢复 timeScale/鼠标并进入 MainMenu | MainMenu 正常加载，timeScale=1 | PASS |
| D20-09 | 计算 3 个 Puglin 到 Player 的 NavMesh 路径 | 3/3 PathComplete | 3/3 PathComplete | PASS |
| D20-10 | 检查新增 Wagon/Crate 的物理碰撞 | 可达装饰不能被 Player 穿透 | 13/13 均有有效非 Trigger BoxCollider；Dash 未穿过 Wagon | PASS |
| D20-11 | 检查 Console | 无 Gameplay Error | 0 Gameplay Error；仅 Unity Pipeline 自动化提示 | PASS |
| D20-12 | 审查并运行 PlayerMotor 禁用清理 | Unity 在 OnDisable 调用时清零速度 | 已修正为 `OnDisable()`；禁用后速度为 0 | PASS |
| D20-13 | 连续完成三轮完整流程 | 每轮均可从菜单进入、战斗、结算、重开/返回 | Victory→Restart、GameOver→MainMenu、MainMenu→Pause/Resume→Victory→MainMenu | PASS |
| D20-14 | 回归 Day 20 受影响范围 | 菜单、流程、暂停、Dash 碰撞、NavMesh、场景保存正常 | Unity MCP 定向复验全部通过 | PASS |

## 修复回归

1. `PlayerMotor.OnDisable()` 已恢复为有效 Unity 生命周期函数。
2. 13/13 Wagon/Crate 已配置简化 BoxCollider；均为 enabled、非 Trigger 且世界尺寸有效。
3. Dash 正面撞向 Wagon 后停在碰撞体前，未穿透（signed distance `-1.706364`）。
4. 三个 Puglin 到 Player 的路径仍为 `PathComplete`。
5. SampleScene 已保存且 `isDirty=false`；多角度检查确认战斗中心保持开阔。

