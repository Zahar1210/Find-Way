using UnityEngine;

public class CarTruck : CarAbstract
{
    private void OnDrawGizmos() {
        Gizmos.color = Color.gray;
        if (TargetDot != null) {
            Gizmos.DrawSphere(TargetDot.Pos, 0.4f);
        }
        if (CheckCar != null && CheckCar.TargetDot == TargetDot) {
            Gizmos.color = Color.yellow; 
            Gizmos.DrawLine(transform.position, CheckCar.transform.position);
        }
        if (CurrentState is CarStatePowerUp) {
            Gizmos.color = Color.blue;
        }
        else if(CurrentState is CarStateSlowDown) {
            Gizmos.color = Color.red;
        }
        else if(CurrentState is CarStateDriving) {
            Gizmos.color = Color.white;
        }
        Gizmos.DrawCube(RayDot.transform.position, new Vector3(0.3f,0.7f,0.7f));
    }
}