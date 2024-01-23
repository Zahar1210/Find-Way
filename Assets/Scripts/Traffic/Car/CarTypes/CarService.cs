using UnityEngine;

public class CarService : CarAbstract
{
    private void OnDrawGizmos() {
        Gizmos.color = Color.gray;
        Gizmos.DrawSphere(TargetDot.Pos, 0.4f);
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
        else {
            Gizmos.color = Color.white;
        }
        Gizmos.DrawCube(new Vector3(transform.position.x + 0.7f ,transform.position.y), new Vector3(0.2f,0.2f,0.2f));
    }
}