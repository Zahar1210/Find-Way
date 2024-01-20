using System;
using System.Collections;
using UnityEngine;
using Zenject;

public class CarDriving: MonoBehaviour
{
    private CarStateDriving _carStateDriving;
    private CrossRoad _crossRoad;

    [Inject]
    private void Construct(CarStateDriving carStateDriving, CrossRoad crossRoad) {
        _crossRoad = crossRoad;
        _carStateDriving = carStateDriving;
    }

    public void Move(TrafficDot.Dot a, TrafficDot.Dot b, Vector3 center, CarAbstract car) {
        car.EndPos = b.Pos;
        car.CarArea = b.DotTraffic.Area.GetComponent<ITrafficable>();
        car.StartCoroutine(CarMove(new TrafficSystem.MoveDots(a,b, center),car));
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
                if (car.isCrossRoad) {
                    car.isCrossRoad = false;
                    _crossRoad.NextCarToMove(car);
                    _carStateDriving.EnterDriving(moveParams.DotB, car);
                    yield break;
                }
                if (!_crossRoad._changeDots.Contains(moveParams.DotB)) {
                    _carStateDriving.EnterDriving(moveParams.DotB, car);
                }
                else {
                    _crossRoad.AddCar(car, moveParams.DotB);
                }
                yield break;
            }
            yield return null;
        }
    }
}