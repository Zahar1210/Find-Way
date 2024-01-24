using System;
using System.Collections.Generic;
using UnityEngine;

public class CheckState : MonoBehaviour
{
    private Dictionary<Type, CheckFSM> _states = new();

    public void AddState(CheckFSM state)
    {
        if (!_states.ContainsKey(state.GetType())) {
            _states.Add(state.GetType(), state);
        }
    }

    public void SetState<T>(CarAbstract car) where T : CheckFSM
    {
        var type = typeof(T);
        if (car.CurrentCehckState != null && car.CurrentCehckState.GetType() == type) 
            return;
        if (_states.TryGetValue(type, out var newState)) {
            car.CurrentCehckState = newState;
        }
    }
}
