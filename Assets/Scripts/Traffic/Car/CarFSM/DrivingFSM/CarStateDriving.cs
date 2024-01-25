using UnityEngine;
using Zenject;

public class CarStateDriving : DrivingFSM
{
    private DotFinding _dotFinding;
    private CarDriving _carDriving;

    [Inject]
    private void Construct(DotFinding dotFinding, CarDriving carDriving, CrossRoad crossRoad)
    {
        _dotFinding = dotFinding;
        _carDriving = carDriving;
    }

    public override void EnterDriving(TrafficDot.Dot a, CarAbstract car)
    {
        TrafficDot.Dot b = _dotFinding.GetDot(a);
        Vector3 center = _dotFinding.GetPivotDot(a, b);
        StartMove(a, b, car, center);
    }

    private void StartMove(TrafficDot.Dot a, TrafficDot.Dot b, CarAbstract car, Vector3 center)
    {
        if (b != null) {
            if (b.DotTraffic.Area.Type == a.DotTraffic.Area.Type)
                _carDriving.Move(a, b, (b.Type == a.Type)? Vector3.zero: center, car);
            else
                _carDriving.Move(a, b, (a.DotTraffic.Area.Type == AreaTypes.Traffic) ? a.DotTraffic.transform.position 
                    : b.DotTraffic.transform.position, car);
        }
    }
}