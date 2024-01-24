using UnityEngine;

public abstract class CarAbstract : MonoBehaviour
{
    public DrivingFSM CurrentState;
    public CheckFSM CurrentCehckState;
    public TrafficDot.Dot TargetDot { get; set; }
    public ITrafficable CarArea;
    public CarTypes Type;
    public Transform RayDot;
    public float TimeForMove; 
    public float Speed; 
    public float FixedSpeed;
    public float RayDistance; 
    public TrafficDot CrossRoadDot;
    public CarAbstract CheckCar;
    public Vector3 CheckDot;
}
