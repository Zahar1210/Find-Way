using System;
using System.Collections;
using UnityEngine;

public class CarService : CarAbstract
{
    private Vector3 start;
    private Vector3 end;
    public override void Move(TrafficDot.Dot a, TrafficDot.Dot b, Vector3 center )
    {
        if (a ==null || b == null) {
            Debug.LogError("ватафак");
            return;
        }
        start = a.Pos;
        end = b.Pos;
        Area = b.DotTraffic.Area.GetComponent<IName>();
        StartCoroutine(CarMove(new TrafficSystem.MoveDots(a,b,center)));
    }

    private bool IsValidPosition(Vector3 position)
    {
        return !float.IsNaN(position.x) && !float.IsNaN(position.y) && !float.IsNaN(position.z);
    }
    private IEnumerator CarMove(TrafficSystem.MoveDots moveParams)
    {
        float journeyLength = Vector3.Distance(moveParams.DotA.Pos, moveParams.DotB.Pos);
        if (journeyLength <= 0.0f) {
            CarPathSelected.TryMove(moveParams.DotA, this);
            yield break;
        }
        float startTime = Time.time;
        while (true) {
            float distCovered = (Time.time - startTime) * speed;
            float fractionOfJourney = distCovered / journeyLength;
            Vector3 newPosition = Bezier.GetPoint(fractionOfJourney, moveParams.DotA, moveParams.DotB, moveParams.CenterDot);
            if (!IsValidPosition(newPosition)) {
                Debug.LogError("Invalid position values (NaN)!");
                yield break;
            }
            transform.position = newPosition;
            if (fractionOfJourney >= 1.0f) {
                CarPathSelected.TryMove(moveParams.DotB, this);
                yield break;
            }
            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(start, end);
    }

    public override void SlowDown()
    {
    }

    public override void Signal()
    {
    }

    public override void CheckForward()
    {
    }

    public override void CheckArea()
    {
    }
}