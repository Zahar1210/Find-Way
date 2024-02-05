using UnityEngine;

public class TrainSystem : MonoBehaviour
{
    [SerializeField] private TrainPool trainPool;
    
    public void TryTrain(AreaAbstract area)
    {
        if (area.Type != AreaTypes.Train)
            return;
        trainPool.SpawnTrain(area);
    }

    public void ResetTrain(AreaAbstract area)
    {
        
    }
}
