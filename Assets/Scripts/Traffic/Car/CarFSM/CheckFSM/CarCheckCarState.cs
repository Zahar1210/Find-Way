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
            _drivingState.SetState<CarStatePowerUp>(new DrivingState.DrivingParams(car, car.FixedSpeed));
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
        if (distance < targetDistance + 2) {
            float t = (car.CheckCar.CheckDot != null) ? 0f : 0.5f;
            car.TargetSpeed = car.CheckCar.TargetSpeed - t;
            car.CheckCar.BehindCar = car;
            _checkState.SetState<CarCheckDistanceCheckCarState>(car);
            _drivingState.SetState<CarStateSlowDown>(new DrivingState.DrivingParams(car, car.TargetSpeed, 0.5f));
        }
        else if(distance > (targetDistance + car.transform.localScale.y / 2)) {
            if (!TryState(car)) {
                car.CheckCar.BehindCar = null;
                car.TargetSpeed = car.FixedSpeed;
                _checkState.SetState<CarCheckCarState>(car);
                _drivingState.SetState<CarStatePowerUp>(new DrivingState.DrivingParams(car, car.TargetSpeed));
            }
        }
    }

    private bool TryState(CarAbstract car) {
        return car.CheckCar.CurrentState is CarStateDriving || car.CheckCar.CurrentState is CarStatePowerUp;
    }
}
