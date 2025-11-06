using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

[DisallowMultipleComponent]
[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
public class XRRifleShoot : MonoBehaviour
{
    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public Transform muzzleTransform;
    public float projectileSpeed = 20f;

    [Header("Fire Control")]
    [Tooltip("Minimum delay between shots (seconds). Lower = faster fire rate.")]
    public float fireRate = 0.25f;

    [Header("Effects")]
    public AudioSource audioSource;
    public AudioClip[] shotSounds;
    public ParticleSystem muzzleParticles;
    public Light muzzleLight;
    public float lightFlashDuration = 0.05f;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab;
    private bool canFire = true;

    private void Awake()
    {
        grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        grab.selectMode = UnityEngine.XR.Interaction.Toolkit.Interactables.InteractableSelectMode.Multiple; // two-hand grab
    }

    private void OnEnable()
    {
        grab.activated.AddListener(OnActivate);
    }

    private void OnDisable()
    {
        grab.activated.RemoveListener(OnActivate);
    }

    private void OnActivate(ActivateEventArgs args)
    {
        if (canFire)
            StartCoroutine(FireRoutine());
    }

    private IEnumerator FireRoutine()
    {
        canFire = false;
        Fire();
        yield return new WaitForSeconds(fireRate);
        canFire = true;
    }

    private void Fire()
    {
        // Projectile
        if (projectilePrefab && muzzleTransform)
        {
            GameObject proj = Instantiate(projectilePrefab, muzzleTransform.position, muzzleTransform.rotation);
            if (proj.TryGetComponent<Rigidbody>(out var rb))
                rb.linearVelocity = muzzleTransform.forward * projectileSpeed;
        }

        // Sound
        if (audioSource && shotSounds.Length > 0)
        {
            AudioClip clip = shotSounds[Random.Range(0, shotSounds.Length)];
            audioSource.PlayOneShot(clip);
        }

        // Particles
        if (muzzleParticles)
            muzzleParticles.Play();

        // Light flash
        if (muzzleLight)
            StartCoroutine(LightFlash());
    }

    private IEnumerator LightFlash()
    {
        muzzleLight.enabled = true;
        yield return new WaitForSeconds(lightFlashDuration);
        muzzleLight.enabled = false;
    }
}
