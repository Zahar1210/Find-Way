using System.Collections;
using UnityEngine;

public class CarSlowSpeedModifier: MonoBehaviour
{
    public IEnumerator ChangeSpeed(CarAbstract car, float targetSpeed, float duration)
    {
        float initialSpeed = car.Speed;
        float elapsedTime = 0f;
        while (elapsedTime < duration) {
            car.Speed = Mathf.Lerp(initialSpeed, targetSpeed, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        car.Speed = targetSpeed;
    }
}
