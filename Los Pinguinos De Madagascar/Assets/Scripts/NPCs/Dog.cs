using UnityEngine;

public class Dog : NPCBase
{
    [Header("Zona de Patrulla")]
    public BoxCollider zonaPatrulla;

    protected override void Awake()
    {
        base.Awake();
        FSM = new StateMachine(this, new System.Type[]
        {
            typeof(PatrolState),
            typeof(AlertedState),
            typeof(InvestigateState),
            typeof(PersecuteState)
        });
    }

    void Start()
    {
        FSM.SetState(typeof(PatrolState));
    }

    void Update()
    {
        FSM.Update();
        PlayerStillInRange = false;
        PlayerIsBeingSeen = false;

        if (CurrentDestination != Vector3.zero)
        {
            Debug.DrawLine(transform.position, CurrentDestination, Color.red);
        }
    }

    public override void SelectNewDestination()
    {
        if (zonaPatrulla == null) return;

        float margen = 1.0f;

        Vector3 centro = zonaPatrulla.transform.TransformPoint(zonaPatrulla.center);
        Vector3 tamanio = Vector3.Scale(zonaPatrulla.size, zonaPatrulla.transform.lossyScale) / 2f;

       
        float rangoX = Mathf.Max(0, tamanio.x - margen);
        float rangoZ = Mathf.Max(0, tamanio.z - margen);

        float x = Random.Range(-rangoX, rangoX);
        float z = Random.Range(-rangoZ, rangoZ);
        Vector3 punto = centro + new Vector3(x, 0f, z);

        CurrentDestination = new Vector3(punto.x, transform.position.y, punto.z);
        Debug.Log("Nuevo destino con margen: " + CurrentDestination);
    }
}
