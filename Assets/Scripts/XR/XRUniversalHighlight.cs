using UnityEngine;

[DisallowMultipleComponent]
public class XRUniversalHighlight : MonoBehaviour
{
    [Header("Highlight Settings")]
    public Material highlightMaterial;
    public bool includeChildren = true;

    private Material[] originalMaterials;
    private Renderer[] renderers;

    private void Awake()
    {
        renderers = includeChildren ? GetComponentsInChildren<Renderer>() : GetComponents<Renderer>();
        originalMaterials = new Material[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
            originalMaterials[i] = renderers[i].sharedMaterial;
    }

    public void SetHighlight()
    {
        if (!highlightMaterial) return;
        foreach (var r in renderers)
            r.material = highlightMaterial;
    }

    public void ClearHighlight()
    {
        for (int i = 0; i < renderers.Length; i++)
            renderers[i].material = originalMaterials[i];
    }
}
