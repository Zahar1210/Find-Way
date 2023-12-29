using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrafficSystem : MonoBehaviour
{
    [SerializeField] private CarPooler carPooler;

    private List<TrafficDot> _trafficDots = new();
    private Dictionary<int, AreaAbstract> _trafficAreas = new();
    private List<TrafficDot.Dot> _crossRoadDots = new();
    private AreaAbstract pastArea;
    public int caunt;
    public int spawnIndex;

    public void SetTraffic()
    {
        foreach (var dot in FindObjectsOfType<TrafficDot>()) {
            if (!_trafficDots.Contains(dot)) {
                dot.SetDot();
                _trafficDots.Add(dot);
                _trafficAreas.Add(dot.Area.SpawnIndex, dot.Area);
                carPooler.SpawnCar(dot.Area);
            }
        }
        foreach (var dot in _trafficDots) {
            GetDot(dot);
        }
        foreach (var dot in _trafficDots) {
            if (dot.Area.SpawnIndex == spawnIndex -1) {
                CheckCrossRoads(dot.Area);
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

    private void CheckCrossRoads(AreaAbstract area)
    {
        if (pastArea == null)
            pastArea = area;
        else if (pastArea.SpawnIndex + 1 == area.SpawnIndex && area.Type != pastArea.Type) {
            IName trafficArea = (pastArea.Type == AreaTypes.Traffic) ? pastArea.GetComponent<IName>() : area.GetComponent<IName>();
            GetCrossRoadDots(trafficArea.Dot);
        }
        pastArea = area;
    }

    private void GetCrossRoadDots(TrafficDot trafficDot)
    {
        foreach (var dot in trafficDot.dots) {
            if (dot.Pos.x < 10 && dot.Pos.x > -10 && !_crossRoadDots.Contains(dot)) { _crossRoadDots.Add(dot); }
        }
        if (trafficDot.FrontDot != null) {
            foreach (var dot in trafficDot.FrontDot.dots) {
                if (!_crossRoadDots.Contains(dot)) { _crossRoadDots.Add(dot); }
            }
        }
        if (trafficDot.BackDot != null) {
            foreach (var dot in trafficDot.BackDot.dots) {
                if (!_crossRoadDots.Contains(dot)) { _crossRoadDots.Add(dot); }
            }
        }
    }

    public void ReSetDot(IName trafficArea)
    {
        carPooler.ReturnToPool(trafficArea);
        trafficArea.Dot.FrontDot = null;
        trafficArea.Dot.BackDot = null;
        _trafficAreas.Remove(trafficArea.Dot.Area.SpawnIndex);
        _trafficDots.Remove(trafficArea.Dot);
        foreach (var dot in trafficArea.Dot.dots)
        {
            if (_crossRoadDots.Contains(dot))
            {
                _crossRoadDots.Remove(dot);
            }
        }
    }
}