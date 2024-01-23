using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class TrafficSystem : MonoBehaviour
{
    public LayerMask LayerMask;
    public float PivotDotDis;
    [SerializeField] private CarPooler carPooler;
    [SerializeField] private CrossRoad crossRoad;
    public Dictionary<int, TrafficDot> _traffic = new();
    private AreaAbstract pastArea;
    private State _state;
    private TrafficFactory _trafficFactory;

    [Inject]
    private void Construct(State state, TrafficFactory trafficFactory) {
        _state = state;
        _trafficFactory = trafficFactory;
    }
    
    private void Start() {
        _state.AddState(_trafficFactory.CreateState<CarStateDriving>());
        _state.AddState(_trafficFactory.CreateState<CarStateSlowDown>());
        _state.AddState(_trafficFactory.CreateState<CarStatePowerUp>());
    }
    

    public void SetTraffic(AreaAbstract areaAbstract) {
        foreach (var dot in FindObjectsOfType<TrafficDot>()) {
            if (!_traffic.TryGetValue(dot.Area.SpawnIndex, out var Dot)) {
                dot.SetDot();
                _traffic.Add(dot.Area.SpawnIndex, dot);
                carPooler.SpawnCar(dot.Area);
            }
        }

        if (areaAbstract.Type == AreaTypes.Mixed || areaAbstract.Type == AreaTypes.Traffic)
            CheckCrossRoad(areaAbstract);
    }

    private void CheckCrossRoad(AreaAbstract areaAbstract)
    {
        if (pastArea == null)
            pastArea = areaAbstract;
        if (areaAbstract.SpawnIndex -1 == pastArea.SpawnIndex && areaAbstract.Type != pastArea.Type) {
            ITrafficable trafficArea = (pastArea.Type == AreaTypes.Traffic) ? pastArea.GetComponent<ITrafficable>() : areaAbstract.GetComponent<ITrafficable>();
            if (!crossRoad._dots.Contains(trafficArea.Dot)) {
                crossRoad._dots.Add(trafficArea.Dot);
                crossRoad.AddDots();
            }
        }
        pastArea = areaAbstract;
    }

    public void ResetTraffic(ITrafficable area)
    {
        carPooler.ReturnToPool(area);
        _traffic.Remove(area.Dot.Area.SpawnIndex);
        if (crossRoad._changeDots.Count != 0) {
            crossRoad._dots.Remove(area.Dot);
            foreach (var dot in area.Dot.dots) {
                if (crossRoad._changeDots.Contains(dot)) {
                    crossRoad._changeDots.Remove(dot);
                    dot.CarSpawn = false;
                    dot.isCross = false;
                }
            }
        }
    }

    public class MoveDots
    {
        public TrafficDot.Dot DotA { get; }
        public TrafficDot.Dot DotB { get; }
        public Vector3 CenterDot { get; }

        public MoveDots(TrafficDot.Dot a, TrafficDot.Dot b, Vector3 center)
        {
            DotA = a;
            DotB = b;
            CenterDot = center;
        }
    }
}