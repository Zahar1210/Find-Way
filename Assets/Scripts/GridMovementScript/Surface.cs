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
    public Renderer renderer;

    void Awake()
    {
        renderer = GetComponent<Renderer>();
        Pos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        tile = transform.parent.GetComponent<Tile>();
    }
}