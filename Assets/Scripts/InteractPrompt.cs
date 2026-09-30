using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractPrompt : MonoBehaviour
{
    public Transform npcTransform;
    public Vector3 offset = new Vector3(0, 1.5f, 0);
    public Camera cam;

    private TextMeshProUGUI promptText;

    void Start()
    {
        promptText = GetComponent<TextMeshProUGUI>();
        if (cam == null) cam = Camera.main;
        Hide();
    }

    void LateUpdate()
    {
        if (npcTransform == null || cam == null) return;

        Vector3 screenPos = cam.WorldToScreenPoint(npcTransform.position + offset);

        transform.position = screenPos;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}