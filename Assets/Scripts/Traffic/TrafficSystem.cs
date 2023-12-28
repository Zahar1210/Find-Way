using System;
using System.Collections.Generic;
using UnityEngine;

public class TrafficSystem : MonoBehaviour
{
    public List<TrafficDot> _trafficDots = new();
    public Dictionary<int, AreaAbstract> _trafficAreas = new();
    public void SetTraffic()
    {
        foreach (var dot in FindObjectsOfType<TrafficDot>()) {
            GetDot(dot);
            if (!_trafficDots.Contains(dot)) {
                dot.SetDot();
                _trafficDots.Add(dot);
                _trafficAreas.Add(dot.Area.SpawnIndex, dot.Area);
            }
        }
    }
    
    public TrafficDot GetDotArea(AreaAbstract area)
    {
        foreach (var dot in FindObjectsOfType<TrafficDot>()) {
            if (dot.Area == area)
                return dot;
        }
        return null;
    }

    private void GetDot(TrafficDot dot)
    {
        if (_trafficAreas.TryGetValue(dot.Area.SpawnIndex + 1, out var area))
            dot.FrontDot = GetDotArea(area);
        if (_trafficAreas.TryGetValue(dot.Area.SpawnIndex - 1, out var _area))
            dot.BackDot = GetDotArea(_area);
    }
}