using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class CarPooler : MonoBehaviour
{
    [SerializeField] private int[] trafficSpawnCount;
    [SerializeField] private int[] mixedSpawnCount;
    [SerializeField] private CarAbstract[] carArray;
    public List<CarAbstract> _cars = new();
    private DrivingState _drivingState;
    private CrossRoad _crossRoad;

    [Inject]
    private void Construct(DrivingState drivingState, CrossRoad crossRoad) {
        _drivingState = drivingState;
        _crossRoad = crossRoad;
    }
    
    public void SpawnCar(AreaAbstract area)
    {
        ITrafficable trafficArea = area.GetComponent<ITrafficable>(); 
        if (trafficArea != null) {
            CarAbstract[] cars = FindCar(area.Type == AreaTypes.Traffic
                ? Random.Range(trafficSpawnCount[0], trafficSpawnCount[1])
                : Random.Range(mixedSpawnCount[0], mixedSpawnCount[2]));
            if (cars != null) {
                SetCar(cars, trafficArea);
            }
        }
    }
    private CarAbstract[] FindCar(int sum)
    {
        List<CarAbstract> cars = new();
        if (sum != 0) {
            foreach (var car in _cars) {
                if (!car.gameObject.activeSelf && cars.Count < sum) {
                    cars.Add(car);
                }
            }
            if (cars.Count < sum) {
                for (int i = 0; i < (sum - cars.Count) + 1; i++) {
                    CarAbstract car = Instantiate(carArray[Random.Range(0, carArray.Length)]);
                    _cars.Add(car);
                    cars.Add(car);
                    car.gameObject.SetActive(false);
                }
            }
            return cars.ToArray();
        }
        return null;
    }
    private void SetCar(CarAbstract[] cars, ITrafficable trafficArea )
    {
        foreach (var car in cars) {
            EnableCar(car, trafficArea,true);
            TrafficDot.Dot dot = SetPos(trafficArea);
            if (dot != null) {
                dot.CarSpawn = true;
                car.transform.position = dot.Pos;
                car.transform.rotation = dot.Rot;
                _drivingState.SetState<CarStateDriving>(new DrivingState.DrivingParams(car), dot);
            }
        }
    }
    private TrafficDot.Dot SetPos(ITrafficable area)
    {
        TrafficDot.Dot dot = null;
        if (area.Dot.Area.Type == AreaTypes.Traffic) {
            for (int i = 0; i < area.Dot.dots.Count; i++) {
                dot = area.Dot.dots[Random.Range(0, area.Dot.dots.Count)];
                if (dot.CarSpawn == false) {
                    return dot;
                }
            }
        }
        else {
            return area.Dot.dots[Random.Range(0, area.Dot.dots.Count)];
        }
        return null;
    }
    private void EnableCar(CarAbstract car, ITrafficable area ,bool isActive)
    {
        if (!isActive) {
            car.CurrentState = null;
            car.CurrentCehckState = null;
            if (car.CrossRoadDot != null) 
                _crossRoad.NextCarToMove(car);
        }
        car.TargetSpeed = car.FixedSpeed;
        car.CrossRoadDot = null;
        car.CarArea = area;
        car.gameObject.SetActive(isActive);
    }
    public void ReturnToPool(ITrafficable area)
    {
        foreach (var car in _cars) {
            if (car.CarArea == area) 
                EnableCar(car, null, false);
        }
    }
}