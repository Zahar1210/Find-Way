using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class TrafficSystem : MonoBehaviour
{
    public float PivotDotDis;
    
    public Dictionary<int, TrafficDot> _traffic = new();
    [SerializeField] private CarPooler carPooler;
    [SerializeField] private CrossRoad crossRoad;
    
    private DrivingState _drivingState;
    private AreaAbstract pastArea;
    private TrafficFactory _trafficFactory;
    private CheckState _checkState;

    [Inject]
    private void Construct(DrivingState drivingState, TrafficFactory trafficFactory, CheckState checkState) {
        _drivingState = drivingState;
        _checkState = checkState;
        _trafficFactory = trafficFactory;
    }
    
    private void Start() {
        _drivingState.AddState(_trafficFactory.CreateState<CarStateDriving>());
        _drivingState.AddState(_trafficFactory.CreateState<CarStateSlowDown>());
        _drivingState.AddState(_trafficFactory.CreateState<CarStatePowerUp>());
        _checkState.AddState(_trafficFactory.CreateState<CarCheckCarState>());
        _checkState.AddState(_trafficFactory.CreateState<CarCheckDotState>());
        _checkState.AddState(_trafficFactory.CreateState<CarCheckStateDistanceCheckCar>());
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
        if (areaAbstract.SpawnIndex - 1 == pastArea.SpawnIndex && areaAbstract.Type != pastArea.Type) {
            ITrafficable trafficArea = (pastArea.Type == AreaTypes.Traffic) ? pastArea.GetComponent<ITrafficable>() : areaAbstract.GetComponent<ITrafficable>();
            if (!crossRoad._dots.Contains(trafficArea.Dot)) {
                crossRoad._dots.Add(trafficArea.Dot);
                crossRoad.AddDots();
            }
            else if(pastArea.Type == AreaTypes.Traffic) 
                crossRoad.AddDots();
        }
        pastArea = areaAbstract;
    }

    public void ResetTraffic(ITrafficable area)
    {
        carPooler.ReturnToPool(area);
        _traffic.Remove(area.Dot.Area.SpawnIndex);
        if (crossRoad._crossRoadDots.Count != 0) {
            crossRoad._dots.Remove(area.Dot);
            foreach (var dot in area.Dot.dots) {
                if (crossRoad._crossRoadDots.Contains(dot)) {
                    crossRoad._crossRoadDots.Remove(dot);
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