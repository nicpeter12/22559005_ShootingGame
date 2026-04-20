using UnityEngine;

public static class IndustrialDroneExplosionVFX
{
    static Material _cachedAdditive;
    static Material _cachedAlpha;

    /// <summary>
    /// 파티클/라이트를 주어진 parent 아래에 구성하고, 전체 최대 지속시간(대략)을 반환합니다.
    /// 프리팹 제작(에디터)과 런타임 생성 모두에서 공용으로 씁니다.
    /// </summary>
    public static float BuildInto(Transform parent, float scale = 1f, bool play = true)
    {
        if (parent == null) return 0f;
        scale = Mathf.Max(0.01f, scale);

        var maxLifetime = 0.0f;

        var oldScale = parent.localScale;
        parent.localScale = Vector3.Scale(oldScale, Vector3.one * scale);

        maxLifetime = Mathf.Max(maxLifetime, AddFlash(parent, play));
        maxLifetime = Mathf.Max(maxLifetime, AddShockwave(parent, play));
        maxLifetime = Mathf.Max(maxLifetime, AddSparks(parent, play));
        maxLifetime = Mathf.Max(maxLifetime, AddDebrisDust(parent, play));
        maxLifetime = Mathf.Max(maxLifetime, AddSmoke(parent, play));
        maxLifetime = Mathf.Max(maxLifetime, AddEmbers(parent, play));
        maxLifetime = Mathf.Max(maxLifetime, AddLight(parent));

        parent.localScale = oldScale;
        return maxLifetime;
    }

    public static GameObject Spawn(
        Vector3 position,
        Quaternion rotation = default,
        Transform parent = null,
        float scale = 1f)
    {
        if (rotation == default) rotation = Quaternion.identity;
        scale = Mathf.Max(0.01f, scale);

        var root = new GameObject("IndustrialDroneExplosionVFX");
        root.transform.SetPositionAndRotation(position, rotation);
        root.transform.localScale = Vector3.one;
        if (parent != null) root.transform.SetParent(parent, true);

        var maxLifetime = BuildInto(root.transform, scale, play: true);

        Object.Destroy(root, Mathf.Clamp(maxLifetime + 0.5f, 1.0f, 12.0f));
        return root;
    }

