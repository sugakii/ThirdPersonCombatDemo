using UnityEngine;

/// <summary>
/// 让世界空间 UI 保持与 Gameplay Camera 相同的旋转，从各个镜头角度维持可读性。
/// </summary>
public class WorldSpaceBillboard : MonoBehaviour
{
    private Transform cameraTransform;

    private void Awake()
    {
        // Prefab 不能保存场景对象引用，因此实例启用时只查找一次 Main Camera，避免在 LateUpdate 重复搜索。
        Camera mainCamera = Camera.main;

        if(mainCamera == null)
        {
            Debug.LogError("WorldSpaceBillboard: Main Camera was not found.", this);

            enabled = false;
            return;
        }

        cameraTransform = mainCamera.transform;
    }

    private void LateUpdate()
    {
        // 在相机完成本帧移动后更新，避免世界空间血条产生一帧滞后。
        transform.rotation = cameraTransform.rotation;
    }
}
