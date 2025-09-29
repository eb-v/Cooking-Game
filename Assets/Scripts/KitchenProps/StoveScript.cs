using UnityEngine;

public class StoveScript : MonoBehaviour, IPrepStation
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
            Debug.LogError("No Skill Check Logic Scriptable Object assigned to StoveScript on " + gameObject.name);
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
        Debug.Log(currentPlacedObject.name + " has been cooked on the stove!");
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
            Debug.Log("Stove is now being used by " + player.name);
        }


        if (isBeingUsed && currentUser != player)
        {
            Debug.Log("Stove is currently being used by another player.");
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
            Debug.Log("No object on stove to cook!");
        }

    }

    private void StopSkillCheck()
    {
        if (skillCheckActive)
        {
            skillCheckActive = false;
            currentUser = null;

            Debug.Log("Skill check stopped on stove.");
        }
    }
}
