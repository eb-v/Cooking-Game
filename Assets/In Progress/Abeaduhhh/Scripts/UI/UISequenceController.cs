using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UISequenceController : MonoBehaviour {
    [System.Serializable]
    public class UIGroup {
        public string name;
        public SpringAPI spring;
        public float delayBeforeNext = 0.3f;
    }

    [Header("UI Groups In Sequence")]
    public List<UIGroup> uiGroups = new();

    [Header("Settings")]
    public bool playOnStart = true;
    public bool reverseOnEnd = true;
    public float reverseDelayBetween = 0.2f;

    private void Start() {
        if (playOnStart)
            StartCoroutine(PlaySequence());
    }

    public IEnumerator PlaySequence() {
        foreach (var group in uiGroups) {
            if (group.spring != null) {
                group.spring.gameObject.SetActive(true);
                group.spring.SetGoalValue(1f);
                group.spring.NudgeSpringVelocity();
            }

            yield return new WaitForSeconds(group.delayBeforeNext);
        }

        if (reverseOnEnd) {
            yield return new WaitForSeconds(1f); // wait before reversing
            yield return StartCoroutine(ReverseSequence());
        }
    }

    public IEnumerator ReverseSequence() {
        for (int i = uiGroups.Count - 1; i >= 0; i--) {
            var group = uiGroups[i];
            if (group.spring != null) {
                group.spring.SetGoalValue(0f);
                group.spring.NudgeSpringVelocity();
            }
            yield return new WaitForSeconds(reverseDelayBetween);
        }
    }
}
