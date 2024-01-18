using UnityEngine;
using Zenject;

public class CarStateDriving : FSM
{
    private CarAbstract _car;
    private DotFinding _dotFinding;
    private CarDriving _carDriving;
    private CheckForward _checkForward;

    [Inject]
    private void Construct(DotFinding dotFinding, CarDriving carDriving, CheckForward checkForward) {
        _dotFinding = dotFinding;
        _carDriving = carDriving;
        _checkForward = checkForward;
    }
    
    public override void Enter(TrafficDot.Dot a, CarAbstract car)
    {
        Debug.Log(_checkForward);
        _car = car;
        TrafficDot.Dot b = _dotFinding.GetDot(a);
        Vector3 center = _dotFinding.GetPivotDot(a, b);
        StartMove(a, b, car, center);
        car.StartCoroutine(_checkForward.ShootRayCoroutine(new TrafficSystem.MoveDots(a, b, center), car));
    }

    private void StartMove(TrafficDot.Dot a, TrafficDot.Dot b, CarAbstract car, Vector3 center)
    {
        if (b != null) {
            if (b.DotTraffic.Area.Type == a.DotTraffic.Area.Type) {
                if (b.Type == a.Type)
                    _carDriving.Move(a, b, Vector3.zero, car);
                else
                    _carDriving.Move(a, b, center, car);
            }
            else {
                if (a.DotTraffic.Area.Type == AreaTypes.Traffic)
                    _carDriving.Move(a, b, a.DotTraffic.transform.position, car);
                else
                    _carDriving.Move(a, b, b.DotTraffic.transform.position, car);
            }
        }
    }

    public override void Exit() {
        _car.CurrentState = null;
    }
}
