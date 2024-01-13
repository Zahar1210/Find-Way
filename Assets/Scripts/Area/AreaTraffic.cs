using UnityEngine;

public class AreaTraffic : AreaAbstract, ITrafficable
{
    [SerializeField] private TrafficDot _dot;
    public TrafficDot Dot { get; set;}

    private void Awake() {
        Dot = _dot;
    }
    private void Start() {
        foreach (Transform child in transform) {
            Tile tile = child.GetComponent<Tile>();
            if (tile) {
                Tiles.Add(tile);
            }
        }
    }

    public override void Action()
    {
    }
}