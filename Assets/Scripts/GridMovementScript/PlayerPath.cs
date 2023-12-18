using System;
using UnityEngine;
public class PlayerPath : MonoBehaviour
{
    [SerializeField] private Directions dirs;
    [SerializeField] private PathFinding pathFinder;
    [SerializeField] private PlayerMove playerMove;
    private Camera _camera;
    private bool isFindPath;

    private void Start()
    {
        _camera = Camera.main;
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            playerMove.StartMove();
            isFindPath = true;
        }

        if (!playerMove.isMoving && !playerMove.isMove && isFindPath && !playerMove.isMove)
        {
            Surface currentSurface = GetCurrentSurface();
            Surface targetSurface = GetTargetSurface();
            if (currentSurface && targetSurface)
                MakePath(pathFinder.GetPath(currentSurface, targetSurface));
            isFindPath = false;
        }
    }
    private void MakePath(Surface[] path)
    {
        playerMove.index = 0;
        Array.Clear(playerMove.surfaces, 0, playerMove.surfaces.Length);
        playerMove.PointRotations.Clear();
        for (int i = 0; i < path.Length - 1; i++)
        {
            if (path[i].tile == path[i + 1].tile)
                playerMove.PointRotations.Add(new PlayerMove.PointRotation(path[i].Dir, path[i + 1].Dir, path[i].tile,
                    false));
            else if (path[i].tile != path[i + 1].tile && path[i].type == path[i + 1].type)
                playerMove.PointRotations.Add(new PlayerMove.PointRotation(path[i].tile.Pos - path[i + 1].tile.Pos,
                    path[i + 1].Dir, path[i + 1].tile, false));
            else if (path[i].tile != path[i + 1].tile && path[i].type != path[i + 1].type)
                playerMove.PointRotations.Add(new PlayerMove.PointRotation(-path[i].Dir, path[i + 1].Dir,
                    path[i + 1].tile, true));
        }

        playerMove.surfaces = path;
        playerMove.Move();
    }
    private Surface GetTargetSurface()
    {
        RaycastHit hit;
        if (Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out hit)) {
            Surface targetSurface = hit.collider.GetComponent<Surface>();
            if (targetSurface != null)
                return targetSurface;
        }
        return null;
    }
    private Surface GetCurrentSurface()
    {
        SurfaceType type = GetDir();
        if (dirs.DirRay.TryGetValue(type, out var dir)) {
            if (pathFinder._tiles.TryGetValue(Vector3Int.RoundToInt(transform.position) + dir, out var tile)) {
                if (tile.surfaces.TryGetValue(type, out Surface surface))
                    return surface;
            }
        }

        return null;
    }
    private SurfaceType GetDir()
    {
        if (playerMove.index != 0) return playerMove.surfaces[playerMove.index].type;
        return SurfaceType.Up;
    }
}