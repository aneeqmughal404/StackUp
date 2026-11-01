using System.Collections.Generic;
using UnityEngine;

// Runtime diagnostics to help identify why FPS/batches are high.
// Logs counts of renderers, unique materials, particle systems and an instantaneous FPS sample.
public class RuntimeStatsReporter : MonoBehaviour
{
    [SerializeField] private float reportInterval = 2f;

    private float timer;

    void Update()
    {
        timer += Time.unscaledDeltaTime;
        if (timer < reportInterval) return;
        timer = 0f;

        var srs = Object.FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None);
        var mrs = Object.FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None);
        var pss = Object.FindObjectsByType<ParticleSystem>(FindObjectsSortMode.None);
        var rbs = Object.FindObjectsByType<Rigidbody>(FindObjectsSortMode.None);

        HashSet<int> materialIds = new HashSet<int>();

        foreach (var sr in srs)
        {
            if (sr.sharedMaterial != null) materialIds.Add(sr.sharedMaterial.GetInstanceID());
        }

        foreach (var mr in mrs)
        {
            var mats = mr.sharedMaterials;
            if (mats == null) continue;
            for (int i = 0; i < mats.Length; i++) if (mats[i] != null) materialIds.Add(mats[i].GetInstanceID());
        }

        int uniqueMaterials = materialIds.Count;
        int spriteCount = srs.Length;
        int meshCount = mrs.Length;
        int particleCount = pss.Length;
        int rigidbodyCount = rbs.Length;
        int activeGameObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None).Length;

        float fps = 1f / Mathf.Max(0.0001f, Time.unscaledDeltaTime);

        Debug.Log($"[RuntimeStats] FPS:{fps:F1} Sprites:{spriteCount} Meshes:{meshCount} Particles:{particleCount} Rigidbodies:{rigidbodyCount} UniqueMaterials:{uniqueMaterials} ActiveGOs:{activeGameObjects}");
    }
}
