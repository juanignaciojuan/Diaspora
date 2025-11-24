using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

/// <summary>
/// Manages a "fake" POV switch by moving the player's XR Rig to an NPC's viewpoint.
/// This avoids the performance cost of a second camera.
/// </summary>
public class XRNPCPovSwitcher : MonoBehaviour
{
    [Header("Core References")]
    [Tooltip("The root of the player's XR Rig to be moved.")]
    public Transform playerXrRig;

    [Tooltip("The target transform on the NPC to align the camera with (e.g., the NPC's head or a 'Camera' child object).")]
    public Transform npcViewpoint;

    [Header("Input")]
    [Tooltip("The Input Action to trigger the POV switch.")]
    public InputActionReference togglePovAction;

    [Header("Player Control Scripts")]
    [Tooltip("Drag all player movement/input scripts here to disable them during the POV switch (e.g. CharacterControllerDriver, movement scripts).")]
    public MonoBehaviour[] playerControlScripts;

    [Header("Visuals")]
    [Tooltip("Objects to hide while in POV mode (e.g. Hand Visualizers).")]
    public GameObject[] objectsToHide;

    [Tooltip("Optional: A prefab (e.g. a character model) to spawn at the player's original position while they are in the drone POV.")]
    public GameObject playerBodyPrefab;

    [Tooltip("The Camera Culling Mask to use while in Drone POV. Useful to reveal the player body layer.")]
    public LayerMask dronePovCullingMask = ~0;

    [Tooltip("Duration to fade audio out/in during switch to prevent crackling.")]
    public float audioFadeDuration = 0.15f;

    [Header("Teleport")]
    [Tooltip("Button to press while in POV to teleport the player to that location.")]
    public InputActionReference teleportAction;
    public AudioSource teleportSound;

    private Vector3 originalPlayerPosition;
    private Quaternion originalPlayerRotation;
    private Transform originalPlayerParent;
    private int originalCullingMask;
    private GameObject ghostTarget;
    private GameObject spawnedBody;
    private Transform originalDroneTarget;

    private bool isSwitchedToNpc = false;
    private float _prePovVolume = 1f;

