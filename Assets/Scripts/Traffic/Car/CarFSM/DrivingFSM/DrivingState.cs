using System;
using System.Collections.Generic;
using UnityEngine;

public class DrivingState: MonoBehaviour
{
    [SerializeField] private CarSpeedModifier carSpeedModifier;
    private Dictionary<Type, DrivingFSM> _states = new();

    public void AddState(DrivingFSM state)
    {
        if (!_states.ContainsKey(state.GetType())) {
            _states.Add(state.GetType(), state);
        }
    }

    public void SetState<T>(CarAbstract car, TrafficDot.Dot a = null)where T: DrivingFSM
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
                car.CurrentState.EnterSlowDown(car, carSpeedModifier.GetTargetSpeed(car), carSpeedModifier.GetTimeForSlowDown(car));
            }
            else if (car.CurrentState is CarStateDriving) {
                car.CurrentState.EnterDriving(a, car);
            }
        }
    }
}