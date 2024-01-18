using UnityEngine;
using Zenject;

public class CarStateSlowDown : FSM
{
    private CarAbstract car;
    private CarSlowDown carSlowDown;

    [Inject]
    public void Construct(CarSlowDown _carSlowDown)
    {
        carSlowDown = _carSlowDown;
    }

    public override void Enter(CarAbstract _car, float targetSpeed) {
        car = _car;
        car.StartCoroutine(carSlowDown.ChangeSpeed(car, targetSpeed, car.TimeForMove));
    }

    public override void Exit() {
        car.CurrentState = null;
    }
}