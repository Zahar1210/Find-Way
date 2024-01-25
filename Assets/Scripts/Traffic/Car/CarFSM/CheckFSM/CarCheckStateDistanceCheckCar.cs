using Zenject;

public class CarCheckStateDistanceCheckCar : CheckFSM
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
        if (distance < _trafficDistanceTracker.GetTargetDistance(car, car.CheckCar)) {
            if (!TryState(car)) 
                _drivingState.SetState<CarStateSlowDown>(car);
        }
    }
    private bool TryState(CarAbstract car) {
        return car.CheckCar.CurrentState is CarStateDriving || car.CheckCar.CurrentState is CarStatePowerUp;
    }
}
