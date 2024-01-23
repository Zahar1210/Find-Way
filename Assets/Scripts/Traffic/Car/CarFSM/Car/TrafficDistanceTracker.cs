using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class TrafficDistanceTracker : MonoBehaviour
{
    [SerializeField] private float checkInterval;
    [SerializeField] private CarPooler carPooler;
    private State _state;
    private TrafficSystem _trafficSystem;
    private CrossRoad _crossRoad;

    [Inject]
    private void Construct(State state, TrafficSystem trafficSystem, CrossRoad crossRoad)
    {
        _crossRoad = crossRoad;
        _trafficSystem = trafficSystem;
        _state = state;
    }

    public CarAbstract GetCarForCheck(CarAbstract currentCar)
    {
        List<CarAbstract> selectedCars = new();
        foreach (var car in carPooler._cars) {
            if (car.TargetDot == currentCar.TargetDot && car != currentCar) { 
                selectedCars.Add(car);
            }
        }
        if (selectedCars.Count != 0) 
             return GetNearCar(selectedCars, currentCar);
        return null;
    } 
    
    public IEnumerator CheckDistanceToTargetDotCoroutine(CarAbstract car)
    {
        while (true) {
            float dis = CheckDistanceToTargetDot(car);
            if (dis < 1 && (car.CurrentState is CarStateDriving || car.CurrentState is CarStateDriving)) {
                _state.SetState<CarStateSlowDown>(car);
                yield break;
            }
            yield return new WaitForSeconds(checkInterval);
        }
    }
    public IEnumerator CheckDistanceCoroutine(CarAbstract car)
    {
        while (car.CheckCar != null && car.CheckCar.TargetDot == car.TargetDot) {
            float dis = CheckDistance(car);
            ProcessDistance(dis, car);
            yield return new WaitForSeconds(checkInterval);
        }
    }
    private float CheckDistance(CarAbstract car)
    {
        if (car.TargetDot == car.CheckCar.TargetDot) {
            return Vector3.Distance(car.transform.position, car.CheckCar.transform.position);
        }
        else {
            car.CheckCar = null;
        }
        return 0;
    }

    private void ProcessDistance(float distance, CarAbstract car)
    {
        Debug.Log(car + " " + "дистанция от данной машины до цели равно " + distance);
        if (distance < 4 && (car.CurrentState is CarStateDriving || car.CurrentState is CarStateDriving)) {
            if (car.CheckCar.CurrentState is CarStateSlowDown) {
                _state.SetState<CarStateSlowDown>(car);
            }
            else if((car.CheckCar.CurrentState is CarStateDriving || car.CheckCar.CurrentState is CarStateDriving) && car.Speed > car.CheckCar.Speed){
                _state.SetState<CarStateSlowDown>(car, null,car.CheckCar);
            }
        }
        else if(distance > 4 && car.CurrentState is CarStateSlowDown || car.CheckCar == null) {
            _state.SetState<CarStatePowerUp>(car);
        }
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

    private float CheckDistanceToTargetDot(CarAbstract car) {
        return Vector3.Distance(car.transform.position, car.TargetDot.Pos);
    }
}
