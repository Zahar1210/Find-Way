using Zenject;

public class CarStateSlowDown : DrivingFSM
{
    private CarSpeedModifier _carSpeedModifier;

    [Inject]
    public void Construct(CarSpeedModifier carSpeedModifier) {
        _carSpeedModifier = carSpeedModifier;
    }
    
    public override void EnterSlowDown(CarAbstract car, float targetSpeed,float timeForMove) {
        car.StartCoroutine(_carSpeedModifier.ChangeSpeed(car, targetSpeed, timeForMove));
    }
}