using System.Collections.Generic;
using UnityEngine;

public abstract class AreaAbstract : MonoBehaviour
{
    public AreaTypes Type;
    public List<Tile> Tiles;
    public abstract void Action();
    public abstract void EnableArea(bool isActive);
}