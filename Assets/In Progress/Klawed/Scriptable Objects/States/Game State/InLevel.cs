using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "InLevel", menuName = "Scriptable Objects/States/Game/InLevel")]
public class InLevel : BaseStateSO
{
    private NpcManager npcManager;
    private TrainManager trainManager;
    
    public override void Enter()
    {
        base.Enter();
        GameObject rootManager = GameObject.Find("Managers");
        if (rootManager == null)
        {
            rootManager = new GameObject("Managers");
        }
        
        npcManager = GameObject.FindFirstObjectByType<NpcManager>();
        if (npcManager == null)
        {
            npcManager = new GameObject("NpcManager").AddComponent<NpcManager>();
            npcManager.transform.parent = rootManager.transform;
        }
        
        trainManager = GameObject.FindFirstObjectByType<TrainManager>();
        if (trainManager == null)
        {
            trainManager = new GameObject("TrainManager").AddComponent<TrainManager>();
            trainManager.transform.parent = rootManager.transform;
        }
        
        // Only initialize train if in Level 2
        if (SceneManager.GetActiveScene().name == "Level 2")
        {
            trainManager.Initialize();
        }
    }
    
    public override void Execute()
    {
        npcManager.RunUpdateLogic();
        trainManager.RunUpdateLogic();
    }
    
    public override void Exit()
    {
        base.Exit();
    }
}