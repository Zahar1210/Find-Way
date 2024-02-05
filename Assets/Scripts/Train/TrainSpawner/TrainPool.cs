using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class TrainPool : MonoBehaviour
{
    [SerializeField] private Train trainPrefab;
    private List<Train> _trains = new();
    
    [Header("Max & Min Length Train")]
    [SerializeField] private int maxCarriage;
    [SerializeField] private int minCarriage;
    
    public void SpawnTrain(AreaAbstract area)
    {
        Train[] trains = GetTrain(Random.Range(minCarriage, maxCarriage));
        Vector3 position = GetTrainPosition(area);
        foreach (var train in trains) 
            train.transform.position = position;
    }

    private Vector3 GetTrainPosition(AreaAbstract area)
    {
        return new Vector3(area.transform.position.x - 20, area.transform.position.y + 2.5f, area.transform.position.z);//пока так 
    }
    
    
    private Train[] GetTrain(int trainLength)
    {
        if (_trains.Count == 0)
            return GetExtraTrains(trainLength).ToArray();
        var activeTrains = _trains.Where(train => train.gameObject.activeSelf && !_trains.Contains(train)).ToList();
        if (activeTrains.Count < trainLength)
            activeTrains.AddRange(GetExtraTrains(trainLength - activeTrains.Count));
        return activeTrains.ToArray();
    }

    private List<Train> GetExtraTrains(int length)
    {
        List<Train> extraTrains = new();
        for (int i = 0; i < length; i++)
        {
            Train train = Instantiate(trainPrefab);
            extraTrains.Add(train);
            _trains.Add(train);
        }
        return extraTrains;
    }

    public void ReturnToPool()
    {
        
    }
}