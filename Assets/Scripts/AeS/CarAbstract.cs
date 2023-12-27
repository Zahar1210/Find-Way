using UnityEngine;

public abstract class CarAbstract : MonoBehaviour
{
    public CarType type;
    public float speed;

    public abstract void Move(TrafficDot a, TrafficDot b);
    public abstract void SlowDown();
    public abstract void Signal();
    public abstract void CheckForward();
    public abstract void CheckArea();
}
