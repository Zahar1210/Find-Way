using Zenject;

public class CarStatePowerUp : FSM
{
    private CarAbstract _car;
    private CarSlowSpeedModifier _carSlowSpeedModifier;

    [Inject]
    public void Construct(CarSlowSpeedModifier carSlowSpeedModifier) {
        _carSlowSpeedModifier = carSlowSpeedModifier;
    }

    public override void EnterRowerUp(CarAbstract car, float targetSpeed) {
        _car = car;
        _car.StartCoroutine(_carSlowSpeedModifier.ChangeSpeed(_car, targetSpeed, _car.TimeForMove));
    }
}
