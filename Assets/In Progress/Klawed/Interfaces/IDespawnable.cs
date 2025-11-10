using System.Collections;
using UnityEngine;

public interface IDespawnable
{
    void Despawn();
    void ResetValues();

    IEnumerator DespawnCoroutine(float delay);
}
