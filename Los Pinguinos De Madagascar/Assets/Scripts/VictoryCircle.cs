using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(SphereCollider))]
public class VictoryZone : MonoBehaviour
{
    public int segments = 50;
    public float radius = 3f;
    public Color circleColor = Color.green;

    private LineRenderer line;
    private SphereCollider trigger;

    void Start()
    {

        line = GetComponent<LineRenderer>();
        line.useWorldSpace = false;
        line.loop = true;
        line.positionCount = segments;
        line.startWidth = 0.05f;
        line.endWidth = 0.05f;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = circleColor;
        line.endColor = circleColor;

        float angle = 0f;
        for (int i = 0; i < segments; i++)
        {
            float x = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            float z = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            line.SetPosition(i, new Vector3(x, 0, z));
            angle += 360f / segments;
        }

        trigger = GetComponent<SphereCollider>();
        trigger.isTrigger = true;
        trigger.radius = radius;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {   
            GameManager.Instance.WinGame();
        }
    }
}
