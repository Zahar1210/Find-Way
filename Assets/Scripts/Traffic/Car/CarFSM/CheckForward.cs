using System.Collections;
using UnityEngine;
using Zenject;

public class CheckForward: MonoBehaviour
{
    private State _state;
    private TrafficSystem _trafficSystem;

    [Inject]
    private void Construct(State state, TrafficSystem trafficSystem)
    {
        _trafficSystem = trafficSystem;
        _state = state;
    }
    public IEnumerator ShootRayCoroutine(TrafficSystem.MoveDots moveParams, CarAbstract car)
    {
        while (true) {
            ShootRay(car, moveParams);
            Debug.DrawRay(car.RayDot.transform.position, car.RayDot.transform.up * car.RayDistance, Color.blue);//
            yield return new WaitForSeconds(0.3f);
        }
    }
    
    private void ShootRay(CarAbstract car, TrafficSystem.MoveDots moveParams)
    {
        if (Physics.Raycast(car.RayDot.transform.position, car.transform.up, out RaycastHit hit, car.RayDistance, _trafficSystem.LayerMask)) {
            if (hit.collider != null) {
                CrossRoadWall wall = hit.collider.gameObject.GetComponent<CrossRoadWall>();
                CarAbstract frontCar = hit.collider.gameObject.GetComponent<CarAbstract>();
                if (frontCar != null && car.CurrentState is CarStateDriving) {
                    car.CurrentState.Exit();
                    _state.SetState<CarStateDriving>(car,null,frontCar);
                }
                if (car.CurrentState is CarStateDriving && wall != null) {
                    car.CurrentState.Exit();
                    _state.SetState<CarStateSlowDown>(car, null, frontCar);
                }
            }
            else {
                if (car.CurrentState is CarStateSlowDown) {
                    car.CurrentState.Exit();
                    _state.SetState<CarStateDriving>(car);
                }
            }
        }
    }
}
