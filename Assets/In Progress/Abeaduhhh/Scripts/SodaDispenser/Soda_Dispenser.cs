using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Interactable))]
public class SodaDispenser : MonoBehaviour
{
    [Header("Snap Point for the Cup")]
    [SerializeField] private Transform _cupSnapPoint;

    [Header("Dispense Settings")]
    [SerializeField] private float dispenseTime = 2f;
    [SerializeField] private Image progressBar;
    [SerializeField] public GameObject CompleteText;
    [SerializeField] public GameObject SodaMenu;

    [SerializeField] private Color progressColor = Color.green;
    [SerializeField] private Color completeColor = Color.yellow;

    [Header("Shake Settings")]
    [SerializeField] private Transform shakeTarget;
    [SerializeField] private float shakeMagnitude = 0.03f;
    [SerializeField] private float shakeFrequency = 25f;

    private Cup _currentCup;
    private bool hasCup => _currentCup != null;

    private bool isDispensing = false;
    private float dispenseTimer = 0f;
    private string drinkToDispense;
    private Color drinkColor;

    private MenuItem selectedDrink;

    // shake state
    private Vector3 baseShakeLocalPos;
    private bool isShaking = false;
    private float shakeTime = 0f;

    private void Awake()
    {
        if (shakeTarget == null)
            shakeTarget = transform;

        baseShakeLocalPos = shakeTarget.localPosition;
    }

    private void OnEnable()
    {
        GenericEvent<OnInteractableInteracted>.GetEvent(gameObject.GetInstanceID().ToString()).AddListener(OnInteract);
        GenericEvent<OnInteractableAltInteracted>.GetEvent(gameObject.GetInstanceID().ToString()).AddListener(OnAltInteract);
    }

    private void OnDisable()
    {
        GenericEvent<OnInteractableInteracted>.GetEvent(gameObject.GetInstanceID().ToString()).RemoveListener(OnInteract);
        GenericEvent<OnInteractableAltInteracted>.GetEvent(gameObject.GetInstanceID().ToString()).RemoveListener(OnAltInteract);
    }

    private void OnDrinkSelected(MenuItem drink) {
        selectedDrink = drink;
        Debug.Log("Dispenser received drink: " + drink.drinkName);

        if (hasCup) {
            Debug.Log("Cup present. Dispensing...");

            drinkColor = drink.drinkColor;
            Dispense(drink.drinkName, drinkColor);
        } else {
            Debug.Log("No cup yet...");
        }
    }

    private void Update()
    {
        if (isDispensing && _currentCup != null)
        {
            dispenseTimer += Time.deltaTime;
            float progress = Mathf.Clamp01(dispenseTimer / dispenseTime);

            if (progressBar != null)
            {
                progressBar.fillAmount = progress;
                progressBar.color = Color.Lerp(progressColor, completeColor, progress);
            }

            if (dispenseTimer >= dispenseTime)
            {
                FinishDispensing();
            }
        }

        if (isShaking)
            UpdateShake();
    }

    public void OnInteract(GameObject player)
{
    GrabScript gs = player.GetComponent<GrabScript>();
    if (gs == null) return;

    if (gs.IsGrabbing)
    {
        Grabable grabbed = gs.grabbedObject;

            GameObject grabbedObject = grabbed.gameObject;
        Cup drink = grabbedObject.GetComponent<Cup>();

        if (drink == null || drink.isFilled || hasCup) return;

        PlaceCupOnDispenser(drink);

        grabbed.Release();
    }

    if (SodaMenu != null)
        SodaMenu.SetActive(true);

        Soda_menu menu = SodaMenu.GetComponent<Soda_menu>();
        menu.ListenToPlayer(player.name);
        GenericEvent<InteractEvent>.GetEvent("SodaDispenser").Invoke(player);
}


    public void OnAltInteract(GameObject player)
    {
        if (!hasCup) return;

        GameObject cupObj = RemoveCupFromDispenser(player);
        if (cupObj == null) return;
    }

    private void PlaceCupOnDispenser(Cup cup)
    {
        _currentCup = cup;

        Rigidbody rb = cup.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        cup.transform.SetParent(_cupSnapPoint);
        cup.transform.position = _cupSnapPoint.position;
        cup.transform.rotation = _cupSnapPoint.rotation;

        Debug.Log("Cup placed. Waiting for drink selection...");

        GenericEvent<SodaSelectedEvent>.GetEvent("SodaDispenser").AddListener(OnDrinkSelected);
    }

    private GameObject RemoveCupFromDispenser(GameObject player)
    {
        if (_currentCup == null) return null;

        GameObject cupObj = _currentCup.gameObject;

        Rigidbody rb = cupObj.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = false;

        cupObj.transform.SetParent(null);
        cupObj.transform.rotation = Quaternion.identity;

        _currentCup = null;

        isDispensing = false;
        StopShake();

        if (progressBar != null) progressBar.fillAmount = 0f;
        if (SodaMenu != null) SodaMenu.SetActive(false);
        if (CompleteText != null) CompleteText.SetActive(false);

        GenericEvent<SodaSelectedEvent>.GetEvent("SodaDispenser").RemoveListener(OnDrinkSelected);

        return cupObj;
    }

    public void Dispense(string drinkName, Color color)
    {
        if (!hasCup || isDispensing) return;

        drinkToDispense = drinkName;
        drinkColor = color;
        isDispensing = true;
        dispenseTimer = 0f;

        if (progressBar != null)
        {
            progressBar.fillAmount = 0f;
            progressBar.color = progressColor;
        }

        StartShake();
        Debug.Log("Started dispensing " + drinkName);
    }

    private void FinishDispensing() {
        if (_currentCup != null && selectedDrink != null) {
            GameObject drinkPrefab = selectedDrink.Prefab;

            if (drinkPrefab != null) {
                GameObject spawnedDrink = Instantiate(drinkPrefab, _currentCup.transform);
                spawnedDrink.transform.localPosition = Vector3.zero;
                spawnedDrink.transform.localRotation = Quaternion.identity;
                spawnedDrink.transform.localScale = Vector3.one;
            }
            _currentCup.FillCup(selectedDrink.drinkName, selectedDrink.drinkColor);
        }

        isDispensing = false;
        dispenseTimer = 0f;

        StopShake();

        if (progressBar != null) progressBar.fillAmount = 1f;
        if (CompleteText != null) CompleteText.SetActive(true);

        Debug.Log("Cup filled with " + drinkToDispense);
    }


    private void StartShake()
    {
        if (shakeTarget == null) return;

        baseShakeLocalPos = shakeTarget.localPosition;
        shakeTime = 0f;
        isShaking = true;
    }

    private void StopShake()
    {
        if (shakeTarget == null) return;

        isShaking = false;
        shakeTarget.localPosition = baseShakeLocalPos;
    }

    private void UpdateShake()
    {
        if (shakeTarget == null) return;

        shakeTime += Time.deltaTime * shakeFrequency;

        float offsetX = (Mathf.PerlinNoise(shakeTime, 0f) - 0.5f) * 2f * shakeMagnitude;
        float offsetY = (Mathf.PerlinNoise(0f, shakeTime) - 0.5f) * 2f * shakeMagnitude;

        shakeTarget.localPosition = baseShakeLocalPos + new Vector3(offsetX, offsetY, 0f);
    }
}
