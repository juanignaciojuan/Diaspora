using UnityEngine;

public class XRDroneModelSwapper : MonoBehaviour
{
    [Header("Faction Models")]
    [Tooltip("The visual model to show for Faction Type A.")]
    public GameObject modelTypeA;
    [Tooltip("The visual model to show for Faction Type B.")]
    public GameObject modelTypeB;
    [Tooltip("The visual model to show for Faction Type C.")]
    public GameObject modelTypeC;

    [Header("Default")]
    [Tooltip("The visual model to show by default (or None).")]
    public GameObject defaultModel;

    private void Start()
    {
        // Ensure only the correct model is active initially (optional, or set in Editor)
        // UpdateModel(FactionType.None); 
    }

    public void SwapModel(FactionType faction)
    {
        // Hide all first
        if (modelTypeA) modelTypeA.SetActive(false);
        if (modelTypeB) modelTypeB.SetActive(false);
        if (modelTypeC) modelTypeC.SetActive(false);
        if (defaultModel) defaultModel.SetActive(false);

        // Show the correct one
        switch (faction)
        {
            case FactionType.TypeA:
                if (modelTypeA) modelTypeA.SetActive(true);
                break;
            case FactionType.TypeB:
                if (modelTypeB) modelTypeB.SetActive(true);
                break;
            case FactionType.TypeC:
                if (modelTypeC) modelTypeC.SetActive(true);
                break;
            default:
                if (defaultModel) defaultModel.SetActive(true);
                break;
        }
        
        Debug.Log($"Drone model swapped to {faction}");
    }
}
