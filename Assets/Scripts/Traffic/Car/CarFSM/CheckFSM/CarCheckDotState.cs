using Zenject;

public class CarCheckDotState : CheckFSM
{
    private DrivingState _drivingState;
    private TrafficDistanceTracker _trafficDistanceTracker;

    [Inject]
    private void Construct(TrafficDistanceTracker trafficDistanceTracker, DrivingState drivingState)
    {
        _trafficDistanceTracker = trafficDistanceTracker;
        _drivingState = drivingState;
    }
    public override void Enter(CarAbstract car)
    {
        if (car.CheckDot != null)
        {
            float distance = _trafficDistanceTracker.GetDistance(car.transform.position, car.TargetDot.Pos);
            ProcessingDistance(car, distance);
        }
    }
    private void ProcessingDistance(CarAbstract car, float distance)
    {
        if (distance < car.transform.localScale.y) {
            if (TryState(car)) 
                _drivingState.SetState<CarStateSlowDown>(car);
        }
    }

    private bool TryState(CarAbstract car)
    {
        return (car.CurrentState is CarStatePowerUp || car.CurrentState is CarStateDriving);
    }
}