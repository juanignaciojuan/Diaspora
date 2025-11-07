using UnityEngine;

/// <summary>
/// Handles the behavior of a bomb, typically dropped by a drone.
/// The bomb explodes on contact with the ground, spawning smaller grenades.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class XRBomb : MonoBehaviour
{
    [Header("Bomb Settings")]
    [Tooltip("The time in seconds before the drone drops the bomb.")]
    public float dropDelay = 5f;

    [Header("Explosion Settings")]
    [Tooltip("The particle effect prefab to instantiate on explosion.")]
    public GameObject explosionEffectPrefab;

    [Tooltip("The sound to play on explosion.")]
    public AudioClip explosionSound;

    [Tooltip("The force applied by the explosion to surrounding objects.")]
    public float explosionForce = 1500f;

    [Tooltip("The radius of the explosion's effect.")]
    public float explosionRadius = 10f;

    [Header("Post-Explosion Settings")]
    [Tooltip("The grenade prefab to spawn after the bomb explodes.")]
    public GameObject grenadePrefab;

    [Tooltip("The number of grenades to spawn after the explosion.")]
    public int numberOfGrenadesToSpawn = 2;

    private AudioSource audioSource;
    private bool hasExploded = false;

    private void Start()
    {
        // Get or add an AudioSource component.
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Explode only once upon hitting an object tagged "Ground".
        if (!hasExploded && collision.gameObject.CompareTag("Ground"))
        {
            hasExploded = true;
            Explode();
        }
    }

    /// <summary>
    /// Triggers the explosion effects and spawns grenades.
    /// </summary>
    private void Explode()
    {
        // 1. Instantiate particle effect
        if (explosionEffectPrefab != null)
        {
            GameObject explosionInstance = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            ParticleSystem ps = explosionInstance.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                Destroy(explosionInstance, ps.main.duration);
            }
            else
            {
                Destroy(explosionInstance, 5f); // Fallback
            }
        }

        // 2. Play explosion sound
        if (audioSource != null && explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, transform.position);
        }

        // 3. Apply physics force
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }

        // 4. Spawn grenades
        if (grenadePrefab != null && numberOfGrenadesToSpawn > 0)
        {
            for (int i = 0; i < numberOfGrenadesToSpawn; i++)
            {
                // Spawn with a slight upward and outward velocity to spread them out
                Vector3 spawnPosition = transform.position + Random.insideUnitSphere * 0.5f;
                GameObject spawnedGrenade = Instantiate(grenadePrefab, spawnPosition, Quaternion.identity);
                
                // Give the spawned grenade a little push
                Rigidbody grenadeRb = spawnedGrenade.GetComponent<Rigidbody>();
                if (grenadeRb != null)
                {
                    grenadeRb.AddExplosionForce(200f, transform.position, 5f);
                }
            }
        }

        // 5. Destroy the bomb GameObject
        Destroy(gameObject);
    }
}
