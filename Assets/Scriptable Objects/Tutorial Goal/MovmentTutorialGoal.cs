using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class MovmentTutorialGoal : TutorialGoal
{
    [SerializeField] private List<GameObject> movementMarkers;
    [SerializeField] private int currentMarkerIndex = 0;

    private void OnEnable()
    {
        GenericEvent<TutorialGoalCompleted>.GetEvent("MovementMarkerReached").AddListener(OnMovementMarkerReached);
    }

    private void OnDisable()
    {
        GenericEvent<TutorialGoalCompleted>.GetEvent("MovementMarkerReached").RemoveListener(OnMovementMarkerReached);
    }


    private void OnMovementMarkerReached()
    {
        currentMarkerIndex++;
        if (currentMarkerIndex >= movementMarkers.Count)
        {
           CompleteGoal();
        }
    }
}
