using UnityEngine;

public class CarService : CarAbstract
{
    private Color _colorCar;
    private Color _colorDot;
    private Color _colorExtraCar;
    private void Awake()
    {
        _colorExtraCar = Color.red;
        _colorDot = Color.green;
        _colorCar = Color.magenta;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.gray;
        if (TargetDot != null) {
            Gizmos.DrawSphere(TargetDot.Pos, 0.4f);
        }
        if (CheckCar != null && CheckCar.TargetDot == TargetDot)
        {
            Gizmos.color = _colorCar;
            Gizmos.DrawLine(transform.position, CheckCar.transform.position);
        }
        else if (CheckDot != null) {
            Gizmos.color = _colorDot; 
            Gizmos.DrawLine(transform.position, CheckDot.Pos);
        }
        else if (ExtraCheckCar != null) 
        {
            Gizmos.color = _colorExtraCar;
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