    private void OnEnable()
    {
        if (togglePovAction != null)
        {
            // Use Started/Canceled for Hold-to-Activate
            togglePovAction.action.started += OnPovStart;
            togglePovAction.action.canceled += OnPovEnd;
            togglePovAction.action.Enable();
        }
        if (teleportAction != null)
        {
            teleportAction.action.started += OnTeleport;
            teleportAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (togglePovAction != null)
        {
            togglePovAction.action.started -= OnPovStart;
            togglePovAction.action.canceled -= OnPovEnd;
            togglePovAction.action.Disable();
        }
        if (teleportAction != null)
        {
            teleportAction.action.started -= OnTeleport;
            teleportAction.action.Disable();
        }

        if (isSwitchedToNpc)
        {
            SwitchBackToPlayer();
        }
    }

    private void OnPovStart(InputAction.CallbackContext context)
    {
        if (playerXrRig == null || npcViewpoint == null) return;
        if (!isSwitchedToNpc) StartCoroutine(SwitchToNpcRoutine());
    }

    private void OnPovEnd(InputAction.CallbackContext context)
    {
        if (isSwitchedToNpc) StartCoroutine(SwitchBackToPlayerRoutine());
    }

    private void OnTeleport(InputAction.CallbackContext context)
    {
        if (isSwitchedToNpc && npcViewpoint != null)
        {
            // Update the return position to the current drone position
            originalPlayerPosition = npcViewpoint.position;
            
            // Update rotation (keep Y only to avoid tilting the player)
            Vector3 droneEuler = npcViewpoint.eulerAngles;
            originalPlayerRotation = Quaternion.Euler(0, droneEuler.y, 0);

            if (teleportSound != null) teleportSound.Play();
        }
    }

    private void Update()
    {
        if (isSwitchedToNpc && playerXrRig != null && npcViewpoint != null)
        {
            Transform cameraTransform = Camera.main.transform;
            Vector3 rigToHeadOffset = cameraTransform.position - playerXrRig.position;
            playerXrRig.position = npcViewpoint.position - rigToHeadOffset;
        }
    }

    private IEnumerator SwitchToNpcRoutine()
    {
        // Store original volume
        _prePovVolume = AudioListener.volume;

        // Fade Audio Out
        for (float t = 0; t < audioFadeDuration; t += Time.deltaTime)
        {
            AudioListener.volume = Mathf.Lerp(_prePovVolume, 0f, t / audioFadeDuration);
            yield return null;
        }
        AudioListener.volume = 0f;

        SwitchToNpc();

        // Fade Audio In
        for (float t = 0; t < audioFadeDuration; t += Time.deltaTime)
        {
            AudioListener.volume = Mathf.Lerp(0f, _prePovVolume, t / audioFadeDuration);
            yield return null;
        }
        AudioListener.volume = _prePovVolume;
    }

    private void SwitchToNpc()
    {
        // 1. Store original state
        originalPlayerPosition = playerXrRig.position;
        originalPlayerRotation = playerXrRig.rotation;
        originalPlayerParent = playerXrRig.parent;

        // 2. Disable player controls
        SetPlayerControls(false);
        
        // 3. Disable Physics/Gravity on Player Rig
        var cc = playerXrRig.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;
        
        var rb = playerXrRig.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        // 4. Hide Hands/Visuals
        SetVisuals(false);

        // 5. Ghost Target Logic (Keep Drone Chasing Original Spot)
        // Find the drone script on the viewpoint or its parents
        var drone = npcViewpoint.GetComponentInParent<XRDroneKamikaze>();
        if (drone != null)
        {
            originalDroneTarget = drone.target;
            
            // Create a ghost target at the player's original position
            ghostTarget = new GameObject("GhostTarget_POV");
            ghostTarget.transform.position = originalPlayerPosition;
            
            // Tell drone to chase the ghost
            drone.target = ghostTarget.transform;
        }

        // Spawn fake body if assigned
        if (playerBodyPrefab != null)
        {
            spawnedBody = Instantiate(playerBodyPrefab, originalPlayerPosition, originalPlayerRotation);
        }

        // 6. Update Camera Culling Mask
        if (Camera.main != null)
        {
            originalCullingMask = Camera.main.cullingMask;
            Camera.main.cullingMask = dronePovCullingMask;
        }

        isSwitchedToNpc = true;
    }

    private IEnumerator SwitchBackToPlayerRoutine()
    {
        // Fade Audio Out
        float currentVol = AudioListener.volume;
        for (float t = 0; t < audioFadeDuration; t += Time.deltaTime)
        {
            AudioListener.volume = Mathf.Lerp(currentVol, 0f, t / audioFadeDuration);
            yield return null;
        }
        AudioListener.volume = 0f;

        SwitchBackToPlayer();

        // Fade Audio In
        for (float t = 0; t < audioFadeDuration; t += Time.deltaTime)
        {
            AudioListener.volume = Mathf.Lerp(0f, _prePovVolume, t / audioFadeDuration);
            yield return null;
        }
        AudioListener.volume = _prePovVolume;
    }

    private void SwitchBackToPlayer()
    {
        // 1. Restore original parent and transform
        if (originalPlayerParent != null)
            playerXrRig.SetParent(originalPlayerParent, worldPositionStays: true);
            
        playerXrRig.position = originalPlayerPosition;
        playerXrRig.rotation = originalPlayerRotation;

        // 2. Re-enable player controls
        SetPlayerControls(true);
        
        // 3. Re-enable Physics/Gravity
        var cc = playerXrRig.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = true;
        
        var rb = playerXrRig.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = false;

        // 4. Show Hands/Visuals
        SetVisuals(true);

        // 5. Restore Drone Target
        var drone = npcViewpoint.GetComponentInParent<XRDroneKamikaze>();
        if (drone != null && originalDroneTarget != null)
        {
            drone.target = originalDroneTarget;
        }
        if (ghostTarget != null) Destroy(ghostTarget);
        if (spawnedBody != null) Destroy(spawnedBody);

        // Restore Culling Mask
        if (Camera.main != null)
        {
            Camera.main.cullingMask = originalCullingMask;
        }

        isSwitchedToNpc = false;
    }

    private void SetVisuals(bool active)
    {
        if (objectsToHide == null) return;
        foreach (var obj in objectsToHide)
        {
            if (obj != null) obj.SetActive(active);
        }
    }

    private void SetPlayerControls(bool enabled)
    {
        foreach (var script in playerControlScripts)
        {
            if (script != null)
            {
                script.enabled = enabled;
            }
        }
    }
}
