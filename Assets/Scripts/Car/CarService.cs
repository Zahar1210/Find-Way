using UnityEngine;

public class CarService : CarAbstract
{
    private void OnDrawGizmos() {
        // Gizmos.DrawLine(transform.position, EndPos);
        // Gizmos.color = Color.red;
        // Gizmos.DrawLine(transform.position, Vector3.up );
        Gizmos.color = Color.gray;
        Gizmos.DrawSphere(EndPos, 0.4f);

    }
}