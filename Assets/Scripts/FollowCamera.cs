using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [Header("Настройки")]
    public float zOffset = 10f;      // Насколько фон дальше камеры
    public float yOffset = 0f;       // Смещение по вертикали
    public bool followX = true;      // Следовать за камерой по X
    public bool followY = true;      // Следовать за камерой по Y

    private Transform cam;

    void Start()
    {
        cam = Camera.main.transform;
    }

    void LateUpdate()
    {
        if (cam == null) return;

        float targetX = followX ? cam.position.x : transform.position.x;
        float targetY = followY ? cam.position.y + yOffset : transform.position.y;

        transform.position = new Vector3(targetX, targetY, cam.position.z + zOffset);
    }
}