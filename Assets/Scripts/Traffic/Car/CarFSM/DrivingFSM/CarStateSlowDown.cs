using UnityEngine;
using Zenject;

public class CarStateSlowDown : DrivingFSM
{
    private CarAbstract _car;
    private CarSpeedModifier _carSpeedModifier;

    [Inject]
    public void Construct(CarSpeedModifier carSpeedModifier) {
        _carSpeedModifier = carSpeedModifier;
    }
    
    public override void EnterSlowDown(CarAbstract car, float targetSpeed,float timeForMove) {
        _car = car;
        _car.StartCoroutine(_carSpeedModifier.ChangeSpeed(_car, targetSpeed, timeForMove));
    }
}