using UnityEngine;
using Zenject;

public class CarStatePowerUp : FSM
{
    private CarSlowSpeedModifier _carSlowSpeedModifier;

    [Inject]
    public void Construct(CarSlowSpeedModifier carSlowSpeedModifier) {
        _carSlowSpeedModifier = carSlowSpeedModifier;
    }

    public override void EnterRowerUp(CarAbstract car, float targetSpeed) {
        car.StartCoroutine(_carSlowSpeedModifier.ChangeSpeed(car, GetTargetSpeed(car) ,car.TimeForMove));
    }

    private float GetTargetSpeed(CarAbstract car)
    {
        if (_carSlowSpeedModifier._speedCars.TryGetValue(car.Type, out var ts)) {
            return ts;
        }
        return 0;
    }
}
