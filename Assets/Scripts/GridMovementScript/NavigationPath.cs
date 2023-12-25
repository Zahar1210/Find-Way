using System.Collections;
using UnityEngine;

public class NavigationPath : MonoBehaviour
{
    [SerializeField] private Light light;
    [SerializeField] private Transform player;
    [SerializeField] private float speed;

    public void PathNavigation(Surface[] path)
    {
        gameObject.SetActive(false);
        transform.position = player.transform.position;
        gameObject.SetActive(true);
        StartCoroutine(LightIntensity(7, 2));
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
        float startIntensity = light.intensity;
        float elapsedTime = 0.0f;
        while (elapsedTime < duration) {
            yield return null;
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            light.intensity = Mathf.Lerp(startIntensity, targetLight, t);
        }
        light.intensity = targetLight;
    }
}