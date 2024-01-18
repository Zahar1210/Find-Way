using Zenject;

public class TrafficFactory
{
    private DiContainer _container;

    [Inject]
    private void Cosnstruct(DiContainer container) {
        _container = container;
    }
    public T CreateState<T>() {
        return _container.Instantiate<T>();
    }
}