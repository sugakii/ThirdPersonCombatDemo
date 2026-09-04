# Asset Audit

> 扫描日期：2026-09-02  
> 扫描源：`MaterialPackage/` 实际本地文件、FBX/GLB 结构、随包 README/导入图和许可证  
> 工程复核：2026-09-04（成果归入 9/3）；Imp/UAL1 已导入且 Avatar 有效，动画播放、完整材质与环境验证未完成。源素材目录树仍为 9/2 扫描快照。

## 1. 审计原则

本文件把结论分为三类：

- **磁盘已验证**：可由实际文件、文件内容或模型结构直接确认。
- **项目决定**：已经确定要采用的素材与范围。
- **待 Unity 验证**：静态扫描不能代替 Unity Model Importer、Avatar Configure、动画预览或场景测试。

不得把“推荐导入设置”写成“已经配置成功”，也不得用素材包宣传图替代本地 Standard 子集的实际清单。

## 2. 扫描总览

`MaterialPackage/` 共扫描到 **1,213 个文件**，总大小 **798,159,480 bytes（约 761 MiB）**，其中 **233 个 FBX**、**0 个 `.meta`**。

| 素材包 | 文件数 | 实际构成摘要 |
|---|---:|---|
| Bestiary - Dungeon Monsters Kit[Standard] | 22 | 2 FBX、2 GLB、16 PNG、1 JPG、1 TXT |
| Medieval Village MegaKit[Standard] | 936 | 176 FBX、176 OBJ、176 MTL、176 glTF、176 BIN、54 PNG、JPG/TXT |
| Modular Character Outfits - Fantasy[Standard] | 121 | 24 Unity FBX、24 glTF、24 BIN、46 PNG、2 TXT、1 JPG |
| Universal Animation Library 2[Standard] | 13 | 3 FBX、3 GLB、3 PNG、3 TXT、1 BLEND |
| Universal Animation Library[Standard] | 9 | 2 FBX、2 GLB、3 PNG、2 TXT |
| Universal Base Characters[Standard] | 112 | 26 FBX、18 glTF、18 BIN、48 PNG、2 TXT |

当前 Unity 工程复核（9/4 快照，计入 9/3）：

- FBX：Imp 与 UAL1，共 2 个；两者 Humanoid Avatar 均 valid=true、human=true。
- Imp：`Assets/_Game/Art/Charactors/Player/Model/Imp.fbx`，Bake Axis Conversion=true，场景 Imp 子对象 Y=180。
- 纹理：红色 BaseColor 1、Normal、Emissive、ORM 已复制。渲染器引用 MI_Imp（URP/Lit），主纹理为红色 BaseColor；无独立 .mat，材质路径仍指向 FBX，持久化与其他贴图效果待验证。
- UAL1：`Assets/_Game/Animations/Source/UAL1_Standard.fbx`，43 条 Clip；实际名称保留 `Armature|` 前缀。Idle_Loop 循环=true；Walk_Loop、Jog_Fwd_Loop、Sprint_Loop 循环=false。Bake Axis Conversion=false，后续根据预览验证。
- PlayerAnimator.controller 已创建，但 Base Layer 没有状态；Player/Imp 两个 Animator 的 Avatar 与 Controller 分置，播放链未完成。
- UAL2、Puglin、环境尚未导入；无 Player Prefab 或庭院。有效 Avatar 不等于动作重定向与视觉效果已通过。

## 3. 正式使用清单

正式工程只从源包复制实际需要的 Unity FBX、必要纹理和许可证。禁止把六个完整包直接拖入 `Assets/`。

| 类别 | 采用内容 | 排除内容 |
|---|---|---|
| Player | `Imp.fbx`、BaseColor 1、Emissive、Normal、ORM | Imp GLB、颜色 2/3、Unreal Normal、其他角色包 |
| Enemy | `Puglin.fbx`、BaseColor 1、Emissive、Normal、ORM | Puglin GLB、颜色 2/3、Unreal Normal |
| Animations | `UAL1_Standard.fbx`、`UAL2_Standard.fbx` | 两个 `_RM.fbx`、GLB、未使用动作资源 |
| Environment | 下文确定的 12 个 FBX 与实际需要的纹理 | OBJ、MTL、glTF、BIN、未使用 164 个模型 |
| VFX | Unity ParticleSystem + TrailRenderer 自制 | 不额外引入 VFX 包 |

## 4. Player

### 模型文件

- **最终选择**：`MaterialPackage/Bestiary - Dungeon Monsters Kit[Standard]/Exports/FBX (Unity)/Imp.fbx`
- 文件格式：FBX Binary 7400。
- FBX 内没有 AnimationStack/内置动画；动作必须通过 Humanoid Avatar 从 UAL 重定向。
- 模型子项：
  - `Imp_Body`
  - `Imp_Chains`
  - `Imp_Mace`
  - `Imp_Shorts`（FBX Geometry 内部名为 `Teen_Male.004`）
  - `Imp_SpikedCollar`

### Rig

- 静态扫描到 **55 个 LimbNode 骨骼**。
- 核心骨名包含：`root`、`pelvis`、`spine_01`～`spine_03`、`neck_01`、`Head`、左右 clavicle/upperarm/lowerarm、`hand_l`、`hand_r`、腿、脚、ball、index/middle/ring/thumb。
- 与 UAL 的 65 骨相比，Imp 缺少左右 pinky 完整链（共 8）以及 `ball_leaf_l`、`ball_leaf_r`（共 2）。
- 随包 `Importing_UnrealUnity.png` 要求在 Unity 中开启 **Bake Axis Conversion**，并把 Rig 的 Animation Type 设置为 **Humanoid**。

### Avatar

- 当前已生成 ImpAvatar，Unity 查询 valid=true、human=true；动画播放与变形仍待验证。
- 预期通过 Unity Humanoid 映射 UAL 动画，但“预期可重定向”不等于已经兼容。
- 导入后必须进入 Configure 确认 Avatar 为绿色有效状态，并逐条检查手臂、手指、脚、脊柱和武器位置。

### Materials

实际纹理均为 2048×2048 RGB：

- `T_Imp_BaseColor_1.png`：已目视确认是红色，作为默认 Player 外观。
- `T_Imp_BaseColor_2.png`：绿色，不导入 MVP。
- `T_Imp_BaseColor_3.png`：蓝色，不导入 MVP。
- `T_Imp_Emissive.png`
- `T_Imp_Normal.png`：Unity/Godot 方向版本，采用。
- `T_Imp_ORM.png`
- `Textures/Unreal Normals/T_Imp_Normal_Unreal.png`：不用于 Unity。

GLB 中材质名为 `MI_Imp`，引用 BaseColor 1、Emissive、Normal 和 ORM；FBX 还列出默认 `Material`。源包没有可直接使用的 Unity `.mat`，需要在 URP 中创建并验证材质。

### 推荐使用

- `Imp.fbx`
- 红色 `T_Imp_BaseColor_1`
- `T_Imp_Emissive`
- `T_Imp_Normal`
- `T_Imp_ORM`（导入时先验证通道，再决定是否重打包）
- Animator 使用非 RM 动画，`Animator.applyRootMotion = false`

### 不推荐使用

- `Exports/GLB (Godot-Unreal)/Imp.glb`：与 FBX 重复且不是 Unity 目标格式。
- `BaseColor_2/3`：当前固定红色方案，不增加无用变体。
- Unreal Normal：法线方向不适合当前 Unity 路径。
- Universal Base Characters + Modular Outfits 作为 Player 主方案：本地 Standard 内容是免费子集，且 Outfit README 明确要求配套 Base Character 的头部用法；没有证据证明它们能直接套在 Imp 上。

### Base/Outfit 备选包的实际限制

- Universal Base Characters Standard 的完整 Unity body 实际只有：
  - `Superhero_Female_FullBody.fbx`
  - `Superhero_Male_FullBody.fbx`
- 另有 8 个 Origin-at-0 发型与 8 个 Rigged-to-Head-Bone 发型。
- Modular Outfits Standard 实际只有 Peasant/Ranger 男女 4 个整套 FBX和 20 个模块 FBX。
- Outfit README 说明穿衣时只保留配套 Base Character 的头；完整身体会穿模并浪费性能。
- 结论：这些包保留作以后学习模块化角色，不进入本月核心工程。

## 5. Animations

### 动画源文件

采用：

- `MaterialPackage/Universal Animation Library[Standard]/Unity/UAL1_Standard.fbx`
- `MaterialPackage/Universal Animation Library 2[Standard]/Unity/UAL2_Standard.fbx`

不采用：

- `UAL1_Standard_RM.fbx`
- `UAL2_Standard_RM.fbx`
- UAL GLB 版本

UAL1 非 RM FBX 有 43 个 AnimationStack/Take；UAL2 非 RM FBX也有 43 个。合计 86 条记录、85 个唯一名称（`A_TPose` 在两包重复）。FBX 中磁盘 Take 全名形如 `Armature|Idle_Loop`；Unity 导入后的显示名称仍待验证。

### Locomotion

| 用途 | 最终映射 | 来源 | 结论 |
|---|---|---|---|
| Idle | `Idle_Loop` | UAL1 | 采用 |
| Walk Forward | `Walk_Loop` | UAL1 | 采用 |
| Walk Backward | 无专用 Clip | — | 本地文件不存在 |
| Strafe Left | 无专用 Clip | — | 本地文件不存在 |
| Strafe Right | 无专用 Clip | — | 本地文件不存在 |
| Run | `Jog_Fwd_Loop` | UAL1 | 采用 |
| Sprint | `Sprint_Loop` | UAL1 | 采用 |

由于没有 backward/strafe，本月不做 Lock-On 八方向移动；CharacterController 仍可任意方向移动，但动画表现使用前进 locomotion，并在角色朝移动方向转向。

### Combat

| 用途 | 最终映射 | 来源 | 备注 |
|---|---|---|---|
| Attack 01 | `Sword_Regular_A_Rec` | UAL2 | 采用；`_Rec` 语义未随包文档化，需预览 |
| Attack 02 | `Sword_Regular_B_Rec` | UAL2 | 采用；`_Rec` 语义未随包文档化，需预览 |
| Attack 03 | `Sword_Regular_C` | UAL2 | 采用；若延期则首先砍掉第三段 |
| Combo | `Sword_Regular_Combo` | UAL2 | 可用于动作参考；正式逻辑优先用三个独立 Clip + 输入缓存 |
| Heavy Combo | `Sword_Heavy_Combo` | UAL2 | 本月不采用 |
| Player Hit | `Hit_Chest` | UAL1 | 采用 |
| Enemy Hit | `Hit_Knockback` | UAL2 | 采用 |
| Death | `Death01` | UAL1 | Player/Enemy 共用候选 |
| Dodge | `Roll` 可替代 | UAL1 | 没有名为 Dodge 的 Clip；本月范围排除 |
| Enemy Attack | `Sword_Attack` | UAL1 | 采用；动作名不代表模型实际持剑 |
| Skill | `Sword_Dash` | UAL2 | 采用；代码负责位移 |

### Animation Properties

- **Humanoid / Generic**：源 FBX 不能证明 Unity 导入类型已经设置；随包说明要求 Unity 设置为 Humanoid。UAL1 已设为 Humanoid 且 Avatar 有效；UAL2 尚未导入。
- **Root Motion**：README 明确 `_RM` 文件把 root motion 烘焙进每条动画；无 `_RM` 文件禁用 root motion。
- **In-Place**：本项目选择的两个非 RM FBX为原地版本；抽查 GLB root translation 恒为 `(0,0,0)`。
- **代码位移**：统一由 CharacterController/PlayerMotor 执行，`Animator.applyRootMotion = false`。
- **Loop**：随包说明要求所有以 `_Loop` 结尾的动作在 Unity 中手工开启 Loop Time；当前 UAL1 Idle_Loop 已开启；Walk/Jog/Sprint 等选用循环还未全部配置。
- **Root Motion Node**：随包 Unity 设置图指定 `Rig/root`；当前非 RM 项目不以此驱动 Gameplay。
- **Forward Axis**：FBX GlobalSettings 为 `UpAxis=2/+1`、`FrontAxis=1/+1`、`CoordAxis=0/-1`；RM GLB 位移方向实测为 +Z。随包要求 Bake Axis Conversion。角色在 Unity Scene 中是否最终面向 `Transform.forward (+Z)` 仍必须导入后验证。

### UAL1 实际 Take 清单（43）

