using System;
using System.Collections.Generic;
using UnityEngine;

public class TrafficSystem : MonoBehaviour
{
    public static TrafficSystem Instance { get; set;}
    private Dictionary<Vector3Int, TrafficTile> _trafficTiles; 

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            return;
        }
        Destroy(gameObject);
    }

    private void FindTrafficTiles() {
        foreach (var tile in FindObjectsOfType<TrafficTile>()) {
            _trafficTiles.Add(Vector3Int.RoundToInt(tile.transform.position), tile);
        }
    }
}
