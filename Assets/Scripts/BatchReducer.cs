using System.Collections.Generic;
using UnityEngine;

public class BatchReducer : MonoBehaviour
{
    [Tooltip("Run automatically on Start to unify sprite materials by texture.")]
    public bool runOnStart = true;

    private void Start()
    {
        if (runOnStart)
            ReduceBatches();
    }

    public void ReduceBatches()
    {
        Dictionary<string, Material> materialsByKey = new Dictionary<string, Material>();

        // SpriteRenderers usually use the sprite texture as the main source of batching data.
        SpriteRenderer[] spriteRenderers = Object.FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None);
        foreach (SpriteRenderer renderer in spriteRenderers)
        {
            if (renderer.sprite == null)
                continue;

            Texture texture = renderer.sprite.texture;
            string key = "Sprites/Default:" + (texture != null ? texture.GetInstanceID().ToString() : "null");

            if (!materialsByKey.TryGetValue(key, out Material material))
            {
                material = new Material(Shader.Find("Sprites/Default"));
                if (texture != null)
                    material.mainTexture = texture;

                materialsByKey[key] = material;
            }

            renderer.sharedMaterial = material;
        }

        // MeshRenderers use the shader and main texture as the batching key.
        MeshRenderer[] meshRenderers = Object.FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None);
        foreach (MeshRenderer renderer in meshRenderers)
        {
            Material[] materials = renderer.sharedMaterials;
            if (materials == null || materials.Length == 0)
                continue;

            Material firstMaterial = materials[0];
            if (firstMaterial == null)
                continue;

            string shaderName = firstMaterial.shader != null ? firstMaterial.shader.name : "null";
            Texture texture = firstMaterial.mainTexture;
            string key = shaderName + ":" + (texture != null ? texture.GetInstanceID().ToString() : "null");

            if (!materialsByKey.TryGetValue(key, out Material sharedMaterial))
            {
                sharedMaterial = new Material(firstMaterial.shader);
                if (texture != null)
                    sharedMaterial.mainTexture = texture;

                if (firstMaterial.HasProperty("_Color"))
                    sharedMaterial.SetColor("_Color", firstMaterial.GetColor("_Color"));

                materialsByKey[key] = sharedMaterial;
            }

            renderer.sharedMaterial = sharedMaterial;
        }
    }
}
