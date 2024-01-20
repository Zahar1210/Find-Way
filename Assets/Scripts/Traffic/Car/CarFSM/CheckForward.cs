using System.Collections;
using UnityEngine;
using Zenject;

public class CheckForward: MonoBehaviour
{
    private State _state;
    private TrafficSystem _trafficSystem;
    private CrossRoad _crossRoad;
    public bool canShootRay = true;

    [Inject]
    private void Construct(State state, TrafficSystem trafficSystem, CrossRoad crossRoad)
    {
        _crossRoad = crossRoad;
        _trafficSystem = trafficSystem;
        _state = state;
    }
    public IEnumerator ShootRayCoroutine(TrafficSystem.MoveDots moveParams, CarAbstract car)
    {
        while (canShootRay) {
            Physics.queriesHitBackfaces = true;
            ShootRay(car, moveParams);
            Debug.DrawRay(car.RayDot.transform.position, car.RayDot.transform.up * car.RayDistance, Color.yellow);
            yield return new WaitForSeconds(0.1f);
        }
    }
    
    private void ShootRay(CarAbstract car, TrafficSystem.MoveDots moveParams)
    {
        if (Physics.Raycast(car.RayDot.transform.position, car.transform.up, out RaycastHit hit, car.RayDistance, _trafficSystem.LayerMask)) {
            CarAbstract frontCar = hit.collider.GetComponent<CarAbstract>();
            if ((car.CurrentState is CarStatePowerUp || car.CurrentState is CarStateDriving) && frontCar) {
                if (frontCar) {
                    car.FixedSpeed = frontCar.FixedSpeed;
                    _state.SetState<CarStateSlowDown>(car,null,0.1f);
                }
            }
        }
        else if(car.CurrentState is CarStateSlowDown) {
            Debug.Log("разгон");
            _state.SetState<CarStatePowerUp>(car);
        }
    }


    private float GetTargetSpeed(CarAbstract frontCar)
    {
        if (_crossRoad._queueCars.TryGetValue(frontCar, out var dot)) {
            Debug.Log("приостановились");
            return 0;
        }
        else {
            Debug.Log("спередм машина надо взять ее скорость");
            return frontCar.Speed;
        }
    }
}
