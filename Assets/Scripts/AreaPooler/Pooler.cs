using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Pooler : MonoBehaviour
{
    [SerializeField] private float spawnInterval;
    [SerializeField] private float checkInterval;
    [SerializeField] private Transform pointReturnToPool;
    [SerializeField] private PathFinding pathFinding;
    [SerializeField] private Transform pointCheckInterval;
    [SerializeField] private AreaAbstract[] areaArray;

    private List<AreaAbstract> _areaAbstracts = new();
    private Dictionary<int, AreaTypes> _typesArea = new();
    private Dictionary<AreaTypes, int> _area = new();
    private Vector3Int _lastSpawnPosition;
    private float _time;
    private bool isSpawn;
    private AreaAbstract queueArea;


    private void Start()
    {
        foreach (var area in FindObjectsOfType<AreaAbstract>())
        {
            _areaAbstracts.Add(area);
        }
        _lastSpawnPosition = Vector3Int.RoundToInt(transform.position);
        _area.Add(AreaTypes.Slowdown, 0);
        _area.Add(AreaTypes.RisingSpikes, 1);
        _area.Add(AreaTypes.MeteorFall, 2);
        _area.Add(AreaTypes.Traffic, 3);
        _typesArea.Add(0, AreaTypes.MeteorFall);
        _typesArea.Add(1, AreaTypes.Slowdown);
        _typesArea.Add(2, AreaTypes.RisingSpikes);
        _typesArea.Add(3, AreaTypes.Traffic);
        queueArea = GetQueueArea();
    }

    private void Timer()
    {
        _time += Time.deltaTime;
        if (_time >= checkInterval) {
            ReturnToPool();
            _time = 0f;
        }
    }

    private void Update()
    {
        Timer();
        Vector3Int currentZPosition = Vector3Int.RoundToInt(pointCheckInterval.position);
        if (currentZPosition.z - _lastSpawnPosition.z >= spawnInterval) {
            _lastSpawnPosition.z = currentZPosition.z;
            SpawnArea(currentZPosition);
            pathFinding.FindTiles();//обновляем данные для поиска
        }
    }

    private void SpawnArea(Vector3Int areaPosition)
    {
        AreaAbstract beforeArea = queueArea;
        Vector3 newPosition = queueArea.transform.position;
        newPosition.z = areaPosition.z;
        queueArea.transform.position = newPosition;
        queueArea.EnableArea(true);
        queueArea = GetQueueArea();
        SetSpawnInterval(beforeArea);
    }

    private AreaAbstract GetQueueArea()
    {
        AreaTypes type = GetRandomTypeArea();
        AreaAbstract area = GetAreaFromPool(type);
        if (area != null) {
            return area;
        }
        return AreaToSpawn(type);
    }

    private AreaAbstract GetAreaFromPool(AreaTypes type)
    {
        foreach (AreaAbstract area in _areaAbstracts) {
            if (!area.gameObject.activeSelf && area.Type == type && area != null)
                return area;
        }
        return null;
    }

    private AreaAbstract AreaToSpawn(AreaTypes type)
    {
        if (_area.TryGetValue(type, out int index)) {
            AreaAbstract area = Instantiate(areaArray[index]);
            _areaAbstracts.Add(area);
            area.EnableArea(false);
            return area;
        }
        Debug.Log("ватафак так быть не долно вообще");
        return null;
    }

    private AreaTypes GetRandomTypeArea()
    {
        if (_typesArea.TryGetValue(Random.Range(0, 4), out AreaTypes type)) return type; // пока так (значения _max) в Range
        return AreaTypes.Slowdown;
    }

    private void ReturnToPool()
    {
        foreach (var area in _areaAbstracts) {
            if (area.transform.position.z <= pointReturnToPool.transform.position.z && area.gameObject.activeSelf)
                area.EnableArea(false);
        }
    }

    private void SetSpawnInterval(AreaAbstract beforeArea) {
        spawnInterval = (beforeArea.AreaLength + queueArea.AreaLength) / 2f;
        if (spawnInterval % 1 != 0) { spawnInterval = beforeArea.AreaLength; }
    }
}