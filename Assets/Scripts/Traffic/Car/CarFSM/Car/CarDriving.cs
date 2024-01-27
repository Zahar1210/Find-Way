using System;
using System.Collections;
using UnityEngine;
using Zenject;

public class CarDriving: MonoBehaviour
{ 
    private CrossRoad _crossRoad;
    private CarChecking _carChecking;
    private DrivingState _drivingState;
    private CarStateDriving _carStateDriving;

    [Inject]
    private void Construct(CarStateDriving carStateDriving, CrossRoad crossRoad, TrafficDistanceTracker trafficDistanceTracker, DrivingState drivingState, CheckState checkState, CarChecking carChecking)
    {
        _crossRoad = crossRoad;
        _carChecking = carChecking;
        _drivingState = drivingState;
        _carStateDriving = carStateDriving;
    }

    public void Move(TrafficDot.Dot a, TrafficDot.Dot b, Vector3 center, CarAbstract car)
    {
        ResetCar(car);
        SetCar(car,b);
        StartMove(car, a);
        car.StartCoroutine(CarMove(new TrafficSystem.MoveDots(a,b, center),car));
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
                if (_crossRoad._crossRoadDots.Contains(moveParams.DotB)) 
                    _crossRoad.AddCar(car, moveParams.DotB);
                else 
                    _carStateDriving.EnterDriving(moveParams.DotB, car);
                yield break;
            }
            yield return null;
        }
    }
    private void StartMove(CarAbstract car, TrafficDot.Dot a)
    {
        if (_crossRoad._crossRoadDots.Contains(a)) {
            _drivingState.SetState<CarStatePowerUp>(new DrivingState.DrivingParams(car, car.FixedSpeed, 2));
        }
        _carChecking.StartChecking(car);
        if (car.CurrentCehckState != null) {
            StartCoroutine(Checking(car));
        }
    }
    
    private IEnumerator Checking(CarAbstract car) {
        while (car.CurrentCehckState != null) {
            car.CurrentCehckState.Enter(car);
            yield return new WaitForSeconds(0.3f);
        }
    }
    
    private void SetCar(CarAbstract car, TrafficDot.Dot b)
    {
        car.TargetDot = b;
        car.CarArea = b.DotTraffic.Area.GetComponent<ITrafficable>();
    }

    private void ResetCar(CarAbstract car)
    {
        car.CurrentCehckState = null;
        car.ExtraCheckCar = null;
        car.BehindCar = null;
        car.CheckCar = null;
        car.CheckDot = null;
    }
}