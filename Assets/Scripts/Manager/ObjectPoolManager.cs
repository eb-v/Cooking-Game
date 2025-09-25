using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;


public class ObjectPoolManager : MonoBehaviour
{
    [SerializeField] private bool _addToDontDestroyOnLoad = false;

    private GameObject _emptyHolder;

    private static GameObject _gameObjectsEmpty;

    private static Dictionary<GameObject, ObjectPool<GameObject>> _objectPools;
    private static Dictionary<GameObject, GameObject> _cloneToPrefabMap;

    public enum PoolType
    {
        GameObjects
    }

    private void Awake()
    {
        _objectPools = new Dictionary<GameObject, ObjectPool<GameObject>>();
        _cloneToPrefabMap = new Dictionary<GameObject, GameObject>();

        SetupEmpties();
    }

    
    //public void Initialize()
    //{
    //    _objectPools = new Dictionary<GameObject, ObjectPool<GameObject>>();
    //    _cloneToPrefabMap = new Dictionary<GameObject, GameObject>();
    //    SetupEmpties();
    //}

    private void SetupEmpties()
    {
        _emptyHolder = new GameObject("Object Pools");

        _gameObjectsEmpty = new GameObject("GameObject");
        _gameObjectsEmpty.transform.SetParent(_emptyHolder.transform);

        if (_addToDontDestroyOnLoad)
        {
            DontDestroyOnLoad(_gameObjectsEmpty.transform.root);
        }
    }

    private static void CreatePool(GameObject prefab, Vector3 pos, Quaternion rot, PoolType poolType = PoolType.GameObjects)
    {
        ObjectPool<GameObject> pool = new ObjectPool<GameObject>(
            createFunc: () => CreateObject(prefab, pos, rot, poolType),
            actionOnGet: obj => OnGetObject(obj),
            actionOnRelease: obj => OnReleaseObject(obj),
            actionOnDestroy: obj => OnDestroyObject(obj)
        );
        _objectPools.Add(prefab, pool);
    }

    private static void CreatePool(GameObject prefab, Transform parent, Quaternion rot, PoolType poolType = PoolType.GameObjects)
    {
        ObjectPool<GameObject> pool = new ObjectPool<GameObject>(
            createFunc: () => CreateObject(prefab, parent, rot, poolType),
            actionOnGet: obj => OnGetObject(obj),
            actionOnRelease: obj => OnReleaseObject(obj),
            actionOnDestroy: obj => OnDestroyObject(obj)
        );

        _objectPools.Add(prefab, pool);
    }

    private static GameObject CreateObject(GameObject prefab, Vector3 pos, Quaternion rot, PoolType poolType = PoolType.GameObjects)
    {
        prefab.SetActive(false);

        GameObject obj = Instantiate(prefab, pos, rot);

        prefab.SetActive(true);

        GameObject parentObject = SetParentObject(poolType);
        obj.transform.SetParent(parentObject.transform);

        return obj;
    }

    private static GameObject CreateObject(GameObject prefab, Transform parent, Quaternion rot, PoolType poolType = PoolType.GameObjects)
    {
        prefab.SetActive(false);

        GameObject obj = Instantiate(prefab, parent);

        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = rot;
        obj.transform.localScale = Vector3.one;

        prefab.SetActive(true);

        return obj;
    }

    private static void OnGetObject(GameObject obj)
    {
        // optional object setup when retrieved from pool
    }

    private static void OnReleaseObject(GameObject obj)
    {
        obj.SetActive(false);
    }

    private static void OnDestroyObject(GameObject obj)
    {
        if (_cloneToPrefabMap.ContainsKey(obj))
        {
            _cloneToPrefabMap.Remove(obj);
        }
        else
        {
            Debug.Log("Object to destroy does not exist in the cloneToPrefab map ");
        }
    }

    private static GameObject SetParentObject(PoolType poolType)
    {
        switch (poolType)
        {
            case PoolType.GameObjects:
                return _gameObjectsEmpty;
            default:
                return null;
        }
    }

    #region Public Spawn Methods used by other scripts

