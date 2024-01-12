using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CarPooler : MonoBehaviour
{
    [SerializeField] private int[] trafficSpawnCount;
    [SerializeField] private int[] mixedSpawnCount;
    [SerializeField] private CarAbstract[] carArray;
    private List<CarAbstract> _carAbstracts = new();
    public void SpawnCar(AreaAbstract area)
    {
        ITrafficable trafficArea = area.GetComponent<ITrafficable>(); 
        if (trafficArea != null) {
            CarAbstract[] cars = FindCar((area.Type == AreaTypes.Traffic) ? Random.Range(trafficSpawnCount[0], trafficSpawnCount[1]) 
                : Random.Range(mixedSpawnCount[0], mixedSpawnCount[1]));
            if (cars != null) {
                SetCar(cars, trafficArea);
            }
        }
    }
    private CarAbstract[] FindCar(int sum)
    {
        List<CarAbstract> cars = new();
        if (sum != 0) {
            foreach (var car in _carAbstracts) {
                if (!car.gameObject.activeSelf && cars.Count < sum) {
                    cars.Add(car);
                }
            }
            if (cars.Count < sum) {
                for (int i = 0; i < (sum - cars.Count) + 1; i++) {
                    CarAbstract car = Instantiate(carArray[Random.Range(0, carArray.Length)]);
                    _carAbstracts.Add(car);
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
            TrafficDot.Dot dot = SetPos(car, trafficArea);
            DotFinding.GetDot(dot, car);
        }
    }
    private TrafficDot.Dot SetPos(CarAbstract car, ITrafficable area)
    {
        TrafficDot.Dot carDot = area.Dot.dots[Random.Range(0, area.Dot.dots.Count)];
        car.transform.position = carDot.Pos;
        car.transform.rotation = carDot.Rot;
        return carDot;
    }
    private void EnableCar(CarAbstract car, ITrafficable area ,bool isActive)
    {
        car.CarArea = area;
        car.gameObject.SetActive(isActive);
    }
    public void ReturnToPool(ITrafficable area)
    {
        foreach (var car in _carAbstracts) {
            if (car.CarArea == area) {
                EnableCar(car, null, false);
            }
        }
    }
}