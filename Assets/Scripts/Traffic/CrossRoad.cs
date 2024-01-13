using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class CrossRoad : MonoBehaviour
{
    public static CrossRoad Instance { get; set; }

    [SerializeField] private WallPooler wallPooler;
    [SerializeField] private float changeTime;
    [SerializeField] private TrafficSystem trafficSystem;
    
    public List<TrafficDot> _dots = new();
    public List<TrafficDot.Dot> _changeDots = new();
    private AreaTypes _changeType = AreaTypes.Traffic;
    
    private float _time;
    private int previosCountDots;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    private void Update() {
        Timer();
    }
    private void Timer()
    {
        _time += Time.deltaTime;
        if (_time >= changeTime) {
            ChangeRoadSide();
            ChangeType();
            _time = 0;
        }
    }
    public void ChangeRoadSide()
    {
        if (previosCountDots < _dots.Count) {
            foreach (var dot in _dots) {
                if (dot.Area.gameObject.activeSelf) 
                    SelectDots(dot);
                else 
                    _dots.Remove(dot);
            }

            foreach (var dot in _changeDots) {
                wallPooler.SpawnCrossRoadWall(dot);
            }
        }
        
        foreach (var wall in wallPooler._walls) {
            if (wall.Dot == null || !wall.Dot.DotTraffic.Area.gameObject.activeSelf) 
                wallPooler.ReturnToPool(wall);
        }

        foreach (var wall in wallPooler._walls) {
            if (wall.IsUse) {
                if (wall.Dot.DotTraffic.Area.Type == _changeType) {
                    wallPooler.ChangeRoadSide(wall, false);
                }
                else {
                    wallPooler.ChangeRoadSide(wall, true);
                    wall.transform.position = wall.Dot.Pos;
                }
            }
        }
        previosCountDots = _dots.Count;
    }
    private void SelectDots(TrafficDot dot)
    {
        TrafficDot mainDot = dot;
        TrafficDot frontDot = GetDot(dot, 1);
        TrafficDot backDot = GetDot(dot, -1);
        foreach (var d in mainDot.dots) {
            if (!_changeDots.Contains(d) && CheckDot(d)) {
                _changeDots.Add(d);
            }
        }
        if (frontDot != null) {
            foreach (var d in frontDot.dots) {
                if (!_changeDots.Contains(d) && (d.Type == DotType.Right && d.DotTraffic.Area.SpawnIndex == mainDot.Area.SpawnIndex + 1))  {
                    _changeDots.Add(d);
                }
            }
        }
        if (backDot != null) {
            foreach (var d in backDot.dots) {
                if (!_changeDots.Contains(d) && (d.Type == DotType.Left && d.DotTraffic.Area.SpawnIndex == mainDot.Area.SpawnIndex - 1)) {
                    _changeDots.Add(d);
                }
            }
        }
    }
    
    private void ChangeType() {
        _changeType = (_changeType == AreaTypes.Traffic) ? AreaTypes.Mixed : AreaTypes.Traffic;
    }

    private TrafficDot GetDot(TrafficDot dot, int offset)
    {
        if (trafficSystem._traffic.TryGetValue(dot.Area.SpawnIndex + offset, out TrafficDot adjacentDot)) { return adjacentDot; }
        return null;
    }

    private bool CheckDot(TrafficDot.Dot dot)
    {
        List<TrafficDot.Dot> dots = new();
        dots = SelectDot(dot);
        if (dots.Count == 2)
            return true;
        return false;
    }

    private List<TrafficDot.Dot> SelectDot(TrafficDot.Dot a)
    {
        List<TrafficDot.Dot> d = new();
        if (a.Type == DotType.Left) 
            d.AddRange(a.DotTraffic.dots.Where(dot => dot.Type == a.Type && dot.СonstantPos.x < a.СonstantPos.x));
        else 
            d.AddRange(a.DotTraffic.dots.Where(dot => dot.Type == a.Type && dot.СonstantPos.x > a.СonstantPos.x));
        return d;
    }
}
