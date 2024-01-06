using System;
using System.Collections.Generic;
using UnityEngine;

public class TrafficSystem : MonoBehaviour
{
    public static TrafficSystem Instance { get; set; }
    [SerializeField] private CarPooler carPooler;
    [SerializeField] private CrossRoad crossRoad;

    public List<TrafficDot> _trafficDots = new();
    public Dictionary<int, AreaAbstract> _trafficAreas = new();
    private AreaAbstract pastArea;
    public int spawnIndex;

    private void Awake()
    {
        if (Instance == null) {
            Instance = this;
            return;
        }
        Destroy(gameObject);
    }

    public void SetTraffic()
    {
        List<TrafficDot> dots = new();
        foreach (var dot in FindObjectsOfType<TrafficDot>()) {
            if (!_trafficDots.Contains(dot)) {
                dot.SetDot();
                _trafficDots.Add(dot);
                _trafficAreas.Add(dot.Area.SpawnIndex, dot.Area);
                dots.Add(dot);
            }
        }
        foreach (var dot in _trafficDots) {
            GetDot(dot);
        }
        foreach (var dot in _trafficDots) {
            if (dot.Area.SpawnIndex == spawnIndex - 1) {
                CheckCrossRoads(dot.Area);
            }
        }
        foreach (var d in dots) {
            carPooler.SpawnCar(d.Area);
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
    private void CheckCrossRoads(AreaAbstract area)
    {
        if (pastArea == null)
            pastArea = area;
        else if (pastArea.SpawnIndex + 1 == area.SpawnIndex && area.Type != pastArea.Type) {
            IName trafficArea = (pastArea.Type == AreaTypes.Traffic) ? pastArea.GetComponent<IName>() : area.GetComponent<IName>();
            if (!crossRoad._dots.Contains(trafficArea.Dot)) {
                crossRoad._dots.Add(trafficArea.Dot);
            }
            crossRoad.ChangeRoadSide();
        }
        pastArea = area;
    }
    public void ReSetDot(IName trafficArea)
    {
        carPooler.ReturnToPool(trafficArea);
        trafficArea.Dot.FrontDot = null;
        trafficArea.Dot.BackDot = null;
        _trafficAreas.Remove(trafficArea.Dot.Area.SpawnIndex);
        _trafficDots.Remove(trafficArea.Dot);
        if (crossRoad._changeDots.Count != 0) {
            foreach (var dot in trafficArea.Dot.dots) {
                if (crossRoad._changeDots.Contains(dot)) {
                    crossRoad._changeDots.Remove(dot);
                }
            }
            crossRoad._dots.Remove(trafficArea.Dot);
        }
    }
    public class MoveDots
    {
        public TrafficDot.Dot DotA { get;}
        public TrafficDot.Dot DotB { get;}
        public Vector3 CenterDot{ get;}

        public MoveDots(TrafficDot.Dot a, TrafficDot.Dot b, Vector3 center) {
            DotA = a;
            DotB = b;
            CenterDot = center;
        }
    }
}