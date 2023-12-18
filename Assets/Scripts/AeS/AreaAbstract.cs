using UnityEngine;

public abstract class AreaAbstract : MonoBehaviour
{
    public AreaTypes Type;
    public Tile[] Tiles;
    public abstract void Action();
    public abstract void EnableArea();
}