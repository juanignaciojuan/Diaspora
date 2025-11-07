using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Handles the behavior of a grenade in a VR environment.
/// The grenade explodes upon contact with objects tagged as "Ground".
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(XRGrabInteractable))]
public class XRGrenade : MonoBehaviour
{
    [Header("Explosion Settings")]
    [Tooltip("The particle effect prefab to instantiate on explosion.")]
    public GameObject explosionEffectPrefab;

    [Tooltip("The sound to play on explosion.")]
    public AudioClip explosionSound;

    [Tooltip("The force applied by the explosion to surrounding objects.")]
    public float explosionForce = 1000f;

    [Tooltip("The radius of the explosion's effect.")]
    public float explosionRadius = 5f;

    private AudioSource audioSource;
    private XRGrabInteractable grabInteractable;
    private bool wasGrabbed = false;
    private bool hasExploded = false;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
    }

    private void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnGrab);
    }

    private void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrab);
    }

    private void Start()
    {
        // Get the AudioSource component attached to this GameObject.
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            // If no AudioSource is present, add one to play the explosion sound.
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        wasGrabbed = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Explode only if it has been grabbed and not yet exploded.
        // Ensure your ground GameObject is tagged with "Ground".
        if (wasGrabbed && !hasExploded && collision.gameObject.CompareTag("Ground"))
        {
            hasExploded = true;
            Explode();
        }
    }

    /// <summary>
    /// Triggers the explosion effects.
    /// </summary>
    private void Explode()
    {
        // 1. Instantiate particle effect
        if (explosionEffectPrefab != null)
        {
            GameObject explosionInstance = Instantiate(explosionEffectPrefab, transform.position, transform.rotation);
            // Destroy the particle effect after it has finished playing.
            // This assumes the particle system is not set to loop.
            ParticleSystem ps = explosionInstance.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                Destroy(explosionInstance, ps.main.duration);
            }
            else
            {
                // Fallback for particle systems without a main ParticleSystem component at the root
                Destroy(explosionInstance, 5f);
            }
        }

        // 2. Play explosion sound
        if (audioSource != null && explosionSound != null)
        {
            // Play the sound at the point of explosion without being attached to the grenade
            AudioSource.PlayClipAtPoint(explosionSound, transform.position);
        }

        // 3. Apply physics force
        // Find all colliders within the explosion radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            // If a collider has a Rigidbody, apply the explosion force
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }

        // 4. Destroy the grenade GameObject
        Destroy(gameObject);
    }
}
