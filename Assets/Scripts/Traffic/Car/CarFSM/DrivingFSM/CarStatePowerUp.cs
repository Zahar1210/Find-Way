using Zenject;

public class CarStatePowerUp : DrivingFSM
{
    private CarSpeedModifier _carSpeedModifier;
    private DrivingState _drivingState;

    [Inject]
    public void Construct(CarSpeedModifier carSpeedModifier, DrivingState drivingState)
    {
        _drivingState = drivingState;
        _carSpeedModifier = carSpeedModifier;
    }

    public override void EnterRowerUp(CarAbstract car, float targetSpeed) {
        car.StartCoroutine(_carSpeedModifier.ChangeSpeed(car, targetSpeed,2));
    }
}