using System;
using Zenject;

public class CheckStateCarDistance : CheckFSM
{
    private CarChecking _carChecking;
    private CheckState _checkState;
    private DrivingState _drivingState;
    private TrafficDistanceTracker _trafficDistanceTracker;

    [Inject]
    private void Construct(TrafficDistanceTracker trafficDistanceTracker, CheckState checkState, DrivingState drivingState, CarChecking carChecking)
    {
        _checkState = checkState;
        _carChecking = carChecking;
        _drivingState = drivingState;
        _trafficDistanceTracker = trafficDistanceTracker;
    }
    
    public override void Enter(CarAbstract car)
    {
        if (car.CheckCar != null) {
            if (car.CheckCar.TargetDot == car.TargetDot) {
                float distance = _trafficDistanceTracker.GetDistance(car.transform.position, car.CheckCar.transform.position);
                ProcessingDistanceCheckCar(car, distance);
            }
            else {
                _drivingState.SetState<CarStatePowerUp>(new DrivingState.DrivingParams(car, car.FixedSpeed, 3f));
                car.CheckCar = null;
                car.CurrentCehckState = null;
                car.CheckDot = _trafficDistanceTracker.GetDotForCheck(car);
                if (car.CheckDot != null) {
                    _checkState.SetState<CarCheckDotState>(car);
                }
            }
        }
        else if (car.ExtraCheckCar != null) {
            if (TryCar(car)) {
                float distance = _trafficDistanceTracker.GetDistance(car.transform.position, car.ExtraCheckCar.transform.position);
                ProcessingDistanceExtraCar(car, distance);
            }
            else {
                _drivingState.SetState<CarStatePowerUp>(new DrivingState.DrivingParams(car, car.FixedSpeed, 0.8f));
                car.ExtraCheckCar = null;
                car.CurrentCehckState = null;
                _carChecking.StartChecking(car);
            }
        }
    }
    private void ProcessingDistanceCheckCar(CarAbstract car, float distance)
    {
        float targetDistance = _trafficDistanceTracker.GetTargetDistance(car, car.CheckCar);
        if (distance > targetDistance + 1f) {
            car.CheckCar.BehindCar = car;
            _checkState.SetState<CheckStateCar>(car);
            _drivingState.SetState<CarStatePowerUp>(new DrivingState.DrivingParams(car, car.FixedSpeed, 0.8f));
        }
        else if (TryDistance(car, car.CheckCar)) {
            _drivingState.SetState<CarStateSlowDown>(new DrivingState.DrivingParams(car, car.CheckCar.Speed, 0.3f));
        }
    }
    
    private void ProcessingDistanceExtraCar(CarAbstract car, float distance)
    {
        float targetDistance = _trafficDistanceTracker.GetTargetDistance(car, car.ExtraCheckCar);
        if (distance > targetDistance + 0.3f) {
            car.ExtraCheckCar.BehindCar = car;
            _checkState.SetState<CheckStateCar>(car);
            _drivingState.SetState<CarStatePowerUp>(new DrivingState.DrivingParams(car, car.FixedSpeed, 0.6f));
        }
        else if (TryDistance(car, car.CheckCar)) {
            _drivingState.SetState<CarStateSlowDown>(new DrivingState.DrivingParams(car, car.CheckCar.Speed, 0.3f));
        }
    }
    
    private bool TryCar(CarAbstract car)
    {
        return  car.ExtraCheckCar.TargetDot != car.TargetDot 
                && car.ExtraCheckCar.TargetDot.DotTraffic.Area.Type == AreaTypes.Mixed 
                && car.ExtraCheckCar.TargetDot.Type == car.TargetDot.Type;
    }

    private bool TryDistance(CarAbstract car, CarAbstract checkCar)
    {
        if (Math.Abs(car.Speed - checkCar.Speed) > 0.1f) {
            return true;
        }
        return false;
    }
}