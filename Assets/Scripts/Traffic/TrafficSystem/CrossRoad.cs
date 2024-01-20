using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class CrossRoad : MonoBehaviour
{
    [SerializeField] private TrafficSystem trafficSystem;
    public List<TrafficDot> _dots = new();
    public List<TrafficDot.Dot> _changeDots = new();
    public Dictionary<CarAbstract, TrafficDot.Dot> _queueCars = new();
    private CarStateDriving _carStateDriving;

    [Inject]
    private void Construct(CarStateDriving carStateDriving) {
        _carStateDriving = carStateDriving;
    }
    
    public void AddDots()
    {
        foreach (var dot in _dots) {
            if (dot.Area.gameObject.activeSelf) 
                SelectDots(dot);
            else
                _dots.Remove(dot);
        }
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
                if (!_changeDots.Contains(d) && d.Type == DotType.Right)  {
                    _changeDots.Add(d);
                }
            }
        }
        if (backDot != null) {
            foreach (var d in backDot.dots) {
                if (!_changeDots.Contains(d) && d.Type == DotType.Left) {
                    _changeDots.Add(d);
                }
            }
        }
    }
    private TrafficDot GetDot(TrafficDot dot, int offset)
    {
        if (trafficSystem._traffic.TryGetValue(dot.Area.SpawnIndex + offset, out TrafficDot Dot)) 
            return Dot; 
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

    public void AddCar(CarAbstract car, TrafficDot.Dot a) {
        if (!_queueCars.TryGetValue(car, out var dotA)) {
            _queueCars.Add(car, a);
            car.isCrossRoad = true;
            if (_queueCars.Count == 1) {
                _carStateDriving.EnterDriving(a, car);
            }
        }
    }

    public void NextCarToMove(CarAbstract previousCar) {
        _queueCars.Remove(previousCar);
        if (_queueCars.Count > 0) {
            List<CarAbstract> cars = _queueCars.Keys.ToList();
            CarAbstract nextCar = cars[Random.Range(0, cars.Count)];
            if (_queueCars.TryGetValue(nextCar, out var dotA)) {
                _carStateDriving.EnterDriving(dotA, nextCar);
            }
        }
    }
}
