using UnityEngine;

public class AreaMeteor_Traffic : AreaAbstract, IName
{
    [SerializeField] private TrafficDot _dot;
    public TrafficDot Dot { get; set;}

    private void Start()
    {
        Dot = _dot;
        foreach (Transform child in transform) {
            Tile tile = child.GetComponent<Tile>();
            if (tile)
                Tiles.Add(tile);
        }
    }

    public override void Action()
    {
    }
}