using Zenject;

public class CarCheckDistanceCheckCarState : CheckFSM
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
            if (car.CheckCar.TargetDot == car.TargetDot || car.ExtraCheckCar != null) {
                // float distance = _trafficDistanceTracker.GetDistance(car.transform.position, car.CheckCar.transform.position);
                // ProcessingDistance(car, distance);
            }
            else {
                car.ExtraCheckCar = null;
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
        if (distance < (targetDistance - 0.3f)) {
            if (!TryState(car.CheckCar) && TryState(car)) {
                car.TargetSpeed = car.CheckCar.TargetSpeed;
                _drivingState.SetState<CarStateSlowDown>(new DrivingState.DrivingParams(car, car.TargetSpeed));
            }
            else {
                _drivingState.SetState<CarStateSlowDown>(new DrivingState.DrivingParams(car, car.CheckCar.Speed - 0.2f));
            }
        }
        else if(distance > (targetDistance + 0.3f)) {
            car.TargetSpeed = car.FixedSpeed;
            _drivingState.SetState<CarStateSlowDown>(new DrivingState.DrivingParams(car, car.TargetSpeed));
        }
    }
    
    private bool TryState(CarAbstract car)
    {
        return car.CurrentState is CarStateDriving || car.CurrentState is CarStatePowerUp;
    }
}
