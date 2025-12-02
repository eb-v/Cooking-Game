using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "AS_Assemble", menuName = "Scriptable Objects/States/Kitchen/Assembly/Assemble")]
public class AS_Assemble : AssemblyState
{
    AssemblyStation assemblyStation;
    private float assemblyTimer = 0f;
    private GameObject progressBar;
    private Image fillImg;

    public override void Enter()
    {
        base.Enter();
        progressBar.SetActive(true);
    }

    public override void Exit()
    {
        base.Exit();
        assemblyTimer = 0f;
        progressBar.SetActive(false);
    }

    public override void FixedUpdateLogic()
    {
        base.FixedUpdateLogic();
    }

    public override void Initialize(GameObject gameObject, StateMachine<AssemblyState> stateMachine)
    {
        base.Initialize(gameObject, stateMachine);
        assemblyStation = gameObject.GetComponent<AssemblyStation>();
        progressBar = assemblyStation.ProgressBar;
        fillImg = assemblyStation.FillImg;
    }

    public override void InteractLogic(GameObject player)
    {
        base.InteractLogic(player);
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();

        assemblyTimer += Time.deltaTime;
        fillImg.fillAmount = assemblyTimer / assemblyStation.TimeToAssemble;
        if (assemblyTimer >= assemblyStation.TimeToAssemble)
        {
            GameObject menuItemObj = AssembleItem();
            PhysicsTransform physTrans = menuItemObj.GetComponent<PhysicsTransform>();
            if (physTrans == null)
            {
                Debug.LogError("Menu Item does not have physics Transform");
            }
            else
            {
                Rigidbody rb = physTrans.physicsTransform.GetComponent<Rigidbody>();
                Vector3 direction = GetDirection();
                rb.AddForce(direction * assemblyStation.launchForce, ForceMode.Impulse);
                //assemblyStation.ChangeState(assemblyStation.IdleStateInstance);
                assemblyTimer = 0f;
            }

        }

    }

    private GameObject AssembleItem()
    {
        GameObject itemPrefab = assemblyStation.SelectedMenuItem.Prefab;
        GameObject createdMenuItemObj = Instantiate(itemPrefab, assemblyStation.SpawnPosition, itemPrefab.transform.rotation);
        return createdMenuItemObj;
    }

    private Vector3 GetDirection()
    {
        float xOffset = Random.Range(assemblyStation.xOffsetRangeMin, assemblyStation.xOffsetRangeMax);
        if (CoinFlip())
        {
            xOffset *= -1f;
        }
        float zOffset = Random.Range(assemblyStation.zOffsetRangeMin, assemblyStation.zOffsetRangeMax);
        if (CoinFlip())
        {
            zOffset *= -1f;
        }
        return new Vector3(xOffset, 1f, zOffset).normalized;
    }

    private bool CoinFlip()
    {
        float flip = Random.Range(0f, 1f);
        if (flip <= 0.5f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
