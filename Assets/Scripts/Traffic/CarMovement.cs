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
        car.CarArea = b.DotTraffic.Area.GetComponent<IName>();
        car.StartCoroutine(CarMove(new TrafficSystem.MoveDots(a,b, center),car));
    }
    private static IEnumerator CarMove(TrafficSystem.MoveDots moveParams,CarAbstract car)
    {
        float journeyLength = Vector3.Distance(moveParams.DotA.Pos, moveParams.DotB.Pos);
        float startTime = Time.time;
        while (true) {
            float distCovered = (Time.time - startTime) * car.speed;
            float fractionOfJourney = distCovered / journeyLength;
            car.transform.position =  CarTransform.GetPosition(fractionOfJourney, moveParams.DotA, moveParams.DotB, moveParams.CenterDot);
            car.transform.rotation = CarTransform.GetRotation(fractionOfJourney, moveParams.DotA, moveParams.DotB);
            if (fractionOfJourney >= 1.0f) {
                DotFinding.GetDot(moveParams.DotB, car);
                yield break;
            }
            yield return null;
        }
    }
}