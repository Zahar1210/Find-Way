using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public static class CarPathSelected
{
    public static void TryMove(TrafficDot.Dot a, CarAbstract car)
    {
        TrafficDot.Dot b = null;
        switch (a.DotTraffic.Area.Type) {
            case AreaTypes.Traffic:
                TrafficDot.Dot extraDot = GetExtraDotTraffic(a);
                if (extraDot != null)
                    b = GetDotTraffic(a, extraDot);
                else
                    b = GetDotTraffic(a);
                break;
            case AreaTypes.Mixed:
                b = GetDotMixed(GetAreaMixed(a), a);
                break;
            default:
                Debug.LogError("странно, но зона данной точки не имеет ни одного из перечисленных типов");
                break;
        }

        if (b != null) {
            if (a.DotTraffic.Area.Type == b.DotTraffic.Area.Type) {
                if (a.Type == b.Type) {
                    car.Move(a, b, Vector3.zero);
                }
                else {
                    car.Move(a, b, GetPivotPoint(a, b));
                }
            }
            else {
                if (a.DotTraffic.Area.Type == AreaTypes.Traffic) {
                    car.Move(a, b, a.DotTraffic.transform.position);
                }
                else {
                    car.Move(a, b, b.DotTraffic.transform.position);
                }
            }
        }
    }

    private static TrafficDot.Dot GetExtraDotTraffic(TrafficDot.Dot a)
    {
        if (a.Pos.x < 10 && a.Pos.x > -11) {
            AreaAbstract extraArea = (a.Type == DotType.Left) ? a.DotTraffic.BackDot?.Area : a.DotTraffic.FrontDot?.Area;
            if (extraArea != null) {
                IName area = extraArea.GetComponent<IName>();
                foreach (var dot in area.Dot.dots) {
                    if (dot.Type != a.Type)
                        return dot;
                }
            }
        }
        return null;
    }

    private static TrafficDot.Dot GetDotTraffic(TrafficDot.Dot a, TrafficDot.Dot extraDot = null)
    {
        TrafficDot.Dot b = null;
        if (extraDot != null)
            b = TryDotMixed();
        else
            b = TryDotTraffic();

        TrafficDot.Dot TryDotTraffic() {
            List<TrafficDot.Dot> dots = new();
            foreach (var dot in a.DotTraffic.dots) {
                if (dot.Type == a.Type && ((a.Type == DotType.Left && dot.Pos.x < a.Pos.x) || (a.Type == DotType.Right && dot.Pos.x > a.Pos.x)))
                    dots.Add(dot);
            }
            if (dots.Count == 0) 
                return GetExtraDot(a);
            return dots[Random.Range(0, dots.Count)];
        }

        TrafficDot.Dot TryDotMixed()
        {
            List<TrafficDot.Dot> dots = new();
            List<TrafficDot.Dot> newDots = new();
            dots.Add(extraDot);
            foreach (var dot in a.DotTraffic.dots) {
                if (dot.Type == a.Type) {
                    dots.Add(dot);
                }
            }

            if (a.DotTraffic.FrontDot != null) {
                foreach (var dot in a.DotTraffic.FrontDot.dots) {
                    if ((dot.Type == DotType.Right && dot.Type != a.Type) || (dot.Type == DotType.Left && dot.Type == a.Type))
                        dots.Add(dot);
                }
            }

            if (a.DotTraffic.BackDot != null) {
                foreach (var dot in a.DotTraffic.BackDot.dots) {
                    if ((dot.Type == DotType.Right && dot.Type == a.Type) || (dot.Type == DotType.Left && dot.Type != a.Type))
                        dots.Add(dot);
                }
            }

            foreach (var dot in dots) {
                if (dot.Type == a.Type && ((a.Type == DotType.Left && dot.Pos.x < a.Pos.x) || (a.Type == DotType.Right && dot.Pos.x > a.Pos.x)))
                    newDots.Add(dot);
            }

            return newDots[Random.Range(0, newDots.Count)];
        }

        return b;
    }

    private static AreaAbstract GetAreaMixed(TrafficDot.Dot a)
    {
        return (a.Type == DotType.Left) ? a.DotTraffic.FrontDot?.Area : a.DotTraffic.BackDot?.Area;
    }

    private static TrafficDot.Dot GetDotMixed(AreaAbstract nextArea, TrafficDot.Dot a)
    {
        TrafficDot.Dot b = null;
        if (nextArea != null) {
            if (nextArea.Type == a.DotTraffic.Area.Type) {
                IName area = nextArea.GetComponent<IName>();
                if (area != null) {
                    b = TryDotMixed(area);
                }
            }
            else {
                IName area = nextArea.GetComponent<IName>();
                if (area != null) {
                    b = TryDotTraffic(area);
                }
            }
        }
        else {
            return GetExtraDot(a);
        }

        TrafficDot.Dot TryDotTraffic(IName area)
        {
            List<TrafficDot.Dot> dots = new();
            foreach (var dot in area.Dot.dots) {
                if (dot.Pos.x < 10 || dot.Pos.x > -11 && dot.Type == a.Type && dot.DotTraffic.Area.gameObject.activeSelf) {
                    dots.Add(dot);
                }
            }

            if (dots.Count > 1) {
                return dots[Random.Range(0, dots.Count)];
            }
            return dots[0];
        }

        TrafficDot.Dot TryDotMixed(IName area)
        {
            foreach (var dot in area.Dot.dots) {
                if (dot.Type == a.Type && dot.DotTraffic.Area.gameObject.activeSelf) {
                    return dot;
                }
            }

            return null;
        }

        return b;
    }

    private static TrafficDot.Dot GetExtraDot(TrafficDot.Dot a)
    {
        TrafficDot.Dot b = null;
        if (a.DotTraffic.Area.Type == AreaTypes.Mixed) {
            foreach (var dot in a.DotTraffic.dots) {
                if (dot.Type != a.Type) {
                    b = dot;
                }
            }
        }
        else
        {
            List<TrafficDot.Dot> dots = new();
            foreach (var dot in a.DotTraffic.dots) {
                if (dot.Type != a.Type) {
                    dots.Add(dot);
                }
            }
            if (a.Type == DotType.Left) {
                b = dots.OrderBy(p => p.Pos.x).First();
            }
            else {
                b = dots.OrderByDescending(p => p.Pos.x).First();
            }
        }

        return b;
    }

    private static Vector3 GetPivotPoint(TrafficDot.Dot a, TrafficDot.Dot b)
    {
        Vector3 direction = CalculateDirection();
        Vector3 midPoint = CalculateMidPoint();
        Debug.LogError(midPoint + " " + a.Pos + " " + b.Pos);
        if (direction.z > 0 || direction.z < 0) {
            return CalculatePosition(true);
        }
        if (direction.x > 0 || direction.x < 0) {
            return CalculatePosition(false);
        }

        Vector3 CalculatePosition(bool isTraffic)
        {
            if (isTraffic) {
                if (a.Type == DotType.Left && b.Type == DotType.Right) {
                    return new Vector3(midPoint.x -3, midPoint.y, midPoint.z);
                }
                if (a.Type == DotType.Right && b.Type == DotType.Left) {
                    return new Vector3(midPoint.x +3, midPoint.y, midPoint.z -1);
                }
            }
            else {
                if (a.Type == DotType.Left && b.Type == DotType.Right) {
                    return new Vector3(midPoint.x, midPoint.y, midPoint.z -3);
                }
                if (a.Type == DotType.Right && b.Type == DotType.Left) {
                    return new Vector3(midPoint.x, midPoint.y, midPoint.z +3);
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
}