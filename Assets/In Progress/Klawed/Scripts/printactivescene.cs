using UnityEngine;

public class printactivescene : MonoBehaviour
{
    void Update()
    {
        Debug.Log("Active Scene: " + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
