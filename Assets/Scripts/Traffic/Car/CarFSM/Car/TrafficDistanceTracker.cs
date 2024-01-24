using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class TrafficDistanceTracker : MonoBehaviour
{
    [SerializeField] private float checkInterval;
    [SerializeField] private CarPooler carPooler;
    private DrivingState _drivingState;
    private TrafficSystem _trafficSystem;
    private CrossRoad _crossRoad;

    [Inject]
    private void Construct(DrivingState drivingState, TrafficSystem trafficSystem, CrossRoad crossRoad)
    {
        _crossRoad = crossRoad;
        _trafficSystem = trafficSystem;
        _drivingState = drivingState;
    }
    public CarAbstract GetCarForCheck(CarAbstract currentCar)
    {
        List<CarAbstract> selectedCars = new();
        foreach (var car in carPooler._cars) {
            if (car.TargetDot == currentCar.TargetDot && car != currentCar) { 
                selectedCars.Add(car);
            }
        }
        if (selectedCars.Count != 0) {
            return GetNearCar(selectedCars, currentCar);
        }
        return null;
    }
    public Vector3 GetDotForCheck(CarAbstract car)
    {
        if (_crossRoad._crossRoadDots.Contains(car.TargetDot)) {
            return car.TargetDot.Pos;
        }
        return Vector3.zero;
    }
    private CarAbstract GetNearCar(List<CarAbstract> selectedCars, CarAbstract currentCar)
    {
        Vector3 currentPosition = currentCar.transform.position;
        CarAbstract nearCar = selectedCars
            .Where(obj => obj != null)
            .OrderBy(obj => Vector3.Distance(currentPosition, obj.transform.position))
            .FirstOrDefault();
        return nearCar;
    }
    
    
    
    
    
    
    
    
    
    public IEnumerator CheckDistanceCoroutine(CarAbstract car)
    {
        while ((car.CheckCar != null && car.CheckCar.TargetDot != null) && car.CheckCar.TargetDot == car.TargetDot) {
            float dis = CheckDistance(car);
            if ((dis < 4 && dis != 0) && car.CheckCar != null && (car.CurrentState is CarStateDriving || car.CurrentState is CarStatePowerUp)
                && car.CheckCar.FixedSpeed <= car.FixedSpeed) 
            {
                _drivingState.SetState<CarStateSlowDown>(car);
            }
            else if(car.Speed < car.FixedSpeed && car.CurrentState is CarStateSlowDown && dis > 4) {
                _drivingState.SetState<CarStatePowerUp>(car);
            }
            yield return new WaitForSeconds(dis/10);
        }
    }
    
    public IEnumerator CheckDistanceToDotCoroutine(CarAbstract car)
    {
        while (car.CheckCar == null && (car.CurrentState is CarStateDriving || car.CurrentState is CarStatePowerUp)) {
            float dis = CheckDistanceToTargetDot(car);
            if ((dis < 3 && dis != 0) && _crossRoad._queueCars.Count > 0) {
                _drivingState.SetState<CarStateSlowDown>(car);
            }
            yield return new WaitForSeconds(dis/10);
        }
    }
    private float CheckDistance(CarAbstract car)
    {
        if (car.TargetDot == car.CheckCar.TargetDot && car.CheckCar != null) 
            return Vector3.Distance(car.transform.position, car.CheckCar.transform.position);
        
        car.CheckCar = null;
        car.CheckDot = GetDotForCheck(car);
        if (car.CheckDot != Vector3.zero) { 
            StartCoroutine(CheckDistanceToDotCoroutine(car));
        }
        // else {
        //     _state.SetState<CarStatePowerUp>(car);
        // }
        return 0;
    }
    private float CheckDistanceToTargetDot(CarAbstract car) {
        return Vector3.Distance(car.transform.position, car.TargetDot.Pos);
    }
}
