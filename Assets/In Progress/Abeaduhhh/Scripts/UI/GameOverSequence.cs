//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;

//public class GameOverSequence : MonoBehaviour {
//    [Header("UI Springs (in order)")]
//    [SerializeField] private List<SpringAPI> springs = new();

//    [Header("Delay between springs (seconds)")]
//    [SerializeField] private float delayBetween = 0.25f;

//    private bool hasPlayed = false;

//    /// <summary>
//    /// Called externally from the Timer script when the game ends.
//    /// </summary>
//    public void PlaySequence() {
//        if (!hasPlayed) {
//            hasPlayed = true;
//            gameObject.SetActive(true); // Ensure the Game Over UI container is visible
//            StartCoroutine(PlaySpringsInOrder());
//        }
//    }

//    private IEnumerator PlaySpringsInOrder() {
//        foreach (var spring in springs) {
//            if (spring != null) {
//                // reset the spring first
//                spring.ResetSpring();
//                spring.SetGoalValue(1f);  // Trigger its motion toward active state
//                spring.NudgeSpringVelocity(); // optional bounce start
//            }
//            yield return new WaitForSeconds(delayBetween);
//        }
//    }
//}
