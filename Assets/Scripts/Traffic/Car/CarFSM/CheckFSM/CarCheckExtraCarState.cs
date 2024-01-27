using Zenject;

public class CarCheckExtraCarState : CheckFSM
{
    private CarChecking _carChecking;
    private CheckState _checkState;
    private DrivingState _drivingState;
    private TrafficDistanceTracker _trafficDistanceTracker;
    
    [Inject]
    private void Construct(TrafficDistanceTracker trafficDistanceTracker, CheckState checkState, DrivingState drivingState, CarChecking carChecking)
    {
        _carChecking = carChecking;
        _checkState = checkState;
        _drivingState = drivingState;
        _trafficDistanceTracker = trafficDistanceTracker;
    }
    
    public override void Enter(CarAbstract car)
    {
        if (car.ExtraCheckCar != null) {
            if (car.ExtraCheckCar.TargetDot != car.TargetDot &&  TryCar(car)) {
                // float distance = _trafficDistanceTracker.GetDistance(car.transform.position, car.ExtraCheckCar.transform.position);
                // ProcessingDistance(car, distance);
            }
            else {
                car.ExtraCheckCar = null;
            }
        }
        else {
            car.CurrentCehckState = null;
            _carChecking.StartChecking(car);
        }
    }

    private void ProcessingDistance(CarAbstract car, float distance)
    {
        float targetDistance = _trafficDistanceTracker.GetTargetDistance(car, car.ExtraCheckCar);
        if (distance < targetDistance) {
            float t = (car.ExtraCheckCar.CheckDot != null) ? 0f : 0.5f;
            car.TargetSpeed = car.ExtraCheckCar.TargetSpeed - t;
            _checkState.SetState<CarCheckDistanceCheckCarState>(car);
            _drivingState.SetState<CarStateSlowDown>(new DrivingState.DrivingParams(car, car.TargetSpeed, 0.5f));
        }
        else if(distance > (targetDistance + car.transform.localScale.y / 2)) {
            if (!TryState(car)) {
                car.TargetSpeed = car.FixedSpeed;
                _checkState.SetState<CarCheckExtraCarState>(car);
                _drivingState.SetState<CarStatePowerUp>(new DrivingState.DrivingParams(car, car.TargetSpeed));
            }
        }
    }

    private bool TryState(CarAbstract car) {
        return car.ExtraCheckCar.CurrentState is CarStateDriving || car.ExtraCheckCar.CurrentState is CarStatePowerUp;
    }

    private bool TryCar(CarAbstract car)
    {
        return car.ExtraCheckCar.TargetDot.DotTraffic.Area.Type == AreaTypes.Mixed &&
               car.ExtraCheckCar.TargetDot.Type == car.TargetDot.Type;
    }
}