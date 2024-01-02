using System.Collections.Generic;
using UnityEngine;

public class CrossRoad : MonoBehaviour
{
    public List<TrafficDot> _dots = new();
    public List<TrafficDot.Dot> _changeDots = new();
    [SerializeField] private float changeTime;
    private float _time;
    private AreaTypes _changeType = AreaTypes.Traffic;
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

    private void ChangeRoadSide()
    {
        foreach (var dot in _dots) {
            if (dot.Area.gameObject.activeSelf) {
                SelectDots(dot, dot.BackDot, dot.FrontDot);
            }
        }
        foreach (var dot in _changeDots) {
            if (dot.AreaType == _changeType) 
                dot.CanMove = false;
            else 
                dot.CanMove = true;
        }
    }

    private void SelectDots(TrafficDot mainDot,TrafficDot backDot,TrafficDot frontDot)
    {
        foreach (var dot in mainDot.dots) {
            if (dot.Pos.x < 10 && dot.Pos.x > -10 && !_changeDots.Contains(dot)) {
                _changeDots.Add(dot);
            }
        }
        if (frontDot != null) {
            foreach (var dot in frontDot.dots) {
                if (!_changeDots.Contains(dot)) {
                    _changeDots.Add(dot);
                }
            }
        }
        if (backDot != null) {
            foreach (var dot in backDot.dots) {
                if (!_changeDots.Contains(dot)) {
                    _changeDots.Add(dot);
                }
            }
        }
    }
}