```text
A_TPose
Crouch_Fwd_Loop
Crouch_Idle_Loop
Dance_Loop
Death01
Driving_Loop
Fixing_Kneeling
Hit_Chest
Hit_Head
Idle_Loop
Idle_Talking_Loop
Idle_Torch_Loop
Interact
Jog_Fwd_Loop
Jump_Land
Jump_Loop
Jump_Start
PickUp_Table
Pistol_Aim_Down
Pistol_Aim_Neutral
Pistol_Aim_Up
Pistol_Idle_Loop
Pistol_Reload
Pistol_Shoot
Punch_Cross
Punch_Jab
Push_Loop
Roll
Sitting_Enter
Sitting_Exit
Sitting_Idle_Loop
Sitting_Talking_Loop
Spell_Simple_Enter
Spell_Simple_Exit
Spell_Simple_Idle_Loop
Spell_Simple_Shoot
Sprint_Loop
Swim_Fwd_Loop
Swim_Idle_Loop
Sword_Attack
Sword_Idle
Walk_Formal_Loop
Walk_Loop
```

### UAL2 实际 Take 清单（43）

```text
A_TPose
Chest_Open
ClimbUp_1m
Consume
Farm_Harvest
Farm_PlantSeed
Farm_Watering
Hit_Knockback
Idle_FoldArms_Loop
Idle_Lantern_Loop
Idle_No_Loop
Idle_Rail_Call
Idle_Rail_Loop
Idle_Shield_Break
Idle_Shield_Loop
Idle_TalkingPhone_Loop
LayToIdle
Melee_Hook
Melee_Hook_Rec
NinjaJump_Idle_Loop
NinjaJump_Land
NinjaJump_Start
OverhandThrow
Shield_Dash
Shield_OneShot
Slide_Exit
Slide_Loop
Slide_Start
Sword_Block
Sword_Dash
Sword_Heavy_Combo
Sword_Regular_A
Sword_Regular_A_Rec
Sword_Regular_B
Sword_Regular_B_Rec
Sword_Regular_C
Sword_Regular_Combo
TreeChopping_Loop
Walk_Carry_Loop
Yes
Zombie_Idle_Loop
Zombie_Scratch
Zombie_Walk_Fwd_Loop
```

## 6. Enemy

### 可选模型

Bestiary Standard 本地实际只有两个怪物 Unity FBX：

- `Imp.fbx`
- `Puglin.fbx`

宣传图或通用导入图中出现的其他怪物不属于当前本地 Standard 子集；例如导入图提到 Werewolf，但磁盘中没有 Werewolf 文件。

### 最终推荐

- **Enemy**：`MaterialPackage/Bestiary - Dungeon Monsters Kit[Standard]/Exports/FBX (Unity)/Puglin.fbx`
- 场景实例数：3；共用一个 Puglin Prefab 和同一份静态配置。
- 默认外观：推荐 `T_Puglin_BaseColor_1.png`；当前工程尚未创建 Material。
- 模型子项：`Puglin_Body`、`Puglin_Stick`、`Puglin_Tusks`。
- FBX Binary 7400；无内置动画。

### Rig

- 55 个 LimbNode，核心骨命名与 Imp 一致。
- 与 65 骨 UAL 相比同样缺少左右 pinky 链与两个 ball_leaf 末端骨。
- 需要按 Humanoid + Bake Axis Conversion 导入，并在 Unity Configure 中验证。

### Materials

- `T_Puglin_BaseColor_1/2/3.png`
- `T_Puglin_Emissive.png`
- `T_Puglin_Normal.png`
- `T_Puglin_ORM.png`
- `Textures/Unreal Normals/T_Puglin_Normal_Unreal.png`（不采用）

纹理均为 2048×2048 RGB。GLB 中材质名为 `MI_Puglin`；Unity `.mat` 尚未创建。

### 可重定向动画

- Idle：`Idle_Loop`
- Chase：`Jog_Fwd_Loop`
- Attack：`Sword_Attack`
- Hit：`Hit_Knockback`
- Death：`Death01`

这些是**计划映射**；Avatar 有效性、动作变形、脚底滑动和 Stick 对动作的视觉适配仍待 Unity 逐条验证。

## 7. Weapons

### 剑

- 六包按文件名和模型结构扫描，**没有独立 Sword/Weapon FBX**。
- 动画名称包含 Sword，不等于随包提供剑模型。
- 当前项目不新增独立剑；Imp 的视觉武器保持内嵌 Mace。

### Player 武器

- `Imp_Mace` 是 `Imp.fbx` 内的 Skinned Mesh 子项，不是独立静态 Prefab，也不是普通 hand child。
- GLB 权重扫描到其 971 个加权项全部绑定 `hand_r`。
- 因此当前攻击应被描述为“三段近战攻击”，不要在作品文档中虚构成“已实现独立剑装备系统”。

### Enemy 武器

- `Puglin_Stick` 是 `Puglin.fbx` 内的 Skinned Mesh 子项。
- 431 个加权项全部绑定 `hand_r`。

### 武器挂点 / Bone

- 骨骼存在：`hand_r`、`hand_l`。
- 模型中不存在名为 `weapon_socket` 的骨或独立挂点。
- 若以后添加外部武器，应在 Prefab 的 `hand_r` 下自行创建 `WeaponSocket` 并校准位置/旋转；这不是本月必须项。

## 8. Environment

### 实际库存

Medieval Village 包有 **176 个唯一模型**，每个又以 FBX、OBJ、glTF/BIN 重复提供。分类如下：

| 类别 | 数量 | 类别 | 数量 |
|---|---:|---|---:|
| Balcony | 4 | Corner | 8 |
| Door | 8 | DoorFrame | 4 |
| Floor | 12 | HoleCover | 5 |
| Overhang | 20 | Props（含藤蔓类） | 23 |
| Roof | 39 | Stairs | 19 |
| Wall | 20 | Window | 6 |
| WindowShutters | 8 | **合计** | **176** |

### 20×20m 庭院正式选择（12 个 FBX）

全部来自 `MaterialPackage/Medieval Village MegaKit[Standard]/FBX/`：

1. `Floor_Brick.fbx` — 地面模块，静态扫描尺寸约 2×0.02×2m。
2. `Wall_Plaster_Straight.fbx` — 直墙，静态扫描尺寸约 2×3.125×0.406m。
3. `Wall_Plaster_Door_Round.fbx` — 带圆门洞墙体。
4. `Corner_Exterior_Wood.fbx` — 外墙转角。
5. `DoorFrame_Round_WoodDark.fbx` — 圆门框。
6. `Door_1_Round.fbx` — 圆门。
7. `Stairs_Exterior_Straight.fbx` — 直楼梯。
8. `Stairs_Exterior_Platform.fbx` — 楼梯平台。
9. `Prop_Crate.fbx` — 木箱障碍。
10. `Prop_Wagon.fbx` — 马车。
11. `Prop_WoodenFence_Single.fbx` — 木栅栏。
12. `Prop_WoodenFence_Extension1.fbx` — 栅栏延伸件。

`DoorFrame_Round_Brick.fbx` 也实际存在，可在 Unity 中作为门框视觉备选；MVP 默认不同时导入两个门框变体。

### 地面

- 主模块：`Floor_Brick.fbx`。
- 以 2m 网格拼出约 20×20m 可玩区。
- 导入后为地面建立简化碰撞体并验证接缝，不默认使用所有渲染 Mesh 作为碰撞。

### 墙体

- `Wall_Plaster_Straight.fbx`
- `Wall_Plaster_Door_Round.fbx`
- `Corner_Exterior_Wood.fbx`
- 采用 Box Collider 或少量复合 Collider；墙角必须专项测试卡死和穿透。

### 建筑

- `DoorFrame_Round_WoodDark.fbx`
- `Door_1_Round.fbx`
- `Stairs_Exterior_Straight.fbx`
- `Stairs_Exterior_Platform.fbx`
- 楼梯 Gameplay 碰撞优先使用不可见斜坡，而不是逐级 Mesh Collider，以减少 CharacterController 抖动。

### Props

- `Prop_Crate.fbx`
- `Prop_Wagon.fbx`
- `Prop_WoodenFence_Single.fbx`
- `Prop_WoodenFence_Extension1.fbx`
- Crate/Fence 使用 Box Collider；Wagon 使用少量复合 Collider。

### Materials

包中没有 Unity `.mat`。glTF 静态扫描到 11 个材质名：

```text
MI_Brick
MI_MetalOrnaments
MI_Plaster
MI_RedBrick
MI_RockTrim
MI_RoundTiles
MI_UnevenBrick
MI_Vine
MI_WindowGlass
MI_WoodTrim
MI_WoodTrim_Wear
```

需要根据实际使用模型在 URP Lit 中建立共享材质并手动关联 BaseColor/Normal。Unity/Godot 法线位于 `Textures/Normals Godot-Unity/`。

Roughness/ORM 不能未经验证直接当作 URP Lit 的 Smoothness 输入：

- Roughness 与 Smoothness 方向相反。
- ORM 的具体 RGB 打包约定没有随包文字说明。
- MVP 先使用 BaseColor + 正确 Normal + 人工 Smoothness；需要金属/遮蔽时再验证或重打包通道。

## 9. Licensing

### CC0 1.0

以下五包的本地许可证文件标明 CC0 1.0：

- Medieval Village MegaKit
- Modular Character Outfits - Fantasy
- Universal Animation Library 1
- Universal Animation Library 2
- Universal Base Characters

### QAL v1.0

Bestiary 的本地 `License_Standard.txt` 标明 QAL v1.0（last updated 2026-08-28）：

- 可免费用于个人、教育和商业项目。
- 无需署名。
- 禁止将原始或修改后的素材作为独立素材重新销售或再分发。
- 不得冒充素材作者。
- 完成的游戏 Build 可以分发。

**公开 Git 规则**：不提交 Bestiary 原始 FBX/PNG；只公开代码、配置、素材下载/放置说明和许可证说明。截图、视频与完成 Build 可作为作品展示。

## 10. Problems Found

1. **Unity 验证部分完成**：Imp/UAL1 Avatar 与部分导入属性已确认；动画播放链、完整材质、循环、重定向表现、比例、碰撞和 NavMesh 未通过验收。
2. **骨骼数量不同**：Imp/Puglin 是 55 骨，UAL 是 65 骨；缺 pinky/ball_leaf。Humanoid 预期可行，但必须以 Configure 和逐条动作预览为准。
3. **Locomotion 不完整**：没有 backward、strafe left/right，因此本月不做 Lock-On 八方向移动。
4. **没有专用 Dodge**：只有 `Roll` 可替代，本月明确排除 Dodge。
5. **没有独立剑模型**：Imp 是内嵌 Mace，Puglin 是内嵌 Stick；动画名里的 Sword 不能被当作武器资产证明。
6. **没有武器挂点**：只有 `hand_r/hand_l` 骨；外部武器需以后自行创建 `WeaponSocket`。
7. **`_Rec` 未文档化**：不能声称它一定代表回中、恢复或重定位；必须在 Unity 动画预览验证。
8. **重复格式很多**：同一环境模型以 FBX、OBJ、glTF/BIN 重复存在；Unity 工程只导入 FBX。
9. **Standard 是部分内容**：宣传图展示内容多于本地免费子集，所有决策必须以文件扫描为准。
10. **URP 材质需手工建立**：源包无 `.mat`；纹理路径、Normal 与 ORM/Roughness 通道必须验证。
11. **FBX 不等于 Collider/Prefab**：环境模型没有 Unity Prefab、Collider、LOD 或现成场景，必须在工程内配置。
12. **许可限制影响仓库**：Bestiary 原始资产不能进入公开 Git 历史，应在第一次提交前配置忽略与放置策略。

## 11. Unity 导入验证清单

完成选择性导入后，必须把结果回写本文件：

- [x] Imp Model Importer 开启 Bake Axis Conversion（Unity 查询确认）。
- [x] Imp Rig = Humanoid，Avatar.isValid/isHuman=true（配置有效；动作表现仍待预览）。
- [ ] Puglin Rig = Humanoid，Avatar Configure 有效。
- [ ] UAL1/UAL2 Rig = Humanoid，并使用可复用 Avatar/正确映射。
- [ ] Unity 中实际 Clip 名称与本文映射一致。
- [ ] 所有 `_Loop` 动作开启 Loop Time，非循环动作未误开。
- [ ] `Animator.applyRootMotion = false`，代码与动画没有重复位移。
- [ ] Imp/Puglin 面向 Unity +Z，手脚和脊柱无明显变形。
- [ ] Mace/Stick 在各攻击、受击、死亡动作中没有异常拉伸。
- [ ] BaseColor/Emissive/Normal 显示正确；ORM 或替代参数已验证。
- [ ] 环境尺寸、Pivot、2m 拼接接缝正确。
- [ ] Floor/Wall/Crate/Fence/Wagon 简化碰撞体可用。
- [ ] 楼梯斜坡碰撞可让 CharacterController 稳定上下。
- [ ] 公共仓库忽略 Bestiary 原始 FBX/PNG。