    static float AddFlash(Transform parent, bool play)
    {
        var go = new GameObject("Flash");
        go.transform.SetParent(parent, false);

        var ps = go.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.loop = false;
        main.duration = 0.12f;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.05f, 0.10f);
        main.startSpeed = 0f;
        main.startSize = new ParticleSystem.MinMaxCurve(0.8f, 1.4f);
        main.startColor = new Color(1f, 0.95f, 0.75f, 0.95f);
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.scalingMode = ParticleSystemScalingMode.Local;
        main.maxParticles = 16;

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new[]
        {
            new ParticleSystem.Burst(0f, 1, 2),
        });

        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.05f;

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(
            new Gradient
            {
                colorKeys = new[]
                {
                    new GradientColorKey(new Color(1f, 0.95f, 0.75f), 0f),
                    new GradientColorKey(new Color(1f, 0.6f, 0.2f), 1f),
                },
                alphaKeys = new[]
                {
                    new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(0f, 1f),
                }
            });

        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.material = MakeMaterial("Particles/Additive");
        renderer.sortingFudge = 2;

        if (play) ps.Play();
        return 0.25f;
    }

    static float AddShockwave(Transform parent, bool play)
    {
        var go = new GameObject("Shockwave");
        go.transform.SetParent(parent, false);
        go.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);

        var ps = go.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.loop = false;
        main.duration = 0.35f;
        main.startLifetime = 0.35f;
        main.startSpeed = 0f;
        main.startSize = 0.25f;
        main.startColor = new Color(1f, 0.9f, 0.7f, 0.65f);
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = 4;

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, 1) });

        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.05f;
        shape.arcMode = ParticleSystemShapeMultiModeValue.Random;

        var sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(
            1f,
            new AnimationCurve(
                new Keyframe(0f, 0.2f),
                new Keyframe(1f, 3.2f)));

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(
            new Gradient
            {
                colorKeys = new[]
                {
                    new GradientColorKey(new Color(1f, 0.85f, 0.6f), 0f),
                    new GradientColorKey(new Color(0.4f, 0.4f, 0.4f), 1f),
                },
                alphaKeys = new[]
                {
                    new GradientAlphaKey(0.6f, 0f),
                    new GradientAlphaKey(0f, 1f),
                }
            });

        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.material = MakeMaterial("Particles/Alpha Blended");
        renderer.sortingFudge = 1;

        if (play) ps.Play();
        return 0.6f;
    }

    static float AddSparks(Transform parent, bool play)
    {
        var go = new GameObject("Sparks");
        go.transform.SetParent(parent, false);

        var ps = go.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.loop = false;
        main.duration = 0.25f;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.15f, 0.5f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(6f, 13f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.03f, 0.08f);
        main.startColor = new Color(1f, 0.85f, 0.35f, 1f);
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.gravityModifier = 1.2f;
        main.maxParticles = 128;

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, 20, 35) });

        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.radius = 0.08f;
        shape.angle = 55f;
        shape.length = 0.08f;

        var velocity = ps.velocityOverLifetime;
        velocity.enabled = true;
        velocity.space = ParticleSystemSimulationSpace.Local;
        velocity.radial = new ParticleSystem.MinMaxCurve(0.0f, 0.75f);

        var limitVelocity = ps.limitVelocityOverLifetime;
        limitVelocity.enabled = true;
        limitVelocity.limit = 22f;
        limitVelocity.dampen = 0.25f;

        var trails = ps.trails;
        trails.enabled = true;
        trails.mode = ParticleSystemTrailMode.PerParticle;
        trails.lifetime = new ParticleSystem.MinMaxCurve(0.05f, 0.12f);
        trails.widthOverTrail = new ParticleSystem.MinMaxCurve(1f, new AnimationCurve(
            new Keyframe(0f, 1f),
            new Keyframe(1f, 0f)));
        trails.colorOverTrail = new ParticleSystem.MinMaxGradient(new Color(1f, 0.65f, 0.2f, 0.6f));

        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Stretch;
        renderer.velocityScale = 0.15f;
        renderer.lengthScale = 2.2f;
        renderer.material = MakeMaterial("Particles/Additive");
        renderer.sortingFudge = 0;

        if (play) ps.Play();
        return 1.2f;
    }

    static float AddDebrisDust(Transform parent, bool play)
    {
        var go = new GameObject("DebrisDust");
        go.transform.SetParent(parent, false);

        var ps = go.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.loop = false;
        main.duration = 0.25f;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.25f, 0.8f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(2.5f, 6.5f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.05f, 0.18f);
        main.startRotation3D = true;
        main.startRotationX = new ParticleSystem.MinMaxCurve(0f, 360f);
        main.startRotationY = new ParticleSystem.MinMaxCurve(0f, 360f);
        main.startRotationZ = new ParticleSystem.MinMaxCurve(0f, 360f);
        main.startColor = new Color(0.25f, 0.25f, 0.25f, 0.9f);
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.gravityModifier = 1.6f;
        main.maxParticles = 96;

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, 8, 14) });

        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.07f;

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(
            new Gradient
            {
                colorKeys = new[]
                {
                    new GradientColorKey(new Color(0.25f, 0.25f, 0.25f), 0f),
                    new GradientColorKey(new Color(0.1f, 0.1f, 0.1f), 1f),
                },
                alphaKeys = new[]
                {
                    new GradientAlphaKey(0.9f, 0f),
                    new GradientAlphaKey(0f, 1f),
                }
            });

        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.material = MakeMaterial("Particles/Alpha Blended");
        renderer.sortingFudge = -1;

        if (play) ps.Play();
        return 1.3f;
    }

    static float AddSmoke(Transform parent, bool play)
    {
        var go = new GameObject("Smoke");
        go.transform.SetParent(parent, false);

        var ps = go.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.loop = false;
        main.duration = 0.65f;
        main.startLifetime = new ParticleSystem.MinMaxCurve(1.2f, 2.2f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.25f, 1.2f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.55f, 1.25f);
        main.startRotation = new ParticleSystem.MinMaxCurve(0f, 360f * Mathf.Deg2Rad);
        main.startColor = new Color(0.18f, 0.18f, 0.18f, 0.75f);
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.gravityModifier = -0.05f;
        main.maxParticles = 64;

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, 10, 16) });

        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.12f;

        var velocity = ps.velocityOverLifetime;
        velocity.enabled = true;
        velocity.space = ParticleSystemSimulationSpace.World;
        // Velocity module의 X/Y/Z는 모두 같은 모드여야 경고가 없습니다.
        velocity.x = new ParticleSystem.MinMaxCurve(0f, 0f);
        velocity.y = new ParticleSystem.MinMaxCurve(0.4f, 1.1f);
        velocity.z = new ParticleSystem.MinMaxCurve(0f, 0f);

        var sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(
            1f,
            new AnimationCurve(
                new Keyframe(0f, 0.55f),
                new Keyframe(0.2f, 1.1f),
                new Keyframe(1f, 1.9f)));

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(
            new Gradient
            {
                colorKeys = new[]
                {
                    new GradientColorKey(new Color(0.20f, 0.20f, 0.20f), 0f),
                    new GradientColorKey(new Color(0.12f, 0.12f, 0.12f), 0.6f),
                    new GradientColorKey(new Color(0.08f, 0.08f, 0.08f), 1f),
                },
                alphaKeys = new[]
                {
                    new GradientAlphaKey(0.0f, 0f),
                    new GradientAlphaKey(0.75f, 0.12f),
                    new GradientAlphaKey(0.55f, 0.5f),
                    new GradientAlphaKey(0.0f, 1f),
                }
            });

        var noise = ps.noise;
        noise.enabled = true;
        noise.strength = 0.35f;
        noise.frequency = 0.35f;
        noise.scrollSpeed = 0.25f;
        noise.damping = true;
        noise.octaveCount = 2;
        noise.octaveScale = 0.5f;
        noise.octaveMultiplier = 0.7f;

        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.material = MakeMaterial("Particles/Alpha Blended");
        renderer.sortingFudge = -2;

        if (play) ps.Play();
        return 3.0f;
    }

    static float AddEmbers(Transform parent, bool play)
    {
        var go = new GameObject("Embers");
        go.transform.SetParent(parent, false);

        var ps = go.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.loop = false;
        main.duration = 0.35f;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.6f, 1.4f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.5f, 2.2f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.02f, 0.05f);
        main.startColor = new Color(1f, 0.5f, 0.1f, 0.85f);
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.gravityModifier = -0.15f;
        main.maxParticles = 64;

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, 10, 18) });

        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.08f;

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(
            new Gradient
            {
                colorKeys = new[]
                {
                    new GradientColorKey(new Color(1f, 0.65f, 0.15f), 0f),
                    new GradientColorKey(new Color(0.55f, 0.18f, 0.05f), 1f),
                },
                alphaKeys = new[]
                {
                    new GradientAlphaKey(0.85f, 0f),
                    new GradientAlphaKey(0.0f, 1f),
                }
            });

        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.material = MakeMaterial("Particles/Additive");
        renderer.sortingFudge = 0;

        if (play) ps.Play();
        return 2.0f;
    }

    static float AddLight(Transform parent)
    {
        var go = new GameObject("FlashLight");
        go.transform.SetParent(parent, false);
        go.transform.localPosition = Vector3.up * 0.1f;

        var light = go.AddComponent<Light>();
        light.type = LightType.Point;
        light.range = 6f;
        light.intensity = 6.5f;
        light.color = new Color(1f, 0.85f, 0.55f);

        var fader = go.AddComponent<LightFader>();
        fader.Duration = 0.18f;
        fader.Curve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

        return 0.25f;
    }

    static Material MakeMaterial(string shaderName)
    {
        // URP 프로젝트에서 Legacy 파티클 셰이더(Particles/*)는 없거나 핑크가 될 수 있어서,
        // URP 파티클 셰이더를 최우선으로 사용합니다.
        var wantAdditive = shaderName != null && shaderName.Contains("Additive");

        if (wantAdditive && _cachedAdditive != null) return _cachedAdditive;
        if (!wantAdditive && _cachedAlpha != null) return _cachedAlpha;

        Shader shader =
            Shader.Find("Universal Render Pipeline/Particles/Unlit") ??
            Shader.Find("Universal Render Pipeline/Particles/Lit") ??
            Shader.Find(shaderName) ??
            Shader.Find("Particles/Standard Unlit") ??
            Shader.Find("Unlit/Transparent");

        var mat = new Material(shader);

        // URP 파티클 셰이더의 Blend 모드는 프로젝트/버전에 따라 프로퍼티가 다를 수 있어
        // 존재하는 경우에만 안전하게 셋업합니다.
        if (mat.HasProperty("_Blend"))
        {
            // URP 기본: 0=Alpha, 1=Premultiply, 2=Additive, 3=Multiply (대부분 이 매핑)
            mat.SetFloat("_Blend", wantAdditive ? 2f : 0f);
        }

        mat.renderQueue = wantAdditive ? 3000 : 3000;

        if (wantAdditive) _cachedAdditive = mat;
        else _cachedAlpha = mat;

        return mat;
    }

    // LightFader는 프리팹 저장을 위해 별도 스크립트 파일로 분리됩니다.
}

