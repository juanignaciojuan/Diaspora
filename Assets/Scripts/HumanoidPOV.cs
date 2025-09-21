using UnityEngine;

public class HumanoidPOV : MonoBehaviour
{
    public Camera humanoidCamera; // Assign in Inspector
    public Camera playerCamera;   // Assign main camera here
    public KeyCode switchKey = KeyCode.Q;

    void Update()
    {
        if (Input.GetKey(switchKey))
        {
            humanoidCamera.enabled = true;
            playerCamera.enabled = false;
        }
        else
        {
            humanoidCamera.enabled = false;
            playerCamera.enabled = true;
        }
    }
}