## 12. 审计结论

最终方案保持不变：

- Player：红色 Imp。
- Enemy：3 个 Puglin。
- Animation：只用 `UAL1_Standard.fbx`、`UAL2_Standard.fbx` 的非 RM 动画。
- Environment：只导入上列 12 个 Medieval Village FBX。
- Movement：代码驱动，不做八方向 Lock-On。
- Skill VFX：ParticleSystem + TrailRenderer 自制。

下一步不是继续挑素材，而是在建立 Git 基线后做一次**选择性导入 + Unity Avatar/材质/动画预检**，并将真实验证结果覆盖更新到本文件。

## Appendix A. 素材包完整文件结构

> 生成时间：2026-09-02
> 数据来源：对 `MaterialPackage/` 的实际递归扫描
> 范围：6 个素材包、52 个子目录、1,213 个文件；不省略重复格式或未采用素材
> 说明：此树仅记录源素材的磁盘结构，不代表这些文件都应导入 Unity。正式导入范围仍以“正式使用清单”为准。

```text
MaterialPackage
├── Bestiary - Dungeon Monsters Kit[Standard]
│   ├── Exports
│   │   ├── FBX (Unity)
│   │   │   ├── Imp.fbx
│   │   │   └── Puglin.fbx
│   │   └── GLB (Godot-Unreal)
│   │       ├── Imp.glb
│   │       └── Puglin.glb
│   ├── Textures
│   │   ├── Unreal Normals
│   │   │   ├── T_Imp_Normal_Unreal.png
│   │   │   └── T_Puglin_Normal_Unreal.png
│   │   ├── T_Imp_BaseColor_1.png
│   │   ├── T_Imp_BaseColor_2.png
│   │   ├── T_Imp_BaseColor_3.png
│   │   ├── T_Imp_Emissive.png
│   │   ├── T_Imp_Normal.png
│   │   ├── T_Imp_ORM.png
│   │   ├── T_Puglin_BaseColor_1.png
│   │   ├── T_Puglin_BaseColor_2.png
│   │   ├── T_Puglin_BaseColor_3.png
│   │   ├── T_Puglin_Emissive.png
│   │   ├── T_Puglin_Normal.png
│   │   └── T_Puglin_ORM.png
│   ├── Importing_Godot.png
│   ├── Importing_UnrealUnity.png
│   ├── License_Standard.txt
│   └── Preview.jpg
├── Medieval Village MegaKit[Standard]
│   ├── FBX
│   │   ├── Balcony_Cross_Corner.fbx
│   │   ├── Balcony_Cross_Straight.fbx
│   │   ├── Balcony_Simple_Corner.fbx
│   │   ├── Balcony_Simple_Straight.fbx
│   │   ├── Corner_Exterior_Brick.fbx
│   │   ├── Corner_Exterior_TopDown.fbx
│   │   ├── Corner_Exterior_TopOnly.fbx
│   │   ├── Corner_Exterior_Wood.fbx
│   │   ├── Corner_ExteriorWide_Brick.fbx
│   │   ├── Corner_ExteriorWide_Wood.fbx
│   │   ├── Corner_Interior_Big.fbx
│   │   ├── Corner_Interior_Small.fbx
│   │   ├── Door_1_Flat.fbx
│   │   ├── Door_1_Round.fbx
│   │   ├── Door_2_Flat.fbx
│   │   ├── Door_2_Round.fbx
│   │   ├── Door_4_Flat.fbx
│   │   ├── Door_4_Round.fbx
│   │   ├── Door_8_Flat.fbx
│   │   ├── Door_8_Round.fbx
│   │   ├── DoorFrame_Flat_Brick.fbx
│   │   ├── DoorFrame_Flat_WoodDark.fbx
│   │   ├── DoorFrame_Round_Brick.fbx
│   │   ├── DoorFrame_Round_WoodDark.fbx
│   │   ├── Floor_Brick.fbx
│   │   ├── Floor_RedBrick.fbx
│   │   ├── Floor_UnevenBrick.fbx
│   │   ├── Floor_WoodDark_Half1.fbx
│   │   ├── Floor_WoodDark_Half2.fbx
│   │   ├── Floor_WoodDark_Half3.fbx
│   │   ├── Floor_WoodDark_OverhangCorner.fbx
│   │   ├── Floor_WoodDark_OverhangCorner2.fbx
│   │   ├── Floor_WoodDark.fbx
│   │   ├── Floor_WoodLight_OverhangCorner.fbx
│   │   ├── Floor_WoodLight_OverhangCorner2.fbx
│   │   ├── Floor_WoodLight.fbx
│   │   ├── HoleCover_90Angle.fbx
│   │   ├── HoleCover_90Half.fbx
│   │   ├── HoleCover_90Stairs.fbx
│   │   ├── HoleCover_Straight.fbx
│   │   ├── HoleCover_StraightHalf.fbx
│   │   ├── Overhang_Plaster_Corner_Front.fbx
│   │   ├── Overhang_Plaster_Corner.fbx
│   │   ├── Overhang_Plaster_Long.fbx
│   │   ├── Overhang_Plaster_Short.fbx
│   │   ├── Overhang_Roof_Plaster.fbx
│   │   ├── Overhang_Roof_UnevenBricks.fbx
│   │   ├── Overhang_RoofIncline_Plaster.fbx
│   │   ├── Overhang_RoofIncline_UnevenBricks.fbx
│   │   ├── Overhang_Side_Plaster_Long_L.fbx
│   │   ├── Overhang_Side_Plaster_Long_R.fbx
│   │   ├── Overhang_Side_Plaster_Short_L.fbx
│   │   ├── Overhang_Side_Plaster_Short_R.fbx
│   │   ├── Overhang_Side_UnevenBrick_Long_L.fbx
│   │   ├── Overhang_Side_UnevenBrick_Long_R.fbx
│   │   ├── Overhang_Side_UnevenBrick_Short_L.fbx
│   │   ├── Overhang_Side_UnevenBrick_Short_R.fbx
│   │   ├── Overhang_UnevenBrick_Corner_Front.fbx
│   │   ├── Overhang_UnevenBrick_Corner.fbx
│   │   ├── Overhang_UnevenBrick_Long.fbx
│   │   ├── Overhang_UnevenBrick_Short.fbx
│   │   ├── Prop_Brick1.fbx
│   │   ├── Prop_Brick2.fbx
│   │   ├── Prop_Brick3.fbx
│   │   ├── Prop_Brick4.fbx
│   │   ├── Prop_Chimney.fbx
│   │   ├── Prop_Chimney2.fbx
│   │   ├── Prop_Crate.fbx
│   │   ├── Prop_ExteriorBorder_Corner.fbx
│   │   ├── Prop_ExteriorBorder_Straight1.fbx
│   │   ├── Prop_ExteriorBorder_Straight2.fbx
│   │   ├── Prop_MetalFence_Ornament.fbx
│   │   ├── Prop_MetalFence_Simple.fbx
│   │   ├── Prop_Support.fbx
│   │   ├── Prop_Vine1.fbx
│   │   ├── Prop_Vine2.fbx
│   │   ├── Prop_Vine4.fbx
│   │   ├── Prop_Vine5.fbx
│   │   ├── Prop_Vine6.fbx
│   │   ├── Prop_Vine9.fbx
│   │   ├── Prop_Wagon.fbx
│   │   ├── Prop_WoodenFence_Extension1.fbx
│   │   ├── Prop_WoodenFence_Extension2.fbx
│   │   ├── Prop_WoodenFence_Single.fbx
│   │   ├── Roof_2x4_RoundTile.fbx
│   │   ├── Roof_Dormer_RoundTile.fbx
│   │   ├── Roof_Front_Brick2.fbx
│   │   ├── Roof_Front_Brick4_Half_L.fbx
│   │   ├── Roof_Front_Brick4_Half_R.fbx
│   │   ├── Roof_Front_Brick4.fbx
│   │   ├── Roof_Front_Brick6_Half_L.fbx
│   │   ├── Roof_Front_Brick6_Half_R.fbx
│   │   ├── Roof_Front_Brick6.fbx
│   │   ├── Roof_Front_Brick8_Half_L.fbx
│   │   ├── Roof_Front_Brick8_Half_R.fbx
│   │   ├── Roof_Front_Brick8.fbx
│   │   ├── Roof_FrontSupports.fbx
│   │   ├── Roof_Log.fbx
│   │   ├── Roof_Modular_RoundTiles.fbx
│   │   ├── Roof_RoundTile_2x1_Long.fbx
│   │   ├── Roof_RoundTile_2x1.fbx
│   │   ├── Roof_RoundTiles_4x4.fbx
│   │   ├── Roof_RoundTiles_4x6.fbx
│   │   ├── Roof_RoundTiles_4x8.fbx
│   │   ├── Roof_RoundTiles_6x10.fbx
│   │   ├── Roof_RoundTiles_6x12.fbx
│   │   ├── Roof_RoundTiles_6x14.fbx
│   │   ├── Roof_RoundTiles_6x4.fbx
│   │   ├── Roof_RoundTiles_6x6.fbx
│   │   ├── Roof_RoundTiles_6x8.fbx
│   │   ├── Roof_RoundTiles_8x10.fbx
│   │   ├── Roof_RoundTiles_8x12.fbx
│   │   ├── Roof_RoundTiles_8x14.fbx
│   │   ├── Roof_RoundTiles_8x8.fbx
│   │   ├── Roof_Support2.fbx
│   │   ├── Roof_Tower_RoundTiles.fbx
│   │   ├── Roof_Wooden_2x1_Center_Mirror.fbx
│   │   ├── Roof_Wooden_2x1_Center.fbx
│   │   ├── Roof_Wooden_2x1_Corner.fbx
│   │   ├── Roof_Wooden_2x1_L.fbx
│   │   ├── Roof_Wooden_2x1_Middle.fbx
│   │   ├── Roof_Wooden_2x1_R.fbx
│   │   ├── Roof_Wooden_2x1.fbx
│   │   ├── Stair_Interior_Rails.fbx
│   │   ├── Stair_Interior_Simple.fbx
│   │   ├── Stair_Interior_Solid.fbx
│   │   ├── Stair_Interior_SolidExtended.fbx
│   │   ├── Stairs_Exterior_NoFirstStep.fbx
│   │   ├── Stairs_Exterior_Platform.fbx
│   │   ├── Stairs_Exterior_Platform45.fbx
│   │   ├── Stairs_Exterior_Platform45Clean.fbx
│   │   ├── Stairs_Exterior_PlatformU.fbx
│   │   ├── Stairs_Exterior_SidePlatform.fbx
│   │   ├── Stairs_Exterior_Sides.fbx
│   │   ├── Stairs_Exterior_Sides45.fbx
│   │   ├── Stairs_Exterior_SidesU.fbx
│   │   ├── Stairs_Exterior_SingleSide.fbx
│   │   ├── Stairs_Exterior_SingleSideThick.fbx
│   │   ├── Stairs_Exterior_Straight_Center.fbx
│   │   ├── Stairs_Exterior_Straight_L.fbx
│   │   ├── Stairs_Exterior_Straight_R.fbx
│   │   ├── Stairs_Exterior_Straight.fbx
│   │   ├── Wall_Arch.fbx
│   │   ├── Wall_BottomCover.fbx
│   │   ├── Wall_Plaster_Door_Flat.fbx
│   │   ├── Wall_Plaster_Door_Round.fbx
│   │   ├── Wall_Plaster_Door_RoundInset.fbx
│   │   ├── Wall_Plaster_Straight_Base.fbx
│   │   ├── Wall_Plaster_Straight_L.fbx
│   │   ├── Wall_Plaster_Straight_R.fbx
│   │   ├── Wall_Plaster_Straight.fbx
│   │   ├── Wall_Plaster_Window_Thin_Round.fbx
│   │   ├── Wall_Plaster_Window_Wide_Flat.fbx
│   │   ├── Wall_Plaster_Window_Wide_Flat2.fbx
│   │   ├── Wall_Plaster_Window_Wide_Round.fbx
│   │   ├── Wall_Plaster_WoodGrid.fbx
│   │   ├── Wall_UnevenBrick_Door_Flat.fbx
│   │   ├── Wall_UnevenBrick_Door_Round.fbx
│   │   ├── Wall_UnevenBrick_Straight.fbx
│   │   ├── Wall_UnevenBrick_Window_Thin_Round.fbx
│   │   ├── Wall_UnevenBrick_Window_Wide_Flat.fbx
│   │   ├── Wall_UnevenBrick_Window_Wide_Round.fbx
│   │   ├── Window_Roof_Thin.fbx
│   │   ├── Window_Roof_Wide.fbx
│   │   ├── Window_Thin_Flat1.fbx
│   │   ├── Window_Thin_Round1.fbx
│   │   ├── Window_Wide_Flat1.fbx
│   │   ├── Window_Wide_Round1.fbx
│   │   ├── WindowShutters_Thin_Flat_Closed.fbx
│   │   ├── WindowShutters_Thin_Flat_Open.fbx
│   │   ├── WindowShutters_Thin_Round_Closed.fbx
│   │   ├── WindowShutters_Thin_Round_Open.fbx
│   │   ├── WindowShutters_Wide_Flat_Closed.fbx
│   │   ├── WindowShutters_Wide_Flat_Open.fbx
│   │   ├── WindowShutters_Wide_Round_Closed.fbx
│   │   └── WindowShutters_Wide_Round_Open.fbx
│   ├── glTF
│   │   ├── Balcony_Cross_Corner.bin
│   │   ├── Balcony_Cross_Corner.gltf
│   │   ├── Balcony_Cross_Straight.bin
│   │   ├── Balcony_Cross_Straight.gltf
│   │   ├── Balcony_Simple_Corner.bin
│   │   ├── Balcony_Simple_Corner.gltf
│   │   ├── Balcony_Simple_Straight.bin
│   │   ├── Balcony_Simple_Straight.gltf
│   │   ├── Corner_Exterior_Brick.bin
│   │   ├── Corner_Exterior_Brick.gltf
│   │   ├── Corner_Exterior_TopDown.bin
│   │   ├── Corner_Exterior_TopDown.gltf
│   │   ├── Corner_Exterior_TopOnly.bin
│   │   ├── Corner_Exterior_TopOnly.gltf
│   │   ├── Corner_Exterior_Wood.bin
│   │   ├── Corner_Exterior_Wood.gltf
│   │   ├── Corner_ExteriorWide_Brick.bin
│   │   ├── Corner_ExteriorWide_Brick.gltf
│   │   ├── Corner_ExteriorWide_Wood.bin
│   │   ├── Corner_ExteriorWide_Wood.gltf
│   │   ├── Corner_Interior_Big.bin
│   │   ├── Corner_Interior_Big.gltf
│   │   ├── Corner_Interior_Small.bin
│   │   ├── Corner_Interior_Small.gltf
│   │   ├── Door_1_Flat.bin
│   │   ├── Door_1_Flat.gltf
│   │   ├── Door_1_Round.bin
│   │   ├── Door_1_Round.gltf
│   │   ├── Door_2_Flat.bin
│   │   ├── Door_2_Flat.gltf
│   │   ├── Door_2_Round.bin
│   │   ├── Door_2_Round.gltf
│   │   ├── Door_4_Flat.bin
│   │   ├── Door_4_Flat.gltf
│   │   ├── Door_4_Round.bin
│   │   ├── Door_4_Round.gltf
│   │   ├── Door_8_Flat.bin
│   │   ├── Door_8_Flat.gltf
│   │   ├── Door_8_Round.bin
│   │   ├── Door_8_Round.gltf
│   │   ├── DoorFrame_Flat_Brick.bin
│   │   ├── DoorFrame_Flat_Brick.gltf
│   │   ├── DoorFrame_Flat_WoodDark.bin
│   │   ├── DoorFrame_Flat_WoodDark.gltf
│   │   ├── DoorFrame_Round_Brick.bin
│   │   ├── DoorFrame_Round_Brick.gltf
│   │   ├── DoorFrame_Round_WoodDark.bin
│   │   ├── DoorFrame_Round_WoodDark.gltf
│   │   ├── Floor_Brick.bin
│   │   ├── Floor_Brick.gltf
│   │   ├── Floor_RedBrick.bin
│   │   ├── Floor_RedBrick.gltf
│   │   ├── Floor_UnevenBrick.bin
│   │   ├── Floor_UnevenBrick.gltf
│   │   ├── Floor_WoodDark_Half1.bin
│   │   ├── Floor_WoodDark_Half1.gltf
│   │   ├── Floor_WoodDark_Half2.bin
│   │   ├── Floor_WoodDark_Half2.gltf
│   │   ├── Floor_WoodDark_Half3.bin
│   │   ├── Floor_WoodDark_Half3.gltf
│   │   ├── Floor_WoodDark_OverhangCorner.bin
│   │   ├── Floor_WoodDark_OverhangCorner.gltf
│   │   ├── Floor_WoodDark_OverhangCorner2.bin
│   │   ├── Floor_WoodDark_OverhangCorner2.gltf
│   │   ├── Floor_WoodDark.bin
│   │   ├── Floor_WoodDark.gltf
│   │   ├── Floor_WoodLight_OverhangCorner.bin
│   │   ├── Floor_WoodLight_OverhangCorner.gltf
│   │   ├── Floor_WoodLight_OverhangCorner2.bin
│   │   ├── Floor_WoodLight_OverhangCorner2.gltf
│   │   ├── Floor_WoodLight.bin
│   │   ├── Floor_WoodLight.gltf
│   │   ├── HoleCover_90Angle.bin
│   │   ├── HoleCover_90Angle.gltf
│   │   ├── HoleCover_90Half.bin
│   │   ├── HoleCover_90Half.gltf
│   │   ├── HoleCover_90Stairs.bin
│   │   ├── HoleCover_90Stairs.gltf
│   │   ├── HoleCover_Straight.bin
│   │   ├── HoleCover_Straight.gltf
│   │   ├── HoleCover_StraightHalf.bin
│   │   ├── HoleCover_StraightHalf.gltf
│   │   ├── Overhang_Plaster_Corner_Front.bin
│   │   ├── Overhang_Plaster_Corner_Front.gltf
│   │   ├── Overhang_Plaster_Corner.bin
│   │   ├── Overhang_Plaster_Corner.gltf
│   │   ├── Overhang_Plaster_Long.bin
│   │   ├── Overhang_Plaster_Long.gltf
│   │   ├── Overhang_Plaster_Short.bin
│   │   ├── Overhang_Plaster_Short.gltf
│   │   ├── Overhang_Roof_Plaster.bin
│   │   ├── Overhang_Roof_Plaster.gltf
│   │   ├── Overhang_Roof_UnevenBricks.bin
│   │   ├── Overhang_Roof_UnevenBricks.gltf
│   │   ├── Overhang_RoofIncline_Plaster.bin
│   │   ├── Overhang_RoofIncline_Plaster.gltf
│   │   ├── Overhang_RoofIncline_UnevenBricks.bin
│   │   ├── Overhang_RoofIncline_UnevenBricks.gltf
│   │   ├── Overhang_Side_Plaster_Long_L.bin
│   │   ├── Overhang_Side_Plaster_Long_L.gltf
│   │   ├── Overhang_Side_Plaster_Long_R.bin
│   │   ├── Overhang_Side_Plaster_Long_R.gltf
│   │   ├── Overhang_Side_Plaster_Short_L.bin
│   │   ├── Overhang_Side_Plaster_Short_L.gltf
│   │   ├── Overhang_Side_Plaster_Short_R.bin
│   │   ├── Overhang_Side_Plaster_Short_R.gltf
│   │   ├── Overhang_Side_UnevenBrick_Long_L.bin
│   │   ├── Overhang_Side_UnevenBrick_Long_L.gltf
│   │   ├── Overhang_Side_UnevenBrick_Long_R.bin
│   │   ├── Overhang_Side_UnevenBrick_Long_R.gltf
│   │   ├── Overhang_Side_UnevenBrick_Short_L.bin
│   │   ├── Overhang_Side_UnevenBrick_Short_L.gltf
│   │   ├── Overhang_Side_UnevenBrick_Short_R.bin
│   │   ├── Overhang_Side_UnevenBrick_Short_R.gltf
│   │   ├── Overhang_UnevenBrick_Corner_Front.bin
│   │   ├── Overhang_UnevenBrick_Corner_Front.gltf
│   │   ├── Overhang_UnevenBrick_Corner.bin
│   │   ├── Overhang_UnevenBrick_Corner.gltf
│   │   ├── Overhang_UnevenBrick_Long.bin
│   │   ├── Overhang_UnevenBrick_Long.gltf
│   │   ├── Overhang_UnevenBrick_Short.bin
│   │   ├── Overhang_UnevenBrick_Short.gltf
│   │   ├── Prop_Brick1.bin
│   │   ├── Prop_Brick1.gltf
│   │   ├── Prop_Brick2.bin
│   │   ├── Prop_Brick2.gltf
│   │   ├── Prop_Brick3.bin
│   │   ├── Prop_Brick3.gltf
│   │   ├── Prop_Brick4.bin
│   │   ├── Prop_Brick4.gltf
│   │   ├── Prop_Chimney.bin
│   │   ├── Prop_Chimney.gltf
│   │   ├── Prop_Chimney2.bin
│   │   ├── Prop_Chimney2.gltf
│   │   ├── Prop_Crate.bin
│   │   ├── Prop_Crate.gltf
│   │   ├── Prop_ExteriorBorder_Corner.bin
│   │   ├── Prop_ExteriorBorder_Corner.gltf
│   │   ├── Prop_ExteriorBorder_Straight1.bin
│   │   ├── Prop_ExteriorBorder_Straight1.gltf
│   │   ├── Prop_ExteriorBorder_Straight2.bin
│   │   ├── Prop_ExteriorBorder_Straight2.gltf
│   │   ├── Prop_MetalFence_Ornament.bin
│   │   ├── Prop_MetalFence_Ornament.gltf
│   │   ├── Prop_MetalFence_Simple.bin
│   │   ├── Prop_MetalFence_Simple.gltf
│   │   ├── Prop_Support.bin
│   │   ├── Prop_Support.gltf
│   │   ├── Prop_Vine1.bin
│   │   ├── Prop_Vine1.gltf
│   │   ├── Prop_Vine2.bin
│   │   ├── Prop_Vine2.gltf
│   │   ├── Prop_Vine4.bin
│   │   ├── Prop_Vine4.gltf
│   │   ├── Prop_Vine5.bin
│   │   ├── Prop_Vine5.gltf
│   │   ├── Prop_Vine6.bin
│   │   ├── Prop_Vine6.gltf
│   │   ├── Prop_Vine9.bin
│   │   ├── Prop_Vine9.gltf
│   │   ├── Prop_Wagon.bin
│   │   ├── Prop_Wagon.gltf
│   │   ├── Prop_WoodenFence_Extension1.bin
│   │   ├── Prop_WoodenFence_Extension1.gltf
│   │   ├── Prop_WoodenFence_Extension2.bin
│   │   ├── Prop_WoodenFence_Extension2.gltf
│   │   ├── Prop_WoodenFence_Single.bin
│   │   ├── Prop_WoodenFence_Single.gltf
│   │   ├── Roof_2x4_RoundTile.bin
│   │   ├── Roof_2x4_RoundTile.gltf
│   │   ├── Roof_Dormer_RoundTile.bin
│   │   ├── Roof_Dormer_RoundTile.gltf
│   │   ├── Roof_Front_Brick2.bin
│   │   ├── Roof_Front_Brick2.gltf
│   │   ├── Roof_Front_Brick4_Half_L.bin
│   │   ├── Roof_Front_Brick4_Half_L.gltf
│   │   ├── Roof_Front_Brick4_Half_R.bin
│   │   ├── Roof_Front_Brick4_Half_R.gltf
│   │   ├── Roof_Front_Brick4.bin
│   │   ├── Roof_Front_Brick4.gltf
│   │   ├── Roof_Front_Brick6_Half_L.bin
│   │   ├── Roof_Front_Brick6_Half_L.gltf
│   │   ├── Roof_Front_Brick6_Half_R.bin
│   │   ├── Roof_Front_Brick6_Half_R.gltf
│   │   ├── Roof_Front_Brick6.bin
│   │   ├── Roof_Front_Brick6.gltf
│   │   ├── Roof_Front_Brick8_Half_L.bin
│   │   ├── Roof_Front_Brick8_Half_L.gltf
│   │   ├── Roof_Front_Brick8_Half_R.bin
│   │   ├── Roof_Front_Brick8_Half_R.gltf
│   │   ├── Roof_Front_Brick8.bin
│   │   ├── Roof_Front_Brick8.gltf
│   │   ├── Roof_FrontSupports.bin
│   │   ├── Roof_FrontSupports.gltf
│   │   ├── Roof_Log.bin
│   │   ├── Roof_Log.gltf
│   │   ├── Roof_Modular_RoundTiles.bin
│   │   ├── Roof_Modular_RoundTiles.gltf
│   │   ├── Roof_RoundTile_2x1_Long.bin
│   │   ├── Roof_RoundTile_2x1_Long.gltf
│   │   ├── Roof_RoundTile_2x1.bin
│   │   ├── Roof_RoundTile_2x1.gltf
│   │   ├── Roof_RoundTiles_4x4.bin
│   │   ├── Roof_RoundTiles_4x4.gltf
│   │   ├── Roof_RoundTiles_4x6.bin
│   │   ├── Roof_RoundTiles_4x6.gltf
│   │   ├── Roof_RoundTiles_4x8.bin
│   │   ├── Roof_RoundTiles_4x8.gltf
│   │   ├── Roof_RoundTiles_6x10.bin
│   │   ├── Roof_RoundTiles_6x10.gltf
│   │   ├── Roof_RoundTiles_6x12.bin
│   │   ├── Roof_RoundTiles_6x12.gltf
│   │   ├── Roof_RoundTiles_6x14.bin
│   │   ├── Roof_RoundTiles_6x14.gltf
│   │   ├── Roof_RoundTiles_6x4.bin
│   │   ├── Roof_RoundTiles_6x4.gltf
│   │   ├── Roof_RoundTiles_6x6.bin
│   │   ├── Roof_RoundTiles_6x6.gltf
│   │   ├── Roof_RoundTiles_6x8.bin
│   │   ├── Roof_RoundTiles_6x8.gltf
│   │   ├── Roof_RoundTiles_8x10.bin
│   │   ├── Roof_RoundTiles_8x10.gltf
│   │   ├── Roof_RoundTiles_8x12.bin
│   │   ├── Roof_RoundTiles_8x12.gltf
│   │   ├── Roof_RoundTiles_8x14.bin
│   │   ├── Roof_RoundTiles_8x14.gltf
│   │   ├── Roof_RoundTiles_8x8.bin
│   │   ├── Roof_RoundTiles_8x8.gltf
│   │   ├── Roof_Support2.bin
│   │   ├── Roof_Support2.gltf
│   │   ├── Roof_Tower_RoundTiles.bin
│   │   ├── Roof_Tower_RoundTiles.gltf
│   │   ├── Roof_Wooden_2x1_Center_Mirror.bin
│   │   ├── Roof_Wooden_2x1_Center_Mirror.gltf
│   │   ├── Roof_Wooden_2x1_Center.bin
│   │   ├── Roof_Wooden_2x1_Center.gltf
│   │   ├── Roof_Wooden_2x1_Corner.bin
│   │   ├── Roof_Wooden_2x1_Corner.gltf
│   │   ├── Roof_Wooden_2x1_L.bin
│   │   ├── Roof_Wooden_2x1_L.gltf
│   │   ├── Roof_Wooden_2x1_Middle.bin
│   │   ├── Roof_Wooden_2x1_Middle.gltf
│   │   ├── Roof_Wooden_2x1_R.bin
│   │   ├── Roof_Wooden_2x1_R.gltf
│   │   ├── Roof_Wooden_2x1.bin
│   │   ├── Roof_Wooden_2x1.gltf
│   │   ├── Stair_Interior_Rails.bin
│   │   ├── Stair_Interior_Rails.gltf
│   │   ├── Stair_Interior_Simple.bin
│   │   ├── Stair_Interior_Simple.gltf
│   │   ├── Stair_Interior_Solid.bin
│   │   ├── Stair_Interior_Solid.gltf
│   │   ├── Stair_Interior_SolidExtended.bin
│   │   ├── Stair_Interior_SolidExtended.gltf
│   │   ├── Stairs_Exterior_NoFirstStep.bin
│   │   ├── Stairs_Exterior_NoFirstStep.gltf
│   │   ├── Stairs_Exterior_Platform.bin
│   │   ├── Stairs_Exterior_Platform.gltf
│   │   ├── Stairs_Exterior_Platform45.bin
│   │   ├── Stairs_Exterior_Platform45.gltf
│   │   ├── Stairs_Exterior_Platform45Clean.bin
│   │   ├── Stairs_Exterior_Platform45Clean.gltf
│   │   ├── Stairs_Exterior_PlatformU.bin
│   │   ├── Stairs_Exterior_PlatformU.gltf
│   │   ├── Stairs_Exterior_SidePlatform.bin
│   │   ├── Stairs_Exterior_SidePlatform.gltf
│   │   ├── Stairs_Exterior_Sides.bin
│   │   ├── Stairs_Exterior_Sides.gltf
│   │   ├── Stairs_Exterior_Sides45.bin
│   │   ├── Stairs_Exterior_Sides45.gltf
│   │   ├── Stairs_Exterior_SidesU.bin
│   │   ├── Stairs_Exterior_SidesU.gltf
│   │   ├── Stairs_Exterior_SingleSide.bin
│   │   ├── Stairs_Exterior_SingleSide.gltf
│   │   ├── Stairs_Exterior_SingleSideThick.bin
│   │   ├── Stairs_Exterior_SingleSideThick.gltf
│   │   ├── Stairs_Exterior_Straight_Center.bin
│   │   ├── Stairs_Exterior_Straight_Center.gltf
│   │   ├── Stairs_Exterior_Straight_L.bin
│   │   ├── Stairs_Exterior_Straight_L.gltf
│   │   ├── Stairs_Exterior_Straight_R.bin
│   │   ├── Stairs_Exterior_Straight_R.gltf
│   │   ├── Stairs_Exterior_Straight.bin
│   │   ├── Stairs_Exterior_Straight.gltf
│   │   ├── T_Brick_BaseColor.png
│   │   ├── T_Brick_Normal.png
│   │   ├── T_Brick_Roughness.png
│   │   ├── T_MetalOrnaments_BaseColor.png
│   │   ├── T_MetalOrnaments_Roughness.png
│   │   ├── T_Plaster_BaseColor.png
│   │   ├── T_Plaster_Normal.png
│   │   ├── T_Plaster_ORM.png
│   │   ├── T_RedBrick_BaseColor.png
│   │   ├── T_RockTrim_BaseColor.png
│   │   ├── T_RockTrim_Normal.png
│   │   ├── T_RockTrim_ORM.png
│   │   ├── T_RoundTiles_BaseColor.png
│   │   ├── T_RoundTiles_Normal.png
│   │   ├── T_RoundTiles_Roughness.png
│   │   ├── T_UnevenBrick_BaseColor.png
│   │   ├── T_UnevenBrick_Normal.png
│   │   ├── T_UnevenBrick_Roughness.png
│   │   ├── T_VineLeaf_png.png
│   │   ├── T_WoodTrim_BaseColor.png
│   │   ├── T_WoodTrim_Normal.png
│   │   ├── T_WoodTrim_Roughness.png
│   │   ├── Wall_Arch.bin
│   │   ├── Wall_Arch.gltf
│   │   ├── Wall_BottomCover.bin
│   │   ├── Wall_BottomCover.gltf
│   │   ├── Wall_Plaster_Door_Flat.bin
│   │   ├── Wall_Plaster_Door_Flat.gltf
│   │   ├── Wall_Plaster_Door_Round.bin
│   │   ├── Wall_Plaster_Door_Round.gltf
│   │   ├── Wall_Plaster_Door_RoundInset.bin
│   │   ├── Wall_Plaster_Door_RoundInset.gltf
│   │   ├── Wall_Plaster_Straight_Base.bin
│   │   ├── Wall_Plaster_Straight_Base.gltf
│   │   ├── Wall_Plaster_Straight_L.bin
│   │   ├── Wall_Plaster_Straight_L.gltf
│   │   ├── Wall_Plaster_Straight_R.bin
│   │   ├── Wall_Plaster_Straight_R.gltf
│   │   ├── Wall_Plaster_Straight.bin
│   │   ├── Wall_Plaster_Straight.gltf
│   │   ├── Wall_Plaster_Window_Thin_Round.bin
│   │   ├── Wall_Plaster_Window_Thin_Round.gltf
│   │   ├── Wall_Plaster_Window_Wide_Flat.bin
│   │   ├── Wall_Plaster_Window_Wide_Flat.gltf
│   │   ├── Wall_Plaster_Window_Wide_Flat2.bin
│   │   ├── Wall_Plaster_Window_Wide_Flat2.gltf
│   │   ├── Wall_Plaster_Window_Wide_Round.bin
│   │   ├── Wall_Plaster_Window_Wide_Round.gltf
│   │   ├── Wall_Plaster_WoodGrid.bin
│   │   ├── Wall_Plaster_WoodGrid.gltf
│   │   ├── Wall_UnevenBrick_Door_Flat.bin
│   │   ├── Wall_UnevenBrick_Door_Flat.gltf
│   │   ├── Wall_UnevenBrick_Door_Round.bin
│   │   ├── Wall_UnevenBrick_Door_Round.gltf
│   │   ├── Wall_UnevenBrick_Straight.bin
│   │   ├── Wall_UnevenBrick_Straight.gltf
│   │   ├── Wall_UnevenBrick_Window_Thin_Round.bin
│   │   ├── Wall_UnevenBrick_Window_Thin_Round.gltf
│   │   ├── Wall_UnevenBrick_Window_Wide_Flat.bin
│   │   ├── Wall_UnevenBrick_Window_Wide_Flat.gltf
│   │   ├── Wall_UnevenBrick_Window_Wide_Round.bin
│   │   ├── Wall_UnevenBrick_Window_Wide_Round.gltf
│   │   ├── Window_Roof_Thin.bin
│   │   ├── Window_Roof_Thin.gltf
│   │   ├── Window_Roof_Wide.bin
│   │   ├── Window_Roof_Wide.gltf
│   │   ├── Window_Thin_Flat1.bin
│   │   ├── Window_Thin_Flat1.gltf
│   │   ├── Window_Thin_Round1.bin
│   │   ├── Window_Thin_Round1.gltf
│   │   ├── Window_Wide_Flat1.bin
│   │   ├── Window_Wide_Flat1.gltf
│   │   ├── Window_Wide_Round1.bin
│   │   ├── Window_Wide_Round1.gltf
│   │   ├── WindowShutters_Thin_Flat_Closed.bin
│   │   ├── WindowShutters_Thin_Flat_Closed.gltf
│   │   ├── WindowShutters_Thin_Flat_Open.bin
│   │   ├── WindowShutters_Thin_Flat_Open.gltf
│   │   ├── WindowShutters_Thin_Round_Closed.bin
│   │   ├── WindowShutters_Thin_Round_Closed.gltf
│   │   ├── WindowShutters_Thin_Round_Open.bin
│   │   ├── WindowShutters_Thin_Round_Open.gltf
│   │   ├── WindowShutters_Wide_Flat_Closed.bin
│   │   ├── WindowShutters_Wide_Flat_Closed.gltf
│   │   ├── WindowShutters_Wide_Flat_Open.bin
│   │   ├── WindowShutters_Wide_Flat_Open.gltf
│   │   ├── WindowShutters_Wide_Round_Closed.bin
│   │   ├── WindowShutters_Wide_Round_Closed.gltf
│   │   ├── WindowShutters_Wide_Round_Open.bin
│   │   └── WindowShutters_Wide_Round_Open.gltf
│   ├── OBJ
│   │   ├── Balcony_Cross_Corner.mtl
│   │   ├── Balcony_Cross_Corner.obj
│   │   ├── Balcony_Cross_Straight.mtl
│   │   ├── Balcony_Cross_Straight.obj
│   │   ├── Balcony_Simple_Corner.mtl
│   │   ├── Balcony_Simple_Corner.obj
│   │   ├── Balcony_Simple_Straight.mtl
│   │   ├── Balcony_Simple_Straight.obj
│   │   ├── Corner_Exterior_Brick.mtl
│   │   ├── Corner_Exterior_Brick.obj
│   │   ├── Corner_Exterior_TopDown.mtl
│   │   ├── Corner_Exterior_TopDown.obj
│   │   ├── Corner_Exterior_TopOnly.mtl
│   │   ├── Corner_Exterior_TopOnly.obj
│   │   ├── Corner_Exterior_Wood.mtl
│   │   ├── Corner_Exterior_Wood.obj
│   │   ├── Corner_ExteriorWide_Brick.mtl
│   │   ├── Corner_ExteriorWide_Brick.obj
│   │   ├── Corner_ExteriorWide_Wood.mtl
│   │   ├── Corner_ExteriorWide_Wood.obj
│   │   ├── Corner_Interior_Big.mtl
│   │   ├── Corner_Interior_Big.obj
│   │   ├── Corner_Interior_Small.mtl
│   │   ├── Corner_Interior_Small.obj
│   │   ├── Door_1_Flat.mtl
│   │   ├── Door_1_Flat.obj
│   │   ├── Door_1_Round.mtl
│   │   ├── Door_1_Round.obj
│   │   ├── Door_2_Flat.mtl
│   │   ├── Door_2_Flat.obj
│   │   ├── Door_2_Round.mtl
│   │   ├── Door_2_Round.obj
│   │   ├── Door_4_Flat.mtl
│   │   ├── Door_4_Flat.obj
│   │   ├── Door_4_Round.mtl
│   │   ├── Door_4_Round.obj
│   │   ├── Door_8_Flat.mtl
│   │   ├── Door_8_Flat.obj
│   │   ├── Door_8_Round.mtl
│   │   ├── Door_8_Round.obj
│   │   ├── DoorFrame_Flat_Brick.mtl
│   │   ├── DoorFrame_Flat_Brick.obj
│   │   ├── DoorFrame_Flat_WoodDark.mtl
│   │   ├── DoorFrame_Flat_WoodDark.obj
│   │   ├── DoorFrame_Round_Brick.mtl
│   │   ├── DoorFrame_Round_Brick.obj
│   │   ├── DoorFrame_Round_WoodDark.mtl
│   │   ├── DoorFrame_Round_WoodDark.obj
│   │   ├── Floor_Brick.mtl
│   │   ├── Floor_Brick.obj
│   │   ├── Floor_RedBrick.mtl
│   │   ├── Floor_RedBrick.obj
│   │   ├── Floor_UnevenBrick.mtl
│   │   ├── Floor_UnevenBrick.obj
│   │   ├── Floor_WoodDark_Half1.mtl
│   │   ├── Floor_WoodDark_Half1.obj
│   │   ├── Floor_WoodDark_Half2.mtl
│   │   ├── Floor_WoodDark_Half2.obj
│   │   ├── Floor_WoodDark_Half3.mtl
│   │   ├── Floor_WoodDark_Half3.obj
│   │   ├── Floor_WoodDark_OverhangCorner.mtl
│   │   ├── Floor_WoodDark_OverhangCorner.obj
│   │   ├── Floor_WoodDark_OverhangCorner2.mtl
│   │   ├── Floor_WoodDark_OverhangCorner2.obj
│   │   ├── Floor_WoodDark.mtl
│   │   ├── Floor_WoodDark.obj
│   │   ├── Floor_WoodLight_OverhangCorner.mtl
│   │   ├── Floor_WoodLight_OverhangCorner.obj
│   │   ├── Floor_WoodLight_OverhangCorner2.mtl
│   │   ├── Floor_WoodLight_OverhangCorner2.obj
│   │   ├── Floor_WoodLight.mtl
│   │   ├── Floor_WoodLight.obj
│   │   ├── HoleCover_90Angle.mtl
│   │   ├── HoleCover_90Angle.obj
│   │   ├── HoleCover_90Half.mtl
│   │   ├── HoleCover_90Half.obj
│   │   ├── HoleCover_90Stairs.mtl
│   │   ├── HoleCover_90Stairs.obj
│   │   ├── HoleCover_Straight.mtl
│   │   ├── HoleCover_Straight.obj
│   │   ├── HoleCover_StraightHalf.mtl
│   │   ├── HoleCover_StraightHalf.obj
│   │   ├── Overhang_Plaster_Corner_Front.mtl
│   │   ├── Overhang_Plaster_Corner_Front.obj
│   │   ├── Overhang_Plaster_Corner.mtl
│   │   ├── Overhang_Plaster_Corner.obj
│   │   ├── Overhang_Plaster_Long.mtl
│   │   ├── Overhang_Plaster_Long.obj
│   │   ├── Overhang_Plaster_Short.mtl
│   │   ├── Overhang_Plaster_Short.obj
│   │   ├── Overhang_Roof_Plaster.mtl
│   │   ├── Overhang_Roof_Plaster.obj
│   │   ├── Overhang_Roof_UnevenBricks.mtl
│   │   ├── Overhang_Roof_UnevenBricks.obj
│   │   ├── Overhang_RoofIncline_Plaster.mtl
│   │   ├── Overhang_RoofIncline_Plaster.obj
│   │   ├── Overhang_RoofIncline_UnevenBricks.mtl
│   │   ├── Overhang_RoofIncline_UnevenBricks.obj
│   │   ├── Overhang_Side_Plaster_Long_L.mtl
│   │   ├── Overhang_Side_Plaster_Long_L.obj
│   │   ├── Overhang_Side_Plaster_Long_R.mtl
│   │   ├── Overhang_Side_Plaster_Long_R.obj
│   │   ├── Overhang_Side_Plaster_Short_L.mtl
│   │   ├── Overhang_Side_Plaster_Short_L.obj
│   │   ├── Overhang_Side_Plaster_Short_R.mtl
│   │   ├── Overhang_Side_Plaster_Short_R.obj
│   │   ├── Overhang_Side_UnevenBrick_Long_L.mtl
│   │   ├── Overhang_Side_UnevenBrick_Long_L.obj
│   │   ├── Overhang_Side_UnevenBrick_Long_R.mtl
│   │   ├── Overhang_Side_UnevenBrick_Long_R.obj
│   │   ├── Overhang_Side_UnevenBrick_Short_L.mtl
│   │   ├── Overhang_Side_UnevenBrick_Short_L.obj
│   │   ├── Overhang_Side_UnevenBrick_Short_R.mtl
│   │   ├── Overhang_Side_UnevenBrick_Short_R.obj
│   │   ├── Overhang_UnevenBrick_Corner_Front.mtl
│   │   ├── Overhang_UnevenBrick_Corner_Front.obj
│   │   ├── Overhang_UnevenBrick_Corner.mtl
│   │   ├── Overhang_UnevenBrick_Corner.obj
│   │   ├── Overhang_UnevenBrick_Long.mtl
│   │   ├── Overhang_UnevenBrick_Long.obj
│   │   ├── Overhang_UnevenBrick_Short.mtl
│   │   ├── Overhang_UnevenBrick_Short.obj
│   │   ├── Prop_Brick1.mtl
│   │   ├── Prop_Brick1.obj
│   │   ├── Prop_Brick2.mtl
│   │   ├── Prop_Brick2.obj
│   │   ├── Prop_Brick3.mtl
│   │   ├── Prop_Brick3.obj
│   │   ├── Prop_Brick4.mtl
│   │   ├── Prop_Brick4.obj
│   │   ├── Prop_Chimney.mtl
│   │   ├── Prop_Chimney.obj
│   │   ├── Prop_Chimney2.mtl
│   │   ├── Prop_Chimney2.obj
│   │   ├── Prop_Crate.mtl
│   │   ├── Prop_Crate.obj
│   │   ├── Prop_ExteriorBorder_Corner.mtl
│   │   ├── Prop_ExteriorBorder_Corner.obj
│   │   ├── Prop_ExteriorBorder_Straight1.mtl
│   │   ├── Prop_ExteriorBorder_Straight1.obj
│   │   ├── Prop_ExteriorBorder_Straight2.mtl
│   │   ├── Prop_ExteriorBorder_Straight2.obj
│   │   ├── Prop_MetalFence_Ornament.mtl
│   │   ├── Prop_MetalFence_Ornament.obj
│   │   ├── Prop_MetalFence_Simple.mtl
│   │   ├── Prop_MetalFence_Simple.obj
│   │   ├── Prop_Support.mtl
│   │   ├── Prop_Support.obj
│   │   ├── Prop_Vine1.mtl
│   │   ├── Prop_Vine1.obj
│   │   ├── Prop_Vine2.mtl
│   │   ├── Prop_Vine2.obj
│   │   ├── Prop_Vine4.mtl
│   │   ├── Prop_Vine4.obj
│   │   ├── Prop_Vine5.mtl
│   │   ├── Prop_Vine5.obj
│   │   ├── Prop_Vine6.mtl
│   │   ├── Prop_Vine6.obj
│   │   ├── Prop_Vine9.mtl
│   │   ├── Prop_Vine9.obj
│   │   ├── Prop_Wagon.mtl
│   │   ├── Prop_Wagon.obj
│   │   ├── Prop_WoodenFence_Extension1.mtl
│   │   ├── Prop_WoodenFence_Extension1.obj
│   │   ├── Prop_WoodenFence_Extension2.mtl
│   │   ├── Prop_WoodenFence_Extension2.obj
│   │   ├── Prop_WoodenFence_Single.mtl
│   │   ├── Prop_WoodenFence_Single.obj
│   │   ├── Roof_2x4_RoundTile.mtl
│   │   ├── Roof_2x4_RoundTile.obj
│   │   ├── Roof_Dormer_RoundTile.mtl
│   │   ├── Roof_Dormer_RoundTile.obj
│   │   ├── Roof_Front_Brick2.mtl
│   │   ├── Roof_Front_Brick2.obj
│   │   ├── Roof_Front_Brick4_Half_L.mtl
│   │   ├── Roof_Front_Brick4_Half_L.obj
│   │   ├── Roof_Front_Brick4_Half_R.mtl
│   │   ├── Roof_Front_Brick4_Half_R.obj
│   │   ├── Roof_Front_Brick4.mtl
│   │   ├── Roof_Front_Brick4.obj
│   │   ├── Roof_Front_Brick6_Half_L.mtl
│   │   ├── Roof_Front_Brick6_Half_L.obj
│   │   ├── Roof_Front_Brick6_Half_R.mtl
│   │   ├── Roof_Front_Brick6_Half_R.obj
│   │   ├── Roof_Front_Brick6.mtl
│   │   ├── Roof_Front_Brick6.obj
│   │   ├── Roof_Front_Brick8_Half_L.mtl
│   │   ├── Roof_Front_Brick8_Half_L.obj
│   │   ├── Roof_Front_Brick8_Half_R.mtl
│   │   ├── Roof_Front_Brick8_Half_R.obj
│   │   ├── Roof_Front_Brick8.mtl
│   │   ├── Roof_Front_Brick8.obj
│   │   ├── Roof_FrontSupports.mtl
│   │   ├── Roof_FrontSupports.obj
│   │   ├── Roof_Log.mtl
│   │   ├── Roof_Log.obj
│   │   ├── Roof_Modular_RoundTiles.mtl
│   │   ├── Roof_Modular_RoundTiles.obj
│   │   ├── Roof_RoundTile_2x1_Long.mtl
│   │   ├── Roof_RoundTile_2x1_Long.obj
│   │   ├── Roof_RoundTile_2x1.mtl
│   │   ├── Roof_RoundTile_2x1.obj
│   │   ├── Roof_RoundTiles_4x4.mtl
│   │   ├── Roof_RoundTiles_4x4.obj
│   │   ├── Roof_RoundTiles_4x6.mtl
│   │   ├── Roof_RoundTiles_4x6.obj
│   │   ├── Roof_RoundTiles_4x8.mtl
│   │   ├── Roof_RoundTiles_4x8.obj
│   │   ├── Roof_RoundTiles_6x10.mtl
│   │   ├── Roof_RoundTiles_6x10.obj
│   │   ├── Roof_RoundTiles_6x12.mtl
│   │   ├── Roof_RoundTiles_6x12.obj
│   │   ├── Roof_RoundTiles_6x14.mtl
│   │   ├── Roof_RoundTiles_6x14.obj
│   │   ├── Roof_RoundTiles_6x4.mtl
│   │   ├── Roof_RoundTiles_6x4.obj
│   │   ├── Roof_RoundTiles_6x6.mtl
│   │   ├── Roof_RoundTiles_6x6.obj
│   │   ├── Roof_RoundTiles_6x8.mtl
│   │   ├── Roof_RoundTiles_6x8.obj
│   │   ├── Roof_RoundTiles_8x10.mtl
│   │   ├── Roof_RoundTiles_8x10.obj
│   │   ├── Roof_RoundTiles_8x12.mtl
│   │   ├── Roof_RoundTiles_8x12.obj
│   │   ├── Roof_RoundTiles_8x14.mtl
│   │   ├── Roof_RoundTiles_8x14.obj
│   │   ├── Roof_RoundTiles_8x8.mtl
│   │   ├── Roof_RoundTiles_8x8.obj
│   │   ├── Roof_Support2.mtl
│   │   ├── Roof_Support2.obj
│   │   ├── Roof_Tower_RoundTiles.mtl
│   │   ├── Roof_Tower_RoundTiles.obj
│   │   ├── Roof_Wooden_2x1_Center_Mirror.mtl
│   │   ├── Roof_Wooden_2x1_Center_Mirror.obj
│   │   ├── Roof_Wooden_2x1_Center.mtl
│   │   ├── Roof_Wooden_2x1_Center.obj
│   │   ├── Roof_Wooden_2x1_Corner.mtl
│   │   ├── Roof_Wooden_2x1_Corner.obj
│   │   ├── Roof_Wooden_2x1_L.mtl
│   │   ├── Roof_Wooden_2x1_L.obj
│   │   ├── Roof_Wooden_2x1_Middle.mtl
│   │   ├── Roof_Wooden_2x1_Middle.obj
│   │   ├── Roof_Wooden_2x1_R.mtl
│   │   ├── Roof_Wooden_2x1_R.obj
│   │   ├── Roof_Wooden_2x1.mtl
│   │   ├── Roof_Wooden_2x1.obj
│   │   ├── Stair_Interior_Rails.mtl
│   │   ├── Stair_Interior_Rails.obj
│   │   ├── Stair_Interior_Simple.mtl
│   │   ├── Stair_Interior_Simple.obj
│   │   ├── Stair_Interior_Solid.mtl
│   │   ├── Stair_Interior_Solid.obj
│   │   ├── Stair_Interior_SolidExtended.mtl
│   │   ├── Stair_Interior_SolidExtended.obj
│   │   ├── Stairs_Exterior_NoFirstStep.mtl
│   │   ├── Stairs_Exterior_NoFirstStep.obj
│   │   ├── Stairs_Exterior_Platform.mtl
│   │   ├── Stairs_Exterior_Platform.obj
│   │   ├── Stairs_Exterior_Platform45.mtl
│   │   ├── Stairs_Exterior_Platform45.obj
│   │   ├── Stairs_Exterior_Platform45Clean.mtl
│   │   ├── Stairs_Exterior_Platform45Clean.obj
│   │   ├── Stairs_Exterior_PlatformU.mtl
│   │   ├── Stairs_Exterior_PlatformU.obj
│   │   ├── Stairs_Exterior_SidePlatform.mtl
│   │   ├── Stairs_Exterior_SidePlatform.obj
│   │   ├── Stairs_Exterior_Sides.mtl
│   │   ├── Stairs_Exterior_Sides.obj
│   │   ├── Stairs_Exterior_Sides45.mtl
│   │   ├── Stairs_Exterior_Sides45.obj
│   │   ├── Stairs_Exterior_SidesU.mtl
│   │   ├── Stairs_Exterior_SidesU.obj
│   │   ├── Stairs_Exterior_SingleSide.mtl
│   │   ├── Stairs_Exterior_SingleSide.obj
│   │   ├── Stairs_Exterior_SingleSideThick.mtl
│   │   ├── Stairs_Exterior_SingleSideThick.obj
│   │   ├── Stairs_Exterior_Straight_Center.mtl
│   │   ├── Stairs_Exterior_Straight_Center.obj
│   │   ├── Stairs_Exterior_Straight_L.mtl
│   │   ├── Stairs_Exterior_Straight_L.obj
│   │   ├── Stairs_Exterior_Straight_R.mtl
│   │   ├── Stairs_Exterior_Straight_R.obj
│   │   ├── Stairs_Exterior_Straight.mtl
│   │   ├── Stairs_Exterior_Straight.obj
│   │   ├── Wall_Arch.mtl
│   │   ├── Wall_Arch.obj
│   │   ├── Wall_BottomCover.mtl
│   │   ├── Wall_BottomCover.obj
│   │   ├── Wall_Plaster_Door_Flat.mtl
│   │   ├── Wall_Plaster_Door_Flat.obj
│   │   ├── Wall_Plaster_Door_Round.mtl
│   │   ├── Wall_Plaster_Door_Round.obj
│   │   ├── Wall_Plaster_Door_RoundInset.mtl
│   │   ├── Wall_Plaster_Door_RoundInset.obj
│   │   ├── Wall_Plaster_Straight_Base.mtl
│   │   ├── Wall_Plaster_Straight_Base.obj
│   │   ├── Wall_Plaster_Straight_L.mtl
│   │   ├── Wall_Plaster_Straight_L.obj
│   │   ├── Wall_Plaster_Straight_R.mtl
│   │   ├── Wall_Plaster_Straight_R.obj
│   │   ├── Wall_Plaster_Straight.mtl
│   │   ├── Wall_Plaster_Straight.obj
│   │   ├── Wall_Plaster_Window_Thin_Round.mtl
│   │   ├── Wall_Plaster_Window_Thin_Round.obj
│   │   ├── Wall_Plaster_Window_Wide_Flat.mtl
│   │   ├── Wall_Plaster_Window_Wide_Flat.obj
│   │   ├── Wall_Plaster_Window_Wide_Flat2.mtl
│   │   ├── Wall_Plaster_Window_Wide_Flat2.obj
│   │   ├── Wall_Plaster_Window_Wide_Round.mtl
│   │   ├── Wall_Plaster_Window_Wide_Round.obj
│   │   ├── Wall_Plaster_WoodGrid.mtl
│   │   ├── Wall_Plaster_WoodGrid.obj
│   │   ├── Wall_UnevenBrick_Door_Flat.mtl
│   │   ├── Wall_UnevenBrick_Door_Flat.obj
│   │   ├── Wall_UnevenBrick_Door_Round.mtl
│   │   ├── Wall_UnevenBrick_Door_Round.obj
│   │   ├── Wall_UnevenBrick_Straight.mtl
│   │   ├── Wall_UnevenBrick_Straight.obj
│   │   ├── Wall_UnevenBrick_Window_Thin_Round.mtl
│   │   ├── Wall_UnevenBrick_Window_Thin_Round.obj
│   │   ├── Wall_UnevenBrick_Window_Wide_Flat.mtl
│   │   ├── Wall_UnevenBrick_Window_Wide_Flat.obj
│   │   ├── Wall_UnevenBrick_Window_Wide_Round.mtl
│   │   ├── Wall_UnevenBrick_Window_Wide_Round.obj
│   │   ├── Window_Roof_Thin.mtl
│   │   ├── Window_Roof_Thin.obj
│   │   ├── Window_Roof_Wide.mtl
│   │   ├── Window_Roof_Wide.obj
│   │   ├── Window_Thin_Flat1.mtl
│   │   ├── Window_Thin_Flat1.obj
│   │   ├── Window_Thin_Round1.mtl
│   │   ├── Window_Thin_Round1.obj
│   │   ├── Window_Wide_Flat1.mtl
│   │   ├── Window_Wide_Flat1.obj
│   │   ├── Window_Wide_Round1.mtl
│   │   ├── Window_Wide_Round1.obj
│   │   ├── WindowShutters_Thin_Flat_Closed.mtl
│   │   ├── WindowShutters_Thin_Flat_Closed.obj
│   │   ├── WindowShutters_Thin_Flat_Open.mtl
│   │   ├── WindowShutters_Thin_Flat_Open.obj
│   │   ├── WindowShutters_Thin_Round_Closed.mtl
│   │   ├── WindowShutters_Thin_Round_Closed.obj
│   │   ├── WindowShutters_Thin_Round_Open.mtl
│   │   ├── WindowShutters_Thin_Round_Open.obj
│   │   ├── WindowShutters_Wide_Flat_Closed.mtl
│   │   ├── WindowShutters_Wide_Flat_Closed.obj
│   │   ├── WindowShutters_Wide_Flat_Open.mtl
│   │   ├── WindowShutters_Wide_Flat_Open.obj
│   │   ├── WindowShutters_Wide_Round_Closed.mtl
│   │   ├── WindowShutters_Wide_Round_Closed.obj
│   │   ├── WindowShutters_Wide_Round_Open.mtl
│   │   └── WindowShutters_Wide_Round_Open.obj
│   ├── Textures
│   │   ├── Normals Godot-Unity
│   │   │   ├── T_Brick_Normal.png
│   │   │   ├── T_Plaster_Normal.png
│   │   │   ├── T_RockTrim_Normal.png
│   │   │   ├── T_RoundTiles_Normal.png
│   │   │   ├── T_UnevenBrick_Normal.png
│   │   │   └── T_WoodTrim_Normal.png
│   │   ├── T_BottomWear.png
│   │   ├── T_Brick_BaseColor.png
│   │   ├── T_Brick_Normal.png
│   │   ├── T_Brick_Roughness.png
│   │   ├── T_BrushedNoise.png
│   │   ├── T_Noise_Terrain.png
│   │   ├── T_Plaster_BaseColor.png
│   │   ├── T_Plaster_Normal.png
│   │   ├── T_Plaster_ORM.png
│   │   ├── T_RedBrick_BaseColor.png
│   │   ├── T_RockTrim_BaseColor.png
│   │   ├── T_RockTrim_Normal.png
│   │   ├── T_RockTrim_ORM.png
│   │   ├── T_RoundTiles_BaseColor.png
│   │   ├── T_RoundTiles_Normal.png
│   │   ├── T_RoundTiles_Roughness.png
│   │   ├── T_TopWear.png
│   │   ├── T_UnevenBrick_BaseColor.png
│   │   ├── T_UnevenBrick_Normal.png
│   │   ├── T_UnevenBrick_Roughness.png
│   │   ├── T_VineLeaf.png
│   │   ├── T_WindowGradient.png
│   │   ├── T_WoodTrim_BaseColor.png
│   │   ├── T_WoodTrim_Normal.png
│   │   ├── T_WoodTrim_ORM.png
│   │   └── T_WoodTrim_Roughness.png
│   ├── License_Standard.txt
│   └── Preview.jpg
├── Modular Character Outfits - Fantasy[Standard]
│   ├── Exports
│   │   ├── FBX (Unity)
│   │   │   ├── Modular Parts
│   │   │   │   ├── Female_Peasant_Arms.fbx
│   │   │   │   ├── Female_Peasant_Body.fbx
│   │   │   │   ├── Female_Peasant_Feet.fbx
│   │   │   │   ├── Female_Peasant_Legs.fbx
│   │   │   │   ├── Female_Ranger_Acc_Pauldrons.fbx
│   │   │   │   ├── Female_Ranger_Arms.fbx
│   │   │   │   ├── Female_Ranger_Body.fbx
│   │   │   │   ├── Female_Ranger_Feet.fbx
│   │   │   │   ├── Female_Ranger_Head_Hood.fbx
│   │   │   │   ├── Female_Ranger_Legs.fbx
│   │   │   │   ├── Male_Peasant_Arms.fbx
│   │   │   │   ├── Male_Peasant_Body.fbx
│   │   │   │   ├── Male_Peasant_Feet.fbx
│   │   │   │   ├── Male_Peasant_Legs.fbx
│   │   │   │   ├── Male_Ranger_Acc_Pauldron.fbx
│   │   │   │   ├── Male_Ranger_Arms.fbx
│   │   │   │   ├── Male_Ranger_Body.fbx
│   │   │   │   ├── Male_Ranger_Feet_Boots.fbx
│   │   │   │   ├── Male_Ranger_Head_Hood.fbx
│   │   │   │   └── Male_Ranger_Legs.fbx
│   │   │   └── Outfits
│   │   │       ├── Female_Peasant.fbx
│   │   │       ├── Female_Ranger.fbx
│   │   │       ├── Male_Peasant.fbx
│   │   │       └── Male_Ranger.fbx
│   │   └── glTF (Godot-Unreal)
│   │       ├── Modular Parts
│   │       │   ├── Female_Peasant_Arms.bin
│   │       │   ├── Female_Peasant_Arms.gltf
│   │       │   ├── Female_Peasant_Body.bin
│   │       │   ├── Female_Peasant_Body.gltf
│   │       │   ├── Female_Peasant_Feet.bin
│   │       │   ├── Female_Peasant_Feet.gltf
│   │       │   ├── Female_Peasant_Legs.bin
│   │       │   ├── Female_Peasant_Legs.gltf
│   │       │   ├── Female_Ranger_Acc_Pauldrons.bin
│   │       │   ├── Female_Ranger_Acc_Pauldrons.gltf
│   │       │   ├── Female_Ranger_Arms.bin
│   │       │   ├── Female_Ranger_Arms.gltf
│   │       │   ├── Female_Ranger_Body.bin
│   │       │   ├── Female_Ranger_Body.gltf
│   │       │   ├── Female_Ranger_Feet.bin
│   │       │   ├── Female_Ranger_Feet.gltf
│   │       │   ├── Female_Ranger_Head_Hood.bin
│   │       │   ├── Female_Ranger_Head_Hood.gltf
│   │       │   ├── Female_Ranger_Legs.bin
│   │       │   ├── Female_Ranger_Legs.gltf
│   │       │   ├── Male_Peasant_Arms.bin
│   │       │   ├── Male_Peasant_Arms.gltf
│   │       │   ├── Male_Peasant_Body.bin
│   │       │   ├── Male_Peasant_Body.gltf
│   │       │   ├── Male_Peasant_Feet.bin
│   │       │   ├── Male_Peasant_Feet.gltf
│   │       │   ├── Male_Peasant_Legs.bin
│   │       │   ├── Male_Peasant_Legs.gltf
│   │       │   ├── Male_Ranger_Acc_Pauldron.bin
│   │       │   ├── Male_Ranger_Acc_Pauldron.gltf
│   │       │   ├── Male_Ranger_Arms.bin
│   │       │   ├── Male_Ranger_Arms.gltf
│   │       │   ├── Male_Ranger_Body.bin
│   │       │   ├── Male_Ranger_Body.gltf
│   │       │   ├── Male_Ranger_Feet_Boots.bin
│   │       │   ├── Male_Ranger_Feet_Boots.gltf
│   │       │   ├── Male_Ranger_Head_Hood.bin
│   │       │   ├── Male_Ranger_Head_Hood.gltf
│   │       │   ├── Male_Ranger_Legs.bin
│   │       │   ├── Male_Ranger_Legs.gltf
│   │       │   ├── T_Peasant_BaseColor.png
│   │       │   ├── T_Peasant_Normal.png
│   │       │   ├── T_Peasant_ORM.png
│   │       │   ├── T_Ranger_BaseColor.png
│   │       │   ├── T_Ranger_Normal.png
│   │       │   ├── T_Ranger_ORM.png
│   │       │   ├── T_Regular_Female_Dark_BaseColor.png
│   │       │   ├── T_Regular_Female_Normal.png
│   │       │   ├── T_Regular_Female_Roughness.png
│   │       │   ├── T_Regular_Male_Dark_BaseColor.png
│   │       │   ├── T_Regular_Male_Normal.png
│   │       │   └── T_Regular_Male_Roughness.png
│   │       └── Outfits
│   │           ├── Female_Peasant.bin
│   │           ├── Female_Peasant.gltf
│   │           ├── Female_Ranger.bin
│   │           ├── Female_Ranger.gltf
│   │           ├── Male_Peasant.bin
│   │           ├── Male_Peasant.gltf
│   │           ├── Male_Ranger.bin
│   │           ├── Male_Ranger.gltf
│   │           ├── T_Peasant_BaseColor.png
│   │           ├── T_Peasant_Normal.png
│   │           ├── T_Peasant_ORM.png
│   │           ├── T_Ranger_BaseColor.png
│   │           ├── T_Ranger_Normal.png
│   │           ├── T_Ranger_ORM.png
│   │           ├── T_Regular_Female_Dark_BaseColor.png
│   │           ├── T_Regular_Female_Normal.png
│   │           ├── T_Regular_Female_Roughness.png
│   │           ├── T_Regular_Male_Dark_BaseColor.png
│   │           ├── T_Regular_Male_Normal.png
│   │           └── T_Regular_Male_Roughness.png
│   ├── Textures
│   │   ├── Base
│   │   │   ├── T_Regular_Female_Dark_BaseColor.png
│   │   │   ├── T_Regular_Female_Normal.png
│   │   │   ├── T_Regular_Female_Roughness.png
│   │   │   ├── T_Regular_Male_Dark_BaseColor.png
│   │   │   ├── T_Regular_Male_Normal.png
│   │   │   └── T_Regular_Male_Roughness.png
│   │   ├── Peasant
│   │   │   ├── Normals-UnrealEngine
│   │   │   │   └── T_Peasant_Normal.png
│   │   │   ├── T_Peasant_2_BaseColor.png
│   │   │   ├── T_Peasant_BaseColor.png
│   │   │   ├── T_Peasant_Normal.png
│   │   │   └── T_Peasant_ORM.png
│   │   └── Ranger
│   │       ├── Base Chars
│   │       │   ├── T_Regular_Female_Dark_BaseColor.png
│   │       │   ├── T_Regular_Female_Normal.png
│   │       │   ├── T_Regular_Female_Roughness.png
│   │       │   ├── T_Regular_Male_Dark_BaseColor.png
│   │       │   ├── T_Regular_Male_Normal.png
│   │       │   └── T_Regular_Male_Roughness.png
│   │       ├── Normals-UnrealEngine
│   │       │   └── T_Ranger_Normal.png
│   │       ├── T_Ranger_3_BaseColor.png
│   │       ├── T_Ranger_BaseColor.png
│   │       ├── T_Ranger_Normal.png
│   │       └── T_Ranger_ORM.png
│   ├── License_Standard.txt
│   ├── Preview.jpg
│   └── Readme.txt
├── Universal Animation Library 2[Standard]
│   ├── Female Mannequin
│   │   ├── Unity
│   │   │   └── Mannequin_F.fbx
│   │   ├── Unreal-Godot
│   │   │   └── Mannequin_F.glb
│   │   ├── Mannequin_F.blend
│   │   └── README.txt
│   ├── Unity
│   │   ├── UAL2_Standard_RM.fbx
│   │   └── UAL2_Standard.fbx
│   ├── Unreal-Godot
│   │   ├── UAL2_Standard_RM.glb
│   │   └── UAL2_Standard.glb
│   ├── Godot_Setup.png
│   ├── License.txt
│   ├── README.txt
│   ├── Unity_Setup.png
│   └── Unreal_Setup.png
├── Universal Animation Library[Standard]
│   ├── Unity
│   │   ├── UAL1_Standard_RM.fbx
│   │   └── UAL1_Standard.fbx
│   ├── Unreal-Godot
│   │   ├── UAL1_Standard_RM.glb
│   │   └── UAL1_Standard.glb
│   ├── Godot_Setup.png
│   ├── License.txt
│   ├── README.txt
│   ├── Unity_Setup.png
│   └── Unreal_Setup.png
└── Universal Base Characters[Standard]
    ├── Base Characters
    │   ├── Godot - UE
    │   │   ├── Superhero_Female_FullBody.bin
    │   │   ├── Superhero_Female_FullBody.gltf
    │   │   ├── Superhero_Male_FullBody.bin
    │   │   ├── Superhero_Male_FullBody.gltf
    │   │   ├── T_Eye_Brown.png
    │   │   ├── T_Eye_Normal.png
    │   │   ├── T_Hair_1_BaseColor_png.png
    │   │   ├── T_Hair_1_BaseColor.png
    │   │   ├── T_Hair_1_Normal.png
    │   │   ├── T_Hair_2_BaseColor_png.png
    │   │   ├── T_Hair_2_BaseColor.png
    │   │   ├── T_Hair_2_Normal.png
    │   │   ├── T_Superhero_Female_Dark_BaseColor.png
    │   │   ├── T_Superhero_Female_Normal.png
    │   │   ├── T_Superhero_Female_Roughness.png
    │   │   ├── T_Superhero_Male_Dark.png
    │   │   ├── T_Superhero_Male_Normal.png
    │   │   └── T_Superhero_Male_Roughness.png
    │   ├── Textures
    │   │   ├── Normals Unity - Godot
    │   │   │   ├── T_Eye_Normal.png
    │   │   │   ├── T_Hair_1_Normal.png
    │   │   │   ├── T_Hair_2_Normal.png
    │   │   │   ├── T_Superhero_Female_Normal.png
    │   │   │   └── T_Superhero_Male_Normal.png
    │   │   ├── T_Eye_Brown.png
    │   │   ├── T_Eye_Normal.png
    │   │   ├── T_Hair_1_BaseColor.png
    │   │   ├── T_Hair_1_Normal.png
    │   │   ├── T_Hair_2_BaseColor.png
    │   │   ├── T_Hair_2_Normal.png
    │   │   ├── T_Superhero_Female_Dark_BaseColor.png
    │   │   ├── T_Superhero_Female_Light_BaseColor.png
    │   │   ├── T_Superhero_Female_Normal.png
    │   │   ├── T_Superhero_Female_Roughness.png
    │   │   ├── T_Superhero_Male_Dark.png
    │   │   ├── T_Superhero_Male_Ligh.png
    │   │   ├── T_Superhero_Male_Normal.png
    │   │   └── T_Superhero_Male_Roughness.png
    │   ├── Unity
    │   │   ├── Superhero_Female_FullBody.fbx
    │   │   └── Superhero_Male_FullBody.fbx
    │   └── Unreal-Engine-README.txt
    ├── Hairstyles
    │   ├── Origin at 0
    │   │   ├── FBX (Unity)
    │   │   │   ├── Eyebrows_Female.fbx
    │   │   │   ├── Eyebrows_Regular.fbx
    │   │   │   ├── Hair_Beard.fbx
    │   │   │   ├── Hair_Buns.fbx
    │   │   │   ├── Hair_Buzzed.fbx
    │   │   │   ├── Hair_BuzzedFemale.fbx
    │   │   │   ├── Hair_Long.fbx
    │   │   │   └── Hair_SimpleParted.fbx
    │   │   ├── FBX (Unreal Engine)
    │   │   │   ├── Eyebrows_Female.fbx
    │   │   │   ├── Eyebrows_Regular.fbx
    │   │   │   ├── Hair_Beard.fbx
    │   │   │   ├── Hair_Buns.fbx
    │   │   │   ├── Hair_Buzzed.fbx
    │   │   │   ├── Hair_BuzzedFemale.fbx
    │   │   │   ├── Hair_Long.fbx
    │   │   │   └── Hair_SimpleParted.fbx
    │   │   └── glTF (Godot)
    │   │       ├── Eyebrows_Female.bin
    │   │       ├── Eyebrows_Female.gltf
    │   │       ├── Eyebrows_Regular.bin
    │   │       ├── Eyebrows_Regular.gltf
    │   │       ├── Hair_Beard.bin
    │   │       ├── Hair_Beard.gltf
    │   │       ├── Hair_Buns.bin
    │   │       ├── Hair_Buns.gltf
    │   │       ├── Hair_Buzzed.bin
    │   │       ├── Hair_Buzzed.gltf
    │   │       ├── Hair_BuzzedFemale.bin
    │   │       ├── Hair_BuzzedFemale.gltf
    │   │       ├── Hair_Long.bin
    │   │       ├── Hair_Long.gltf
    │   │       ├── Hair_SimpleParted.bin
    │   │       ├── Hair_SimpleParted.gltf
    │   │       ├── T_Hair_1_BaseColor.png
    │   │       ├── T_Hair_1_Normal.png
    │   │       ├── T_Hair_2_BaseColor.png
    │   │       └── T_Hair_2_Normal.png
    │   ├── Rigged to Head Bone
    │   │   ├── FBX (Unity)
    │   │   │   ├── Eyebrows_Female.fbx
    │   │   │   ├── Eyebrows_Regular.fbx
    │   │   │   ├── Hair_Beard.fbx
    │   │   │   ├── Hair_Buns.fbx
    │   │   │   ├── Hair_Buzzed.fbx
    │   │   │   ├── Hair_BuzzedFemale.fbx
    │   │   │   ├── Hair_Long.fbx
    │   │   │   └── Hair_SimpleParted.fbx
    │   │   └── glTF (Godot -Unreal)
    │   │       ├── Eyebrows_Female.bin
    │   │       ├── Eyebrows_Female.gltf
    │   │       ├── Eyebrows_Regular.bin
    │   │       ├── Eyebrows_Regular.gltf
    │   │       ├── Hair_Beard.bin
    │   │       ├── Hair_Beard.gltf
    │   │       ├── Hair_Buns.bin
    │   │       ├── Hair_Buns.gltf
    │   │       ├── Hair_Buzzed.bin
    │   │       ├── Hair_Buzzed.gltf
    │   │       ├── Hair_BuzzedFemale.bin
    │   │       ├── Hair_BuzzedFemale.gltf
    │   │       ├── Hair_Long.bin
    │   │       ├── Hair_Long.gltf
    │   │       ├── Hair_SimpleParted.bin
    │   │       ├── Hair_SimpleParted.gltf
    │   │       ├── T_Hair_1_BaseColor.png
    │   │       ├── T_Hair_1_Normal.png
    │   │       ├── T_Hair_2_BaseColor.png
    │   │       └── T_Hair_2_Normal.png
    │   └── Textures
    │       ├── Normals Unity - Godot
    │       │   ├── T_Hair_1_Normal.png
    │       │   └── T_Hair_2_Normal.png
    │       ├── T_Hair_1_BaseColor.png
    │       ├── T_Hair_1_Normal.png
    │       ├── T_Hair_2_BaseColor.png
    │       └── T_Hair_2_Normal.png
    ├── License_Standard.txt
    └── Preview.png
```
