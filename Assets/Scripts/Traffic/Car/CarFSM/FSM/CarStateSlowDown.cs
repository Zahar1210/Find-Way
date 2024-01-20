using UnityEngine;
using Zenject;

public class CarStateSlowDown : FSM
{
    private CarAbstract _car;
    private CarSlowSpeedModifier _carSlowSpeedModifier;

    [Inject]
    public void Construct(CarSlowSpeedModifier carSlowSpeedModifier) {
        _carSlowSpeedModifier = carSlowSpeedModifier;
    }
    
    public override void EnterSlowDown(CarAbstract car, float targetSpeed,float timeForMove) {
        _car = car;
        _car.StartCoroutine(_carSlowSpeedModifier.ChangeSpeed(_car, targetSpeed, timeForMove));
    }
}