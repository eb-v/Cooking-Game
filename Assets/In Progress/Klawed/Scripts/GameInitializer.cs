using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    [SerializeField] private GameObject _gameStateManager;


    void Start()
    {
        Instantiate(_gameStateManager);
    }

    
}
