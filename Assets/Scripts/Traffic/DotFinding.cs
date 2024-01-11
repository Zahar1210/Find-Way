using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class DotFinding
{
    static CrossRoad crossRoad = CrossRoad.Instance;
    static TrafficSystem trafficSystem = TrafficSystem.Instance;

    public static void GetDot(TrafficDot.Dot a, CarAbstract car)
    {
        TrafficDot.Dot b = GetFinalDot(a);
        StartMove(a,b,car);
    }
    private static void StartMove(TrafficDot.Dot a, TrafficDot.Dot b, CarAbstract car) 
    {
        if (b != null) {
            if (b.DotTraffic.Area.Type == a.DotTraffic.Area.Type){
                if (b.Type == a.Type) {
                    CarMovement.Move(a, b, Vector3.zero, car);
                }
                else {
                    CarMovement.Move(a, b, GetPivotDot(a, b), car);
                }
            }
            else {
                if (a.DotTraffic.Area.Type == AreaTypes.Traffic) {
                    CarMovement.Move(a, b, a.DotTraffic.transform.position, car);
                }
                else {
                    CarMovement.Move(a, b, b.DotTraffic.transform.position, car);
                }
            }
        }
    }
    private static TrafficDot.Dot GetFinalDot(TrafficDot.Dot a)
    {
        TrafficDot.Dot b = null;
        if (a.DotTraffic.Area.Type == AreaTypes.Traffic) {
            if (crossRoad._dots.Contains(a.DotTraffic) && CheckCrossRoadDot(a)) {
                return GetDotToTraffic(a);
            }
            else { 
                b = GetDotInTraffic(a);
                if (b == null) {
                    b = GetExtraDot(a);
                }
            }
        }
        else if (a.DotTraffic.Area.Type == AreaTypes.Mixed) {
            TrafficDot dot = GetDotArea(1, a);
            if (dot != null) {
                if (dot.Area.Type == AreaTypes.Traffic) {
                    b = GetDotToMixed(a, dot);
                }
                else { 
                    b = GetDotInMixed(a.Type, dot);
                }
            }
            else {
                b = GetExtraDot(a);
            }
        }
        else {
            Debug.Log("ват");
        }
        
        return b;
    }
    private static TrafficDot.Dot GetDotInTraffic(TrafficDot.Dot a) 
    {
        List<TrafficDot.Dot> dots = GetDots(a);
        if (dots != null)
            return dots.OrderBy(dot => Vector3.Distance(dot.СonstantPos, a.СonstantPos)).FirstOrDefault();
        return null;
    }
    private static TrafficDot.Dot GetDotInMixed(DotType aType, TrafficDot dot)
    {
        foreach (var d in dot.dots) {
            if (d.Type == aType)
                return d;
        }
        return null;
    }
    private static TrafficDot.Dot GetDotToMixed(TrafficDot.Dot a, TrafficDot dot)
    {
        List<TrafficDot.Dot> dots = new();
        dots = GetTrafficDots();
        TrafficDot dotArea = GetDotArea(2, a);
        if (dotArea != null) {
            if (dotArea.Area.Type == AreaTypes.Mixed) {
                foreach (var d in dotArea.dots) {
                    if (d.Type == a.Type)
                        dots.Add(d);
                }
            }
        }
        List<TrafficDot.Dot> GetTrafficDots() {
            List<TrafficDot.Dot> copyDots = new();
            List<TrafficDot.Dot> _dots = new();
            foreach (var d in dot.dots) {
                copyDots = GetDots(d);
                if (copyDots.Count == 1) {
                    _dots.Add(d);
                }
                copyDots.Clear();
            }
            return _dots;
        }

        return dots[Random.Range(0, dots.Count)];
    }
    private static TrafficDot.Dot GetDotToTraffic(TrafficDot.Dot a)
    {
        List<TrafficDot.Dot> dots = new();
        TrafficDot.Dot dotTraffic = GetDotInTraffic(a);
        dots.Add(dotTraffic);

        TrafficDot dotMixedFront = GetDotMixed(true);
        TrafficDot dotMixedBack = GetDotMixed(false);

        if (dotMixedFront != null) {
            TrafficDot.Dot front = GetDotInMixed(DotType.Left, dotMixedFront);
            if (front != null)
                dots.Add(front);
        }
        if (dotMixedBack != null) {
            TrafficDot.Dot back = GetDotInMixed(DotType.Right, dotMixedBack);
            if (back != null) 
                dots.Add(back);
        }
        
        return dots[Random.Range(0, dots.Count)];
        
        
        TrafficDot GetDotMixed(bool isFront) {
            int checkIndex = 0;
            checkIndex = (isFront) ? checkIndex + 1 : checkIndex - 1;
            if (trafficSystem._traffic.TryGetValue(a.DotTraffic.Area.SpawnIndex + checkIndex, out var dot)) {
                if (dot != null)
                    return dot;
            }
            return null;
        }
    }
    private static TrafficDot.Dot GetExtraDot(TrafficDot.Dot a)
    {
        if (a.DotTraffic.Area.Type == AreaTypes.Mixed) {
            foreach (var dot in a.DotTraffic.dots) {
                if (dot.Type != a.Type)
                    return dot;
            }
        }
        else {
            List<TrafficDot.Dot> dots = new();
            foreach (var dot in a.DotTraffic.dots) {
                if (dot.Type != a.Type)
                    dots.Add(dot);
            }
            return dots.OrderBy(dot => Vector3.Distance(dot.СonstantPos, a.СonstantPos)).FirstOrDefault();
        }

        return null;
    }
    private static List<TrafficDot.Dot> GetDots(TrafficDot.Dot a)
    {
        List<TrafficDot.Dot> d = new();
        if (a.Type == DotType.Left) {
            d.AddRange(a.DotTraffic.dots.Where(dot => dot.Type == a.Type && dot.СonstantPos.x < a.СonstantPos.x));
        }
        else {
            d.AddRange(a.DotTraffic.dots.Where(dot => dot.Type == a.Type && dot.СonstantPos.x > a.СonstantPos.x));
        }
        return d;
    }
    private static TrafficDot GetDotArea(int index, TrafficDot.Dot a)
    {
        int checkIndex = 0;
        checkIndex = (a.Type == DotType.Left) ? checkIndex + index : checkIndex - index;
        if (trafficSystem._traffic.TryGetValue(a.DotTraffic.Area.SpawnIndex + checkIndex, out var dotArea)) {
            return dotArea;
        }
        return null;
    }
    private static Vector3 GetPivotDot(TrafficDot.Dot a, TrafficDot.Dot b)
    {
        Vector3 direction = CalculateDirection();
        Vector3 midPoint = CalculateMidPoint();
        if (direction.z > 0 || direction.z < 0) {
            return CalculatePosition(true);
        }
        if (direction.x > 0 || direction.x < 0) {
            return CalculatePosition(false);
        }

        Vector3 CalculatePosition(bool isTraffic) {
            if (isTraffic) {
                if (a.Type == DotType.Left && b.Type == DotType.Right) {
                    return new Vector3(midPoint.x - 3, midPoint.y, midPoint.z);
                }
                if (a.Type == DotType.Right && b.Type == DotType.Left) {
                    return new Vector3(midPoint.x + 3, midPoint.y, midPoint.z); 
                }
            }
            else {
                if (a.Type == DotType.Left && b.Type == DotType.Right) {
                    return new Vector3(midPoint.x, midPoint.y, midPoint.z + 3);
                }
                if (a.Type == DotType.Right && b.Type == DotType.Left) {
                    return new Vector3(midPoint.x, midPoint.y, midPoint.z - 3);
                }
            }
            return Vector3.zero;
        }
        Vector3 CalculateDirection() {
            return (a.Pos - b.Pos);
        }
        Vector3 CalculateMidPoint() {
            return (a.Pos + b.Pos) / 2f;
        }
        return Vector3.zero;
    }
    private static bool CheckCrossRoadDot(TrafficDot.Dot a)
    {
        List<TrafficDot.Dot> dots = new();
        dots = GetDots(a);
        if (dots.Count == 2)
            return true;
        return false;
    }
}