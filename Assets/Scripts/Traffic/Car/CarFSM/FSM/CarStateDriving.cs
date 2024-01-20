using UnityEngine;
using Zenject;

public class CarStateDriving : FSM
{
    private DotFinding _dotFinding;
    private CarDriving _carDriving;
    private CheckForward _checkForward;
    private CrossRoad _crossRoad;

    [Inject]
    private void Construct(DotFinding dotFinding, CarDriving carDriving, CheckForward checkForward, CrossRoad crossRoad)
    {
        _crossRoad = crossRoad;
        _dotFinding = dotFinding;
        _carDriving = carDriving;
        _checkForward = checkForward;
    }

    public override void EnterDriving(TrafficDot.Dot a, CarAbstract car)
    {
        TrafficDot.Dot b = _dotFinding.GetDot(a);
        Vector3 center = _dotFinding.GetPivotDot(a, b);
        StartMove(a, b, car, center);
        car.StartCoroutine(_checkForward.ShootRayCoroutine(new TrafficSystem.MoveDots(a, b, center), car));
    }

    private void StartMove(TrafficDot.Dot a, TrafficDot.Dot b, CarAbstract car, Vector3 center)
    {
        if (b != null) {
            if (b.DotTraffic.Area.Type == a.DotTraffic.Area.Type)
                _carDriving.Move(a, b, (b.Type == a.Type)? Vector3.zero: center, car);
            else
                _carDriving.Move(a, b, (a.DotTraffic.Area.Type == AreaTypes.Traffic)
                        ? a.DotTraffic.transform.position
                        : b.DotTraffic.transform.position, car);
        }
    }
}