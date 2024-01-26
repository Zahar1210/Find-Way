using UnityEngine;
using Zenject;

public class TrafficInstaler : MonoInstaller
{
    [SerializeField] private CarChecking carChecking;
    [SerializeField] private CheckState checkState;
    [SerializeField] private DotTransform dotTransform;
    [SerializeField] private DrivingState drivingState;
    [SerializeField] private DotFinding dotFinding;
    [SerializeField] private CarDriving carDriving;
    [SerializeField] private CarSpeedModifier carSpeedModifier;
    [SerializeField] private TrafficSystem trafficSystem;
    [SerializeField] private CrossRoad crossRoad;
    [SerializeField] private TrafficDistanceTracker trafficDistanceTracker;

    public override void InstallBindings()
    {
        BindCarChecking();
        BindCheckState();
        BindTrafficDistanceTracker();
        BindDotTransform();
        BindTrafficFactory();
        BindFinding();
        BindCarStateDriving();
        BindDrivingState();
        BindCarDriving();
        BindCarSlowDown();
        BindTrafficSystem();
        BindCrossRoad();
    }

    private void BindCarChecking()
    {
        Container
            .Bind<CarChecking>()
            .FromInstance(carChecking)
            .AsSingle();
    }

    private void BindTrafficDistanceTracker()
    {
        Container
            .Bind<TrafficDistanceTracker>()
            .FromInstance(trafficDistanceTracker)
            .AsSingle();
    }

    private void BindDotTransform()
    {
        Container
            .Bind<DotTransform>()
            .FromInstance(dotTransform)
            .AsSingle();
    }

    private void BindCarStateDriving()
    {
        Container
            .Bind<CarStateDriving>()
            .AsSingle();
    }

    private void BindTrafficFactory()
    {
        Container
            .Bind<TrafficFactory>()
            .AsSingle();
    }

    private void BindDrivingState()
    {
        Container
            .Bind<DrivingState>()
            .FromInstance(drivingState)
            .AsSingle();
    }
    
    private void BindCheckState()
    {
        Container
            .Bind<CheckState>()
            .FromInstance(checkState)
            .AsSingle();
    }

    private void BindFinding()
    {
        Container
            .Bind<DotFinding>()
            .FromInstance(dotFinding)
            .AsSingle();
    }
    private void BindCarDriving()
    {
        Container
            .Bind<CarDriving>()
            .FromInstance(carDriving)
            .AsSingle();
    }
    private void BindCarSlowDown()
    {
        Container
            .Bind<CarSpeedModifier>()
            .FromInstance(carSpeedModifier)
            .AsSingle();
    }

    private void BindTrafficSystem()
    {
        Container
            .Bind<TrafficSystem>()
            .FromInstance(trafficSystem)
            .AsSingle();
    }
    
    private void BindCrossRoad()
    {
        Container
            .Bind<CrossRoad>()
            .FromInstance(crossRoad)
            .AsSingle();
    }
}