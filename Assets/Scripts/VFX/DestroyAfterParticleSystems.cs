using UnityEngine;

/// <summary>
/// 자식 파티클이 모두 끝나면(대략의 최대 지속시간 이후) GameObject를 제거합니다.
/// 프리팹으로 폭발 이펙트를 사용할 때 잔존 오브젝트를 남기지 않기 위함입니다.
/// </summary>
public sealed class DestroyAfterParticleSystems : MonoBehaviour
{
    [Min(0f)] public float extraDelay = 0.5f;

    void OnEnable()
    {
        var max = GetMaxLifetimeSeconds(gameObject);
        Destroy(gameObject, Mathf.Clamp(max + extraDelay, 0.25f, 20f));
    }

    static float GetMaxLifetimeSeconds(GameObject root)
    {
        var systems = root.GetComponentsInChildren<ParticleSystem>(true);
        var max = 0f;

        foreach (var ps in systems)
        {
            if (ps == null) continue;

            var main = ps.main;
            var dur = main.duration;
            var life = main.startLifetime;

            float lifeMax;
            switch (life.mode)
            {
                case ParticleSystemCurveMode.Constant:
                    lifeMax = life.constant;
                    break;
                case ParticleSystemCurveMode.TwoConstants:
                    lifeMax = life.constantMax;
                    break;
                default:
                    lifeMax = 2f;
                    break;
            }

            max = Mathf.Max(max, dur + lifeMax);
        }

        return max;
    }
}

