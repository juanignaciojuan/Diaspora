using UnityEngine;
using TMPro;

/// <summary>
/// Singleton manager that displays a single world-space label for the currently grabbed object.
/// Add one instance to the scene and assign a world-space Canvas + a TMP label inside it.
/// </summary>
public class GrabLabelManager : MonoBehaviour
{
    public static GrabLabelManager Instance { get; private set; }

    [Tooltip("World-space Canvas that contains the label UI.")]
    public Canvas worldCanvas;

    [Tooltip("RectTransform of the label panel (child of the canvas).")]
    public RectTransform labelPanel;

    [Tooltip("TMP Text component to show label text.")]
    public TextMeshProUGUI labelText;

    [Tooltip("Offset from target position in world space.")]
    public Vector3 targetOffset = new Vector3(0f, 0.2f, 0f);

    [Tooltip("How quickly the label follows the target.")]
    public float followSpeed = 10f;

    [Tooltip("Whether the label should face the main camera.")]
    public bool faceCamera = true;

    Transform currentTarget;
    bool visible = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        if (worldCanvas != null)
            worldCanvas.gameObject.SetActive(false);
    }

    public void Show(Transform target, string text)
    {
        Show(target, text, targetOffset);
    }

    public void Show(Transform target, string text, Vector3 offset)
    {
        currentTarget = target;
        if (labelText != null) labelText.text = text;
        targetOffset = offset;
        if (worldCanvas != null) worldCanvas.gameObject.SetActive(true);
        visible = true;
        UpdateLabelPosition(true);
    }

    public void Hide()
    {
        visible = false;
        currentTarget = null;
        if (worldCanvas != null) worldCanvas.gameObject.SetActive(false);
    }

    /// <summary>
    /// Hides the label only if the requester is the current target.
    /// Prevents one object from hiding another object's label.
    /// </summary>
    public void Hide(Transform requester)
    {
        if (currentTarget == requester)
        {
            Hide();
        }
    }

    void LateUpdate()
    {
        if (!visible || currentTarget == null) return;
        UpdateLabelPosition(false);
    }

    void UpdateLabelPosition(bool snap)
    {
        Vector3 worldPos = currentTarget.position + targetOffset;

        if (worldCanvas != null && worldCanvas.renderMode == RenderMode.WorldSpace)
        {
            if (snap) worldCanvas.transform.position = worldPos;
            else worldCanvas.transform.position = Vector3.Lerp(worldCanvas.transform.position, worldPos, Time.deltaTime * followSpeed);

            if (faceCamera && Camera.main != null)
                worldCanvas.transform.rotation = Quaternion.LookRotation(worldCanvas.transform.position - Camera.main.transform.position);
        }
        else if (labelPanel != null)
        {
            if (snap) labelPanel.position = worldPos;
            else labelPanel.position = Vector3.Lerp(labelPanel.position, worldPos, Time.deltaTime * followSpeed);

            if (faceCamera && Camera.main != null)
                labelPanel.rotation = Quaternion.LookRotation(labelPanel.position - Camera.main.transform.position);
        }
    }
}
