using UnityEngine;

public class CarService : CarAbstract
{
    private void OnDrawGizmos()
    {
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
            if (CurrentCehckState is CarCheckCarState)
            {
                Gizmos.color = Color.yellow;
            }
            else if(CurrentCehckState is CarCheckDistanceCheckCarState)
            {
                Gizmos.color = Color.red; 
            }
            else if(CurrentCehckState is CarCheckDotState)
            {
                Gizmos.color = Color.cyan; 
            }
            else if(CurrentCehckState is CarCheckExtraCarState)
            {
                Gizmos.color = Color.magenta;
            }
            Gizmos.DrawCube(RayDot.transform.position, new Vector3(0.3f,0.7f,0.7f));
        }
    }
}