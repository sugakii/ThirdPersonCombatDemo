# Asset Audit

> 扫描日期：2026-09-02  
> 扫描源：`MaterialPackage/` 实际本地文件、FBX/GLB 结构、随包 README/导入图和许可证  
> 工程状态：所选素材尚未导入 `Assets/`；所有 Unity Avatar、材质、动画导入设置和场景表现仍待 Unity 验证

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

当前 Unity `Assets/` 中：

- FBX：0
- 为这些模型生成的 Avatar：0
- 正式 `.mat`：0
- Animator Controller：0

所以当前审计能确认源文件内容，但不能声称 Unity 中已经完成 Humanoid、材质、Loop、重定向、朝向、缩放或碰撞验证。

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

- 当前工程中 **尚无 Imp Avatar**。
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

- **Humanoid / Generic**：源 FBX 不能证明 Unity 导入类型已经设置；随包说明要求 Unity 设置为 Humanoid。当前尚未配置。
- **Root Motion**：README 明确 `_RM` 文件把 root motion 烘焙进每条动画；无 `_RM` 文件禁用 root motion。
- **In-Place**：本项目选择的两个非 RM FBX为原地版本；抽查 GLB root translation 恒为 `(0,0,0)`。
- **代码位移**：统一由 CharacterController/PlayerMotor 执行，`Animator.applyRootMotion = false`。
- **Loop**：随包说明要求所有以 `_Loop` 结尾的动作在 Unity 中手工开启 Loop Time；当前只是命名和导入要求，尚未配置。
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

1. **Unity 验证尚未发生**：素材仍在 `Assets/` 外；Avatar、材质、Clip 名称、Loop、重定向、朝向、比例、Pivot、碰撞和 NavMesh 均未在工程中确认。
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

- [ ] Imp Model Importer 开启 Bake Axis Conversion。
- [ ] Imp Rig = Humanoid，Avatar Configure 有效。
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

