using System;
using System.Collections.Generic;
using UnityEngine;

public class State: MonoBehaviour
{
    private Dictionary<Type, FSM> _states = new();

    public void AddState(FSM state)
    {
        if (!_states.ContainsKey(state.GetType())) {
            _states.Add(state.GetType(), state);
        }
    }

    public void SetState<T>(CarAbstract car, TrafficDot.Dot a = null, float frontCarSpeed = 0)where T: FSM
    {
        var type = typeof(T);
        if (car.CurrentState != null && car.CurrentState.GetType() == type) 
            return;
        
        if (_states.TryGetValue(type, out var newState)) {
            car.CurrentState = newState;
            if (car.CurrentState is CarStatePowerUp) {
                car.CurrentState.EnterRowerUp(car, car.FixedSpeed);
            }
            else if (car.CurrentState is CarStateSlowDown) {
                car.CurrentState.EnterSlowDown(car, (frontCarSpeed != 0) ? frontCarSpeed : 0, car.TimeForMove);
            }
            else if (car.CurrentState is CarStateDriving) {
                car.CurrentState.EnterDriving(a, car);
            }
        }
    }
}