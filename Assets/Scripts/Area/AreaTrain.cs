using UnityEngine;

public class AreaTrain : AreaAbstract
{
    private void Start()
    {
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
