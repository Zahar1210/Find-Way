using UnityEngine;
using Zenject;

public class TrafficInstaler : MonoInstaller
{
    [SerializeField] private DotTransform dotTransform;
    [SerializeField] private State state;
    [SerializeField] private DotFinding dotFinding;
    [SerializeField] private CarDriving carDriving;
    [SerializeField] private CarSlowSpeedModifier carSlowSpeedModifier;
    [SerializeField] private CheckForward checkForward;
    [SerializeField] private TrafficSystem trafficSystem;
    [SerializeField] private CrossRoad crossRoad;
    [SerializeField] private TrafficDistanceTracker trafficDistanceTracker;

    public override void InstallBindings()
    {
        BindTrafficDistanceTracker();
        BindDotTransform();
        BindTrafficFactory();
        BindFinding();
        BindCarStateDriving();
        BindState();
        BindCarDriving();
        BindCarSlowDown();
        BindCarCheckForward();
        BindTrafficSystem();
        BindCrossRoad();
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

    private void BindState()
    {
        Container
            .Bind<State>()
            .FromInstance(state)
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
            .Bind<CarSlowSpeedModifier>()
            .FromInstance(carSlowSpeedModifier)
            .AsSingle();
    }
    private void BindCarCheckForward()
    {
        Container
            .Bind<CheckForward>()
            .FromInstance(checkForward)
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