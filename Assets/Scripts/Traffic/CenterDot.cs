using System;
using System.Collections.Generic;
using UnityEngine;

public class CenterDot : MonoBehaviour
{
    public AreaAbstract Area { get; set;}
    public List<TrafficDot> trafficDots;

    private void Start()
    {
        Area = GetComponentInParent<AreaAbstract>();
        foreach (Transform child in transform) {
            TrafficDot dot = child.GetComponent<TrafficDot>();
            if (dot) {
                trafficDots.Add(dot);
            }
        }
    }
    
    
}