using UnityEngine;
using UnityEditor;

public static class IndustrialDroneExplosionPrefabTools
{
    const string PrefabPath = "Assets/Prefab/IndustrialDroneExplosion.prefab";
    const string MonsterPrefabPath = "Assets/Prefab/Monster.prefab";
    const string VfxRootFolder = "Assets/VFX";
    const string VfxMatFolder = "Assets/VFX/Materials";
    const string VfxTexFolder = "Assets/VFX/Textures";
    const string AddMatPath = "Assets/VFX/Materials/IndustrialDroneExplosion_Additive.mat";
    const string AlphaMatPath = "Assets/VFX/Materials/IndustrialDroneExplosion_Alpha.mat";
    const string SoftCircleTexPath = "Assets/VFX/Textures/IndustrialDroneParticle_SoftCircle.asset";

    [MenuItem("Tools/VFX/Create Industrial Drone Explosion Prefab")]
    public static void CreatePrefab()
    {
        EnsureFolders();
        EnsureVfxFolders();

        var root = new GameObject("IndustrialDroneExplosion");
        root.AddComponent<DestroyAfterParticleSystems>();

        IndustrialDroneExplosionVFX.BuildInto(root.transform, scale: 1f, play: false);
        SetPlayOnAwakeRecursive(root, true);
        AssignMaterials(root);

        var prefab = PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
        Object.DestroyImmediate(root);

        EditorUtility.SetDirty(prefab);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Selection.activeObject = prefab;
    }

    [MenuItem("Tools/VFX/Create + Replace Monster Explosion Prefab")]
    public static void CreateAndReplaceMonsterExplosion()
    {
        CreatePrefab();

        var explosionPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
        if (explosionPrefab == null)
        {
            Debug.LogError($"Explosion prefab not found at {PrefabPath}");
            return;
        }

        var monsterPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(MonsterPrefabPath);
        if (monsterPrefab == null)
        {
            Debug.LogError($"Monster prefab not found at {MonsterPrefabPath}");
            return;
        }

        var monster = monsterPrefab.GetComponent<Monster>();
        if (monster == null)
        {
            Debug.LogError($"Monster component not found on {MonsterPrefabPath}");
            return;
        }

        monster.prefabsExplosion = explosionPrefab;
        EditorUtility.SetDirty(monsterPrefab);
        PrefabUtility.SavePrefabAsset(monsterPrefab);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Selection.activeObject = monsterPrefab;
        Debug.Log("Monster.prefabsExplosion replaced with IndustrialDroneExplosion prefab.");
    }

    static void EnsureFolders()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Editor"))
            AssetDatabase.CreateFolder("Assets", "Editor");

        if (!AssetDatabase.IsValidFolder("Assets/Prefab"))
            AssetDatabase.CreateFolder("Assets", "Prefab");
    }

    static void EnsureVfxFolders()
    {
        if (!AssetDatabase.IsValidFolder(VfxRootFolder))
            AssetDatabase.CreateFolder("Assets", "VFX");
        if (!AssetDatabase.IsValidFolder(VfxMatFolder))
            AssetDatabase.CreateFolder(VfxRootFolder, "Materials");
        if (!AssetDatabase.IsValidFolder(VfxTexFolder))
            AssetDatabase.CreateFolder(VfxRootFolder, "Textures");
    }

    static void AssignMaterials(GameObject root)
    {
        var tex = EnsureSoftCircleTexture(SoftCircleTexPath, 96);
        var additive = EnsureMaterial(AddMatPath, additive: true);
        var alpha = EnsureMaterial(AlphaMatPath, additive: false);

        ApplyTexture(additive, tex);
        ApplyTexture(alpha, tex);

        var renderers = root.GetComponentsInChildren<ParticleSystemRenderer>(true);
        foreach (var r in renderers)
        {
            if (r == null) continue;
            var n = r.gameObject.name;

            // Flash/Sparks/Embers는 밝게 튀는 느낌이 좋아서 Additive,
            // Shockwave/Smoke/Dust는 Alpha 블렌딩이 자연스럽습니다.
            var useAdd = n.Contains("Flash") || n.Contains("Sparks") || n.Contains("Embers");
            r.sharedMaterial = useAdd ? additive : alpha;
        }
    }

    static Material EnsureMaterial(string path, bool additive)
    {
        var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (mat != null) return mat;

        Shader shader =
            Shader.Find("Universal Render Pipeline/Particles/Unlit") ??
            Shader.Find("Universal Render Pipeline/Particles/Lit") ??
            Shader.Find("Particles/Standard Unlit") ??
            Shader.Find("Unlit/Transparent");

        mat = new Material(shader);
        mat.name = additive ? "IndustrialDroneExplosion_Additive" : "IndustrialDroneExplosion_Alpha";

        if (mat.HasProperty("_Blend"))
            mat.SetFloat("_Blend", additive ? 2f : 0f);

        AssetDatabase.CreateAsset(mat, path);
        AssetDatabase.ImportAsset(path);
        return mat;
    }

    static void ApplyTexture(Material mat, Texture2D tex)
    {
        if (mat == null || tex == null) return;

        if (mat.HasProperty("_BaseMap"))
            mat.SetTexture("_BaseMap", tex);
        if (mat.HasProperty("_MainTex"))
            mat.SetTexture("_MainTex", tex);
    }

    static Texture2D EnsureSoftCircleTexture(string path, int size)
    {
        var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        if (tex != null) return tex;

        size = Mathf.Clamp(size, 16, 256);
        tex = new Texture2D(size, size, TextureFormat.RGBA32, mipChain: false, linear: true);
        tex.name = "IndustrialDroneParticle_SoftCircle";
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Bilinear;

        var center = (size - 1) * 0.5f;
        var invR = 1f / (center + 0.0001f);

        for (var y = 0; y < size; y++)
        {
            for (var x = 0; x < size; x++)
            {
                var dx = (x - center) * invR;
                var dy = (y - center) * invR;
                var r = Mathf.Sqrt(dx * dx + dy * dy);

                // 중앙은 진하고, 가장자리는 부드럽게 0으로 떨어지는 알파.
                var a = Mathf.Clamp01(1f - r);
                a = Mathf.SmoothStep(0f, 1f, a);
                a = Mathf.Pow(a, 1.35f);

                // RGB는 흰색(색은 파티클 컬러에서 제어)
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
            }
        }

        tex.Apply(updateMipmaps: false, makeNoLongerReadable: true);
        AssetDatabase.CreateAsset(tex, path);
        AssetDatabase.ImportAsset(path);
        return tex;
    }

    static void SetPlayOnAwakeRecursive(GameObject root, bool value)
    {
        var systems = root.GetComponentsInChildren<ParticleSystem>(true);
        foreach (var ps in systems)
        {
            var main = ps.main;
            main.playOnAwake = value;
        }
    }
}

