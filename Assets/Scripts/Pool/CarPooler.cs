using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CarPooler : MonoBehaviour
{
    [SerializeField] private CarMoveController carMoveController;
    [SerializeField] private CarAbstract[] carArray;
    private List<CarAbstract> _carAbstracts = new();
    public void SpawnCar(AreaAbstract area)
    {
        IName trafficArea = area.GetComponent<IName>(); 
        if (trafficArea != null) {
            CarAbstract[] cars = FindCar((area.Type == AreaTypes.Traffic) ? Random.Range(1, 5) : Random.Range(0, 2));
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
    private void SetCar(CarAbstract[] cars, IName trafficArea )
    {
        foreach (var car in cars) {
            EnableCar(car, trafficArea,true);
            TrafficDot.Dot dot = SetPos(car, trafficArea);
            carMoveController.GetMovementDirection(dot, car);
        }
    }
    private TrafficDot.Dot SetPos(CarAbstract car, IName area)
    {
        TrafficDot.Dot carDot = area.Dot.dots[Random.Range(0, area.Dot.dots.Count)];
        car.transform.position = carDot.Pos;
        car.transform.rotation = carDot.Rot;
        return carDot;
    }
    public void EnableCar(CarAbstract car, IName area ,bool isActive)
    {
        car.Area = area;
        car.gameObject.SetActive(isActive);
    }
    public void ReturnToPool(IName area)
    {
        foreach (var car in _carAbstracts) {
            if (car.Area == area) {
                EnableCar(car, null, false);
            }
        }
    }
}