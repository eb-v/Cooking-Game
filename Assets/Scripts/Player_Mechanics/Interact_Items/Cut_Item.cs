using UnityEngine;

public class Cut_Item : MonoBehaviour, IInteractable {
    [SerializeField] private SkillCheckManager skillCheckManager;
    [SerializeField] private GameObject finishedItemPrefab;

    [Header("Settings")]
    //[SerializeField] private float interactRadius = 2f;

    private bool itemCut = false;
    private Transform player;
    private bool skillCheckActive = false;
    private int playerColliderCount = 0;
    private bool playerInRange;

  

    private void OnTriggerEnter(Collider other) {
        GameObject root = other.transform.root.gameObject;
        if (root.CompareTag("Player")) {
            player = root.transform;
            playerColliderCount++;

            if (playerColliderCount == 1) {
                Debug.Log("TRIGGER ON CUT_ITEM ENTERED");
                playerInRange = true;

                CheckInteract ci = root.GetComponent<CheckInteract>();
                if (ci != null) ci.SetCurrentInteractable(this);
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        GameObject root = other.transform.root.gameObject;
        if (root.CompareTag("Player")) {
            playerColliderCount--;

            if (playerColliderCount <= 0) {
                playerColliderCount = 0;
                Debug.Log("TRIGGER ON CUT_ITEM EXITED");
                player = null;
                playerInRange = false;

                CheckInteract ci = root.GetComponent<CheckInteract>();
                if (ci != null) ci.ClearCurrentInteractable(this);
            }
        }
    }

    public void Interact() {
        if (itemCut || skillCheckActive) return;
        if (!playerInRange) return;

        if (skillCheckManager != null) {
            skillCheckActive = true;

            CheckInteract ci = player.GetComponent<CheckInteract>();
            if (ci != null) ci.SetCurrentInteractable(skillCheckManager);

            skillCheckManager.OnSkillCheckFinished += OnSkillCheckFinished;
            skillCheckManager.StartSkillCheck();
            Debug.Log("Skill check started from Cut_Item!");
        }
    }

    public void StopInteract() {
        //if (skillCheckActive && skillCheckManager != null) {
            //skillCheckManager.OnSkillCheckFinished -= OnSkillCheckFinished;
            //skillCheckActive = false;
         Debug.Log("Player stopped interaction on SKILL CHECK");
        //}
    }

    private void OnSkillCheckFinished(bool success) {
        skillCheckManager.OnSkillCheckFinished -= OnSkillCheckFinished;

        skillCheckActive = false;

        if (success) {
            Debug.Log("Skill Check Completed");
            FinishCut();
        } else {
            Debug.Log("Skill Check Failed");
        }
    }

    private void FinishCut() {
        if (itemCut) return;

        itemCut = true;

        if (finishedItemPrefab != null) {
            Instantiate(finishedItemPrefab, transform.position, transform.rotation);
        }

        Destroy(gameObject);
        Debug.Log($"{name} has been cut and replaced.");
    }

}
