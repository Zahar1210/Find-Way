using System.Collections;
using UnityEngine;

public static class CarMovement
{
    public static void Move(TrafficDot.Dot a, TrafficDot.Dot b, Vector3 center, CarAbstract car) {
        if (a ==null || b == null) {
            Debug.LogError("ватафак");
            return;
        }
        car.EndPos = b.Pos;
        car.CarArea = b.DotTraffic.Area.GetComponent<ITrafficable>();
        car.StartCoroutine(CarMove(new TrafficSystem.MoveDots(a,b, center),car));
        car.StartCoroutine(ShootRayCoroutine(car, new TrafficSystem.MoveDots(a,b, center)));
    }
    private static IEnumerator CarMove(TrafficSystem.MoveDots moveParams,CarAbstract car)
    {
        float length = Vector3.Distance(moveParams.DotA.Pos, moveParams.DotB.Pos);
        float startTime = Time.time;
        while (true) {
            float distCovered = (Time.time - startTime) * car.Speed;
            float fractionOfJourney = distCovered / length;
            car.transform.position =  CarTransform.GetPosition(fractionOfJourney, moveParams.DotA, moveParams.DotB, moveParams.CenterDot);
            car.transform.rotation = CarTransform.GetRotation(fractionOfJourney, moveParams.DotA, moveParams.DotB);
            if (fractionOfJourney >= 1.0f) {
                DotFinding.GetDot(moveParams.DotB, car);
                yield break;
            }
            yield return null;
        }
    }

    private static IEnumerator ShootRayCoroutine(CarAbstract car, TrafficSystem.MoveDots moveParams)
    {
        while (true) {
            ShootRay(car, moveParams);
            Debug.DrawRay(car.RayDot.transform.position,car.RayDot.transform.up * car.RayDistance, Color.blue);
            yield return new WaitForSeconds(0.2f);
        }
    }
    private static void ShootRay(CarAbstract car, TrafficSystem.MoveDots moveParams)
    {
        if (Physics.Raycast(car.RayDot.transform.position, car.RayDot.transform.up, out RaycastHit hit, car.RayDistance, car.layerMask)) {
            if (hit.collider != null ) {
                CarAbstract frontCar = hit.collider.gameObject.GetComponent<CarAbstract>();
                CrossRoadWall wall = hit.collider.gameObject.GetComponent<CrossRoadWall>();
                if (frontCar != null && !Mathf.Approximately(car.Speed, frontCar.Speed) ) {
                    Debug.Log("передо мной машина");
                    car.StartCoroutine(SlowDown(car, frontCar));
                }
                else if (!Mathf.Approximately(car.Speed, 0) && wall != null) {
                    Debug.Log("тормозим");
                    car.StartCoroutine(SlowDown(car));
                }
                else if (frontCar == null && wall == null && car.Speed != car.FixedSpeed){
                    car.StartCoroutine(PowerUp(car));
                }
            }
        }
    }

    private static IEnumerator SlowDown(CarAbstract car, CarAbstract frontCar = null) {
        yield return ChangeSpeed(car, (frontCar != null) ? frontCar.Speed - 1 : 0f, car.TimeForMove);
    }

    private static IEnumerator ChangeSpeed(CarAbstract car, float targetSpeed, float duration)
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

    private static IEnumerator PowerUp(CarAbstract car) {
        yield return ChangeSpeed(car, car.FixedSpeed, car.TimeForMove + 2);
    }

}