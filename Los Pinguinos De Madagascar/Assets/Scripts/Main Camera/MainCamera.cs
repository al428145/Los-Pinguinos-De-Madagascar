using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    private new Camera camera;
    private Vector2 nearPlaneSize;

    public Transform follow;
    public float maxDistance = 5f;
    public Vector2 sensitivity = new Vector2(5f, 3f);

    public float minYAngle = 20f;
    public float maxYAngle = 30f;
    public Vector3 lookOffset = new Vector3(0, 1.5f, 0);

    private float angleX = 0f;
    private float angleY = 20f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        camera = GetComponent<Camera>();
        CalculateNearPlaneSize();
    }

    private void CalculateNearPlaneSize()
    {
        float height = Mathf.Tan(camera.fieldOfView * Mathf.Deg2Rad / 2f) * camera.nearClipPlane;
        float width = height * camera.aspect;
        nearPlaneSize = new Vector2(width, height);
    }

    void Update()
    {
        float hor = Input.GetAxis("Mouse X") * sensitivity.x;
        float ver = Input.GetAxis("Mouse Y") * sensitivity.y;

        angleX -= hor;
        angleY -= ver;
        angleY = Mathf.Clamp(angleY, minYAngle, maxYAngle);
    }

    void LateUpdate()
    {
        float yaw = angleX * Mathf.Deg2Rad;
        float pitch = angleY * Mathf.Deg2Rad;

        float cosPitch = Mathf.Cos(pitch);
        Vector3 direction = new Vector3(
            Mathf.Cos(yaw) * cosPitch,
            Mathf.Sin(pitch),
            Mathf.Sin(yaw) * cosPitch
        );

        Vector3 cameraTarget = follow.position + lookOffset;
        float distance = maxDistance;

        Vector3 center = cameraTarget + direction * (camera.nearClipPlane + 0.2f);
        Vector3 right = transform.right * nearPlaneSize.x;
        Vector3 up = transform.up * nearPlaneSize.y;

        Vector3[] collisionPoints = new Vector3[]
        {
            center - right + up,
            center + right + up,
            center - right - up,
            center + right - up
        };

        foreach (Vector3 point in collisionPoints)
        {
            if (Physics.Raycast(point, direction, out RaycastHit hit, maxDistance))
            {
                float hitDist = (hit.point - follow.position).magnitude;
                if (hitDist < distance) distance = hitDist;
            }
        }

        transform.position = follow.position + direction * distance;
        transform.rotation = Quaternion.LookRotation(cameraTarget - transform.position);
    }
}
