using UnityEngine;
using Zenject;

public class CheckStateCar : CheckFSM
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
        if (distance < targetDistance + 1) {
            car.isDraw = false;
            car.CheckCar.BehindCar = car;
            _checkState.SetState<CheckStateCarDistance>(car);
            _drivingState.SetState<CarStateSlowDown>(new DrivingState.DrivingParams(
                car,
                (car.CheckCar.Speed > 0.5) ? car.CheckCar.Speed - 0.5f : car.CheckCar.Speed, 
                0.5f));
        }
        else {
            car.isDraw = true;
        }
    }
    
    private void ProcessingDistanceExtraCar(CarAbstract car, float distance)
    {
        float targetDistance = _trafficDistanceTracker.GetTargetDistance(car, car.ExtraCheckCar);
        if (distance < targetDistance + 1) {
            car.isDraw = false;
            car.ExtraCheckCar.BehindCar = car;
            _checkState.SetState<CheckStateCarDistance>(car);
            _drivingState.SetState<CarStateSlowDown>(new DrivingState.DrivingParams(car,
                (car.ExtraCheckCar.Speed > 0.5) ? car.ExtraCheckCar.Speed - 0.5f : car.ExtraCheckCar.Speed,
                0.3f));
        }
        else
        {
            car.isDraw = true;
        }
    }
    private bool TryCar(CarAbstract car)
    {
        return  car.ExtraCheckCar.TargetDot != car.TargetDot 
                && car.ExtraCheckCar.TargetDot.DotTraffic.Area.Type == AreaTypes.Mixed 
                && car.ExtraCheckCar.TargetDot.Type == car.TargetDot.Type;
    }
}