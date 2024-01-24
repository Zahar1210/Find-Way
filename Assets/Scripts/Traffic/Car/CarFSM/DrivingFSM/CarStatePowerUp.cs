using UnityEngine;
using Zenject;

public class CarStatePowerUp : DrivingFSM
{
    private CarSpeedModifier _carSpeedModifier;

    [Inject]
    public void Construct(CarSpeedModifier carSpeedModifier) {
        _carSpeedModifier = carSpeedModifier;
    }

    public override void EnterRowerUp(CarAbstract car, float targetSpeed) {
        car.StartCoroutine(_carSpeedModifier.ChangeSpeed(car, car.FixedSpeed,car.TimeForMove));
    }
}
