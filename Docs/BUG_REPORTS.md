# Bug Reports

> 项目：Third-Person Combat Demo  
> 维护规则：只记录实际复现的缺陷；修复后必须回归，不能仅凭代码修改关闭。
> 最近回归：2026-09-08 / Day 6；新增并关闭 PlayMode 初始化时序缺陷，另保留一项空中控制问题。

## 状态定义

| 状态 | 含义 |
|---|---|
| Open | 已确认，尚未修复 |
| Resolved | 已修改，等待或正在回归 |
| Closed | 修复后回归通过 |
| Cannot Reproduce | 按既定步骤无法再次复现，仍保留记录 |

## 严重程度

| 等级 | 判断标准 |
|---|---|
| Blocker | 无法启动、无法继续主要流程或数据严重损坏 |
| Critical | 核心功能失效、崩溃或存在严重错误结果 |
| Major | 主要功能明显异常，但存在绕过方式 |
| Minor | 不阻断主要流程的局部问题 |
| Trivial | 轻微表现或文本问题 |

---

## BUG-001：Imp 使用错误 Avatar 配置时腿脚明显扭曲

### 基本信息

- 发现日期：2026-09-04
- 首次发现版本：Day 3 开发阶段，准确 Commit 未记录
- 当前验证版本：`b051871` 之后的 Day 4 工作树
- Unity：`6000.5.6f1`
- 场景：`Assets/_Game/Scenes/SampleScene.unity`
- 状态：Closed
- 严重程度：Major
- 优先级：High
- 复现率：首次排查时可稳定复现，未记录精确次数

### 前置条件

- Imp 的 Rig 设置为 Humanoid。
- PlayerAnimator 播放 UAL1 的 `Armature|Idle_Loop`。
- Animator 使用发生问题时的 Avatar 配置。

### 复现步骤

1. 打开 `SampleScene`。
2. 为 Imp 配置 UAL1 的 `Idle_Loop`。
3. 进入 Play Mode。
4. 观察双腿与脚部。
5. 打开 Avatar Configure 并尝试 Enforce T-Pose。

### 实际结果

- 腿脚出现明显扭曲。
- Enforce T-Pose 后异常仍存在。
- Unity 报告：

  ```text
  Rig Error: Avatar Rig Configuration mis-match.
  Bone length in configuration does not match position in animation file.
  ```

- 已记录的位置误差：

  ```text
  thigh_l / thigh_r ≈ 31 mm
  calf_l / calf_r   ≈ 546 mm
  foot_l / foot_r   ≈ 646 mm
  ball_l / ball_r   ≈ 213 mm
  ```

### 预期结果

Imp 能正常播放重定向后的 Idle 动画，腿、脚和武器没有明显变形，也不出现 Avatar 配置不匹配错误。

### 影响范围

可能影响 Player 的 Humanoid 动画重定向，阻塞移动和战斗动画接入。

### 根因

未完整保留当时的配置差异，因此不能可靠确认。现有证据表明问题与 Avatar/骨架配置不匹配有关，但这只是问题范围，不是最终根因。

### 修复结果

- 当前 Imp Animator 使用有效的 ImpAvatar。
- PlayerAnimator 默认播放 `Armature|Idle_Loop`。
- `Animator.applyRootMotion=false`。
- 用户在 Day 3 手工回归中报告 Idle 循环和 Humanoid 表现正常。

### 回归结果

| 项目 | 结果 |
|---|---|
| Idle_Loop | PASS（用户手工测试） |
| Root Motion 漂移 | 未观察到 |
| 保存后的配置检查 | PASS |
| Walk/Jog/Sprint | PASS（用户 Day 4 手工测试） |
| Console 中旧 Rig Error 是否再次出现 | PASS（用户 Day 4 全量回归未报告复现） |

### 关闭条件

关闭条件已满足：用户在保存后的当前工程完成 Day 4 全量测试，Idle/Walk/Jog/Sprint 视觉测试通过，Console 未报告 Rig Error 再现。

---

## BUG-002：A_TPose 被误开启 Loop Time 与 Loop Pose

### 基本信息

- 发现日期：2026-09-07
- 首次发现版本：Day 4 工作树，尚未提交
- Unity：`6000.5.6f1`
- 资源：`Assets/_Game/Animations/Source/UAL1_Standard.fbx`
- 状态：Closed
- 严重程度：Minor
- 优先级：Low
- 复现率：静态配置稳定存在

### 复现步骤

