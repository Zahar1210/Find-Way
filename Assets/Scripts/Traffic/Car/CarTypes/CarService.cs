using UnityEngine;

public class CarService : CarAbstract
{
    private void OnDrawGizmos() {
        Gizmos.color = Color.gray;
        Gizmos.DrawSphere(EndPos, 0.4f);
    }
}