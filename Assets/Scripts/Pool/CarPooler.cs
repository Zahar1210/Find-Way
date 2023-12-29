using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CarPooler : MonoBehaviour
{
    [SerializeField] private CarAbstract[] carArray;
    private List<CarAbstract> _carAbstracts = new();
    
    private void Start()
    {
        foreach (var car in FindObjectsOfType<CarAbstract>()) { _carAbstracts.Add(car); }
    }

    public void SpawnCar(AreaAbstract area)
    {
        Debug.Log(area);
        IName trafficArea = area.GetComponent<IName>(); 
        if (trafficArea != null) {
            CarAbstract[] cars = FindCar((area.Type == AreaTypes.Traffic) ? Random.Range(1, 5) : Random.Range(0, 2));
            if (cars != null) {
                SetCar(cars, area, trafficArea);
            }
        }
    }
    
    
    private CarAbstract[] FindCar(int sum)
    {
        Debug.Log(sum + " это количество нужных");
        List<CarAbstract> cars = new();
        if (sum != 0) {
            foreach (var car in _carAbstracts) {
                if (!car.gameObject.activeSelf && cars.Count < sum) {
                    cars.Add(car);
                }
            }
            Debug.Log(cars.Count + " столько получили");
            if (cars.Count < sum) {
                Debug.Log("не хватает " + (sum - cars.Count));
                for (int i = 0; i < (sum - cars.Count) + 1; i++) {
                    CarAbstract car = Instantiate(carArray[Random.Range(0, carArray.Length)]);
                    _carAbstracts.Add(car);
                    cars.Add(car);
                    car.gameObject.SetActive(false);
                }
            }
            Debug.Log(cars.Count + " столько вернули в итоге");
            return cars.ToArray();
        }
        return null;
    }

    private void SetCar(CarAbstract[] cars, AreaAbstract area, IName trafficArea)
    {
        Debug.Log(trafficArea.Dot);
        foreach (var car in cars)
        {
            EnableCar(car, area, true);
            SetPos(car, trafficArea);
        }
    }

    private void SetPos(CarAbstract car, IName area)
    {
        TrafficDot.Dot carDot = area.Dot.dots[Random.Range(0, area.Dot.dots.Count + 1)];
        car.transform.position = carDot.Pos;
        car.transform.rotation = carDot.Rot;
        Debug.Log(carDot.Rot);
    }
    
    private void EnableCar(CarAbstract car, AreaAbstract area ,bool isActive)
    {
        car.Area = area;
        car.gameObject.SetActive(isActive);
    }
}