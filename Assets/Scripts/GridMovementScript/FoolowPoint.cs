using UnityEngine;

public class FoolowPoint : MonoBehaviour
{
    [SerializeField] private float deathDistance;
    [SerializeField] private Transform target;
    private Vector3 _lastPosition;
    private bool isDead;

    void Update()
    {
        if (target.transform.position.z > _lastPosition.z)
        {
            _lastPosition.z = target.transform.position.z;
            transform.position = new Vector3(transform.position.x, transform.position.y, target.position.z - 1);
        }

        if (_lastPosition.z - deathDistance >= target.transform.position.z && !isDead)
        {
            isDead = true;
            Debug.Log("умерли");
        }
    }
}