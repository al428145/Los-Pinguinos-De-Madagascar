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
    }

    public override void SelectNewDestination()
    {
        if (zonaPatrulla == null) return;

        Vector3 centro = zonaPatrulla.transform.TransformPoint(zonaPatrulla.center);
        Vector3 tamanio = Vector3.Scale(zonaPatrulla.size, zonaPatrulla.transform.lossyScale) / 2f;

        float x = Random.Range(-tamanio.x, tamanio.x);
        float z = Random.Range(-tamanio.z, tamanio.z);
        Vector3 punto = centro + new Vector3(x, 0f, z);

        CurrentDestination = new Vector3(punto.x, transform.position.y, punto.z);
        Debug.Log(CurrentDestination);
    }
}
