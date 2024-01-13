using System.Collections;
using UnityEngine;

public static class CarMovement
{
    private static TrafficSystem trafficSystem = TrafficSystem.Instance;
    public static void Move(TrafficDot.Dot a, TrafficDot.Dot b, Vector3 center, CarAbstract car) {
        car.EndPos = b.Pos;
        car.CarArea = b.DotTraffic.Area.GetComponent<ITrafficable>();
        car.StartCoroutine(CarMove(new TrafficSystem.MoveDots(a,b, center),car));
        // car.StartCoroutine(ShootRayCoroutine(car, new TrafficSystem.MoveDots(a,b, center)));
    }
    private static IEnumerator CarMove(TrafficSystem.MoveDots moveParams,CarAbstract car)
    {
        float length = Vector3.Distance(moveParams.DotA.Pos, moveParams.DotB.Pos);
        float startTime = Time.time;
        while (true) {
            float distCovered = (Time.time - startTime) * car.Speed * car.SpeedMultiplier;
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
            // Debug.DrawRay(car.RayDot.transform.position,car.RayDot.transform.up * car.carStatus.RayDistance, Color.blue);
            yield return null;
        }
    }
    private static void ShootRay(CarAbstract car, TrafficSystem.MoveDots moveParams)
    {
        if (Physics.Raycast(car.RayDot.transform.position, car.RayDot.transform.up, out RaycastHit hit, car.carStatus.RayDistance, trafficSystem.LayerMask)) {
            if (hit.collider != null ) {
                Debug.DrawLine(car.RayDot.transform.position, hit.transform.position);
                CrossRoadWall wall = hit.collider.gameObject.GetComponent<CrossRoadWall>();
                // CarAbstract frontCar = hit.collider.gameObject.GetComponent<CarAbstract>();
                // if (frontCar != null && !car.IsSlowDown) {
                //     Debug.Log("передо мной машина" + " " + car.IsSlowDown);
                //     car.IsSlowDown = true;
                //     Debug.Log(car.IsSlowDown);
                //     car.StartCoroutine(SlowDown(car, frontCar));
                // }
                if (!car.IsSlowDown && wall != null) {
                    Debug.Log("тормозим");
                    car.IsSlowDown = true;
                    car.StartCoroutine(SlowDown(car));
                }
                else if (wall == null && car.IsSlowDown ){
                    car.IsSlowDown = false;
                    Debug.Log("разгон");
                    car.StartCoroutine(PowerUp(car));
                }
            }
            else
            {
                Debug.Log("никого нету" + car.transform.name);
            }
        }
    }
    
    private static IEnumerator ChangeSpeed(CarAbstract car, float targetSpeed, float duration)
    {
        float initialSpeed = car.SpeedMultiplier;
        float elapsedTime = 0f;
        while (elapsedTime < duration) {
            car.SpeedMultiplier = Mathf.Lerp(initialSpeed, targetSpeed, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        car.SpeedMultiplier = targetSpeed;
    }
    
    private static IEnumerator SlowDown(CarAbstract car, CarAbstract frontCar = null) { 
        yield return ChangeSpeed(car, (frontCar != null) ? frontCar.SpeedMultiplier - 0.3f : 0f, car.carStatus.TimeForMove);
    }
    
    private static IEnumerator PowerUp(CarAbstract car) {
        yield return ChangeSpeed(car, 1, car.carStatus.TimeForMove + 2);
    }
}