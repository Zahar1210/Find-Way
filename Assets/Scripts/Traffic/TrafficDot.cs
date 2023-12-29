using System;
using System.Collections.Generic;
using UnityEngine;

public class TrafficDot : MonoBehaviour
{
    public AreaAbstract Area;
    public List<Dot> dots = new();
    public TrafficDot FrontDot;
    public TrafficDot BackDot;
    private Vector3[] dotPositions = {
            new Vector3(-4, 0, -1),
            new Vector3(4, 0, -1),
            new Vector3(-19, 0, -1),
            new Vector3(10, 0, -1),
            new Vector3(-4, 0, 1),
            new Vector3(4, 0, 1),
            new Vector3(-19, 0, 1),
            new Vector3(10, 0, 1)
    };
    private Quaternion[] dotRotations = {
        new Quaternion(0.0f, 0.0f, 90.0f, 0f),
        new Quaternion(0f, 90f, 90f, 0f),
    };
    public void Awake()
    {
        if (Area.Type == AreaTypes.Traffic) {
            foreach (Vector3 position in dotPositions) {
                dots.Add(new Dot(position,(position.z > 0) ? DotType.Left : DotType.Right, dotRotations[0]));
            }
        }
        else {
            dots.Add(new Dot(new Vector3(-1,0,0),DotType.Right, dotRotations[1]));
            dots.Add(new Dot(new Vector3(1,0,0),DotType.Left, dotRotations[1]));
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
        public Quaternion Rot { get;}
        public Vector3 Pos { get; set; }
        private Vector3 СonstantPos { get; }
        public DotType Type { get; }
        public Dot(Vector3 pos, DotType type, Quaternion rot)
        {
            Rot =  Quaternion.Euler(rot.x, rot.y, rot.z);
            СonstantPos = pos;
            Type = type;
        }
        public void ChangePos(Vector3 trafficDotPos) {
            Pos = new Vector3(trafficDotPos.x + СonstantPos.x, trafficDotPos.y + СonstantPos.y,trafficDotPos.z + СonstantPos.z);
        }
    }
}