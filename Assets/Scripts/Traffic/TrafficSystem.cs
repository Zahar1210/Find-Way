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
    private void GetDot(TrafficDot dot)
    {
        if (dot.FrontDot == null && _trafficAreas.TryGetValue(dot.Area.SpawnIndex + 1, out var area)) {
            IName trafficArea = area.GetComponent<IName>();
            if (trafficArea != null)
                dot.FrontDot = trafficArea.Dot;
        }
        if (dot.BackDot == null && _trafficAreas.TryGetValue(dot.Area.SpawnIndex - 1, out var _area)) {
            IName trafficArea = _area.GetComponent<IName>();
            if (trafficArea != null)
                dot.BackDot = trafficArea.Dot;
        }
    }
}