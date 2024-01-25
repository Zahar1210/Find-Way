using System.Collections;
using UnityEngine;
using Zenject;

public class CarDriving: MonoBehaviour
{
    private CrossRoad _crossRoad;
    private CheckState _checkState;
    private DrivingState _drivingState;//
    private CarStateDriving _carStateDriving;
    private TrafficDistanceTracker _trafficDistanceTracker;

    [Inject]
    private void Construct(CarStateDriving carStateDriving, CrossRoad crossRoad, TrafficDistanceTracker trafficDistanceTracker, DrivingState drivingState, CheckState checkState)
    {
        _crossRoad = crossRoad;
        _checkState = checkState;
        _drivingState = drivingState;
        _carStateDriving = carStateDriving;
        _trafficDistanceTracker = trafficDistanceTracker;
    }

    public void Move(TrafficDot.Dot a, TrafficDot.Dot b, Vector3 center, CarAbstract car)
    {
        car.TargetDot = b;
        car.CarArea = b.DotTraffic.Area.GetComponent<ITrafficable>();
        StartChecking(car);
        if (car.CurrentCehckState != null) 
            StartCoroutine(Checking(car));
        
        car.StartCoroutine(CarMove(new TrafficSystem.MoveDots(a,b, center),car));
        // if (_crossRoad._crossRoadDots.Contains(a) && car.Speed != car.FixedSpeed) {
        //     _state.SetState<CarStatePowerUp>(car);
        // }
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
    
    private void StartChecking(CarAbstract car)
    {
        car.CheckCar = _trafficDistanceTracker.GetCarForCheck(car);
        if (car.CheckCar != null) {
            _checkState.SetState<CarCheckCarState>(car);
            return;
        }
        car.CheckDot = _trafficDistanceTracker.GetDotForCheck(car);
        if (car.CheckDot != null) { 
            _checkState.SetState<CarCheckDotState>(car);
        }
    }
    
    private IEnumerator Checking(CarAbstract car) {
        while (car.CurrentCehckState != null) {
            car.CurrentCehckState.Enter(car);
            yield return new WaitForSeconds(0.3f);
        }
    }
}