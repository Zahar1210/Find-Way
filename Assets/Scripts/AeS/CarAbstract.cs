using UnityEngine;
using UnityEngine.Serialization;

public abstract class CarAbstract : MonoBehaviour
{
    public Vector3 EndPos { get; set; }
    public ITrafficable CarArea { get; set; }
    public CarType Type;
    public float Speed;
    public float FixedSpeed;
    public float RayDistance;
    public float TimeForMove;
    public LayerMask layerMask;
    public Transform RayDot;
}
