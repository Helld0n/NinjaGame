using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Цель слежения")]
    public Transform target;

    [Header("Настройки слежения")]
    public float smoothSpeed = 5f;      // Плавность
    public Vector3 offset = new Vector3(0f, 1f, -10f); // Смещение от цели

    [Header("Ограничения камеры")]
    public bool useBounds = false;
    public float minX = -50f;
    public float maxX = 50f;
    public float minY = -10f;
    public float maxY = 50f;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;

        if (useBounds)
        {
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, minY, maxY);
        }

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }
}