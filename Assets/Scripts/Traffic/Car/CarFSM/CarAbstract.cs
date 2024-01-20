using UnityEngine;
using UnityEngine.Serialization;

public abstract class CarAbstract : MonoBehaviour
{
    public FSM CurrentState;
    public Vector3 EndPos { get; set; }
    public ITrafficable CarArea { get; set; }
    public CarTypes Type;
    public Transform RayDot;
    public float TimeForMove; 
    public float Speed; 
    public float FixedSpeed;
    public float RayDistance; 
    public TrafficDot CrossRoadDot;
}
