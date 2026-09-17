# Day 13 Test Case Report — Puglin Enemy Prefab

> 计划日期：2026-09-16  
> 首轮验收：2026-09-17  
> Unity：6000.5.6f1  
> 场景：`Assets/_Game/Scenes/SampleScene.unity`  
> 复验：2026-09-17（Unity MCP 编辑态与 Play Mode 实时检查）  
> 结论：PASS（15/15）

## 验收范围

- Puglin 模型、必要纹理、Humanoid Avatar 和动画重定向。
- 独立 URP 材质与 FBX Material Remap。
- Enemy Layer、Capsule Collider、Health=50 和世界空间血条。
- `Puglin.prefab` 内部引用、场景实例及重新拖入后的持久化。

## 测试结果

| ID | 测试内容 | 预期结果 | 实际结果 | 状态 |
|---|---|---|---|---|
| D13-01 | Puglin ModelImporter | Humanoid、有效 Avatar、Bake Axis Conversion=true | 配置已保存 | PASS |
| D13-02 | Idle/Jog/Attack/Hit/Death 重定向 | 无明显变形、异常位移或武器拉伸 | 用户手工验证通过 | PASS |
| D13-03 | Animator Root Motion | 不推动 Enemy 根节点 | `m_ApplyRootMotion=0`，用户回归通过 | PASS |
| D13-04 | 外部材质 Remap | FBX 使用 `MI_Puglin.mat` | Remap GUID 正确 | PASS |
| D13-05 | BaseColor 与 Normal | 显示正常 | 用户手工验证通过，序列化引用正确 | PASS |
| D13-06 | Emissive | 发光贴图产生预期视觉效果 | Unity 实时值：Emission Color=白色、`T_Puglin_Emissive` 已绑定、`_EMISSION` 已启用且资产非 Dirty | PASS |
| D13-07 | Enemy Layer | 根节点及受击 Collider 位于 Enemy Layer | Layer=3（Enemy） | PASS |
| D13-08 | Capsule Collider | 覆盖主体且不包住整根武器 | Center/Radius/Height 已保存，用户碰撞回归通过 | PASS |
| D13-09 | Health 初始值 | 50 HP | `initialMaxHealth=50` | PASS |
| D13-10 | 三段攻击与血条 | 分别扣血、同一刀不重复、UI 同步 | 用户实机回归通过 | PASS |
| D13-11 | 世界空间血条四方向 | 新 Prefab 实例自动取得 Main Camera 并始终朝向屏幕 | Play Mode 中 Billboard 启用；其旋转与 Main Camera 完全一致 | PASS |
| D13-12 | Prefab 内部引用 | 重新拖入后无需手工补场景相机引用 | Awake 使用 `Camera.main` 一次性缓存；脚本已重新编译 | PASS |
| D13-13 | Animator 默认状态 | 新实例默认播放 Idle | Controller 默认 State 已恢复为 `Idle_Loop` | PASS |
| D13-14 | 场景 Prefab 实例 | `SampleScene` 使用正式 `Puglin.prefab` 实例 | 场景已保存正式 Prefab 来源 GUID | PASS |
| D13-15 | Console / Missing Script | 无新增红错、无 Missing Script | Unity Play Mode 无游戏 Error；仅有 Pipeline 非自动化模式提示 | PASS |

## 静态检查证据

- `Puglin.fbx.meta`：`animationType=3`、`avatarSetup=1`、`bakeAxisConversion=1`。
- `Puglin.prefab`：Enemy Layer、Capsule Collider、Health=50、HealthBarPresenter 内部 Health/Slider 引用均已保存。
- `Puglin.prefab` 不保存场景 Main Camera 引用；`WorldSpaceBillboard.Awake` 使用 `Camera.main` 一次性缓存，找不到时记录明确 Error 并禁用自身。`Game.Runtime.dll` 已晚于源码重新生成。
- `PuglinTest.controller`：默认 State fileID 已指向 `Armature|Idle_Loop`。
- `MI_Puglin.mat`：BaseMap、BumpMap、EmissionMap 均有引用；Unity 实时检查 `_EmissionColor=(1,1,1,1)`、`_EMISSION=true`，资产已保存。
- `SampleScene.unity`：正式 `Puglin.prefab` GUID 已保存，场景实例保持 Prefab 连接。

## 复验结论

- Unity 编辑态实时检查确认材质、Animator、Prefab 和场景均已保存且非 Dirty。
- Play Mode 中 Puglin 为 50/50 HP，Billboard 与 Main Camera 旋转一致，未产生游戏 Error。
- Day 13 正式验收通过，可以进入 Day 14。
