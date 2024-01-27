using UnityEngine;

public abstract class CarAbstract : MonoBehaviour
{
    public TrafficDot.Dot TargetDot { get; set; }
    public TrafficDot CrossRoadDot { get; set; }
    public ITrafficable CarArea { get; set; }
    
    public DrivingFSM CurrentState;
    public CheckFSM CurrentCehckState;
    public CarTypes Type;
    public Transform RayDot;
    public DrivingState.DrivingParams DrivingParams;
    
    [Header("Check")]
    public CarAbstract CheckCar;
    public TrafficDot.Dot CheckDot;
    public CarAbstract ExtraCheckCar;
    
    [Header("Speed")]
    public float TargetSpeed;
    public float FixedSpeed;
    public float Speed;

    [Header("GizmosDraw")]
    public Color ColorCar;
    public Color ColorExtraCar;
    public Color ColorDot;
}
