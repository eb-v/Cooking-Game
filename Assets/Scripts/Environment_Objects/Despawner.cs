using System.Collections;
using UnityEngine;

public class Despawner : MonoBehaviour
{

    private void OnEnable()
    {
        foreach(var ps in GetComponentsInChildren<ParticleSystem>())
        {
            ps.time = 0f;
            ps.Play();
        }


        float duration = GetLongestParticleDuration();
        StartCoroutine(DespawnAfterDelay(duration));
    }


    private float GetLongestParticleDuration()
    {
        float max = 0f;
        foreach (var ps in GetComponentsInChildren<ParticleSystem>())
        {
            var main = ps.main;

            float time = main.duration + main.startLifetime.constantMax;
            if (time > max)
            {
                max = time;
            }
        }
        return max;
    }

    IEnumerator DespawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ObjectPoolManager.ReturnObjectToPool(gameObject);
    }
    

}
