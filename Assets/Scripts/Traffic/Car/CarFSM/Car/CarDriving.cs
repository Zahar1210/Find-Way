using System;
using System.Collections;
using UnityEngine;
using Zenject;

public class CarDriving: MonoBehaviour
{
    private State _state;
    private CrossRoad _crossRoad;
    private CarStateDriving _carStateDriving;
    private TrafficDistanceTracker _trafficDistanceTracker;

    [Inject]
    private void Construct(CarStateDriving carStateDriving, CrossRoad crossRoad, TrafficDistanceTracker trafficDistanceTracker, State state) {
        _state = state;
        _crossRoad = crossRoad;
        _carStateDriving = carStateDriving;
        _trafficDistanceTracker = trafficDistanceTracker;
    }

    public void Move(TrafficDot.Dot a, TrafficDot.Dot b, Vector3 center, CarAbstract car) {
        car.TargetDot = b;
        car.CarArea = b.DotTraffic.Area.GetComponent<ITrafficable>();
        if (_crossRoad._changeDots.Contains(a)) {
            _state.SetState<CarStatePowerUp>(car);
        }
        else if(_crossRoad._changeDots.Contains(b)) {
            car.StartCoroutine(_trafficDistanceTracker.CheckDistanceToTargetDotCoroutine(car));
        }
        car.CheckCar = _trafficDistanceTracker.GetCarForCheck(car);
        car.StartCoroutine(CarMove(new TrafficSystem.MoveDots(a,b, center),car));
        if (car.CheckCar != null) {
            car.StartCoroutine(_trafficDistanceTracker.CheckDistanceCoroutine(car));
        }
    }
    private IEnumerator CarMove(TrafficSystem.MoveDots moveParams,CarAbstract car)
    {
        float currentTime = 0f;
        float length = Vector3.Distance(moveParams.DotA.Pos, moveParams.DotB.Pos);
        while (true) {
            float t = Mathf.Lerp(0f, 1f, currentTime/ length);
            car.transform.position =  CarTransform.GetPosition(t, moveParams.DotA, moveParams.DotB, moveParams.CenterDot);
            car.transform.rotation = CarTransform.GetRotation(t, moveParams.DotA, moveParams.DotB);
            currentTime += Time.deltaTime * car.Speed;
            if (currentTime >= length) {
                if (car.CrossRoadDot != null || _crossRoad._queueCars.ContainsKey(car)) {
                    _crossRoad.NextCarToMove(car);
                    _carStateDriving.EnterDriving(moveParams.DotB, car);
                    yield break;
                }
                if (_crossRoad._changeDots.Contains(moveParams.DotB)) 
                    _crossRoad.AddCar(car, moveParams.DotB);
                else 
                    _carStateDriving.EnterDriving(moveParams.DotB, car);
                yield break;
            }
            yield return null;
        }
    }
}