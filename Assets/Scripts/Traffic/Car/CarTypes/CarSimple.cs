using UnityEngine;

public class CarSimple : CarAbstract
{
    private Color _colorCar1;
    private Color _colorDot1;
    private Color _colorECar1;
    private void Awake()
    {
        _colorCar1 = Color.white;
        _colorDot1 = Color.cyan;
        _colorECar1 = Color.yellow;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.gray;
        if (TargetDot != null) {
            Gizmos.DrawSphere(TargetDot.Pos, 0.4f);
        }
        if (CheckCar != null && CheckCar.TargetDot == TargetDot)
        {
            Gizmos.color = _colorCar1;
            Gizmos.DrawLine(transform.position, CheckCar.transform.position);
        }
        else if (CheckDot != null) {
            Gizmos.color = _colorDot1; 
            Gizmos.DrawLine(transform.position, CheckDot.Pos);
        }
        else if (ExtraCheckCar != null) 
        {
            Gizmos.color = _colorECar1;
            Gizmos.DrawLine(transform.position, ExtraCheckCar.transform.position);
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