using UnityEngine;

public class CarSimple : CarAbstract
{
    private void OnDrawGizmos() {
        Gizmos.DrawLine(transform.position, EndPos);
    }
}