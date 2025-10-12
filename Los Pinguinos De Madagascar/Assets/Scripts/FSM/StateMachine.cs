using System;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    private NPCBase owner;
    private State currentState;
    private Dictionary<Type, State> states = new Dictionary<Type, State>();

    public StateMachine(NPCBase owner, IEnumerable<Type> stateTypes)
    {
        this.owner = owner;

        foreach (var type in stateTypes)
        {
            if (!states.ContainsKey(type))
                states[type] = (State)Activator.CreateInstance(type);
        }

    }

    public void SetState(Type stateType)
    {
        if (currentState != null)
            currentState.Exit(owner);

        currentState = states[stateType];
        currentState.Enter(owner);
    }

    public void Update()
    {
        currentState?.Execute(owner);
    }

    public void TriggerEvent(StateEvent evt)
    {
        Debug.Log("FSM.TriggerEvent " + evt + ", currentState: " + currentState.GetType());

        if (currentState == null) return;

        Type next = currentState.GetNextStateForEvent(evt);
        if (next != null)
            SetState(next);
    }

    public State getState()
    {
        return currentState;
    }
}
