using UnityEngine;

public class XRFactionZone : MonoBehaviour
{
    [Tooltip("The faction that this zone belongs to.")]
    public FactionType zoneFaction;

    private void OnValidate()
    {
        // Ensure we have a trigger collider
        if (GetComponent<Collider>() == null || !GetComponent<Collider>().isTrigger)
        {
            Debug.LogWarning($"XRFactionZone on {name} needs a Collider with IsTrigger = true!", this);
        }
    }
}
