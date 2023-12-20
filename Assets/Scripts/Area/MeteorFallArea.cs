using System;
using UnityEngine;

public class MeteorFallArea : AreaAbstract
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
            foreach (var t in Tiles) {
                t.SetValue();
            }
        }
        else if (!isActive)
        {
            foreach (var t in Tiles) {
                if (_pathFinding._tiles.TryGetValue(t.Pos, out var Tile)) {
                    Tile.step = 0;
                    Tile.Pos = Vector3Int.zero;
                    _pathFinding.tiles.Remove(Tile);
                    _pathFinding._tiles.Remove(Tile.Pos);
                }
            }
        }
        gameObject.SetActive(isActive);
    }
}