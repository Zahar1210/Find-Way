using System.Collections.Generic;
using UnityEngine;

public class CrossRoad : MonoBehaviour
{
    [SerializeField] private float changeTime;
    
    public List<TrafficDot> _dots = new();
    public List<TrafficDot.Dot> _changeDots = new();
    private AreaTypes _changeType = AreaTypes.Traffic;
    private float _time;
    public int count;

    private void Update() {
        Timer();
    }
    private void Timer()
    {
        _time += Time.deltaTime;
        if (_time >= changeTime) {
            ChangeRoadSide();
            _time = 0;
            if (_changeType == AreaTypes.Traffic)
                _changeType = AreaTypes.Mixed;
            else
                _changeType = AreaTypes.Traffic;
        }
        count = _changeDots.Count;
    }

    public void ChangeRoadSide()
    {
        foreach (var dot in _dots) {
            if (dot.Area.gameObject.activeSelf) {
                SelectDots(dot, dot.BackDot, dot.FrontDot);
            }
            else {
                foreach (var d in dot.dots) {
                    _changeDots.Remove(d);
                }
            }
        }
        foreach (var dot in _changeDots) {
            if (dot.DotTraffic.Area.Type == _changeType) 
                dot.CanMove = false;
            else 
                dot.CanMove = true;
        }
    }

    private void SelectDots(TrafficDot mainDot,TrafficDot backDot,TrafficDot frontDot)
    {
        foreach (var dot in mainDot.dots) {
            if (!_changeDots.Contains(dot) && dot.DotTraffic.Area.gameObject.activeSelf) {
                _changeDots.Add(dot);
            }
            foreach (var d in mainDot.dots) {
                if (d.Pos.x < -11 || d.Pos.x > 9) {
                    _changeDots.Remove(d);
                }
            }
        }
        if (frontDot != null) {
            foreach (var dot in frontDot.dots) {
                if (!_changeDots.Contains(dot) && dot.DotTraffic.Area.gameObject.activeSelf) {
                    _changeDots.Add(dot);
                }
            }
        }
        if (backDot != null) {
            foreach (var dot in backDot.dots) {
                if (!_changeDots.Contains(dot) && dot.DotTraffic.Area.gameObject.activeSelf) {
                    _changeDots.Add(dot);
                }
            }
        }
    }
}