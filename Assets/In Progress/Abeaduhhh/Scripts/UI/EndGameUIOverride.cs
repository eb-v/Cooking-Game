using UnityEngine;
using System.Collections;

public abstract class EndGameUIOverride : MonoBehaviour {
    public virtual void ShowEndGameUI() { }

    public virtual IEnumerator AnimateScore(int finalScore) { yield break; }
    public virtual IEnumerator ShowStars(int stars) { yield break; }
}
