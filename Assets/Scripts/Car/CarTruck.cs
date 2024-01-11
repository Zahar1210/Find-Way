using UnityEngine;

public class CarTruck : CarAbstract
{
    private void OnDrawGizmos() {
        Gizmos.DrawLine(transform.position, EndPos);
    }
}