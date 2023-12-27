using System.Collections.Generic;
using UnityEngine;

public abstract class AreaAbstract : MonoBehaviour
{
    public int Index;
    public int AreaLength;
    public float Chance;
    public AreaTypes Type;
    public AreaAbstract[] Areas;
    public List<Tile> Tiles = new();
    public int SpawnIndex;
    public abstract void Action();
    public abstract void EnableArea(bool isActive);
}