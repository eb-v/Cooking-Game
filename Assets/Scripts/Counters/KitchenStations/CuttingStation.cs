//using System.Runtime.CompilerServices;
//using UnityEngine;

//public class CuttingStation : BaseStation
//{
//    [Header("Remove Pop Force")]
//    [SerializeField] private float verticalForceMultiplier = 8f;
//    [SerializeField] private float horizontalForceMultiplier = 8f;

//    [Header("Skill Check Logic")]
//    [SerializeField] private SkillCheckBaseLogic skillCheckLogicBase;
//    [SerializeField] private GameObject skillCheckUI;
//    private SkillCheckBaseLogic skillCheckLogicInstance;
//    private bool isCutting = false;


//    [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;

//    private void Awake()
//    {
//        GenericEvent<InteractEvent>.GetEvent(gameObject.name).AddListener(Interact);
//        GenericEvent<RemovePlacedObject>.GetEvent(gameObject.name).AddListener(RemovePlacedKitchenObj);
//        GenericEvent<AlternateInteractInput>.GetEvent(gameObject.name).AddListener(AlternateInteract);
//        GenericEvent<SkillCheckCompleted>.GetEvent(gameObject.name).AddListener(ProduceOutput);
//        GenericEvent<SkillCheckCompleted>.GetEvent(gameObject.name).AddListener(StopCutting);
//        GenericEvent<PlayerStoppedLookingAtInteractable>.GetEvent(gameObject.name).AddListener(OnPlayerLookAway);
//        GenericEvent<InteractableLookedAtChanged>.GetEvent(gameObject.name).AddListener(OnPlayerLookAway);
//    }

//    private void Start()
//    {
//        skillCheckLogicInstance = Instantiate(skillCheckLogicBase);
//        skillCheckLogicInstance.Initialize(gameObject);
//    }



//    private void Update()
//    {
//        if (isCutting)
//        {
//            skillCheckLogicInstance.RunUpdateLogic();
//        }
//    }




//    public override void Interact(GameObject player)
//    {
//        RagdollController ragdollController = player.GetComponent<RagdollController>();

//        // if player is carrying something
//        if (ragdollController.IsHoldingSomething())
//        {
//            // if station is empty
//            if (!HasKitchenObject())
//            {
//                GameObject heldKitchenObj = ragdollController.GetHeldObject();
//                // if that something is an ingredient that can be cut
//                if (HasRecipeWithInput(heldKitchenObj))
//                {
//                    // logic for detaching object from hand


//                    // find out which hand is holding the object

//                    // if left hand is holding something
//                    if (ragdollController.leftHand.GetComponent<GrabDetection>().isGrabbing)
//                    {
//                        GameObject grabbedObj = ragdollController.leftHand.GetComponent<GrabDetection>().grabbedObj;
//                        // is the object held by the left hand the same as the one we are checking?
//                        if (grabbedObj == heldKitchenObj)
//                        {
//                            // remove fixed joint from left hand and place object on counter
//                            GameObject leftArm = ragdollController.leftHand.transform.parent.gameObject;
//                            Destroy(leftArm.GetComponent<FixedJoint>());
//                            ragdollController.leftHand.GetComponent<GrabDetection>().isGrabbing = false;
//                            ragdollController.leftHand.GetComponent<GrabDetection>().grabbedObj = null;
//                        }
//                    }

//                    if (ragdollController.rightHand.GetComponent<GrabDetection>().isGrabbing)
//                    {
//                        GameObject grabbedObj = ragdollController.rightHand.GetComponent<GrabDetection>().grabbedObj;
//                        // is the object held by the right hand the same as the one we are checking?
//                        if (grabbedObj == heldKitchenObj)
//                        {
//                            // remove fixed joint from right hand and place object on counter
//                            GameObject rightArm = ragdollController.rightHand.transform.parent.gameObject;
//                            Destroy(rightArm.GetComponent<FixedJoint>());
//                            ragdollController.rightHand.GetComponent<GrabDetection>().isGrabbing = false;
//                            ragdollController.rightHand.GetComponent<GrabDetection>().grabbedObj = null;
//                        }
//                    }

//                    // logic for placing object on counter

//                    Collider stationCollider = player.GetComponent<Env_Interaction>().currentlyLookingAt.GetComponent<Collider>();

//                    float stationYOffset = stationCollider.bounds.extents.y;
//                    Vector3 placePos = stationCollider.bounds.center;
//                    placePos.y += stationYOffset;

