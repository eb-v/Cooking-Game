using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
// this script holds the data used by the UIAnimManager to run a sequence of UI animations
[RequireComponent(typeof(SpringAPI))]
public class UIGroupScript : MonoBehaviour
{
    public SpringAPI springAPI;
    // delay before starting the next spring in the UI sequence
    public float delay = 0.0f;


    private void Start()
    {
        springAPI = GetComponent<SpringAPI>();
    }
}
