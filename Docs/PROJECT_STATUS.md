# Current Project Status

> Last Updated：2026-09-16
> Current Learning Day：Day 12 已完成
> Current Phase：Phase A / 中世纪庭院主体完成
> Next Checkpoint：Learning Day 13 — Puglin 导入与 Enemy Prefab
> Source of Truth：当前 Unity 工程 + Git + Docs
> Remaining Plan：`Docs/plans/2026-09-13-remaining-learning-days.md`

## 验收结论

**Day 12：PASS。** 约 22×22m 中世纪庭院主体已保存；地面、四面墙、门洞/门框/门、四个墙角、楼梯、平台和必要简化碰撞完成。门洞、墙角、楼梯、平台、边缘、镜头及全场基础碰撞手工回归通过，Console 无新增红错。Props、统一材质与灯光留到 Day 23，不阻断本日交付。

## Implemented

- 从 Medieval Village 包选择性导入 8 个结构 FBX，没有导入 OBJ/glTF 重复格式。
- `Environment_Courtyard` 使用 2m `Floor_Brick` 模块形成约 22×22m 地面，并完成四面外围墙。
- 场景保存门洞墙、圆门框、圆门和四个外墙转角。
- 楼梯使用视觉模型 + 隐藏斜坡 Box Collider，平台使用简化 Box Collider。
- 地面、墙体、门口与平台碰撞已按第三人称 CharacterController 动线校准。
- 今日没有新增玩法脚本；代码注释检查无新增项。

## Test Evidence

| 范围 | 结果 |
|---|---|
| 场景结构持久化 | PASS：庭院根节点、四面墙、门口、RaisedArea、楼梯与平台均已保存 |
| 正式环境引用 | PASS：121 地面模块、43 直墙、门洞墙/门框/门、4 转角及楼梯平台已保存 |
| 简化碰撞 | PASS：15 个 Box Collider；未给全部视觉 Mesh 使用复杂 Mesh Collider |
| 墙体、墙角和门洞 | PASS（用户实机） |
| 楼梯、平台和边缘 | PASS（用户实机） |
| 镜头靠墙/墙角/平台 | PASS（用户实机） |
| 全场移动与碰撞回归 | PASS（用户实机） |
| Console | PASS：用户确认无新增红色错误；当前 Editor.log 未检出近期异常 |
| MeleeHitbox PlayMode 去重测试 | DEFERRED：空骨架已再次删除，不计为自动化证据 |
| Day 12 结论 | PASS：13 项手工/静态验收通过 |

详细用例见 `Docs/TEST_REPORT/TEST_CASE_DAY12.md`。

## Current Architecture

```text
UAL2 Animation Events
        ↓
PlayerCombatAnimationEvents（只转发）
        ↓
PlayerCombat（读取当前 AttackDefinition.Damage）
        ↓ DamageInfo
MeleeHitbox
├─ OverlapSphere + Enemy LayerMask
├─ Vector3.Dot 前半球过滤
├─ HashSet<IDamageable> 单窗口去重
└─ IDamageable.TakeDamage
        ↓
Health
```

- Animator 只提供伤害窗口时机；命中检测、伤害配置和生命规则仍彼此分离。
- PlayerCombat 不依赖具体 Enemy；MeleeHitbox 只面向 `IDamageable`。
- 当前物理查询使用 `OverlapSphere`，短窗口内会分配数组；是否改 NonAlloc 留到 Profiler 日依据数据决定。

## Files

- `Assets/_Game/Art/Environment/MedievalVillage/Models/`（8 个结构 FBX 及 `.meta`）
- `Assets/_Game/Scenes/SampleScene.unity`
- `Docs/ASSET_AUDIT.md`
- `Docs/ROADMAP.md`
- `Docs/PROJECT_STATUS.md`
- `Docs/TEST_REPORT/TEST_CASE_DAY12.md`

## Known Bugs / Risks

1. `BUG-004` Open：角色离地后仍保留完整水平控制速度；进入技能位移前处理。
2. `MeleeHitbox` PlayMode 测试延期且不计为自动化证据；空骨架不得保留以免假通过。
3. Day 12 环境 FBX 当前未导入统一纹理/材质；视觉统一、灯光和 Props 留到 Day 23。
4. Day 12 场景直接复用模型 Prefab，尚未建立额外 Environment Prefab；只有出现稳定复用配置时再提取，避免为目录而建空壳。
5. `Assets/_Recovery/` 是恢复文件，不纳入正式项目提交。

## Git

- 当前分支：`main`；提交前以 `git rev-parse --short HEAD` 复核实际 HEAD。
- Day 12 已通过，可以创建正式完成提交。
- 默认提交全部自有代码、对应 `.meta`、配置资产、Scene、测试与 Docs。
- 排除 Unity Assistant Settings、SceneTemplateSettings、`Assets/_Recovery/`、空 Debug 目录及未经确认的 ProjectSettings 变化。
- Bestiary 原始 FBX/PNG 不进入公开仓库。

## Next Task

1. Learning Day 13 选择性导入 `Puglin.fbx` 与必要纹理，验证 Humanoid Avatar 和动画重定向。
2. 创建 `MI_Puglin.mat` 与 `Puglin.prefab`，保存 Collider、Health 和世界空间血条引用。
3. 先只建立 1 个可复用 Enemy Prefab；3 个实例留到 AI/Combat 集成日。
4. 执行材质、比例、Idle/Jog/Attack/Hit/Death、Root Motion 与 Prefab 持久化回归。

## Update Rules

- 未运行写 NOT RUN；测试名、步骤、断言和结果必须一致。
- 收到“验收”请求时，先扫描并补齐本次涉及代码的必要注释，再执行验证、更新 Docs，并在可提交时给出包含全部自有代码的 Git 指令。
- 注释解释职责、关键 API、边界和原因，不逐行复述代码。
- 每次验收结束后，根据当天实际内容提出 3–5 个理解题；回答情况用于安排后续教学，不篡改工程验收结果。
- Day 结束后同步工程、Git 和 Docs；已验收架构默认冻结。
