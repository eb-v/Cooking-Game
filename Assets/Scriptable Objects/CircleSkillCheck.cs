//using UnityEngine;
//using UnityEngine.UI;

//[CreateAssetMenu(fileName = "CircleSkillCheck", menuName = "Scriptable Objects/Kitchen Props/Skill Checks/Circle Skill Check")]
//public class CircleSkillCheck : SkillCheckBaseLogic
//{
//    private RectTransform dial;
//    private RectTransform pointer;
//    private RectTransform successZone;
//    private Image successZoneImage;
//    private bool clockwise;

//    public float currentAngle;
//    public float rotationSpeed = 1f; // degrees per second

//    private float radius;

//    private int fillOrigin;
//    public float startAngle;
//    public float endAngle;

//    private Image dialImage;
//    [SerializeField] private Color defaultColor = Color.white;
//    [SerializeField] private Color successColor = Color.green;
//   // [SerializeField] private float colorChangeDuration = 0.3f;



//    [SerializeField] private float punishmentValue = 10f;

//    public override void Initialize(GameObject gameObject)
//    {
//        base.Initialize(gameObject);

//       dial = gameObject.transform.Find("SkillCheckCanvas/Dial").gameObject.GetComponent<RectTransform>();
//       pointer = gameObject.transform.Find("SkillCheckCanvas/Dial/Pointer").gameObject.GetComponent<RectTransform>();
//       successZone = gameObject.transform.Find("SkillCheckCanvas/SuccessZone").gameObject.GetComponent<RectTransform>();
//        // successZone = gameObject.transform.Find("SkillCheckCanvas/SuccessZone").gameObject.GetComponent<RectTransform>();
//        successZoneImage = successZone.GetComponent<Image>();
//        clockwise = successZoneImage.fillClockwise;

//        skillChecksCompleted = 0;

//        radius = dial.rect.width / 2f;

//        dial = gameObject.transform.Find("SkillCheckCanvas/Dial").GetComponent<RectTransform>();
//        dialImage = dial.GetComponent<Image>();
//        defaultColor = dialImage.color;

//    }

//    public override void RunUpdateLogic()
//    {
//        // rotate pointer
//        currentAngle += rotationSpeed * Time.deltaTime;

//        float x = radius * Mathf.Cos(currentAngle * Mathf.Deg2Rad);
//        float y = radius * Mathf.Sin(currentAngle * Mathf.Deg2Rad);


//        currentAngle = Mathf.Repeat(currentAngle, 360f);
//        pointer.localPosition = new Vector3(x, y, 0);
//        // keep pointer facing towards center
//        //pointer.up = (Vector3.zero - pointer.localPosition).normalized;
//        pointer.localRotation = Quaternion.Euler(0, 0, currentAngle - 90f);



//        // determine if pointer is in success zone
//        fillOrigin = successZoneImage.fillOrigin;

//        startAngle = DetermineStartAngle(fillOrigin);

//        startAngle = Mathf.Repeat(startAngle, 360f);

//        endAngle = DetermineEndAngle(startAngle, successZoneImage.fillAmount, clockwise);


//        if (startAngle > endAngle)
//        {
//            float temp = startAngle;
//            temp = endAngle;
//            endAngle = startAngle;
//            startAngle = temp;
//        }



//    }

//    public override void DoAttemptSkillCheckLogic()
//    {
//        // determine if pointer is in success zone when player attempts skill check
//        if (IsPointerInSuccessZone())
//        {
//            skillChecksCompleted++;
//            successZone.gameObject.GetComponent<MonoBehaviour>().StartCoroutine(FlashDial());
//        }
//        else
//        {
//            GameObject player = gameObject.GetComponent<BaseStation>().GetRegisteredPlayer();
//            GenericEvent<SkillCheckAttemptFailed>.GetEvent(player.name).Invoke(punishmentValue);
//        }

//        ChangeSuccessZonePosition();

//        if (skillChecksCompleted >= skillChecksRequired)
//        {
//            ResetValues();
//            GenericEvent<SkillCheckCompleted>.GetEvent(gameObject.name).Invoke();
//        }
//    }

//    private float DetermineStartAngle(int fillOrigin)
//    {
//        switch (fillOrigin)
//        {
//            case 0: // Bottom
//                return 270f;
//            case 1: // Right
//                return 359.99f;
//            case 2: // Top
//                return 90f;
//            case 3: // Left
//                return 180f;
//            default:
//                return 0f;
//        }
//    }

//    private float DetermineEndAngle(float startAngle, float fillAmount, bool clockwise)
//    {
//        float endAngle = clockwise ? startAngle - (fillAmount * 360f) : startAngle + (fillAmount * 360f);
//        return Mathf.Repeat(endAngle, 360f);
//    }

//    private bool IsPointerInSuccessZone()
//    {
//        return (currentAngle >= startAngle && currentAngle <= endAngle);
//    }

//    public override void ResetValues()
//    {
//        skillChecksCompleted = 0;
//        currentAngle = 0f;
//    }

//    private void ChangeSuccessZonePosition()
//    {
//        int newFillOrigin = Random.Range(0, 4); // 0 to 3
//        while (newFillOrigin == fillOrigin) // ensure new position is different
//        {
//            newFillOrigin = Random.Range(0, 4);
//        }
        
//        successZoneImage.fillOrigin = newFillOrigin;
//        float newFillAmount = Random.Range(0.1f, 0.15f); // between 10% and 30%
//        successZoneImage.fillAmount = newFillAmount;
//    }

//    System.Collections.IEnumerator FlashDial() {

//        dialImage.color = successColor;

//        yield return new WaitForSeconds(0.15f);


//        dialImage.color = defaultColor;
//    }

//}
