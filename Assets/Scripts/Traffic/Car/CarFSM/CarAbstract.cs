using UnityEngine;

public abstract class CarAbstract : MonoBehaviour
{
    public FSM CurrentState;
    public Vector3 EndPos { get; set; }
    public ITrafficable CarArea { get; set; }
    public Transform RayDot;
    public float TimeForMove; 
    public float Speed; 
    public float FixedSpeed;
    public float RayDistance;
    public bool isCrossRoad;

}
