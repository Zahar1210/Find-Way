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
        TrafficDot frontDot = GetDot(mainDot, 1);
        TrafficDot backDot = GetDot(mainDot, -1);
        foreach (var d in mainDot.dots) {
            if (!_changeDots.Contains(d) && CheckDot(d)) {
                _changeDots.Add(d);
                d.isCross = true;
            }
        }
        if (frontDot != null) {
            foreach (var d in frontDot.dots) {
                if (!_changeDots.Contains(d) && d.Type == DotType.Right)  {
                    _changeDots.Add(d);
                    d.isCross = true;
                }
            }
        }
        if (backDot != null) {
            foreach (var d in backDot.dots) {
                if (!_changeDots.Contains(d) && d.Type == DotType.Left) {
                    _changeDots.Add(d);
                    d.isCross = true;
                }
            }
        }
    }
    private TrafficDot GetDot(TrafficDot mainDot, int offset)
    {
        if (trafficSystem._traffic.TryGetValue(mainDot.Area.SpawnIndex + offset, out TrafficDot dot)) 
            return dot; 
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
            car.CrossRoadDot = GetCrossRoadDot(a);
            if (_queueCars.Count == 1 || TryMove(car)) {
                _carStateDriving.EnterDriving(a, car);
            }
        }
    }

    public void NextCarToMove(CarAbstract previousCar) {
        _queueCars.Remove(previousCar);
        if (_queueCars.Count > 0) {
            List<CarAbstract> cars = _queueCars.Keys.ToList();
            if (cars.Count != 0) {
                CarAbstract nextCar = GetNextCar(previousCar.CrossRoadDot, cars);
                if (_queueCars.TryGetValue(nextCar, out var dotA)) {
                    _carStateDriving.EnterDriving(dotA, nextCar);
                }
            }
        }
        previousCar.CrossRoadDot = null;
    }

    private TrafficDot GetCrossRoadDot(TrafficDot.Dot a)
    {
        if (a.DotTraffic.Area.Type == AreaTypes.Mixed) {
            return a.Type == DotType.Left
                ? trafficSystem._traffic.TryGetValue(a.DotTraffic.Area.SpawnIndex + 1, out var dot) ? dot : null
                : trafficSystem._traffic.TryGetValue(a.DotTraffic.Area.SpawnIndex - 1, out dot) ? dot : null;
        }
        else {
            return a.DotTraffic;
        }
    }

    private CarAbstract GetNextCar(TrafficDot dot, List<CarAbstract> cars)
    {
        List<CarAbstract> selectedCars = new();
        foreach (var car in cars) {
            if (car.CrossRoadDot == dot) {
                selectedCars.Add(car);
            }
        }
        return selectedCars[Random.Range(0, selectedCars.Count)];
    }

    private bool TryMove(CarAbstract car)
    {
        List<CarAbstract> cars = _queueCars.Keys.ToList();
        int selectCount = 0;
        foreach (var c in cars) {
            if (c.CrossRoadDot == car.CrossRoadDot) {
                selectCount++;
            }
        }
        if (selectCount == 1) {
            return true;
        }
        return false;
    }
}