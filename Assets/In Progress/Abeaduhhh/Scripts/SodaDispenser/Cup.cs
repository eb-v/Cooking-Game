using UnityEngine;

public class Cup : MonoBehaviour, IGrabable, IInteractable, IAltInteractable {
    public bool isFilled = false;
    public string drinkType = "";

    public bool isGrabbed { get; set; }
    [field: SerializeField] public GrabData grabData { get; set; }
    [field: SerializeField] public GameObject currentPlayer { get; set; }

    [SerializeField] public GameObject liquidObject;

    public void FillCup(string drinkName, Color drinkColor) {
        if (isFilled) return;

        drinkType = drinkName;
        isFilled = true;

        if (liquidObject != null)
            liquidObject.SetActive(true);
            Renderer rend = liquidObject.GetComponent<Renderer>();
            if (rend != null) {
                rend.material.color = drinkColor;
            }
        Debug.Log("Cup filled with: " + drinkName);
    }

    public void GrabObject(GameObject player) {
        if (isGrabbed) return;

        // Get the root cup object in case the player grabbed a child
        GameObject grabbedObj = GetRootCupObject();
        if (grabbedObj == null) {
            Debug.LogWarning("No valid cup object found to grab!");
            return;
        }

        PhysicsTransform physicsTransform = grabbedObj.GetComponent<PhysicsTransform>();
        if (physicsTransform == null) {
            Debug.LogWarning("PhysicsTransform not found on cup object!");
            return;
        }

        GrabSystem.GrabObject(player, physicsTransform.physicsTransform.gameObject, grabData);
        
        Debug.Log("The player grabbed: " + grabbedObj.name);

        isGrabbed = true;
        currentPlayer = player;

        Debug.Log("Cup grabbed by player");
    }

    public void ReleaseObject(GameObject player) {
        GrabSystem.ReleaseObject(player);
        isGrabbed = false;
        currentPlayer = null;

        Debug.Log("Cup released by player");
    }

    public GameObject GetGameObject() => gameObject;

    public void OnInteract(GameObject player) => GrabObject(player);

    public void OnAltInteract(GameObject player) => ReleaseObject(player);

    // Helper to get the root Cup object in case a child is grabbed
    private GameObject GetRootCupObject() {
        Cup cupComponent = GetComponent<Cup>()
                          ?? GetComponentInChildren<Cup>()
                          ?? GetComponentInParent<Cup>();

        return cupComponent?.gameObject;
    }
}
