# Current Project Status

> Last Updated：2026-09-04
> 本次验收归属：Learning Day 2（2026-09-03），按用户要求计入该日成果
> 下一执行日：Learning Day 3（2026-09-04）
> Current Phase：Week 1 / Day 2 部分完成，待补齐
> Source of Truth：当前 Unity 工程 + Git + Docs

## Status Summary

本次读取的是 9/4 的当前磁盘与 Unity Editor 快照，不是对 9/3 历史提交的还原。按用户要求，将本次成果归入 9/3。
工程已从空模板进入 Imp 导入、输入读取与镜头原型阶段，但尚未完成 Day 2 的动画播放验证、材质持久化和庭院 Blockout。不能判定 Day 2 全部通过。

## 验收结果

| 项目 | 证据 | 结论 |
|---|---|---|
| 工程与 MCP | Unity 6000.5.6f1；只读诊断命令编译执行成功；检查时非 Play、非 compiling | 本次检查通过；不等于完整 Build 通过 |
| Console | 检查时 0 Error、2 Warning，均为 Pipeline/签名工具提示 | 未发现当前错误；未执行完整 Play 回归 |
| Imp 导入 | FBX、红色 BaseColor、Normal、Emissive、ORM 已存在 | 导入完成 |
| Imp Avatar | Human、Bake Axis Conversion=true；ImpAvatar valid=true、human=true | 导入配置通过；动作变形尚待测试 |
| UAL1 | 已导入非 RM FBX，Human Avatar 有效，共 43 条 Clip | 导入完成；只有 Idle_Loop 已确认循环开启 |
| 动画播放链 | Player Animator 有 Controller 无 Avatar；Imp Animator 有 Avatar 无 Controller且 Root Motion=true；Controller Base Layer 为 0 状态 | 未完成，不可验收为动画播放成功 |
| 材质 | 场景渲染器使用 MI_Imp，URP/Lit，引用红色纹理；材质路径仍指向 FBX，无独立 .mat | 部分配置；持久化及各贴图效果待验证 |
| 镜头与输入 | PlayerInput → PlayerInputReader（LookInput/MoveInput）→ CameraController → CameraTarget；引用已连接 | 代码与配置完成；鼠标、俯仰、焦点回归未执行 |
| 移动/场地 | 有 CharacterController，无 PlayerMotor，无地面/庭院/楼梯，无 Player Prefab | 未完成 |
| 自动化 | 3 份 asmdef 已建立；无测试脚本或执行报告 | 未执行，不能标记通过 |

## Current Architecture

- 场景：`Assets/_Game/Scenes/SampleScene.unity`。
- Main Camera 挂 CinemachineBrain；CinemachineCamera 挂 ThirdPersonFollow、RotationComposer。
- Player 挂 CharacterController、Animator、PlayerInput、PlayerInputReader、CameraController。
- Player/Imp 的 local Y rotation=180；Player/CameraTarget local position=(0,1.5,0)。
- CameraController 的 cameraTarget 已绑定，Inspector 灵敏度为 0.3。
- PlayerInput 使用 InputSystem_Actions，Default Action Map=Player。
- PlayerInputReader 只实现 Look/Move 读取；还没有完整 Sprint/Attack/Skill/Restart 意图接口。
- 两个脚本当前均位于 Runtime/Input；CameraController 在 Day 5 移到 Runtime/Camera，保留 .meta GUID。
- Game.Runtime 已增加 Input System 程序集引用；测试程序集仍为空。
- Health、Combat、Enemy、Skill、UI、GameFlow 尚未实现。

## Files

- `Assets/_Game/Runtime/Input/PlayerInputReader.cs`
- `Assets/_Game/Runtime/Input/CameraController.cs`
- `Assets/_Game/Runtime/Game.Runtime.asmdef`
- `Assets/_Game/Scenes/SampleScene.unity`
- `Assets/_Game/Animations/Player/PlayerAnimator.controller`
- `Assets/_Game/Animations/Source/UAL1_Standard.fbx`
- `Assets/_Game/Art/Charactors/Player/Model/Imp.fbx`
- `Assets/_Game/Art/Charactors/Player/Textures/`
- `Assets/InputSystem_Actions.inputactions`
- `Assets/_Game/Tests/EditMode/Game.Tests.EditMode.asmdef`
- `Assets/_Game/Tests/PlayMode/Game.Tests.PlayMode.asmdef`

## Known Issues / 待验证风险

1. 动画播放链尚未完成，不能因 Avatar 有效就声称动画正常；先确定唯一动画驱动组件，再接 Idle 并关闭其 Root Motion。
2. Walk/Jog/Sprint 的 Loop 当前为 false；UAL1 Bake Axis Conversion=false，需按导入说明与实际预览确认，不直接判作损坏。
3. CharacterController center=(0,0,0)、height=2、radius=0.5，胶囊跨过原点上下各 1m；需对齐脚底与真实模型尺寸后测碰撞。
4. CameraController 缺少依赖/目标为空的保护；旋转角初始为 0，非零初始镜头角可能跳变。属于代码风险，尚未运行复现。
5. InputReader 与 CameraController 都在 Update，未显式约定采样与消费顺序；焦点切换、禁用重启、手柄 Look 行为未验证。
6. 当前 Scene dirty=true，部分 Editor 状态尚未保存，Git 无法保存未落盘场景。
7. Bestiary 导入目录尚未被 Git ignore 覆盖；Imp FBX/PNG 当前未跟踪，也未被忽略。禁止直接 add 全工程；首次提交原始资产前先落实排除和素材还原说明。
8. “旧 Input.GetAxis 与新 Input System 冲突”的旧代码已移除；这里只确认源码修正，完整运行回归仍待执行。

## Git

- 当前分支：main；检查时 HEAD：`a387ac2`（素材文件树文档提交）。
- 存在未提交脚本、场景、程序集与文档修改，以及未跟踪素材/动画文件。
- 本次只更新四份 Docs，不自动保存场景、不修改 Gameplay、不暂存或提交。
- 既有 .gitignore 忽略 MaterialPackage 和 Unity 生成目录，但不覆盖 Assets 内的 Bestiary。

## Next Task

**9/4 / Learning Day 3：先补 Day 2 最小依赖，不重新做已完成的输入读取。总计 3–4 小时。**

1. 开发学习约 45 分钟：整理 Animator 播放链、接 Idle、关闭 Root Motion；验证有效 Avatar 在动画下的表现。
2. 开发学习约 45 分钟：保存共享 URP 材质；以基础体搭 20×20m 地面和一面墙，校准 CharacterController。先不做装饰庭院。
3. 开发学习约 60 分钟：复用 PlayerInputReader.MoveInput，学习并实现最小 PlayerMotor（移动、斜向限速、重力）；未完成则保留明确待办，不跳过测试。
4. 测试约 45–60 分钟：Idle、镜头左右/俯仰边界、墙体与落地、Console；保存场景，检查素材排除策略并回传实际结果。

后续安排见 ROADMAP。3 小时版本缩减开发任务，保留测试时间；本次没有提前实现以上任务。

## Update Rules

- 只记录最新事实，区分已实现、已配置、已测试。
- 当前实现以本地文件和 Editor 为准；未保存场景、未提交文件应明确标注。
- Day 结束后由验收任务复核；学习会话负责步骤与知识，Bug 会话负责定位修复。
- 教学遵循 ROADMAP 的“教学代码披露规则”：先 API 与验收条件，再独立练习、提示与必要的完整实现。
