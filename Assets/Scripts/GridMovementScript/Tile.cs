using System;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{ 
    public Surface surfaceRight;
    public Surface surfaceLeft;
    public Surface surfaceUp;
    public Surface surfaceDown;
    public Surface surfaceFront;
    public Surface surfaceBack;
    public Vector3Int Pos { get; set; }
    public Dictionary<SurfaceType, Surface> surfaces = new();
    public List<Surface> tileSurfaces = new();
    public bool Barrier;
    
    private void Start() {
        Pos = Vector3Int.RoundToInt(transform.position);
    }
}