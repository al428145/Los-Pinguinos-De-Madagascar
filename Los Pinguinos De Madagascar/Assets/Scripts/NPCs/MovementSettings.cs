using System;

[Serializable]
public class MovementSettings
{
    public float idle = 0f;
    public float walk = 2f;
    public float run = 3f;

    public float GetSpeed(MovementType type)
    {
        switch (type)
        {
            case MovementType.Walk: return walk;
            case MovementType.Run: return run;
            default: return idle;
        }
    }
}

public enum MovementType { Idle, Walk, Run }
