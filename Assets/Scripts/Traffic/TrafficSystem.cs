using System;
using System.Collections.Generic;
using UnityEngine;

public class TrafficSystem : MonoBehaviour
{
    public static TrafficSystem Instance { get; set; }
    public Dictionary<Vector3, TrafficDot> _trafficDots = new();
    public Dictionary<int, AreaAbstract> _trafficAreas = new();

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            return;
        }
        Destroy(gameObject);
    }
    public void FindTraffic()
    {
        foreach (var dot in FindObjectsOfType<TrafficDot>()) {
            if (!_trafficDots.ContainsValue(dot)) 
                _trafficDots.Add(dot.transform.position, dot);
        }
        foreach (var area in FindObjectsOfType<AreaAbstract>()) {
            if (!_trafficAreas.ContainsValue(area) && area.Type == AreaTypes.Traffic || area.Type == AreaTypes.Mixed) 
                _trafficAreas.Add(area.SpawnIndex, area);
        }
    }
}