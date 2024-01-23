using System.Collections.Generic;
using UnityEngine;
using Zenject;

public enum DotType {
    Left,
    Right
}
public class TrafficDot : MonoBehaviour
{
    public AreaAbstract Area;
    public List<Dot> dots = new();
    private DotTransform _dotTransform;
    
    public void Awake()
    {
        _dotTransform = DotTransform.Instance;
        if (Area.Type == AreaTypes.Traffic) {
            foreach (Vector3 position in _dotTransform.dotPositions) {
                dots.Add(new Dot(position, (position.z > 0) ? DotType.Left : DotType.Right,
                    (position.z > 0) ? _dotTransform.dotRotations[0] : _dotTransform.dotRotations[1], this));
            }
        }
        else
        {
            dots.Add(new Dot(new Vector3(-1, 0, 0), DotType.Right, _dotTransform.dotRotations[3], this));
            dots.Add(new Dot(new Vector3(1, 0, 0), DotType.Left, _dotTransform.dotRotations[2], this));
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
                if (dot.isCross) {
                    Gizmos.color = Color.green;
                    Gizmos.DrawSphere(dot.Pos, 0.2f);
                }
                else {
                    Gizmos.color = Color.white;
                    Gizmos.DrawSphere(dot.Pos,0.2f);
                }
            }
        }
    }

    public class Dot
    {
        public Quaternion Rot { get; }
        public Vector3 Pos { get; set; }
        public Vector3 СonstantPos { get; set; }
        public TrafficDot DotTraffic { get; set; } 
        public DotType Type { get; }
        public bool isCross;

        public bool CarSpawn { get; set; }
        public Dot(Vector3 pos, DotType type, Quaternion rot, TrafficDot parentDot)
        {
            DotTraffic = parentDot;
            Rot = Quaternion.Euler(rot.x, rot.y, rot.z);
            СonstantPos = pos;
            Type = type;
        }

        public void ChangePos(Vector3 trafficDotPos)
        {
            Pos = new Vector3(trafficDotPos.x + СonstantPos.x, trafficDotPos.y + СonstantPos.y, trafficDotPos.z + СonstantPos.z);
        }
    }
}