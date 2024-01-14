using System.Collections.Generic;
using UnityEngine;

public class WallPooler : MonoBehaviour
{
    [SerializeField] private CrossRoadWall crossRoadWall;
    public List<CrossRoadWall> _walls = new();
    
    public void SpawnCrossRoadWall(TrafficDot.Dot dot)
    {
        CrossRoadWall wall = FindWall();
        if (!_walls.Contains(wall)) {
            _walls.Add(wall);
        }
        wall.Dot = dot;
        wall.IsUse = true;
        wall.gameObject.SetActive(true);
    }

    private CrossRoadWall FindWall()
    {
        foreach (var wall in _walls) {
            if (!wall.IsUse && wall.Dot == null) 
                return wall;
        }
        return Instantiate(crossRoadWall);
    }

    public void ChangeRoadSide(CrossRoadWall wall, bool isChange) {
        wall.gameObject.SetActive(isChange); 
    }

    public void ReturnToPool(CrossRoadWall wall) {
        wall.IsUse = false;
        wall.Dot = null;
        wall.gameObject.SetActive(false);
    }
}
