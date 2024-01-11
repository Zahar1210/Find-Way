using UnityEngine;

public class CarService : CarAbstract
{
    private void OnDrawGizmos() {
        Gizmos.DrawLine(transform.position, EndPos);
    }
}