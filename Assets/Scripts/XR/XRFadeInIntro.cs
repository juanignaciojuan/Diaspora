using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages the intro sequence: Black screen with red text, then fades out.
/// Attach this to a Canvas (World Space, placed in front of camera) with a CanvasGroup.
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class XRFadeInIntro : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("The GameObject containing the second image to show after the main title audio finishes.")]
    public GameObject secondImageObject;
    [Tooltip("Time in seconds to keep the first text visible.")]
    public float holdTime1 = 3f;
    [Tooltip("Time in seconds to keep the second text visible.")]
    public float holdTime2 = 3f;
    [Tooltip("Time in seconds to keep the third text visible.")]
    public float holdTimeText3 = 3f;
    [Tooltip("Time in seconds to keep the controls image visible.")]
    public float holdTime3 = 3f;
    [Tooltip("Time in seconds to fade from black to transparent.")]
    public float fadeDuration = 2f;

    [Header("Content")]
    [Tooltip("Optional: The text component to ensure it is red.")]
    public TMP_Text introText;
    [Tooltip("Optional: The GameObject containing the controls image to show after texts.")]
    public GameObject controlsImageObject;
    [Tooltip("Optional: The GameObject containing the main game title image.")]
    public GameObject mainTitleImageObject;
    [Tooltip("The AudioClip to play with the main game title image.")]
    public AudioClip mainTitleAudio;
    [Tooltip("AudioSource to play the main title audio.")]
    public AudioSource audioSource;
    [Tooltip("Time in seconds to wait after the main title audio finishes before showing the controls image.")]
    public float holdAfterTitleAudio = 1f;
    [TextArea] public string text1 = "Este personaje está habitando un origen";
    [TextArea] public string text2 = "Pero algo ha cambiado...";
    [TextArea] public string text3 = "Prepárate...";

    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        if (introText != null)
        {
            introText.color = Color.red;
            introText.text = text1;
        }
        if (controlsImageObject != null)
        {
            controlsImageObject.SetActive(false);
        }
        if (mainTitleImageObject != null)
        {
            mainTitleImageObject.SetActive(false);
        }
    }

    private IEnumerator Start()
    {
        // 1. Show First Text
        if (introText != null)
        {
            introText.gameObject.SetActive(true);
            introText.text = text1;
        }
        yield return new WaitForSeconds(holdTime1);

        // 2. Show Second Text
        if (introText != null)
        {
            introText.text = text2;
        }
        yield return new WaitForSeconds(holdTime2);

        // 3. Show Third Text
        if (introText != null)
        {
            introText.text = text3;
        }
        yield return new WaitForSeconds(holdTimeText3);

        // 4. Show Main Title Image and Play Audio
        if (introText != null) introText.gameObject.SetActive(false);
        if (mainTitleImageObject != null) mainTitleImageObject.SetActive(true);
        float audioLength = 0f;
        if (mainTitleAudio != null && audioSource != null)
        {
            audioSource.clip = mainTitleAudio;
            audioSource.enabled = true;
            audioSource.volume = 1f;
            audioSource.Play();
            audioLength = mainTitleAudio.length;
        }
        yield return new WaitForSeconds(audioLength);
        if (mainTitleImageObject != null) mainTitleImageObject.SetActive(false);
        if (secondImageObject != null) secondImageObject.SetActive(true);
        yield return new WaitForSeconds(holdAfterTitleAudio);
        if (secondImageObject != null) secondImageObject.SetActive(false);

        // 5. Show Controls Image
        if (controlsImageObject != null) controlsImageObject.SetActive(true);
        yield return new WaitForSeconds(holdTime3);

        // 6. Fade out the whole canvas
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }

        _canvasGroup.alpha = 0f;

        // Ensure controls are hidden after fade so they return to default state
        if (controlsImageObject != null) controlsImageObject.SetActive(false);

        gameObject.SetActive(false); // Disable canvas to save resources
    }
}
