using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpeedModifier: MonoBehaviour
{
    [SerializeField] private CrossRoad crossRoad;
    public Dictionary<CarTypes, float> _speedCars = new();
    [SerializeField] private float simpleCarSpeed;
    [SerializeField] private float serviceCarSpeed;
    [SerializeField] private float truckCarSpeed;

    private void Start() {
        _speedCars.Add(CarTypes.CarService, serviceCarSpeed);
        _speedCars.Add(CarTypes.CarSimple, simpleCarSpeed);
        _speedCars.Add(CarTypes.CarTruck, truckCarSpeed);        
    }

    public IEnumerator ChangeSpeed(CarAbstract car, float targetSpeed, float duration)
    {
        DrivingFSM startState = car.CurrentState;
        float initialSpeed = car.Speed;
        float elapsedTime = 0f;
        while (elapsedTime < duration) {
            if (car.CurrentState == startState) {
                car.Speed = Mathf.Lerp(initialSpeed, targetSpeed, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            else {
                yield break;
            }
        }
        car.Speed = targetSpeed;
    }

    public float GetTargetSpeed(CarAbstract currentCar)
    {
        if (currentCar.CheckCar != null) {
            if (currentCar.CheckCar.CheckCar != null) {
                return currentCar.CheckCar.DrivingParams.TargetSpeed;
            }
            else {
                return currentCar.CheckCar.Speed;
            }
        }
        if (currentCar.CheckDot != null && crossRoad._queueCars.Count == 0) 
            return currentCar.Speed;
        if (currentCar.CheckDot != null && crossRoad._queueCars.Count > 0)
            return 0.1f;
        return 0.1f;
    }

    public float GetTimeForSlowDown(CarAbstract car)
    {
        if (car.CheckDot != null) {
            return 3;
        }

        return car.DrivingParams.TimeForMove;
    }
}
