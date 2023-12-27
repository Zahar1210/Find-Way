using System;
using UnityEngine;

public class TrafficDot : MonoBehaviour
{
    public DotType Type;
    public CenterDot CenterDot;

    private void Start() {
        CenterDot = GetComponentInParent<CenterDot>();
    }
}