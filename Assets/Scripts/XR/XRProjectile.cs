using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
public class XRProjectile : MonoBehaviour
{
    [Header("Lifetime")]
    [Tooltip("Total lifetime of the projectile in seconds.")]
    public float lifetime = 20f;
    [Tooltip("How many seconds before destruction the fade-out should start.")]
    public float fadeDuration = 2f;

    [Header("Impact Effect")]
    public GameObject impactEffect;

    private Transform cameraTransform;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        // Find the main camera to face it.
        if (Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Start the fade-out process.
        StartCoroutine(FadeOutAndDestroy());
    }

    private void Update()
    {
        // Billboarding: Make the sprite always face the camera.
        if (cameraTransform != null)
        {
            transform.LookAt(transform.position + cameraTransform.rotation * Vector3.forward,
                             cameraTransform.rotation * Vector3.up);
        }
    }

    private IEnumerator FadeOutAndDestroy()
    {
        // Wait for the main part of the projectile's life.
        yield return new WaitForSeconds(lifetime - fadeDuration);

        // Fade out over the remaining time.
        float timer = 0;
        Color startColor = spriteRenderer.color;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(startColor.a, 0f, timer / fadeDuration);
            spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        // Finally, destroy the projectile.
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (impactEffect)
            Instantiate(impactEffect, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
