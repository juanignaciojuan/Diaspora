using UnityEngine;
using UnityEngine.UI;

public class UI_PickupItem : MonoBehaviour
{
    public Image pickupDisplay; // UI image in canvas
    public AudioSource audioSource;
    public AudioClip pickupSound;
    public AudioClip dropSound;

    private bool isPickedUp = false;
    private static bool anyPicked = false; // avoid multiple pickups

    void OnMouseDown()
    {
        if (anyPicked && !isPickedUp) return; // prevent picking another object

        if (!isPickedUp)
        {
            Pickup();
        }
        else
        {
            Drop();
        }
    }

    void Pickup()
    {
        isPickedUp = true;
        anyPicked = true;
        pickupDisplay.gameObject.SetActive(true);

        if (audioSource && pickupSound) audioSource.PlayOneShot(pickupSound);
    }

    void Drop()
    {
        isPickedUp = false;
        anyPicked = false;
        pickupDisplay.gameObject.SetActive(false);

        if (audioSource && dropSound) audioSource.PlayOneShot(dropSound);
    }
}
