using UnityEngine;
using Zenject;

public class CarChecking : MonoBehaviour
{
    private CheckState _checkState;
    private DotFinding _dotFinding;
    private TrafficSystem _trafficSystem;
    private TrafficDistanceTracker _trafficDistanceTracker;

    [Inject]
    private void Construct(TrafficDistanceTracker trafficDistanceTracker, CheckState checkState, DotFinding dotFinding, TrafficSystem trafficSystem)
    {
        _checkState = checkState;
        _dotFinding = dotFinding;
        _trafficSystem = trafficSystem;
        _trafficDistanceTracker = trafficDistanceTracker;
    }
    public void StartChecking(CarAbstract car)
    {
        car.CheckCar = _trafficDistanceTracker.GetCarForCheck(car, car.TargetDot);
        if (car.CheckCar != null) {
            _checkState.SetState<CarCheckCarState>(car);
        }
        else {
            CarAbstract extraCar = GetExtraCheckCar(car);
            if (extraCar != null) {
                car.ExtraCheckCar = extraCar;
                _checkState.SetState<CarCheckExtraCarState>(car);
            }
            return;
        }
        car.CheckDot = _trafficDistanceTracker.GetDotForCheck(car);
        if (car.CheckDot != null) { 
            _checkState.SetState<CarCheckDotState>(car);
        }
    }

    private CarAbstract GetExtraCheckCar(CarAbstract car)
    {
        CarAbstract extraCar = null;
        if (car.TargetDot.DotTraffic.Area.Type == AreaTypes.Mixed && TryArea(car)) {
            TrafficDot.Dot extraDot = _dotFinding.GetDot(car.TargetDot);
            extraCar = _trafficDistanceTracker.GetCarForCheck(car, extraDot);
            if (extraCar != null) 
                return extraCar;
        }

        return null;
    }


    private bool TryArea(CarAbstract car)
    {
        int index = 0;
        index = (car.TargetDot.Type == DotType.Left) ? 1: -1;
        if (car.TargetDot.DotTraffic.Area.SpawnIndex != _trafficSystem.pastArea.SpawnIndex) {
            if (_trafficSystem._traffic.TryGetValue(car.TargetDot.DotTraffic.Area.SpawnIndex + index, out var areaDot) && areaDot.Area.Type == AreaTypes.Mixed) {
                return true;
            }
        }
        return false;
    }
}