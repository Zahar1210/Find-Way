using UnityEngine;

public class Surface : MonoBehaviour
{
    public bool barrier;
    public SurfaceGroup groupType { get; set; }
    public SurfaceType type { get; set; }
    public float distance { get; set; }
    public Tile tile { get; set; }
    public Vector3Int Dir { get; set; }
    public Vector3Int[] Directions { get; set; }
    public Vector3 Pos { get; set; }

    void Start() {
        tile = transform.parent.GetComponent<Tile>();
        GetValue();
    }
    public void GetValue() {
        Pos = Vector3Int.RoundToInt(transform.position);
    }
}