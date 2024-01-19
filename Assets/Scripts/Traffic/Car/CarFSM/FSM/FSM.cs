
public abstract class FSM
{
    public virtual void EnterDriving(TrafficDot.Dot a, CarAbstract car){}
    public virtual void EnterSlowDown(CarAbstract car, float targetSpeed, float timeForMove){}
    public virtual void EnterRowerUp(CarAbstract car, float targetSpeed){}
}
