using UnityEngine;

public class GameState : BaseStateSO<GameState>
{
    public virtual void PerformSceneTransition() { }

    public virtual void PlaceHolder() { }

    // add more game state functions here
}
