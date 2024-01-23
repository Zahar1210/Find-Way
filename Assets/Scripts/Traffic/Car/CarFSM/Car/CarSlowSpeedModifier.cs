using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSlowSpeedModifier: MonoBehaviour
{
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
        float initialSpeed = car.Speed;
        float elapsedTime = 0f;
        while (elapsedTime < duration) {
            car.Speed = Mathf.Lerp(initialSpeed, targetSpeed, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        car.Speed = targetSpeed;
    }
}
