using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    [SerializeField] private SetValues setValues;
    [SerializeField] private Directions dir;
    public Dictionary<Vector3Int, Tile> _tiles = new();
    public List<Tile> tiles = new();

    private void Awake() {
        FindTiles();
    }
    public void FindTiles() {
        setValues.Set();
        foreach (var tile in FindObjectsOfType<Tile>()) {
            if (!_tiles.ContainsValue(tile)) {
                _tiles.Add(Vector3Int.RoundToInt(tile.transform.position), tile);
                tiles.Add(tile);
            }
        }
    }
    public Surface[] GetPath(Surface a, Surface b) {
        List<Vector3Int> queue = new();
        Dictionary<Vector3Int, TileInfo> visited = new();
        int step = 0;
        queue.Add(a.tile.Pos);
        SetValue(a.tile.tileSurfaces, a);
        SetFreeValue();
        while (true) {
            if (queue.Count == 0)
                break;
            GetQueue(queue, visited, step, b, a);
            step++;
            if (visited.ContainsKey(b.tile.Pos)) {
                break;
            }
        }
        Surface[] pointArray = GetFinalPath(visited, a, b).ToArray();
        Array.Reverse(pointArray);
        return pointArray;
    }
    private void GetQueue(List<Vector3Int> queueTiles, Dictionary<Vector3Int, TileInfo> visitedTiles, int step, Surface targetSurface, Surface startSurface)
    {
        Vector3Int[] tiles = queueTiles.ToArray();
        foreach (var t in tiles) {
            queueTiles.Remove(t);
            visitedTiles.Add(t, new TileInfo(step));
            foreach (var direction in dir.directions) {
                if (_tiles.TryGetValue(t + direction, out var tile)) {
                    if (!visitedTiles.ContainsKey(tile.Pos) && !queueTiles.Contains(tile.Pos) && !tile.Barrier) {
                        queueTiles.Add(tile.Pos);
                        SetValue(tile.tileSurfaces, startSurface);
                    }
                }
            }
        }
    }
    private List<Surface> GetFinalPath(Dictionary<Vector3Int, TileInfo> visited, Surface a, Surface b)
    {
        List<Surface> path = new();
        List<Tile> selectTiles = new();
        List<Tile> selectTilesCopy = new();
        Surface currentSurface = b;
        for (int i = 0; i < 55; i++) {
            selectTiles.Clear();
            selectTilesCopy.Clear();
            path.Add(currentSurface);
            foreach (var direction in SelectDirection(currentSurface)) {
                Vector3Int tilePos = currentSurface.tile.Pos + direction;
                if (visited.TryGetValue(tilePos, out var tileSurface) && tileSurface.Step == visited[currentSurface.tile.Pos].Step - 1) {
                    if (_tiles.TryGetValue(tilePos, out Tile tile) && tile.surfaces.TryGetValue(currentSurface.type, out var surface)) {
                        if (!path.Contains(surface) && surface.gameObject.activeSelf && !surface.barrier) {
                            selectTiles.Add(tile);
                            if (surface == a) {
                                    path.Add(surface);
                                    return path; 
                            }
                        }
                    }
                }
            }
            foreach (var dir in currentSurface.Directions) {
                Vector3Int posTile = currentSurface.tile.Pos + dir;
                if (_tiles.TryGetValue(posTile, out Tile tile) && tile != null)
                    selectTilesCopy.Add(tile);
            }
            currentSurface = SelectTileSurfaces(selectTiles, currentSurface, selectTilesCopy, visited).OrderBy(s => s.distance).FirstOrDefault();
            if (currentSurface == a) {
                path.Add(currentSurface);
                return path;
            }
        }
        return path;
    }
    private List<Surface> SelectTileSurfaces(List<Tile> tiles, Surface currentSurface, List<Tile> selectedTilesCopy,Dictionary<Vector3Int, TileInfo> visited)
    {
        List<Surface> selectedSurfaces = new();
        foreach (var tile in tiles) {
            if (tile.surfaces.TryGetValue(currentSurface.type, out var surface))
                selectedSurfaces.Add(surface);
        }
        foreach (var s in currentSurface.tile.tileSurfaces) {
            if (s.groupType != currentSurface.groupType && s.gameObject.activeSelf && !s.barrier)
                selectedSurfaces.Add(s);
        }
        foreach (var tile in selectedTilesCopy) {
            if (visited.TryGetValue(tile.Pos, out TileInfo tileInfo) && tileInfo != null) {
                selectedSurfaces.Add(SelectSurfacesTile(tile,currentSurface));
            }
        }
        return selectedSurfaces;
    }
    private Surface GetSurface(Tile tile, SurfaceType surfaceType)
    {
        if (tile.surfaces.TryGetValue(surfaceType, out Surface sur) && sur.gameObject.activeSelf && !sur.barrier) return sur;
        return null;
    }
    private void SetValue(List<Surface> surfaces, Surface startSurface)
    {
        foreach (var s in surfaces)
            if (s.gameObject.activeSelf) {
                float tileDis = Vector3.Distance(s.tile.Pos, startSurface.tile.Pos);
                float surfaceDis = Vector3.Distance(s.Pos, startSurface.Pos);
                s.distance = surfaceDis + tileDis;
            }
    }
    private void SetFreeValue()
    {
        foreach (var tile in tiles) {
            foreach (var surface in tile.tileSurfaces)
                surface.gameObject.SetActive(!CheckTileSurface(surface));
        }
    }
    private bool CheckTileSurface(Surface surface) {
        if (_tiles.TryGetValue(surface.tile.Pos + surface.Dir, out var t)) return t;
        return false;
    }
    private Surface SelectSurfacesTile(Tile tile, Surface currentSurface)
    {
        SurfaceType type = new();
        if (currentSurface.groupType == SurfaceGroup.Horizontal) {
            if (tile.Pos.z == currentSurface.tile.Pos.z)
                type = currentSurface.tile.Pos.y < tile.Pos.y ? SurfaceType.Down : SurfaceType.Up;
            if (tile.Pos.y == currentSurface.tile.Pos.y)
                type= currentSurface.tile.Pos.z < tile.Pos.z ? SurfaceType.Back : SurfaceType.Front;
        } 
        else if(currentSurface.groupType == SurfaceGroup.Vertically) {
            if (tile.Pos.x == currentSurface.tile.Pos.x)
                type = currentSurface.tile.Pos.z < tile.Pos.z ? SurfaceType.Back : SurfaceType.Front;
            if (tile.Pos.z == currentSurface.tile.Pos.z)
                type = currentSurface.tile.Pos.x < tile.Pos.x ? SurfaceType.Left : SurfaceType.Right;
        }
        else if (currentSurface.groupType == SurfaceGroup.Depth) {
            if (tile.Pos.y == currentSurface.tile.Pos.y)
                type = currentSurface.tile.Pos.x < tile.Pos.x ? SurfaceType.Left : SurfaceType.Right;
            if (tile.Pos.x == currentSurface.tile.Pos.x)
                type = currentSurface.tile.Pos.y < tile.Pos.y ? SurfaceType.Down : SurfaceType.Up;
        }
        return GetSurface(tile, type);
    }
    private Vector3Int[] SelectDirection(Surface currentSurface)
    {
        if (currentSurface.groupType == SurfaceGroup.Horizontal)
            return dir.dirHorizontal;
        if (currentSurface.groupType == SurfaceGroup.Vertically)
            return dir.dirVertical;
        if (currentSurface.groupType == SurfaceGroup.Depth)
            return dir.dirDepth;
        return null;
    }
    private class TileInfo {
        public int Step { get; }
        public TileInfo(int step) {
            Step = step;
        }
    }
}