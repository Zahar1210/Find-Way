using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class NavigationPath : MonoBehaviour
{
    [SerializeField] private Light Light;
    [SerializeField] private Transform player;
    [SerializeField] private float speed;

    public void PathNavigation(Surface[] path)
    {
        gameObject.SetActive(false);
        transform.position = player.transform.position;
        gameObject.SetActive(true);
        Light.intensity = 0;
        StartCoroutine(LightIntensity(5, 1));
        StartCoroutine(MoveAlongPath(path));
    }
    private IEnumerator MoveAlongPath(Surface[] path)
    {
        foreach (var point in path) {
            Vector3 targetPos = point.tile.Pos + point.Dir;
            while (transform.position != targetPos) {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.fixedDeltaTime);
                yield return null;
            }
        }
        StartCoroutine(LightIntensity(0, 2));
    }
    private IEnumerator LightIntensity(float targetLight, float duration)
    {
        float startIntensity = Light.intensity;
        float elapsedTime = 0.0f;
        while (elapsedTime < duration) {
            yield return null;
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            Light.intensity = Mathf.Lerp(startIntensity, targetLight, t);
        }
        Light.intensity = targetLight;
    }
}