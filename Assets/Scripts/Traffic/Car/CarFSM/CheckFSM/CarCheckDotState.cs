using Zenject;

public class CarCheckDotState : CheckFSM
{
    private CrossRoad _crossRoad;
    private DrivingState _drivingState;
    private TrafficDistanceTracker _trafficDistanceTracker;

    [Inject]
    private void Construct(TrafficDistanceTracker trafficDistanceTracker, DrivingState drivingState, CrossRoad crossRoad) {
        _crossRoad = crossRoad;
        _trafficDistanceTracker = trafficDistanceTracker;
        _drivingState = drivingState;
    }
    public override void Enter(CarAbstract car)
    {
        if (car.CheckDot != null) {
            float distance = _trafficDistanceTracker.GetDistance(car.transform.position, car.TargetDot.Pos);
            ProcessingDistance(car, distance);
        }
    }
    private void ProcessingDistance(CarAbstract car, float distance)
    {
        if (distance < car.transform.localScale.y + 1f) {
            if (TryState(car) && _crossRoad._queueCars.Count > 0 && car.Speed > 1) {
                _drivingState.SetState<CarStateSlowDown>(new DrivingState.DrivingParams(car, 0.1f, 3f));
            }
        }
    }

    private bool TryState(CarAbstract car)
    {
        return (car.CurrentState is CarStatePowerUp || car.CurrentState is CarStateDriving);
    }
}