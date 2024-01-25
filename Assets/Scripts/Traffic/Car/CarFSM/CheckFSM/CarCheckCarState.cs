using UnityEngine;
using Zenject;

public class CarCheckCarState : CheckFSM
{
    private CheckState _checkState;
    private DrivingState _drivingState;
    private TrafficDistanceTracker _trafficDistanceTracker;
    
    [Inject]
    private void Construct(TrafficDistanceTracker trafficDistanceTracker, CheckState checkState, DrivingState drivingState)
    {
        _checkState = checkState;
        _drivingState = drivingState;
        _trafficDistanceTracker = trafficDistanceTracker;
    }
    
    public override void Enter(CarAbstract car)
    {
        if (car.CheckCar != null) {
            if (car.CheckCar.TargetDot == car.TargetDot) {
                float distance = _trafficDistanceTracker.GetDistance(car.transform.position, car.CheckCar.transform.position);
                ProcessingDistance(car, distance);
            }
            else {
                car.CheckCar = null;
            }
        }
        else {
            car.CheckDot = _trafficDistanceTracker.GetDotForCheck(car);
            if (car.CheckDot != null) { 
                _checkState.SetState<CarCheckDotState>(car);
                return;
            }
            car.CurrentCehckState = null;
        }
    }

    private void ProcessingDistance(CarAbstract car, float distance)
    {
        float targetDistance = _trafficDistanceTracker.GetTargetDistance(car, car.CheckCar);
        if (distance < targetDistance) {
            if (TryState(car)) {
                _checkState.SetState<CarCheckStateDistanceCheckCar>(car);
                _drivingState.SetState<CarStateSlowDown>(car);
            }
            else {
                _drivingState.SetState<CarStateSlowDown>(car);
            }
        }
        else if(distance > (targetDistance + car.transform.localScale.y / 2)) {
            if (!TryState(car)) {
                _drivingState.SetState<CarStatePowerUp>(car);
            }
        }
    }

    private bool TryState(CarAbstract car)
    {
        return car.CheckCar.CurrentState is CarStateDriving || car.CheckCar.CurrentState is CarStatePowerUp;
    }
}
