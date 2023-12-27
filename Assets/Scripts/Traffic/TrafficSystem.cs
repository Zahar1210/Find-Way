using System;
using System.Collections.Generic;
using UnityEngine;

public class TrafficSystem : MonoBehaviour
{
    public static TrafficSystem Instance { get; set; }
    private Dictionary<Vector3, TrafficDot> _trafficDots;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            return;
        }
        Destroy(gameObject);
    }

    private void FindTrafficDot()
    {
        foreach (var dot in FindObjectsOfType<TrafficDot>()) {
            if (!_trafficDots.ContainsValue(dot)) {
                _trafficDots.Add(dot.transform.position, dot);
            }
        }
    }
}