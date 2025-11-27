using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Plays a list of audio clips in sequence and loops back to the beginning.
/// Requires an AudioSource component on the same GameObject.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class AudioPlaylistPlayer : MonoBehaviour
{
    [Tooltip("The list of audio clips to be played in order.")]
    [SerializeField]
    private List<AudioClip> playlist = new List<AudioClip>();

    private AudioSource audioSource;
    private int currentTrackIndex = 0;
    
    [Header("Pitch Shifter")]
    [Tooltip("Enable runtime pitch shifting for tracks.")]
    public bool enablePitchShifter = false;
    [Tooltip("Base pitch multiplier (1 = normal).")]
    public float basePitch = 1f;
    [Tooltip("Max random variation added/subtracted from basePitch per track.")]
    public float pitchVariation = 0.05f;
    [Tooltip("If true, choose a random variation for each track. Otherwise uses a deterministic variation based on track index.")]
    public bool randomizePerTrack = true;
    [Tooltip("Smoothly interpolate pitch when switching tracks.")]
    public bool smoothPitchChange = true;
    [Tooltip("Time in seconds to smooth pitch changes.")]
    public float pitchSmoothTime = 0.15f;

    private Coroutine _pitchCoroutine;

    void Awake()
    {
        // Get the AudioSource component attached to this GameObject.
        // The [RequireComponent] attribute ensures it will always exist.
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        // Configure the AudioSource to not loop individual tracks, as we are handling the playlist loop.
        audioSource.loop = false;

        // Start playing the first track if the playlist is not empty.
        if (playlist.Count > 0)
        {
            PlayTrack(currentTrackIndex);
        }
    }

    void Update()
    {
        // Check if the playlist has tracks and if the current track has finished playing.
        if (playlist.Count > 0 && !audioSource.isPlaying)
        {
            // The current song has ended, so advance to the next one.
            PlayNextTrack();
        }
    }

    /// <summary>
    /// Plays the next track in the playlist, looping to the start if necessary.
    /// </summary>
    private void PlayNextTrack()
    {
        // Increment the track index.
        currentTrackIndex++;

        // If we've gone past the end of the playlist, loop back to the first track.
        if (currentTrackIndex >= playlist.Count)
        {
            currentTrackIndex = 0;
        }

        // Play the new track.
        PlayTrack(currentTrackIndex);
    }

    /// <summary>
    /// Plays a specific track from the playlist by its index.
    /// </summary>
    /// <param name="trackIndex">The index of the track to play.</param>
    private void PlayTrack(int trackIndex)
    {
        // Ensure the index is valid before trying to play.
        if (trackIndex >= 0 && trackIndex < playlist.Count)
        {
            audioSource.clip = playlist[trackIndex];
            // Apply pitch shifter before playing
            if (enablePitchShifter)
            {
                float target = ComputePitchForTrack(trackIndex);
                ApplyPitch(target, smoothPitchChange ? pitchSmoothTime : 0f);
            }
            else
            {
                ApplyPitch(basePitch, 0f);
            }

            audioSource.Play();
        }
    }

    private float ComputePitchForTrack(int trackIndex)
    {
        float variation = 0f;
        if (randomizePerTrack)
        {
            // Use a seeded pseudo-random so results are stable between runs in editor play
            variation = (Random.value * 2f - 1f) * pitchVariation;
        }
        else
        {
            // Deterministic variation based on index
            float t = (trackIndex % 100) / 100f; // normalized [0,1)
            variation = (t * 2f - 1f) * pitchVariation;
        }
        float p = basePitch + variation;
        return Mathf.Clamp(p, -3f, 3f);
    }

    private void ApplyPitch(float targetPitch, float smoothTime)
    {
        if (_pitchCoroutine != null)
        {
            StopCoroutine(_pitchCoroutine);
            _pitchCoroutine = null;
        }
        if (smoothTime > 0f)
            _pitchCoroutine = StartCoroutine(SmoothPitchTo(targetPitch, smoothTime));
        else
            audioSource.pitch = targetPitch;
    }

    private IEnumerator SmoothPitchTo(float target, float time)
    {
        float start = audioSource.pitch;
        float timer = 0f;
        while (timer < time)
        {
            timer += Time.deltaTime;
            audioSource.pitch = Mathf.Lerp(start, target, timer / Mathf.Max(time, 0.0001f));
            yield return null;
        }
        audioSource.pitch = target;
    }

    /// <summary>
    /// Public API to set pitch immediately at runtime.
    /// </summary>
    public void SetPitchImmediate(float pitch)
    {
        if (audioSource == null) return;
        if (_pitchCoroutine != null) { StopCoroutine(_pitchCoroutine); _pitchCoroutine = null; }
        audioSource.pitch = Mathf.Clamp(pitch, -3f, 3f);
    }

    /// <summary>
    /// Public API to set pitch with smoothing.
    /// </summary>
    public void SetPitchSmooth(float pitch, float smoothTime)
    {
        if (audioSource == null) return;
        ApplyPitch(Mathf.Clamp(pitch, -3f, 3f), smoothTime);
    }
}
