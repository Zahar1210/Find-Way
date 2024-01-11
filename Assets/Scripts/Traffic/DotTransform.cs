
using UnityEngine;

public class DotTransform : MonoBehaviour
{
    public static DotTransform Instance { get; set; }
    private void Awake() {
        if (Instance == null) {
            Instance = this;
            return;
        }
        Destroy(gameObject);
    }

    public Vector3[] dotPositions = {
        new Vector3(-4, 0, -1),
        new Vector3(4, 0, -1),
        new Vector3(-19, 0, -1),
        new Vector3(10, 0, -1),
        new Vector3(-4, 0, 1),
        new Vector3(4, 0, 1),
        new Vector3(-19, 0, 1),
        new Vector3(10, 0, 1)
    };
    public Quaternion[] dotRotations = {
        new Quaternion(0f, 0f, 90f, 0f),
        new Quaternion(0f, 180f, 90f, 0f),
        new Quaternion(0f, 90f, 90f, 0f),
        new Quaternion(0f, 270f, 90f, 0f),
    };
}