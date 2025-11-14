using UnityEngine;
using UnityEngine.SceneManagement;
[CreateAssetMenu(fileName = "New Game State", menuName = "Scriptable Object/States/Game State")]
public class GameStateSO : ScriptableObject
{
    [SerializeField] private SceneField scene;
    [SerializeField] private GameObject _neededGameObjects;

    public virtual void Enter()
    {
        SceneManager.LoadScene(scene);
    }

    public virtual void Exit()
    {

    }

}
