using UnityEngine;

public class DoorInteraction : InteractableBase
{
    [Header("Door Settings")]
    public float openAngle = 90f;
    public float closeAngle = 0f;
    public float rotationSpeed = 5f;

    private bool isOpen = false;
    private Quaternion targetRotation;

    private void Start()
    {
        targetRotation = transform.rotation;
    }

    private void Update()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    public override void Interact()
    {
        isOpen = !isOpen;
        float yRotation = isOpen ? openAngle : closeAngle;
        targetRotation = Quaternion.Euler(transform.eulerAngles.x, yRotation, transform.eulerAngles.z);
    }
}
