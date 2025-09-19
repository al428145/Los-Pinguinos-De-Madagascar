using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class NoiseCircle : MonoBehaviour
{
    public int segments = 50;
    public float radius = 1f;
    public bool visible = false;

    private LineRenderer line;

    void Start()
    {
        line = GetComponent<LineRenderer>();
        line.useWorldSpace = false;
        line.loop = true;
        line.positionCount = segments;
    }

    void Update()
    {
        line.enabled = visible;

        if (visible)
        {
            float angle = 0f;
            for (int i = 0; i < segments; i++)
            {
                float x = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
                float z = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
                line.SetPosition(i, new Vector3(x, 0, z));
                angle += 360f / segments;
            }
        }
    }
}
