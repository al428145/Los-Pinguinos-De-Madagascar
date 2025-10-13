using UnityEngine;

public class CameraVigilando : State
{
    public override void Enter(NPCBase owner)
    {
        Debug.Log("?? Cámara vigilando...");
        var cam = owner as SecurityCamNPC;
        if (cam?.alarmSound != null)
            cam.alarmSound.Stop();
    }

    public override void Execute(NPCBase owner)
    {
        // Aquí no hace falta rotación — otro script se encarga.
        // Solo se queda vigilando visualmente.
    }

    public override System.Type GetNextStateForEvent(StateEvent evt)
    {
        if (evt == StateEvent.PlayerSeen)
            return typeof(CamaraWarning);

        return null;
    }
}
