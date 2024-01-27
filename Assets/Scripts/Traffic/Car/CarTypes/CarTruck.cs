using UnityEngine;

public class CarTruck : CarAbstract
{
    private void OnDrawGizmos() {
        DrawLine();
        DrawState();
        void DrawLine()
        {
            if (TargetDot != null) {
                Gizmos.color = Color.grey;
                Gizmos.DrawSphere(TargetDot.Pos, 0.4f);
            }
            if (CheckCar != null && CheckCar.TargetDot == TargetDot) {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, CheckCar.transform.position);
            }
            else if (CheckDot != null) {
                Gizmos.color = Color.cyan; 
                Gizmos.DrawLine(transform.position, CheckDot.Pos);
            }
            else if (ExtraCheckCar != null) 
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(transform.position, ExtraCheckCar.transform.position);
            }
        }

        void DrawState()
        {
            if (CurrentCehckState is CheckStateCar) {
                Gizmos.color = Color.yellow;
            }
            else if(CurrentCehckState is CheckStateCarDistance) {
                Gizmos.color = Color.red; 
            }
            else if(CurrentCehckState is CarCheckDotState) {
                Gizmos.color = Color.green; 
            }
            Gizmos.DrawCube(RayDot.transform.position, new Vector3(0.3f,0.7f,0.7f));
        }
        if (isDraw && CurrentCehckState is CheckStateCar)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(RayDot.transform.position, 0.6f);
        }
        // if (CurrentState is CarStatePowerUp) {
        //     Gizmos.color = Color.blue;
        // }
        // else if(CurrentState is CarStateSlowDown) {
        //     Gizmos.color = Color.red;
        // }
        // else if(CurrentState is CarStateDriving) {
        //     Gizmos.color = Color.white;
        // }
        // Gizmos.DrawCube(RayDot.transform.position, new Vector3(0.3f,0.7f,0.7f));
    }
}