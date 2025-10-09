using System;

public abstract class State
{
    public virtual void Enter(NPCBase owner) { }
    public virtual void Execute(NPCBase owner) { }
    public virtual void Exit(NPCBase owner) { }
    public virtual Type GetNextStateForEvent(StateEvent evt) => null;
}
