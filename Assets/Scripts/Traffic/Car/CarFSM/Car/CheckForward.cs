using System.Collections;
using UnityEngine;
using Zenject;

public class CheckForward: MonoBehaviour
{
    private DrivingState _drivingState;
    private TrafficSystem _trafficSystem;
    private CrossRoad _crossRoad;
    public bool canShootRay = true;

    [Inject]
    private void Construct(DrivingState drivingState, TrafficSystem trafficSystem, CrossRoad crossRoad)
    {
        _crossRoad = crossRoad;
        _trafficSystem = trafficSystem;
        _drivingState = drivingState;
    }
    public IEnumerator ShootRayCoroutine(CarAbstract car)
    {
        while (canShootRay) {
            Physics.queriesHitBackfaces = true;
            ShootRay(car);
            Debug.DrawRay(car.RayDot.transform.position, car.RayDot.transform.up * car.RayDistance, Color.yellow);
            yield return new WaitForSeconds(0.3f);
        }
    }
    
    private void ShootRay(CarAbstract car)
    {
        if (Physics.Raycast(car.RayDot.transform.position, car.transform.up, out RaycastHit hit, car.RayDistance, _trafficSystem.LayerMask)) {
            CarAbstract frontCar = hit.collider.GetComponent<CarAbstract>();
            if ((car.CurrentState is CarStatePowerUp || car.CurrentState is CarStateDriving) && frontCar) {
                if (frontCar) {
                    car.FixedSpeed = frontCar.FixedSpeed;
                    _drivingState.SetState<CarStateSlowDown>(car);
                }
            }
        }
        else if(car.CurrentState is CarStateSlowDown && car.FixedSpeed != 0) {
            _drivingState.SetState<CarStatePowerUp>(car);
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
