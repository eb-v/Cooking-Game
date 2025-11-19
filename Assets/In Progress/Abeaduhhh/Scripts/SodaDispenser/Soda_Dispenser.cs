using UnityEngine;
using UnityEngine.UI;

public class SodaDispenser : MonoBehaviour, IInteractable, IAltInteractable {
    [Header("Snap Point for the Cup")]
    [SerializeField] private Transform _cupSnapPoint;

    [Header("Dispense Settings")]
    [SerializeField] private float dispenseTime = 2f;
    [SerializeField] private Image progressBar;
    [SerializeField] public GameObject CompleteText;

    [SerializeField] public GameObject SodaMenu;


    [SerializeField] private Color progressColor = Color.green;
    [SerializeField] private Color completeColor = Color.yellow;

    private Cup _currentCup;
    private bool hasCup => _currentCup != null;

    private bool isDispensing = false;
    private float dispenseTimer = 0f;
    private string drinkToDispense;
    private Color drinkColor;

    private MenuItem selectedDrink;

    private void Start() {
        GenericEvent<SodaSelectedEvent>.GetEvent("SodaDispenser")
            .AddListener(OnDrinkSelected);
    }

    private void OnDrinkSelected(MenuItem drink) {
        selectedDrink = drink;
        Debug.Log("Dispenser received drink selection: " + drink.name);

        if (hasCup) {
            Debug.Log("Cup present. Dispensing now...");

            Dispense(drink.name, Color.red);
        } else {
            Debug.Log("No cup. Waiting for cup placement...");
        }
    }

    private void Update() {
        if (isDispensing && _currentCup != null) {
            dispenseTimer += Time.deltaTime;
            float progress = Mathf.Clamp01(dispenseTimer / dispenseTime);

            if (progressBar != null) {
                progressBar.fillAmount = progress;
                progressBar.color = Color.Lerp(progressColor, completeColor, progress);
            }

            if (dispenseTimer >= dispenseTime) {
                FinishDispensing();
            }
        }
    }

    public void OnInteract(GameObject player) {
        GrabScript gs = player.GetComponent<GrabScript>();
        if (gs == null) return;

        if (gs.isGrabbing) {
            GameObject grabbedObject = gs.grabbedObject.GetGameObject();
            Cup drink = grabbedObject.GetComponent<Cup>();

            if (drink == null || drink.isFilled || hasCup) return;

            PlaceCupOnDispenser(drink);

            gs.grabbedObject.ReleaseObject(player);
            gs.grabbedObject = null;
        }
    }

    public void OnAltInteract(GameObject player) {
        if (!hasCup) return;

        GameObject cupObj = RemoveCupFromDispenser();
        IGrabable grabable = cupObj.GetComponent<IGrabable>();
        if (grabable != null)
            grabable.GrabObject(player);
    }

    private void PlaceCupOnDispenser(Cup cup) {
        if (cup.isGrabbed && cup.currentPlayer != null)
            cup.ReleaseObject(cup.currentPlayer);

        _currentCup = cup;

        Rigidbody rb = cup.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        cup.transform.SetParent(_cupSnapPoint);
        cup.transform.position = _cupSnapPoint.position;
        cup.transform.localRotation = Quaternion.Euler(-90f, 90f, 0f); // cup upright and rotated Y=90

        Debug.Log("Cup placed. Waiting for drink selection...");

        if(SodaMenu != null) {
            SodaMenu.SetActive(true);
        }
    }

    private GameObject RemoveCupFromDispenser() {
        GameObject cupObj = _currentCup.gameObject;

        Rigidbody rb = cupObj.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = false;

        cupObj.transform.SetParent(null);
        _currentCup = null;

        if (progressBar != null) progressBar.fillAmount = 0f;
        if (SodaMenu != null) {
            SodaMenu.SetActive(false);
        }
        if (CompleteText != null) CompleteText.SetActive(false);
        return cupObj;

    }


    public void Dispense(string drinkName, Color color) {
        if (!hasCup || isDispensing) return;

        drinkToDispense = drinkName;
        drinkColor = color;
        isDispensing = true;
        dispenseTimer = 0f;

        if (progressBar != null) {
            progressBar.fillAmount = 0f;
            progressBar.color = progressColor;
        }

        Debug.Log("Started dispensing " + drinkName);
    }

    private void FinishDispensing() {
        _currentCup.FillCup(drinkToDispense, drinkColor);

        isDispensing = false;
        dispenseTimer = 0f;

        if (progressBar != null) progressBar.fillAmount = 1f;
        if(CompleteText != null) CompleteText.SetActive(true);
        Debug.Log("Cup filled with " + drinkToDispense);
    }
}
