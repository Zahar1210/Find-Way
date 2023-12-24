using System;
using System.Collections.Generic;
using UnityEngine;

public class Directions : MonoBehaviour
{
    public Dictionary<SurfaceType, Vector3Int> DirRay = new();

    private void Start() {
        DirRay.Add(SurfaceType.Right, Vector3Int.left);
        DirRay.Add(SurfaceType.Left, Vector3Int.right);
        DirRay.Add(SurfaceType.Up, Vector3Int.down);
        DirRay.Add(SurfaceType.Down, Vector3Int.up);
        DirRay.Add(SurfaceType.Front, Vector3Int.back);
        DirRay.Add(SurfaceType.Back, Vector3Int.forward);
    }

    public Vector3Int[] dirHorizontal = {
        Vector3Int.forward, Vector3Int.back, Vector3Int.up, Vector3Int.down, 
    };
    public Vector3Int[] dirVertical = {
        Vector3Int.back,Vector3Int.forward, Vector3Int.right,Vector3Int.left, 
    };
    public Vector3Int[] dirDepth = {
       Vector3Int.right, Vector3Int.left,Vector3Int.up, Vector3Int.down, 
    };
    public Vector3Int[] dirRight = {
        new Vector3Int(1, 1, 0), new Vector3Int(1, 0, 1), new Vector3Int(1, -1, 0), new Vector3Int(1, 0, -1),
    };
    public Vector3Int[] dirLeft = {
        new Vector3Int(-1, 1, 0), new Vector3Int(-1, -1, 0), new Vector3Int(-1, 0, 1), new Vector3Int(-1, 0, -1),
    };
    public Vector3Int[] dirUp = {
        new Vector3Int(1, 1, 0), new Vector3Int(-1, 1, 0), new Vector3Int(0, 1, 1), new Vector3Int(0, 1, -1),
    };
    public Vector3Int[] dirDown = {
        new Vector3Int(1, -1, 0), new Vector3Int(-1, -1, 0), new Vector3Int(0, -1, 1), new Vector3Int(0, -1, -1),
    };
    public Vector3Int[] dirFront = {
        new Vector3Int(1, 0, 1), new Vector3Int(-1, 0, 1), new Vector3Int(0, 1, 1), new Vector3Int(0, -1, 1),
    };
    public Vector3Int[] dirBack = {
        new Vector3Int(1, 0, -1), new Vector3Int(-1, 0, -1), new Vector3Int(0, 1, -1), new Vector3Int(0, -1, -1),
    };
    public Vector3Int[] directions = {
        Vector3Int.forward, Vector3Int.right, Vector3Int.back, Vector3Int.up, Vector3Int.left, Vector3Int.down,
    };
}
