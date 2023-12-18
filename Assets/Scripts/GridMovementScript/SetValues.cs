using UnityEngine;

public class SetValues : MonoBehaviour
{
    [SerializeField] private Directions directions;
    void Start()
    {
        foreach (var tile in FindObjectsOfType<Tile>()) {
            SetTileDictionary(tile);
            FindAndAddChildrenToList(tile);
        }
    }
    private void SetTileDictionary(Tile tile)
    {
        tile.surfaces.Add(SurfaceType.Right, tile.surfaceRight);
        tile.surfaces.Add(SurfaceType.Left, tile.surfaceLeft);
        tile.surfaces.Add(SurfaceType.Up, tile.surfaceUp);
        tile.surfaces.Add(SurfaceType.Down, tile.surfaceDown);
        tile.surfaces.Add(SurfaceType.Front, tile.surfaceFront);
        tile.surfaces.Add(SurfaceType.Back, tile.surfaceBack);
    }
    private void FindAndAddChildrenToList(Tile tile)
    {
        foreach (Transform child in tile.transform) {
            Surface sur = child.GetComponent<Surface>();
            if (sur != null) {
                GetSurfaceType(sur);
                tile.tileSurfaces.Add(sur);
            }
        }
    }
    private void GetSurfaceType(Surface sur)
    {
        Vector3 pos = sur.transform.localPosition;
        if (pos.x < 0f) {
            sur.type = SurfaceType.Left;
            sur.groupType = SurfaceGroup.Horizontal;
            sur.Dir = Vector3Int.left;
            sur.Directions = directions.dirLeft;
        }
        else if (pos.x > 0f) {
            sur.type = SurfaceType.Right;
            sur.groupType = SurfaceGroup.Horizontal;
            sur.Dir = Vector3Int.right;
            sur.Directions = directions.dirRight;
        }
        
        if (pos.y < 0f) {
            sur.type = SurfaceType.Down;
            sur.groupType = SurfaceGroup.Vertically;
            sur.Dir = Vector3Int.down;
            sur.Directions = directions.dirDown;

        }
        else if (pos.y > 0f) {
            sur.type = SurfaceType.Up;
            sur.groupType = SurfaceGroup.Vertically;
            sur.Dir = Vector3Int.up;
            sur.Directions = directions.dirUp;

        }

        if (pos.z < 0f) {
            sur.type = SurfaceType.Back;
            sur.groupType = SurfaceGroup.Depth;
            sur.Dir = Vector3Int.back;
            sur.Directions = directions.dirBack;

        }
        else if (pos.z > 0f) {
            sur.type = SurfaceType.Front;
            sur.groupType = SurfaceGroup.Depth;
            sur.Dir = Vector3Int.forward;
            sur.Directions = directions.dirFront;
        }
    }
}
