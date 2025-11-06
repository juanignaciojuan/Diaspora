using UnityEngine;

[DisallowMultipleComponent]
public class ExplodableObject : MonoBehaviour
{
    [Header("Explosion Settings")]
    public float explosionForce = 700f;
    public float explosionRadius = 5f;
    public GameObject explosionEffect;
    public AudioSource explosionAudio;

    private bool exploded = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (exploded) return;
        if (collision.gameObject.GetComponent<Projectile>())
        {
            Explode();
        }
    }

    public void Explode()
    {
        if (exploded) return;
        exploded = true;

        // VFX
        if (explosionEffect)
            Instantiate(explosionEffect, transform.position, Quaternion.identity);

        // SFX
        if (explosionAudio)
            explosionAudio.Play();

        // Physics
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var nearby in colliders)
        {
            if (nearby.TryGetComponent<Rigidbody>(out var rb))
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
        }

        // Auto destroy this object
        Destroy(gameObject, 0.1f);
    }
}
