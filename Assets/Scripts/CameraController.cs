using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float liftSpeed = 2f;

    private float targetHeight;
    private Transform cachedTransform;

    private void Awake()
    {
        cachedTransform = transform;
        targetHeight = cachedTransform.position.y;
    }

    private void LateUpdate()
    {
        float currentY = cachedTransform.position.y;

        if (Mathf.Abs(currentY - targetHeight) > 0.001f)
        {
            float newY = Mathf.MoveTowards(currentY, targetHeight, liftSpeed * Time.deltaTime);
            cachedTransform.position = new Vector3(cachedTransform.position.x, newY, cachedTransform.position.z);
        }
    }

    public void LiftCamera(float amount)
    {
        targetHeight += amount > 0f ? amount : 0.5f;
    }
}