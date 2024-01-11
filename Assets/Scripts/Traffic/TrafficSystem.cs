using System;
using System.Collections.Generic;
using UnityEngine;

public class TrafficSystem : MonoBehaviour
{
    public static TrafficSystem Instance { get; set; }
    [SerializeField] private CarPooler carPooler;
    [SerializeField] private CrossRoad crossRoad;
    public Dictionary<int, TrafficDot> _traffic = new();
    private AreaAbstract pastArea;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    public void SetTraffic(AreaAbstract areaAbstract) {
        foreach (var dot in FindObjectsOfType<TrafficDot>()) {
            if (!_traffic.TryGetValue(dot.Area.SpawnIndex, out var Dot)) {
                dot.SetDot();
                _traffic.Add(dot.Area.SpawnIndex, dot);
                carPooler.SpawnCar(dot.Area);
            }
        }

        if (areaAbstract.Type == AreaTypes.Mixed || areaAbstract.Type == AreaTypes.Traffic)
            CheckCrossRoad(areaAbstract);
    }

    private void CheckCrossRoad(AreaAbstract areaAbstract)
    {
        if (pastArea == null)
            pastArea = areaAbstract;
        if (areaAbstract.SpawnIndex -1 == pastArea.SpawnIndex && areaAbstract.Type != pastArea.Type) {
            IName trafficArea = (pastArea.Type == AreaTypes.Traffic) ? pastArea.GetComponent<IName>() : areaAbstract.GetComponent<IName>();
            if (!crossRoad._dots.Contains(trafficArea.Dot)) {
                crossRoad._dots.Add(trafficArea.Dot);
                crossRoad.ChangeRoadSide();
            }
        }
        pastArea = areaAbstract;
    }

    public void ResetDot(IName area)
    {
        carPooler.ReturnToPool(area);
        _traffic.Remove(area.Dot.Area.SpawnIndex);
        if (crossRoad._changeDots.Count != 0) {
            foreach (var dot in area.Dot.dots) {
                if (crossRoad._changeDots.Contains(dot)) {
                    crossRoad._changeDots.Remove(dot);
                }
            }
            crossRoad._dots.Remove(area.Dot);
        }
    }

    public class MoveDots
    {
        public TrafficDot.Dot DotA { get; }
        public TrafficDot.Dot DotB { get; }
        public Vector3 CenterDot { get; }

        public MoveDots(TrafficDot.Dot a, TrafficDot.Dot b, Vector3 center)
        {
            DotA = a;
            DotB = b;
            CenterDot = center;
        }
    }
}