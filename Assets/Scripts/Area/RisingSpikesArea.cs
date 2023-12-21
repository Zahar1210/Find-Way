using UnityEngine;

public class RisingSpikesArea : AreaAbstract
{
    private PathFinding _pathFinding;
    private void Start()
    {
        _pathFinding = PathFinding.Instance;
        foreach (Transform child in transform) {
            Tile tile = child.GetComponent<Tile>();
            if (tile)
                Tiles.Add(tile);
        }
    }
    public override void Action()
    {
      
    }
    public override void EnableArea(bool isActive)
    {
        if (isActive) {
            foreach (var t in Tiles) {
                t.SetValue();
            }
        }
        else if (!isActive) {
            foreach (var t in Tiles) {
                if (_pathFinding._tiles.TryGetValue(t.Pos, out var Tile)) {
                    _pathFinding.tiles.Remove(Tile);
                    _pathFinding._tiles.Remove(Tile.Pos);
                    Tile.Pos = Vector3Int.zero;
                }
            }
        }
        gameObject.SetActive(isActive);
    }
}
