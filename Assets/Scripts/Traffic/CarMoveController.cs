using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CarMoveController : MonoBehaviour
{ 
    public static CarMoveController Instance { get; set; }

    private void Awake()
    {
        if (Instance == null) {
            Instance = this;
            return;
        }
        Destroy(gameObject);
    }

    public void GetMovementDirection(TrafficDot.Dot currentDot, CarAbstract car)
    {
        TrafficDot.Dot targetDot = null;
        List<IName> areas = GetArea(currentDot.DotTraffic);
        List<TrafficDot.Dot> dots = GetDots(areas);
        switch (currentDot.DotTraffic.Area.Type) {
            case AreaTypes.Traffic: 
                targetDot = GetTargetDotTraffic(SelectDotsTraffic(currentDot, dots), currentDot);
                break;
            case AreaTypes.Mixed:
                targetDot = GetTargetDotMixed( SelectDotsMixed(currentDot.DotTraffic.Area.SpawnIndex + (currentDot.Type == DotType.Left ? 1 : -1), dots, currentDot));
                break;
        }

        if (targetDot != null) {
            if (currentDot.DotTraffic.Area.Type == targetDot.DotTraffic.Area.Type) {
                car.Move(currentDot, targetDot,Vector3.zero);
            }
            else {
                if (currentDot.DotTraffic.Area.Type == AreaTypes.Traffic) {
                    car.Move(currentDot, targetDot, currentDot.DotTraffic.transform.position);
                }
                else {
                    car.Move(currentDot, targetDot, targetDot.DotTraffic.transform.position);
                }
            }
        }
        else {
            Debug.LogError("ьшмцтттушцирсцсирцшср");
        }
    }

    private List<IName> GetArea(TrafficDot current)
    {
        List<IName> areas = new();
        IName name1 = current.Area.GetComponent<IName>();
        if (name1 != null) {
            areas.Add(name1);
        }
        if (current.FrontDot != null) {
            IName name2 = current.FrontDot.Area.GetComponent<IName>();
            if (name2 != null) {
                areas.Add(name2);
            }
        }
        if (current.BackDot != null) {
            IName name3 = current.BackDot.Area.GetComponent<IName>();
            if (name3 != null) {
                areas.Add(name3);
            }
        }
        return areas;
    }

    private List<TrafficDot.Dot> GetDots(List<IName> areas)
    {
        List<TrafficDot.Dot> dots = new();
        foreach (var dot in areas) {
            foreach (var d in dot.Dot.dots) {
                dots.Add(d);
            }
        }
        return dots;
    }

    private List<TrafficDot.Dot> SelectDotsMixed(int index, List<TrafficDot.Dot> dots, TrafficDot.Dot currentDot)
    {
        List<TrafficDot.Dot> newDots = new();
        foreach (var dot in dots) {
            if (dot.DotTraffic.Area.SpawnIndex != index || (dot.Pos.x < 10 || dot.Pos.x > -10) || dot.Type == currentDot.Type && dot != currentDot) {
                newDots.Add(dot);
            }
        }
        return newDots;
    }

    private List<TrafficDot.Dot> SelectDotsTraffic(TrafficDot.Dot current, List<TrafficDot.Dot> dots)
    {
        List<TrafficDot.Dot> newDots = new();
        foreach (var dot in dots) {
            if (dot.Type == current.Type && dot != current) {
                newDots.Add(dot);
            }
        }
        if (current.Pos.x < 10 || current.Pos.x > -10)
        {
            AreaAbstract areaAbstract = null;
            if (current.Type == DotType.Left) {
                if (current.DotTraffic.FrontDot != null) {
                    areaAbstract = current.DotTraffic.FrontDot.Area;
                }
            }
            else {
                if (current.DotTraffic.BackDot != null) {
                    areaAbstract = current.DotTraffic.BackDot.Area;
                }
            }
            if (areaAbstract != null) {
                IName name = areaAbstract.GetComponent<IName>();
                if (name != null) {
                    foreach (var dot in name.Dot.dots) {
                        if (dot.Type == current.Type) {
                            newDots.Add(dot);
                        }
                    }
                }
            }
        }
        else {
            foreach (var dot in dots) {
                if (dot.DotTraffic.Area == current.DotTraffic.Area && dot.Type == current.Type && dot != current) {
                    newDots.Add(dot);
                }
            }
        }
        return newDots;
    }

    private TrafficDot.Dot GetTargetDotMixed(List<TrafficDot.Dot> dots)
    {
        if (dots.Count == 0) {
            Debug.LogError("lf,pefmwmfwpomfepofeofepwomfpefeopfwopfpe");
        }
        if (dots.Count > 1) {
            return dots[Random.Range(0, dots.Count)];
        }
        return dots[0];
    }

    private TrafficDot.Dot GetTargetDotTraffic(List<TrafficDot.Dot> dots, TrafficDot.Dot currentDot)
    {
        List<TrafficDot.Dot> newDots = new();
        if (dots.Count > 1) {
            if (currentDot.Type == DotType.Right) {
                foreach (var dot in dots)
                {
                    if (dot.Pos.x > currentDot.Pos.x) {
                        newDots.Add(dot);
                    }
                }
            } 
            else {
                foreach (var dot in dots) {
                    if (dot.Pos.x < currentDot.Pos.x) {
                        newDots.Add(dot);
                    }
                }
            }
            return dots[Random.Range(0, dots.Count)];
        }
        
        return dots[0];
    }
}