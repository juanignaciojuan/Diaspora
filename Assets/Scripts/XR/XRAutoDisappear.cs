using UnityEngine;

[DisallowMultipleComponent]
public class AutoDisappear : MonoBehaviour
{
    [Tooltip("Seconds before object is destroyed.")]
    public float lifetime = 5f;

    private void OnEnable()
    {
        Destroy(gameObject, lifetime);
    }
}
