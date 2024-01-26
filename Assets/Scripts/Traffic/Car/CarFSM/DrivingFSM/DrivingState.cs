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

    public void SetState<T>(DrivingParams drivingParams, TrafficDot.Dot a = null)where T: DrivingFSM
    {
        CarAbstract car = drivingParams.Car;
        var type = typeof(T);
        if (car.CurrentState != null && car.CurrentState.GetType() == type) 
            return;
        
        if (_states.TryGetValue(type, out var newState)) {
            car.CurrentState = newState;
            if (car.CurrentState is CarStatePowerUp) {
                car.CurrentState.EnterRowerUp(car, car.FixedSpeed);
            }
            else if (car.CurrentState is CarStateSlowDown) {
                car.CurrentState.EnterSlowDown(car, car.DrivingParams.TargetSpeed, car.DrivingParams.TimeForMove);
            }
            else if (car.CurrentState is CarStateDriving) {
                car.CurrentState.EnterDriving(a, car);
            }
        }
    }

    public void Set<T>(CarAbstract car) where T: DrivingFSM 
    {
        var type = typeof(T);
        if (car.CurrentState != null && car.CurrentState.GetType() == type) 
            return;
        if (_states.TryGetValue(type, out var newState)) {
            car.CurrentState = newState;
        }
    }
    
    public class DrivingParams
    {
        public CarAbstract Car { get; set; } 
        public float TargetSpeed { get; set; }
        public float TimeForMove { get; set; }

        public DrivingParams(CarAbstract car, float targetSpeed = 0, float timeForMove = 0)
        {
            Car = car;
            car.DrivingParams = this;
            TargetSpeed = targetSpeed;
            TimeForMove = timeForMove;
        }
    }
}