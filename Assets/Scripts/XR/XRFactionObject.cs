using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class XRFactionObject : MonoBehaviour
{
    [Tooltip("The faction this object belongs to.")]
    public FactionType objectFaction;

    private XRGrabInteractable _grab;
    private XRFactionZone _currentZone;

    private void Awake()
    {
        _grab = GetComponent<XRGrabInteractable>();
    }

    private void OnEnable()
    {
        _grab.selectExited.AddListener(OnReleased);
    }

    private void OnDisable()
    {
        _grab.selectExited.RemoveListener(OnReleased);
    }

    private void OnTriggerEnter(Collider other)
    {
        var zone = other.GetComponent<XRFactionZone>();
        if (zone != null)
        {
            _currentZone = zone;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var zone = other.GetComponent<XRFactionZone>();
        if (zone != null && _currentZone == zone)
        {
            _currentZone = null;
        }
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        // Check if we are in a zone
        if (_currentZone != null)
        {
            // Check if it's an "unaffiliated/enemy" place (i.e., different faction)
            if (_currentZone.zoneFaction != this.objectFaction)
            {
                TriggerDroneReaction(this.objectFaction);
            }
        }
    }

    private void TriggerDroneReaction(FactionType factionToSwitchTo)
    {
        // Find the Kamikaze Drone in the scene
        var drone = Object.FindFirstObjectByType<XRDroneKamikaze>();
        if (drone != null)
        {
            var swapper = drone.GetComponent<XRDroneModelSwapper>();
            if (swapper != null)
            {
                swapper.SwapModel(factionToSwitchTo);
            }
            else
            {
                Debug.LogWarning("XRDroneKamikaze found, but it lacks the XRDroneModelSwapper component.");
            }
        }
    }
}
