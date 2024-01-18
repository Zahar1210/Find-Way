
public abstract class FSM
{
    public virtual void Enter(TrafficDot.Dot a, CarAbstract car){}
    public virtual void Enter(CarAbstract _car, float targetSpeed){}
    public virtual void Exit(){}
}