    // returns some component type from the spawned object with Vector3 position
    public static T SpawnObject<T>(T typePrefab, Vector3 spawnPos, Quaternion spawnRotation, PoolType poolType = PoolType.GameObjects) where T : Component
    {
        return SpawnObject<T>(typePrefab.gameObject, spawnPos, spawnRotation, poolType);
    }
    // returns the spawned GameObject with Vector3 position
    public static GameObject SpawnObject(GameObject objectToSpawn, Vector3 spawnPos, Quaternion spawnRotation, PoolType poolType = PoolType.GameObjects)
    {
        return SpawnObject<GameObject>(objectToSpawn, spawnPos, spawnRotation, poolType);
    }
    // returns some component type from the spawned object with it being a child of some parent Transform
    public static T SpawnObject<T>(T typePrefab, Transform parent, Quaternion spawnRotation, PoolType poolType = PoolType.GameObjects) where T : Component
    {
        return SpawnObject<T>(typePrefab.gameObject, parent, spawnRotation, poolType);
    }
    // returns the spawned GameObject with it being a child of some parent Transform
    public static GameObject SpawnObject(GameObject objectToSpawn, Transform parent, Quaternion spawnRotation, PoolType poolType = PoolType.GameObjects)
    {
        return SpawnObject<GameObject>(objectToSpawn, parent, spawnRotation, poolType);
    }

    #endregion
    // Spawn object that uses a parent Transform
    private static T SpawnObject<T>(GameObject objectToSpawn, Transform parent, Quaternion spawnRotation, PoolType poolType = PoolType.GameObjects) where T : Object
    {
        if (!_objectPools.ContainsKey(objectToSpawn))
        {
            CreatePool(objectToSpawn, parent, spawnRotation, poolType);
        }
        GameObject obj = _objectPools[objectToSpawn].Get();

        if (obj != null)
        {
            if (!_cloneToPrefabMap.ContainsKey(obj))
            {
                _cloneToPrefabMap.Add(obj, objectToSpawn);
            }

            obj.transform.SetParent(parent);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = spawnRotation;
            obj.SetActive(true);

            if (typeof(T) == typeof(GameObject))
            {
                return obj as T;
            }

            T component = obj.GetComponent<T>();
            if (component == null)
            {
                Debug.LogError($"SpawnObject<{typeof(T)}>(): The spawned object '{obj.name}' does not have the requested component of type '{typeof(T)}'.");
                return null;
            }

            return component;
        }

        return null;
    }
    // Spawn object that uses a Vector3 position
    private static T SpawnObject<T>(GameObject objectToSpawnPrefab, Vector3 spawnPos, Quaternion spawnRotation, PoolType poolType = PoolType.GameObjects) where T : Object
    {
        if (!_objectPools.ContainsKey(objectToSpawnPrefab))
        {
            CreatePool(objectToSpawnPrefab, spawnPos, spawnRotation, poolType);
        }
        GameObject objInstance = _objectPools[objectToSpawnPrefab].Get();

        if (!_cloneToPrefabMap.ContainsKey(objInstance))
        {
            _cloneToPrefabMap.Add(objInstance, objectToSpawnPrefab);
        }

        objInstance.transform.position = spawnPos;
        objInstance.transform.rotation = spawnRotation;
        objInstance.SetActive(true);

        if (typeof(T) == typeof(GameObject))
        {
            return objInstance as T;
        }

        T component = objInstance.GetComponent<T>();

        if (component == null)
        {
            Debug.Log($"SpawnObject<{typeof(T)}>(): The spawned object '{objInstance.name}' does not have the requested component of type '{typeof(T)}'.");
            return null;
        }

        return component;
    }

    public static void ReturnObjectToPool(GameObject obj, PoolType poolType = PoolType.GameObjects)
    {
        if (_cloneToPrefabMap.TryGetValue(obj, out GameObject prefab))
        {
            GameObject parentObject = SetParentObject(poolType);

            if (obj.transform.parent != parentObject.transform)
            {
                obj.transform.SetParent(parentObject.transform);
            }

            if (_objectPools.TryGetValue(prefab, out ObjectPool<GameObject> pool))
            {
                pool.Release(obj);
            }
        }
        else
        {
            Debug.LogWarning($"ReturnObjectToPool(): The object '{obj.name}' does not belong to any pool and cannot be returned.");
        }
    }


}