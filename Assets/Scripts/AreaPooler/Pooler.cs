using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Pooler : MonoBehaviour
{
    [SerializeField] private PathFinding pathFinding;
    [SerializeField] private Transform pointReturnToPool;
    [SerializeField] private Transform pointCheckInterval;
    [SerializeField] private int spawnInterval;
    [SerializeField] private float checkInterval;
    [SerializeField] private AreaAbstract[] areaArray;
    
    private List<AreaAbstract> _areaAbstracts = new();
    private Dictionary<int, AreaTypes> _typesMap = new();
    private Dictionary<AreaTypes, int> _maps = new();
    private Vector3Int _lastSpawnPosition;
    private float _time;
    private bool isSpawn;
    private Vector3Int currentXPosition = new Vector3Int(0, 0 ,5);

    private void Start()//инициализация 
    {
        _lastSpawnPosition = Vector3Int.RoundToInt(transform.position); 
        // _maps.Add(AreaTypes.Traffic, 0);
        _maps.Add(AreaTypes.Slowdown, 0);
        _maps.Add(AreaTypes.RisingSpikes, 1);
        _maps.Add(AreaTypes.MeteorFall, 2);
        _typesMap.Add(0, AreaTypes.MeteorFall);
        _typesMap.Add(1, AreaTypes.Slowdown);
        _typesMap.Add(2, AreaTypes.RisingSpikes);
        // typesMap.Add(3, AreaTypes.Traffic);
        foreach (var area in FindObjectsOfType<AreaAbstract>()) {
            _areaAbstracts.Add(area);
        }
    }

    private void Update()
    {
        _time += Time.deltaTime;
        Vector3Int currentXPosition = Vector3Int.RoundToInt(pointCheckInterval.position);//узнаем когда нужно спавнить новую терииторию
        if (currentXPosition.z - _lastSpawnPosition.z >= spawnInterval) {
            SpawnArea(currentXPosition);
            Debug.Log("заспавнили");
            pathFinding.FindTiles();
            _lastSpawnPosition.z = currentXPosition.z;
        }
        // if (Input.GetMouseButtonUp(1))
        // {
        //     SpawnArea(currentXPosition);
        //     Debug.Log("заспавнили");
        //     pathFinding.FindTiles();
        //     _lastSpawnPosition.z = currentXPosition.z;
        //     currentXPosition.z += 5;
        // }
        if (_time >= checkInterval)
        {
            ReturnToPool();
            _time = 0f;
        }
    }

    private void SpawnArea(Vector3Int areaPosition)
    {
        AreaAbstract area = GetArea();
        if (area != null) {
            
            Vector3 newPosition = area.transform.position;
            newPosition.z = areaPosition.z;
            area.transform.position = newPosition;
            area.EnableArea(true);
        }
    }

    private AreaAbstract GetArea()
    {
        AreaTypes type = GetRandomTypeArea();//выбираем тип 
        AreaAbstract area = GetAreaFromPool(type);//выбирае нужную территорию 
        if (area != null)
        {
            // Debug.LogError("так быть не должно");
            return area;
        }

        return AreaToSpawn(type);//справним в случае если не нашли нужную территорию
    }

    private AreaAbstract GetAreaFromPool(AreaTypes type)
    {
        foreach (AreaAbstract area in _areaAbstracts) {//находим нужную 
            if (!area.gameObject.activeSelf && area.Type == type && area != null)
                return area;
        }
        return null;
    }

    private AreaAbstract AreaToSpawn(AreaTypes type)
    {
        if (_maps.TryGetValue(type, out int index)) {
            AreaAbstract area = Instantiate(areaArray[index]);//спавним, добавляем и возвращаем
            _areaAbstracts.Add(area);
            // Debug.Log("так быть должно");
            return area;
        }
        Debug.Log("ватафак так быть не долно вообще");
        return null;
    }

    private AreaTypes GetRandomTypeArea()
    {
        if (_typesMap.TryGetValue(Random.Range(0, 3), out AreaTypes type)) return type; // пока так (значения _max) в Range
        return AreaTypes.Slowdown;
    }

    private void ReturnToPool()
    {
        foreach (var area in _areaAbstracts) {
            if (area.transform.position.z <= pointReturnToPool.transform.position.z && area.gameObject.activeSelf) {
                // Debug.Log("вернули в пул");
                area.EnableArea(false);
            }
        }
    }
}