using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class AreaPooler : MonoBehaviour
{
    [SerializeField] private float spawnInterval;
    [SerializeField] private float checkInterval;
    [SerializeField] private Transform pointReturnToPool;
    [SerializeField] private PathFinding pathFinding;
    [SerializeField] private Transform pointCheckInterval;
    [SerializeField] private AreaAbstract[] areaArray;

    private List<AreaAbstract> _areaAbstracts = new();
    private AreaAbstract queueArea;
    private Vector3Int _lastSpawnPosition;
    private float _time;
    private void Start()
    {
        foreach (var area in FindObjectsOfType<AreaAbstract>()) { _areaAbstracts.Add(area); }
        _lastSpawnPosition = Vector3Int.RoundToInt(transform.position);
        queueArea = areaArray[Random.Range(0, 10)];
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
        if (currentZPosition.z - _lastSpawnPosition.z >= spawnInterval)
        {
            _lastSpawnPosition.z = currentZPosition.z;
            SpawnArea(currentZPosition);
            pathFinding.FindTiles(); //обновляем данные для поиска пути :)
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
        int index = GetIndex();
        AreaAbstract area = GetAreaFromPool(index);
        if (area != null) { return area; }
        return AreaToSpawn(index);
    }
    private AreaAbstract GetAreaFromPool(int index)
    {
        foreach (AreaAbstract area in _areaAbstracts) {
            if (!area.gameObject.activeSelf && area.Index == index && area != null)
                return area;
        }
        return null;
    }
    private AreaAbstract AreaToSpawn(int index)
    {
        AreaAbstract area = Instantiate(areaArray[index]);
        _areaAbstracts.Add(area);
        area.EnableArea(false);
        return area;
    }
    private void ReturnToPool()
    {
        foreach (var area in _areaAbstracts) {
            if (area.transform.position.z <= pointReturnToPool.transform.position.z && area.gameObject.activeSelf)
                area.EnableArea(false);
        }
    }
    private int GetIndex()
    {
        float totalChance = Random.Range(0, queueArea.Areas.Sum(area => area.Chance));
        for (int i = 0; i < queueArea.Areas.Length; i++) {
            if (totalChance > queueArea.Areas[i].Chance) 
                totalChance -= queueArea.Areas[i].Chance;
            else 
               return queueArea.Areas[i].Index;
        }
        return 0;
    }
    private void SetSpawnInterval(AreaAbstract beforeArea)
    {
        spawnInterval = (beforeArea.AreaLength + queueArea.AreaLength) / 2f;
        if (spawnInterval % 1 != 0) { spawnInterval = beforeArea.AreaLength; }
    }
}