//                    Collider heldObjCollider = heldKitchenObj.GetComponent<Collider>();
//                    heldKitchenObj.transform.position = placePos;
//                    heldKitchenObj.transform.rotation = Quaternion.identity;
//                    heldKitchenObj.GetComponent<Rigidbody>().isKinematic = true;
//                    SetStationObject(heldKitchenObj);
//                }
//                else
//                {
//                    Debug.Log("Player not holding an ingredient that can be cut");
//                }

//            }
//            else
//            {
//                Debug.Log("CuttingStation already has an object placed on it.");
//            }
//        }
//        else
//        {
//            Debug.Log("Player not carrying anything");
//        }

//    }

//    // logic for alternate interact (cutting)
//    public void AlternateInteract(GameObject player)
//    {
//        if (GetRegisteredPlayer() == null)
//        {
//            RegisterPlayer(player);
//        }
//        else if (GetRegisteredPlayer() != player)
//        {
//            Debug.Log("Another player is already using the CuttingStation");
//            return;
//        }


//        if (HasKitchenObject())
//        {
//            // if the cutting/skillCheck process has not been started yet, start it
//            if (!isCutting)
//            {
//                StartCutting();
//            }
//            else
//            {
//                skillCheckLogicInstance.DoAttemptSkillCheckLogic();
//            }

//        }
//        else
//        {
//            Debug.Log("CuttingStation has no object to cut");
//        }


//    }


//    public override void RemovePlacedKitchenObj(GameObject player)
//    {
//        if (HasKitchenObject())
//        {
//            GameObject kitchenObj = GetKitchenObject();
//            Rigidbody kitchenObjRb = kitchenObj.GetComponent<Rigidbody>();
//            kitchenObjRb.isKinematic = false;

//            Vector3 playerPos = player.GetComponent<RagdollController>().centerOfMass.position;

//            Vector3 popDirection = (playerPos - transform.position).normalized;
//            popDirection.y = 0f;

//            kitchenObjRb.AddForce(Vector3.up * verticalForceMultiplier, ForceMode.Impulse);
//            kitchenObjRb.AddForce(popDirection * horizontalForceMultiplier, ForceMode.Impulse);

//            StopCutting();
//            ClearStationObject();
//        }
//        else
//        {
//            Debug.Log("CuttingStation has no object to remove");
//        }
//    }

//    private bool HasRecipeWithInput(GameObject kitchenObject)
//    {
//        CuttingRecipeSO cuttingRecipe = GetCuttingRecipeWithInput(kitchenObject);
//        return cuttingRecipe != null;
//    }

//    private CuttingRecipeSO GetCuttingRecipeWithInput(GameObject kitchenObject)
//    {
//        string kitchenObjectName = kitchenObject.name.Replace("(Clone)", "").Trim();

//        foreach (CuttingRecipeSO cuttingRecipeSO in cuttingRecipeSOArray)
//        {
//            if (cuttingRecipeSO.input.name == kitchenObjectName)
//            {
//                return cuttingRecipeSO;
//            }
//        }
//        return null;
//    }

//    private void StartCutting()
//    {
//        isCutting = true;
//        skillCheckUI.SetActive(true);
//    }

//    private void StopCutting()
//    {
//        if (isCutting)
//        {
//            isCutting = false;
//            skillCheckUI.SetActive(false);
//            skillCheckLogicInstance.ResetValues();
//            ClearPlayer();
//        }
//    }

//    private void ProduceOutput()
//    {
//        GameObject kitchenObj = GetKitchenObject();
//        Vector3 position = kitchenObj.transform.position;
//        CuttingRecipeSO cuttingRecipe = GetCuttingRecipeWithInput(kitchenObj);

//        Destroy(kitchenObj);
//        ClearStationObject();


//        GameObject output = ObjectPoolManager.SpawnObject(cuttingRecipe.output, position, Quaternion.identity);
//        Rigidbody rb = output.GetComponent<Rigidbody>();

//        rb.isKinematic = false;
//        rb.AddForce(Vector3.up * verticalForceMultiplier, ForceMode.Impulse);
//    }

//    private void OnPlayerLookAway(GameObject player)
//    {
//        if (isCutting)
//        {
//            if (player == GetRegisteredPlayer())
//            {
//                StopCutting();
//                ClearPlayer();
//            }
//        }
//    }

//}
