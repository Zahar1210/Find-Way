using System.Collections;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using Zenject;

public class CheckForward: MonoBehaviour
{
    private State _state;
    private TrafficSystem _trafficSystem;
    public bool canShootRay = true;

    [Inject]
    private void Construct(State state, TrafficSystem trafficSystem)
    {
        _trafficSystem = trafficSystem;
        _state = state;
    }
    public IEnumerator ShootRayCoroutine(TrafficSystem.MoveDots moveParams, CarAbstract car)
    {
        while (canShootRay) {
            ShootRay(car, moveParams);
            Debug.DrawRay(car.RayDot.transform.position, car.RayDot.transform.up * car.RayDistance, Color.yellow);
            yield return new WaitForSeconds(0.3f);
        }
    }
    
    private void ShootRay(CarAbstract car, TrafficSystem.MoveDots moveParams)
    {
        if (Physics.Raycast(car.RayDot.transform.position, car.transform.up, out RaycastHit hit, car.RayDistance, _trafficSystem.LayerMask)) {
            CrossRoadWall wall = hit.collider.GetComponent<CrossRoadWall>();
            CarAbstract frontCar = hit.collider.GetComponent<CarAbstract>();
            if ((car.CurrentState is CarStatePowerUp || car.CurrentState is CarStateDriving) && (frontCar || wall)) {
                if (frontCar) {
                    Debug.Log("тормозим перед машиной");
                    _state.SetState<CarStateSlowDown>(car,null,GetTargetSpeed(frontCar, car));
                }
                else if (wall) {
                    Debug.Log("тормозим перед стеной");
                    _state.SetState<CarStateSlowDown>(car);
                }
            }
        }
        else if(car.CurrentState is CarStateSlowDown) {
            Debug.Log("разгон");
            _state.SetState<CarStatePowerUp>(car);
        }
    }


    private float GetTargetSpeed(CarAbstract frontCar, CarAbstract car)
    {
        float dis = Vector3.Distance(frontCar.transform.position, car.transform.position);
        float targetDis = frontCar.transform.localScale.y + car.transform.localScale.y / 1.8f;
        if (dis < targetDis - 0.1f) {
            return 0.2f;
        }
        else {
            return (frontCar.Speed >= car.Speed) ? car.Speed : frontCar.Speed;
        }
    }
}
