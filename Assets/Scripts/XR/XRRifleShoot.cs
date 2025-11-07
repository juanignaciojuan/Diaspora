using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

[DisallowMultipleComponent]
[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
public class XRRifleShoot : MonoBehaviour
{
    [Header("Projectile Settings")]
    public GameObject[] projectilePrefabs; // multiple projectile types
    public int selectedProjectileIndex = 0;
    public Transform muzzleTransform;
    public float projectileSpeed = 20f;

    [Header("Fire Control")]
    public float fireRate = 0.25f;

    [Header("Effects")]
    public AudioSource audioSource;
    public AudioClip[] shotSounds;
    public ParticleSystem muzzleParticles;
    public Light muzzleLight;
    public float lightFlashDuration = 0.05f;

    [Header("Recoil")]
    [Tooltip("The physical force applied to the rifle when firing.")]
    public float recoilForce = 5f;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab;
    private Rigidbody rifleRigidbody;
    private bool canFire = true;

    private void Awake()
    {
        grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        rifleRigidbody = GetComponent<Rigidbody>();
        grab.selectMode = UnityEngine.XR.Interaction.Toolkit.Interactables.InteractableSelectMode.Multiple;
    }

    private void OnEnable() => grab.activated.AddListener(OnActivate);
    private void OnDisable() => grab.activated.RemoveListener(OnActivate);

    private void OnActivate(ActivateEventArgs args)
    {
        if (canFire) StartCoroutine(FireRoutine());
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
        if (projectilePrefabs.Length > 0 && muzzleTransform)
        {
            GameObject prefab = projectilePrefabs[Mathf.Clamp(selectedProjectileIndex, 0, projectilePrefabs.Length - 1)];
            GameObject proj = Instantiate(prefab, muzzleTransform.position, muzzleTransform.rotation);
            if (proj.TryGetComponent<Rigidbody>(out var rb))
                rb.linearVelocity = muzzleTransform.forward * projectileSpeed;

            // Apply recoil
            if (rifleRigidbody != null)
            {
                rifleRigidbody.AddForce(-muzzleTransform.forward * recoilForce, ForceMode.Impulse);
            }

            // Audio
            if (audioSource && shotSounds.Length > 0)
            {
                AudioClip clip = shotSounds[Random.Range(0, shotSounds.Length)];
                audioSource.PlayOneShot(clip);
            }

            // Visuals
            if (muzzleParticles) muzzleParticles.Play();
            if (muzzleLight) StartCoroutine(LightFlash());
        }
    }

    private IEnumerator LightFlash()
    {
        muzzleLight.enabled = true;
        yield return new WaitForSeconds(lightFlashDuration);
        muzzleLight.enabled = false;
    }

    // Optional: change projectile type externally
    public void SetProjectileType(int index)
    {
        selectedProjectileIndex = Mathf.Clamp(index, 0, projectilePrefabs.Length - 1);
    }
}
