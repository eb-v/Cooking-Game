using UnityEngine;

public class CuttingTableScript : MonoBehaviour, IPrepStation
{
    [SerializeField] private SkillCheckBaseLogic skillCheckLogicSO;
    private SkillCheckBaseLogic skillCheckLogicInstance;
    private bool skillCheckActive = false;
    public GameObject currentUser { get; set; }
    public bool isBeingUsed { get; set; }

    public GameObject currentPlacedObject { get; set; }
    public bool containsObject { get; set; }

    public bool ingredientPrepared;


    private void Start()
    {
        GenericEvent<SkillCheckInput>.GetEvent(gameObject.name).AddListener(AttemptSkillCheck);
        GenericEvent<SkillCheckCompleted>.GetEvent(gameObject.name).AddListener(PrepIngredient);
        GenericEvent<SkillCheckCompleted>.GetEvent(gameObject.name).AddListener(StopSkillCheck);
        GenericEvent<ObjectRemovedFromKitchenStation>.GetEvent(gameObject.name).AddListener(StopSkillCheck);
        GenericEvent<PlayerStoppedLookingAtKitchenStation>.GetEvent(gameObject.name).AddListener(StopSkillCheck);

        if (skillCheckLogicSO != null)
        {
            skillCheckLogicInstance = Instantiate(skillCheckLogicSO);
            skillCheckLogicInstance.Initialize(gameObject);
        }
        else
        {
            Debug.LogError("No Skill Check Logic Scriptable Object assigned on " + gameObject.name);
        }

    }




    void Update()
    {
        isBeingUsed = skillCheckActive;

        if (skillCheckActive)
        {
            skillCheckLogicInstance.RunUpdateLogic();
        }

    }

    // change ingredient prefab to prepared version
    public void PrepIngredient()
    {



        currentPlacedObject.GetComponent<IIngredient>().isPrepared = true;
    }

    private void AttemptSkillCheck(GameObject player)
    {
        if (currentPlacedObject != null && currentPlacedObject.GetComponent<IIngredient>().isPrepared)
        {
            Debug.Log("Ingredient is already prepared!");
            return;
        }

        if (currentUser == null && currentPlacedObject != null)
        {
            currentUser = player;
            Debug.Log(gameObject.name + " is now being used by " + player.name);
        }


        if (isBeingUsed && currentUser != player)
        {
            Debug.Log(gameObject.name + " is currently being used by another player.");
            return;
        }


        // if skill check is not active when player attempts skill check, activate it

        if (currentPlacedObject != null)
        {
            if (!skillCheckActive)
            {
                skillCheckActive = true;
            }
            else
            {
                // place holder for skill check success detection
                skillCheckLogicInstance.DoAttemptSkillCheckLogic();
            }
        }
        else
        {
            Debug.Log("No object on " + gameObject.name);
        }

    }

    private void StopSkillCheck()
    {
        if (skillCheckActive)
        {
            skillCheckActive = false;
            currentUser = null;

            Debug.Log("Skill check stopped on " + gameObject.name + ".");
        }
    }


}
