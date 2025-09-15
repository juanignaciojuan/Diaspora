using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using StarterAssets;
using UnityEngine.EventSystems;

public class GameStartManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject blackScreenObject;
    public Button playButton;
    public Button skipButton;   // NUEVO botón para saltar intro
    public GameObject titleTextObject;  // 👈 Nueva referencia al texto del título
    public GameObject instructionsTextObject;  // 👈 Nueva referencia al texto de las instrucciones
    public AudioSource introAudio;

    [Header("Fade Settings")]
    public float fadeDuration = 5f;
    public AnimationCurve fadeCurve;

    [Header("Player")]
    public FirstPersonController playerController;

    private Image blackScreen;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        blackScreen = blackScreenObject.GetComponent<Image>();
        playButton.onClick.AddListener(BeginGame);

        EventSystem.current.SetSelectedGameObject(playButton.gameObject); // 👈 fuerza selección

        if (playButton != null)
            playButton.onClick.AddListener(BeginGame);

        if (skipButton != null)
            skipButton.onClick.AddListener(SkipIntro);

        if (playerController != null)
            playerController.enabled = false;
    }

    void Update()
    {
        // Atajo de teclado: tecla J para saltar intro
        if (Input.GetKeyDown(KeyCode.J))
        {
            SkipIntro();
        }
    }

    public void BeginGame()
    {
        playButton.gameObject.SetActive(false);
        if (skipButton != null)
            skipButton.gameObject.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        blackScreenObject.SetActive(true);
        if (introAudio != null) introAudio.Play();

        StartCoroutine(FadeAndEnablePlayer());

        if (titleTextObject != null)
            titleTextObject.SetActive(false);

        if (instructionsTextObject != null)
            instructionsTextObject.SetActive(false);
    }

    public void SkipIntro()
    {
        Debug.Log("[GameStartManager] SkipIntro called");

        // Desactiva botones
        if (playButton != null) playButton.gameObject.SetActive(false);
        if (skipButton != null) skipButton.gameObject.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Quitar pantalla negra de inmediato
        if (blackScreen != null)
        {
            Color color = blackScreen.color;
            color.a = 0;
            blackScreen.color = color;
            blackScreenObject.SetActive(false);
        }

        if (introAudio != null && introAudio.isPlaying)
            introAudio.Stop();

        if (playerController != null)
            playerController.enabled = true;

        if (titleTextObject != null)
            titleTextObject.SetActive(false);

        if (instructionsTextObject != null)
            instructionsTextObject.SetActive(false);
    }

    IEnumerator FadeAndEnablePlayer()
    {
        float t = 0f;
        Color color = blackScreen.color;

        while (t < fadeDuration)
        {
            float alpha = 1f - fadeCurve.Evaluate(t / fadeDuration);
            color.a = alpha;
            blackScreen.color = color;
            t += Time.deltaTime;
            yield return null;
        }

        color.a = 0;
        blackScreen.color = color;
        blackScreenObject.SetActive(false);

        float waitTime = introAudio != null && introAudio.clip != null
            ? Mathf.Max(0f, introAudio.clip.length - fadeDuration)
            : 0f;

        yield return new WaitForSeconds(waitTime);

        if (playerController != null)
            playerController.enabled = true;
    }
}
