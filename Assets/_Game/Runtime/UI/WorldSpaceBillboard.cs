using UnityEngine;

/// <summary>
/// 让世界空间 UI 保持与 Gameplay Camera 相同的旋转，从各个镜头角度维持可读性。
/// </summary>
public class WorldSpaceBillboard : MonoBehaviour
{
    // 由场景或 Prefab 显式指定，避免每帧搜索 Main Camera。
    [SerializeField]
    private Transform cameraTransform;

    private void LateUpdate()
    {
        // 在相机完成本帧移动后更新，避免世界空间血条产生一帧滞后。
        transform.rotation = cameraTransform.rotation;
    }
}
