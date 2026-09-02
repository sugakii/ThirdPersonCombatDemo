# Current Project Status

> Last Updated：2026-09-02  
> Current Learning Day：Learning Day 1（2026-09-02）
> Current Phase：Week 1 / Learning Day 1 已验收
> Source of Truth：Unity 工程 + Git + Docs

## Status Summary

Learning Day 1 的工程基线任务已通过验收：Unity 可正常编译，Git、`.gitignore`、Docs、目标目录、三份 asmdef 与 Cinemachine 均已落地，Unity MCP 已连接当前 Quaternius Editor。**Gameplay 尚未开始实现。** 下一任务进入 Learning Day 2 的选择性素材导入与 Unity 内验证。

## Implemented

- Unity `6000.5.6f1 (0e0577a1a2ac)` URP Empty Template 已创建。
- `ProjectSettings/ProjectVersion.txt` 已存在并记录当前编辑器版本。
- `SampleScene` 已在 Build Settings 中；当前只有 Main Camera、Directional Light、Global Volume。
- 已安装并锁定：
  - Input System `1.20.0`
  - AI Navigation `2.0.14`
  - Test Framework `1.7.0`
  - URP `17.5.0`
  - Pipeline `0.5.0-exp.1`
  - AI Inference `2.6.1`
  - Unity Assistant / MCP Bridge `2.18.0-pre.2`
- 默认 `InputSystem_Actions.inputactions` 已注册为全局 Action Asset。
- 素材源文件位于 `MaterialPackage/`，且已完成静态文件审计。
- 已创建项目计划与四份跨会话基线文档。
- 2026-09-02 已在运行中的 Quaternius Editor 完成 Package resolve；Assistant/MCP 包已进入 `packages-lock.json`。
- Codex `unityMCP` 配置已从 Tiny Swords 切换到 `D:\Unity Project\Quaternius`。
- Unity MCP Named Pipe 握手成功：`unity-mcp 2.0`，发现 54 个 Unity 工具，服务状态为 `connected=true`。
- Git `main` 当前工作区干净；基线提交 `fd3b1f6`，Cinemachine 提交 `fc2eb58`。
- Unity `.gitignore` 已排除生成目录、Build、IDE 文件、`MaterialPackage/` 等非仓库内容。
- 已建立 `Game.Runtime`、`Game.Tests.EditMode`、`Game.Tests.PlayMode` 三份 asmdef。
- Cinemachine `3.1.7` 已安装并写入 Package lock。
- Learning Day 1 验收时 Unity Console 为 0 Error、0 Warning。

## Not Implemented

- 尚未把选定 Quaternius 素材导入 `Assets/`。
- 尚未生成或验证 Imp/Puglin 的 Unity Humanoid Avatar。
- 没有 Player/Enemy Prefab、Material、Animator Controller 或正式场景。
- 目标目录与测试目录已建立，但尚未填入 Gameplay 内容。
- Gameplay 与 Tests asmdef 已建立，但尚未编写自动化测试。
- 没有 `PlayerInputReader`、`PlayerMotor`、CharacterController 角色或第三人称镜头。
- 没有 Combat、Health、Enemy AI、Skill、UI、GameFlow 或 VFX 实现。
- 没有 NavMesh 烘焙数据。
- 没有 Windows Build、README、测试报告、Bug Report 或演示视频。

## Current Architecture

```text
InputSystem_Actions.inputactions
└─ 已注册，但没有 Runtime consumer

SampleScene
├─ Main Camera
├─ Directional Light
└─ Global Volume

Assembly-CSharp
└─ Unity 模板 Readme.cs

Game.Runtime
└─ Gameplay Runtime（当前为空）

Game.Tests.EditMode
└─ 引用 Game.Runtime（当前无测试）

Game.Tests.PlayMode
└─ 引用 Game.Runtime（当前无测试）
```

目标架构尚未落地，详见 [`PROJECT_ARCHITECTURE.md`](PROJECT_ARCHITECTURE.md)。

## Current Files

