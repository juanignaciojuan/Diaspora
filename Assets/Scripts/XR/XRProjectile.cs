using UnityEngine;

[DisallowMultipleComponent]
public class Projectile : MonoBehaviour
{
    [Header("Lifetime")]
    public float lifetime = 3f;

    [Header("Impact Effect")]
    public GameObject impactEffect;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (impactEffect)
            Instantiate(impactEffect, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
