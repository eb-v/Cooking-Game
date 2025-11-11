using System.Collections;
using UnityEngine;

public class Cube : MonoBehaviour, IFlamable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Ignite();
        StartCoroutine(StopFireAfterDelay(3f));
    }

    // Update is called once per frame
    private IEnumerator StopFireAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Extinguish();
    }

    public void Ignite()
    {
        FireSystem.IgniteObject(gameObject);
    }

    public void Extinguish()
    {
        FireSystem.ExtinguishObject(gameObject);
    }
}
