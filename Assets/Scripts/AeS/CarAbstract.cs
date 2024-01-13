using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class CarAbstract : MonoBehaviour
{
    public Vector3 EndPos { get; set; }
    public ITrafficable CarArea { get; set; }
    public float SpeedMultiplier =  1; 
    public CarStatus carStatus => new CarStatus(Speed,this);
    
    public bool IsSlowDown;
    public float Speed; 
    public Transform RayDot;
    
    public class CarStatus
    {
        public float TimeForMove { get; set; }
        public float RayDistance { get; set; } 
        public float FixedSpeed;
        public CarStatus(float fixedSpeed, CarAbstract car)
        {
            TimeForMove = car.transform.localScale.y;
            RayDistance = TimeForMove + 0.5f;
            FixedSpeed = fixedSpeed;
        }
    }
}
