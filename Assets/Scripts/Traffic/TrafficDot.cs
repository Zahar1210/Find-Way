using System;
using System.Collections.Generic;
using UnityEngine;

public class TrafficDot : MonoBehaviour
{
    public AreaAbstract Area;
    private List<Dot> dots = new();
    public TrafficDot FrontDot;
    public TrafficDot BackDot;
    Vector3[] dotPositions = {
            new Vector3(-4, 0, -1),
            new Vector3(4, 0, -1),
            new Vector3(-19, 0, -1),
            new Vector3(10, 0, -1),
            new Vector3(-4, 0, 1),
            new Vector3(4, 0, 1),
            new Vector3(-19, 0, 1),
            new Vector3(10, 0, 1)
    };
    public void Awake()
    {
        if (Area.Type == AreaTypes.Traffic) {
            foreach (Vector3 position in dotPositions) {
                dots.Add(new Dot(position,(position.z > 0) ? DotType.Left : DotType.Right));
            }
        }
        else {
            dots.Add(new Dot(new Vector3(-1,0,0),DotType.Right));
            dots.Add(new Dot(new Vector3(1,0,0),DotType.Left));
        }
    }
    public void SetDot()
    {
        foreach (var dot in dots) {
            dot.ChangePos(transform.position);
        }
    }
    private void OnDrawGizmos()
    {
        if (dots != null) {
            foreach (var dot in dots) {
                if (dot.Type == DotType.Right) {
                    Gizmos.DrawSphere(dot.Pos, 0.2f);
                }
                else {
                    Gizmos.DrawSphere(dot.Pos, 0.4f);
                }
            }
        }
    }
    public class Dot
    {
        public Vector3 Pos { get; set; }
        private Vector3 СonstantPos { get; }
        public DotType Type { get; }
        public Dot(Vector3 pos, DotType type)
        {
            СonstantPos = pos;
            Type = type;
        }
        public void ChangePos(Vector3 trafficDotPos) {
            Pos = new Vector3(trafficDotPos.x + СonstantPos.x, trafficDotPos.y + СonstantPos.y,trafficDotPos.z + СonstantPos.z);
        }
    }
}