- `Assets/Scenes/SampleScene.unity`
- `Assets/InputSystem_Actions.inputactions`
- `Assets/_Game/Runtime/Game.Runtime.asmdef`
- `Assets/_Game/Tests/EditMode/Game.Tests.EditMode.asmdef`
- `Assets/_Game/Tests/PlayMode/Game.Tests.PlayMode.asmdef`
- `Assets/TutorialInfo/Scripts/Readme.cs`
- `Assets/TutorialInfo/Scripts/Editor/ReadmeEditor.cs`
- `Packages/manifest.json`
- `Packages/packages-lock.json`
- `ProjectSettings/ProjectVersion.txt`
- `MaterialPackage/`（6 个素材包；位于 Unity Assets 外）
- `docs/plans/2026-09-02-unity-3d-internship-four-week-plan.md`
- `docs/ASSET_AUDIT.md`
- `docs/PROJECT_ARCHITECTURE.md`
- `docs/ROADMAP.md`
- `docs/PROJECT_STATUS.md`

## Input Status

当前 Player Action Map 已有：Move、Look、Attack、Interact、Crouch、Jump、Previous、Next、Sprint。

- 已有计划所需绑定：WASD、鼠标 Look、左键 Attack、Left Shift Sprint。
- 缺少：Skill(Q)、Restart(R)。
- `generateWrapperCode` 当前为 `0`，尚未生成 C# wrapper。

## Package Status

- Cinemachine `3.1.7`：已安装并完成 Package resolve。
- `com.unity.ai.assistant 2.18.0-pre.2`：已安装，作为 Unity MCP Editor Bridge 的开发工具依赖。
- 历史日志曾记录 Package Manager `ECONNRESET`；之后 Pipeline 已成功解析并参与编译，因此不能把它写成当前已确认阻塞。
- Unity 当前正以 Quaternius 项目运行；包解析和 MCP 连接验证已完成。

## Tooling Integration

- Codex 配置：`C:\Users\33309\.codex\config.toml`。
- MCP 命令：`C:\Users\33309\.unity\relay\relay_win.exe --mcp --project-path "D:\Unity Project\Quaternius"`。
- Unity Editor Bridge：由 `com.unity.ai.assistant` 启动，使用本机 Named Pipe 连接。
- 连接授权：已在 Unity 的 New MCP Connection 窗口允许 Codex 访问当前项目。
- 验证结果：当前 Codex 任务已实际注册 54 个 `unityMCP` 工具；已通过 `Unity_GetProjectData` 读取到 Quaternius 的 `Assets/_Game`、`Assets/ThirdParty/Quaternius` 和 URP Settings 目录。
- Unity Console 最新联机检查结果：`0 Error / 0 Warning`。

## Known Bugs

- 暂无可确认的 Gameplay Bug：Gameplay 尚不存在，不能把未执行测试写成“通过”。
- Tooling Warning：Unity 无法解析 Codex Store 可执行文件的 Windows 签名，因而首次连接显示“unsigned or not recognized”；用户授权后连接与 54 个工具均工作正常。

## Risks / Blockers

- Skill 与 Restart Input Action 尚未添加。
- `MaterialPackage/` 位于 `Assets/` 外；Avatar、材质、朝向和动画重定向尚未在 Unity 中验证。
- Bestiary 使用的 QAL Standard 许可不允许公开重新分发原始资产，Git 导入策略必须先确定。

## Next Task

**Learning Day 2（2026-09-03）：选择性素材导入与 Unity 内验证。**

1. 严格按 `ASSET_AUDIT.md` 复制 Imp、动画和庭院所需文件，不整包导入。
2. 配置红色 Imp 的 URP 材质、Humanoid Rig 与 Avatar。
3. 在 Unity 中预览 Idle/Walk/Jog/Sprint 与选定战斗动画，记录重定向、朝向和循环属性。
4. 搭建约 20×20m 庭院 Blockout，并配置基础碰撞。
5. 将 Unity 内实测结果同步回 `ASSET_AUDIT.md` 与本文件。

## Update Rules

- 本文件始终只保留**当前最新版状态**，不得追加成开发日记。
- 每次会话结束前根据工程事实覆盖更新 Last Updated、Implemented、Files、Known Bugs 和 Next Task。
- “已计划”“已创建文件”“已在 Unity 中验证”必须严格区分。
- Git 初始化后补充当前分支和 commit SHA；历史过程由 Git 保存。
- 聊天内容只能解释或评审本文件，不能成为另一份项目状态。
