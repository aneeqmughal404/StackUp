using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BatchOptimizerWindow : EditorWindow
{
    [MenuItem("Tools/Batch Optimizer")]
    public static void ShowWindow()
    {
        GetWindow<BatchOptimizerWindow>("Batch Optimizer");
    }

    void OnGUI()
    {
        GUILayout.Label("Batch Optimizer", EditorStyles.boldLabel);

        if (GUILayout.Button("Run BatchReducer (scene)"))
        {
            var br = Object.FindFirstObjectByType<BatchReducer>();

            if (br == null)
            {
                Debug.LogWarning("No BatchReducer in scene. Adding one to an empty GameObject.");
                var go = new GameObject("_BatchReducer");
                br = go.AddComponent<BatchReducer>();
            }

            br.ReduceBatches();
            Debug.Log("BatchReducer executed.");
        }

        if (GUILayout.Button("Make Blocks Batching-Static"))
        {
            MakeBlocksStatic();
        }

        if (GUILayout.Button("Report Scene Stats"))
            ReportSceneStats();

        if (GUILayout.Button("Add Runtime Stats Reporter to Scene"))
        {
            var existing = Object.FindFirstObjectByType<RuntimeStatsReporter>();

            if (existing == null)
            {
                var go = new GameObject("_RuntimeStatsReporter");
                go.AddComponent<RuntimeStatsReporter>();
                Debug.Log("Added RuntimeStatsReporter to scene.");
            }
            else
            {
                Debug.Log("RuntimeStatsReporter already present in scene.");
            }
        }

        if (GUILayout.Button("Assign Shared Block Material"))
        {
            AssignSharedBlockMaterial();
        }
    }

    private void MakeBlocksStatic()
    {
        var blocks = Object.FindObjectsByType<BlockController>(FindObjectsSortMode.None);

        int count = 0;

        foreach (var b in blocks)
        {
            var go = b.gameObject;
            GameObjectUtility.SetStaticEditorFlags(go, StaticEditorFlags.BatchingStatic);
            count++;
        }

        Debug.Log($"Marked {count} BlockController objects as Batching-Static.");
    }

    private void AssignSharedBlockMaterial()
    {
        string matDir = "Assets/Materials";

        if (!System.IO.Directory.Exists(matDir))
            System.IO.Directory.CreateDirectory(matDir);

        string matPath = System.IO.Path.Combine(matDir, "BlockShared.mat");

        Material shared = AssetDatabase.LoadAssetAtPath<Material>(matPath);

        if (shared == null)
        {
            Shader shader = Shader.Find("Unlit/Color") ?? Shader.Find("Sprites/Default");

            shared = new Material(shader);

            AssetDatabase.CreateAsset(shared, matPath);
            AssetDatabase.SaveAssets();
        }

        var blocks = Object.FindObjectsByType<BlockController>(FindObjectsSortMode.None);

        int count = 0;

        foreach (var b in blocks)
        {
            var mr = b.GetComponent<MeshRenderer>();

            if (mr == null)
                continue;

            mr.sharedMaterial = shared;
            count++;
        }

        Debug.Log($"Assigned shared block material to {count} blocks. Material: {matPath}");
    }

    private void ReportSceneStats()
    {
        var srs = Object.FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None);
        var mrs = Object.FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None);
        var pss = Object.FindObjectsByType<ParticleSystem>(FindObjectsSortMode.None);

        HashSet<int> materialIds = new HashSet<int>();

        foreach (var sr in srs)
        {
            if (sr.sharedMaterial != null)
                materialIds.Add(sr.sharedMaterial.GetInstanceID());
        }

        foreach (var mr in mrs)
        {
            var mats = mr.sharedMaterials;

            if (mats != null)
            {
                foreach (var m in mats)
                {
                    if (m != null)
                        materialIds.Add(m.GetInstanceID());
                }
            }
        }

        Debug.Log($"[SceneReport] SpriteRenderers:{srs.Length} MeshRenderers:{mrs.Length} Particles:{pss.Length} UniqueMaterials:{materialIds.Count}");
    }
}