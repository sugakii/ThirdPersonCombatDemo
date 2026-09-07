# Bug Reports

> 项目：Third-Person Combat Demo  
> 维护规则：只记录实际复现的缺陷；修复后必须回归，不能仅凭代码修改关闭。

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
- 状态：Resolved
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
| Walk/Jog/Sprint | NOT RUN |
| Console 中旧 Rig Error 是否再次出现 | 保存后 Play 回归待确认 |

### 关闭条件

保存后重新进入 Play，Idle 无明显变形且 Console 不再出现该 Rig Error；随后至少抽查 Walk、Jog、Sprint。条件全部满足后将状态改为 Closed。

---

## BUG-002：A_TPose 被误开启 Loop Time 与 Loop Pose

### 基本信息

- 发现日期：2026-09-07
- 首次发现版本：Day 4 工作树，尚未提交
- Unity：`6000.5.6f1`
- 资源：`Assets/_Game/Animations/Source/UAL1_Standard.fbx`
- 状态：Open
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

- [ ] 关闭 A_TPose 的 Loop Time 与 Loop Pose，并 Apply。
- [ ] 确认 Idle/Walk/Jog/Sprint 的 Loop Time 仍为 true。
- [ ] 确认 PlayerAnimator 默认状态和运行表现不受影响。

---

## 不登记为 Bug 的观察项

### OBS-001：角色停止时镜头轻微追随

- 现象：Player 突然停止时，画面有轻微的镜头追随感。
- 当前结论：用户接受，不影响移动控制。
- 处理：暂不登记为 Bug。Day 4 镜头回归中若出现明显跳动、穿模或持续漂移，再建立独立 Bug。
