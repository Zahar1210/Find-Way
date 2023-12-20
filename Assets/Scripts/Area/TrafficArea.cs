using UnityEngine;

public class TrafficArea : AreaAbstract
{
    private PathFinding _pathFinding;

    private void Start()
    {
        _pathFinding = PathFinding.Instance;
        foreach (Transform child in transform)
        {
            Tile tile = child.GetComponent<Tile>();
            if (tile)
            {
                Tiles.Add(tile);
            }
        }
    }

    public override void Action()
    {
    }

    public override void EnableArea(bool isActive)
    {
        if (isActive)
        {
            foreach (var t in Tiles)
            {
                t.SetValue();
                Debug.Log(t.Pos);
            }
        }
        else if (!isActive)
        {
            foreach (var t in Tiles)
            {
                if (_pathFinding._tiles.TryGetValue(t.Pos, out var Tile))
                {
                    Tile.Pos = Vector3Int.zero;
                    Tile.step = 0;
                    _pathFinding.tiles.Remove(Tile);
                    _pathFinding._tiles.Remove(Tile.Pos);
                }
            }
        }

        gameObject.SetActive(isActive);
    }
}