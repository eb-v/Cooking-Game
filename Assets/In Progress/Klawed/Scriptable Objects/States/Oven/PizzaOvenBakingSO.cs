//using NUnit.Framework;
//using UnityEngine;
//using System.Collections.Generic;

//[CreateAssetMenu(fileName = "PizzaOvenBakingSO", menuName = "Scriptable Objects/States/Kitchen/PizzaOvenBakingSO")]
//public class PizzaOvenBakingSO : BaseStateSO
//{
//    PizzaOvenScript pizzaOvenScript;
//    [SerializeField] private GameObject cookedPizzaPrefab;
//    [SerializeField] private float bakingTime = 5f;
//    private float currentTimer = 0f;

//    public override void Initialize(GameObject gameObject, StateMachine _stateMachine)
//    {
//        base.Initialize(gameObject, _stateMachine);

//        pizzaOvenScript = gameObject.GetComponent<PizzaOvenScript>();
//        if (pizzaOvenScript == null)
//        {
//            Debug.LogError("PizzaOvenScript component not found on the GameObject.");
//        }
//    }

//    public override void Enter()
//    {
        
//    }

//    public override void Execute()
//    {
//        RunBakingLogic();
//    }

//    public override void Exit()
//    {
//        currentTimer = 0f;
//    }
    
//    private void RunBakingLogic()
//    {
//        // timer logic to delay the creation of the cooked pizzas
//        if (currentTimer < bakingTime)
//        {
//            currentTimer += Time.deltaTime;
//            return;
//        }


//        PizzaDoughBase pizzaDough = pizzaOvenScript.pizzaInOven;
//        Vector3 pizzaDoughPos = pizzaDough.transform.position;
//        RemovePizzaDoughBase(pizzaDough);
//        SpawnCookedPizza(pizzaDoughPos);

//        stateMachine.ChangeState(pizzaOvenScript._idleStateInstance);
//    }

//    private void SpawnCookedPizza(Vector3 spawnPos)
//    {
//        ObjectPoolManager.SpawnObject(cookedPizzaPrefab, spawnPos, Quaternion.identity);
//    }

//    private void RemovePizzaDoughBase(PizzaDoughBase pizzaDough)
//    {
//        List<GameObject> ingredientInstances = pizzaDough.GetIngredientInstancesOnPizza();
//        foreach (GameObject ingredientInstance in ingredientInstances)
//        {
//            ObjectPoolManager.ReturnObjectToPool(ingredientInstance);
//        }
//        pizzaDough.ClearIngredientInstanceList();
//        Vector3 pizzaDoughPos = pizzaDough.transform.position;
//        ObjectPoolManager.ReturnObjectToPool(pizzaDough.gameObject);
//        pizzaOvenScript.pizzaInOven = null;

//    }

    


//}
