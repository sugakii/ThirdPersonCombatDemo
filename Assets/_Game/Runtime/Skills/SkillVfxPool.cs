using UnityEngine;
using UnityEngine.Pool;
using System.Collections;

/// <summary>
/// 复用火焰突进表现，并在归还对象前清理粒子和 Trail 的运行时残留。
/// </summary>
public class SkillVfxPool : MonoBehaviour
{
    [SerializeField]
    private GameObject vfxPrefab;

    [SerializeField]
    private SkinnedMeshRenderer weaponRenderer;

    private int defaultCapacity = 2;

    private int maxSize = 4;

    private ObjectPool<GameObject> pool;

    private GameObject CreateVfx()
    {
        GameObject vfxInstance = Instantiate(vfxPrefab);

        vfxInstance.SetActive(false);

        return vfxInstance;
    }

    private void OnGetVfx(GameObject vfx)
    {
        vfx.SetActive(true);
    }

    private void OnReleaseVfx(GameObject vfx)
    {
        ParticleSystem particles = vfx.GetComponentInChildren<ParticleSystem>();
        TrailRenderer trail = vfx.GetComponentInChildren<TrailRenderer>();

        particles.Stop(
            true,
            ParticleSystemStopBehavior.StopEmittingAndClear
        );

        trail.emitting = false; 
        trail.Clear();

        vfx.transform.SetParent(null);

        vfx.SetActive(false);
    }

    private void DestroyVfx(GameObject vfx)
    {
        if (vfx == null)
        {
            return;
        }

        if (Application.isPlaying)
        {
            Destroy(vfx);
        }
        else
        {
            DestroyImmediate(vfx);
        }
    }

    public void Play(Transform followTarget, float duration)
    {
        if (weaponRenderer == null)
        {
            Debug.LogWarning("未绑定特效使用的武器模型。", this);
            return;
        }

        GameObject vfx = pool.Get();

        ParticleSystem particles =
            vfx.GetComponentInChildren<ParticleSystem>();

        TrailRenderer trail =
            vfx.GetComponentInChildren<TrailRenderer>();

        vfx.transform.SetParent(followTarget);
        vfx.transform.localPosition = vfxPrefab.transform.localPosition;
        vfx.transform.localRotation = Quaternion.identity;

        // 清除上次播放残留的粒子。
        particles.Stop(
            true,
            ParticleSystemStopBehavior.StopEmittingAndClear
        );

        // 火焰和子物体上的火星，都从武器表面发射。
        foreach (ParticleSystem system in
                 particles.GetComponentsInChildren<ParticleSystem>(true))
        {
            var main = system.main;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            var shape = system.shape;
            shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.SkinnedMeshRenderer;
            shape.skinnedMeshRenderer = weaponRenderer;
            shape.meshShapeType = ParticleSystemMeshShapeType.Triangle;
            shape.useMeshColors = false;

            // 清除预制体原来的盒状发射范围变换。
            shape.position = Vector3.zero;
            shape.rotation = Vector3.zero;
            shape.scale = Vector3.one;
        }

        trail.Clear();
        trail.emitting = true;
        particles.Play(true);

        StartCoroutine(ReleaseAfterDelay(vfx, duration));
    }

    private IEnumerator ReleaseAfterDelay(GameObject vfx, float duration)
    {
        yield return new WaitForSeconds(duration);

        ParticleSystem particles =
            vfx.GetComponentInChildren<ParticleSystem>();

        TrailRenderer trail =
            vfx.GetComponentInChildren<TrailRenderer>();

        // 冲刺结束，只停止产生新粒子，保留已有粒子。
        particles.Stop(
            true,
            ParticleSystemStopBehavior.StopEmitting
        );

        trail.emitting = false;

        // 等轨迹消退。
        yield return new WaitForSeconds(trail.time);

        // 等火焰和子物体上的火星全部消散。
        while (particles.IsAlive(true))
        {
            yield return null;
        }

        pool.Release(vfx);
    }

    private void Awake()
    {
        pool = new ObjectPool<GameObject>(
            CreateVfx,
            OnGetVfx,
            OnReleaseVfx,
            DestroyVfx,

            true,
            defaultCapacity,
            maxSize
        );
    }
}
