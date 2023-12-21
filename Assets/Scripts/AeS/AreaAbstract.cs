using System.Collections.Generic;
using UnityEngine;

public abstract class AreaAbstract : MonoBehaviour
{
    public int AreaLength;
    public AreaTypes Type;
    public List<Tile> Tiles = new();
    public int[] AreaIndex;
    public abstract void Action();
    public abstract void EnableArea(bool isActive);
}