
using System.Collections.Generic;
using UnityEngine;

public class CrossRoad : MonoBehaviour
{
    public static CrossRoad Instance { get; set; }
    [SerializeField] private float changeTime;
    [SerializeField] private TrafficSystem trafficSystem;
    public List<TrafficDot> _dots = new();
    public List<TrafficDot.Dot> _changeDots = new();
    private AreaTypes _changeType = AreaTypes.Traffic;
    private float _time;

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
        foreach (var dot in _dots) {
            if (dot.Area.gameObject.activeSelf) {
                SelectDots(dot);
            }
            else {
                _dots.Remove(dot);
            }
        }
        foreach (var dot in _changeDots) {
            dot.CanMove = (dot.DotTraffic.Area.Type != _changeType);
        }
    }
    private void SelectDots(TrafficDot dot)
    {
        TrafficDot mainDot = dot;
        TrafficDot frontDot = GetDot(dot, 1);
        TrafficDot backDot = GetDot(dot, -1);
        foreach (var d in mainDot.dots) {
            if (!_changeDots.Contains(d) && d.DotTraffic.Area.gameObject.activeSelf && !(d.Pos.x < -11 || d.Pos.x > 9)) {
                _changeDots.Add(d);
            }
        }
        if (frontDot != null) {
            foreach (var d in frontDot.dots) {
                if (!_changeDots.Contains(d) && d.DotTraffic.Area.gameObject.activeSelf) {
                    _changeDots.Add(d);
                }
            }
        }
        if (backDot != null) {
            foreach (var d in backDot.dots) {
                if (!_changeDots.Contains(d) && d.DotTraffic.Area.gameObject.activeSelf) {
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

}
