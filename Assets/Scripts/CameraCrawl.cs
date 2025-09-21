using UnityEngine;

public class CameraCrawl : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform;
    public CharacterController characterController; // optional

    [Header("Heights")]
    public float normalHeight = 1.8f;
    public float crawlHeight = 0.6f;
    public float smoothSpeed = 8f;

    private void Start()
    {
        if (cameraTransform == null)
        {
            var cam = GetComponentInChildren<Camera>();
            if (cam != null) cameraTransform = cam.transform;
        }
    }

    private void Update()
    {
        if (cameraTransform == null) return;

        float targetY = Input.GetKey(KeyCode.Q) ? crawlHeight : normalHeight;
        Vector3 localPos = cameraTransform.localPosition;
        localPos.y = Mathf.Lerp(localPos.y, targetY, Time.deltaTime * smoothSpeed);
        cameraTransform.localPosition = localPos;

        if (characterController != null)
        {
            float targetHeight = Input.GetKey(KeyCode.Q) ? 0.9f : 1.8f;
            characterController.height = Mathf.Lerp(characterController.height, targetHeight, Time.deltaTime * smoothSpeed);
            Vector3 center = characterController.center;
            center.y = characterController.height / 2f;
            characterController.center = center;
        }
    }
}
