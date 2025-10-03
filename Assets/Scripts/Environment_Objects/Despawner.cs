using UnityEngine;

public class Despawner : MonoBehaviour
{
    [SerializeField] private float despawnTime = 3f;
    private float timer;

    private void OnEnable()
    {
        PlayEffects();
    }

    private void OnDisable()
    {
        ClearEffects();
        timer = 0f;
    }



    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= despawnTime)
        {
            Despawn();
        }
    }

    private void Despawn()
    {
        ObjectPoolManager.ReturnObjectToPool(gameObject);
    }

    private void PlayEffects()
    {
        foreach (var ps in GetComponentsInChildren<ParticleSystem>())
        {
            ps.Play();
        }
    }


    private void ClearEffects()
    {
        foreach (var ps in GetComponentsInChildren<ParticleSystem>())
        {
            ps.Clear();
        }
    }

}