1. 在 Project 窗口选中 `UAL1_Standard.fbx`。
2. 打开 Animation 导入页。
3. 选中 `Armature|A_TPose`。
4. 检查 Loop Time 与 Loop Pose。

### 实际结果

`A_TPose` 的 Loop Time 与 Loop Pose 均为 true。该 Clip 不以 `_Loop` 结尾，也不作为持续 Gameplay 动画。

### 预期结果

`A_TPose` 的 Loop Time 与 Loop Pose 均为 false；Idle、Walk、Jog、Sprint 等实际循环动作按需开启。

### 影响范围

当前 Animator 未使用 A_TPose，因此不阻断运行；但导入配置与约定不一致，可能在以后误用时掩盖状态或过渡问题。

### 修复与回归

- [x] 关闭 A_TPose 的 Loop Time 与 Loop Pose，并 Apply。
- [x] 确认 Idle/Walk/Jog/Sprint 的 Loop Time 仍为 true。
- [x] 确认 PlayerAnimator 默认状态和运行表现不受影响。

当前磁盘复核：A_TPose 为 `loopTime=0 / loopBlend=0`；Idle、Walk、Jog、Sprint 均为 `loopTime=1`。用户已完成 Play Mode 回归，未报告 T Pose、默认状态异常或新增 Console 错误，Bug Closed。

---

## BUG-003：PlayMode 输入测试因场景初始化时序偶发假失败

### 基本信息

- 发现日期：2026-09-08
- 首次发现版本：Day 6 工作树，尚未提交
- Unity：`6000.5.6f1`
- 测试：`Assets/_Game/Tests/PlayMode/PlayerMotorPlayModeTests.cs`
- 状态：Closed
- 严重程度：Minor
- 优先级：High
- 类型：Test Defect / Flaky Test

### 复现步骤

1. 使用 `SceneManager.LoadScene("SampleScene")` 加载场景。
2. 只执行一次 `yield return null`。
3. 立即通过虚拟 Keyboard 发送 W 输入并读取 `CurrentMoveSpeed`。
4. 重复 Run All，或在 Unity 冷启动后首次运行。

### 实际结果

`normalSpeed` 偶发为 `0`，触发 `Expected: greater than 0.1; But was: 0.0`。相同代码重跑可能 PASS，属于假失败。

### 预期结果

测试必须在场景和 PlayerInput 完成初始化后发送虚拟输入；相同工程状态下结果稳定。

### 根因

测试把“加载场景后固定等待一帧”等同于“场景和输入系统已经准备完成”。这个时序假设偶尔不成立。

### 修复

改用 `SceneManager.LoadSceneAsync()`，`yield return loadOperation` 明确等待完成，再额外等待一帧后获取 Player 和发送输入。

### 回归结果

| 项目 | 结果 |
|---|---|
| 连续 Run All 5 次 | 2/2 PASS |
| Unity 完全关闭后冷启动首次 Run All | 2/2 PASS |
| 结论 | Closed；初始化稳定性恢复 |

---

## BUG-004：角色离地后仍保留完整水平控制速度

### 基本信息

- 发现日期：2026-09-08
- 首次发现版本：Day 6 工作树，尚未提交
- Unity：`6000.5.6f1`
- 场景：`Assets/_Game/Scenes/SampleScene.unity`
- 状态：Open
- 严重程度：Minor
- 优先级：Medium
- 复现率：用户 Day 6 测试中可观察

### 复现步骤

1. 进入 `SampleScene`。
2. 让 Player 从平台边缘走出。
3. 离地期间继续输入方向，或快速改变方向。

### 实际结果

Player 离地后仍保留与地面相同的完整水平控制能力和速度。

### 预期结果

空中控制应有明确规则：保持离地瞬间水平速度，或只允许受限的空中修正；不应无意中沿用完整地面控制。

### 影响范围

当前项目不做跳跃，因此不阻断 Week 1 地面移动验收；后续平台掉落与 Dash 位移可能受影响。

### 处理计划

暂不在 Day 6 顺带修改 PlayerMotor。进入技能位移前确定最小空中控制规则，再实现并增加平台边缘回归用例。

---

## 不登记为 Bug 的观察项

### OBS-001：角色停止时镜头轻微追随

- 现象：Player 突然停止时，画面有轻微的镜头追随感。
- 当前结论：用户接受，不影响移动控制。
- 处理：暂不登记为 Bug。Day 4 镜头回归中若出现明显跳动、穿模或持续漂移，再建立独立 Bug。
