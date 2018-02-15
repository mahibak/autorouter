using UnityEngine;
using System.Collections;
using System.Collections.Generic;
 
/// <summary>
/// The object pool we use to handle instantiation. In most cases, we should be using this instead of directly using GameObject.Instantiate().
/// </summary>
public class EntityPool
{
    private Dictionary<GameObject, Queue<GameObject>> _objectPools;

    private Transform _containerPooled;
    private Transform _containerActive;
    
    private static EntityPool _instance;
    
    public static void CreateInstance()
    {
        if (_instance == null)
        {
            _instance = new EntityPool();
        }
    }

    public static EntityPool GetInstance()
    {
        return _instance;
    }

    private EntityPool()
    {
        _objectPools = new Dictionary<GameObject, Queue<GameObject>>();
        _containerPooled = new GameObject("ObjectsPooled").transform;
        _containerActive = new GameObject("ObjectsActive").transform;
    }

    public static GameObject Spawn(GameObject original)
    {
        return Spawn(original, Vector3.zero, Quaternion.identity);
    }

    public static GameObject Spawn(GameObject original, Vector3 position)
    {
        return Spawn(original, position, Quaternion.identity);
    }

    public static GameObject Spawn(GameObject original, Transform transform)
    {
        return Spawn(original, transform.position, transform.rotation);
    }

    public static GameObject Spawn(GameObject original, Vector3 position, Quaternion rotation)
    {
        if (original == null)
        {
            Debug.LogWarning("Tried to instantiate a null object.");
            return null;
        }

        Queue<GameObject> pool = _instance.GetPool(original);

        if (pool.Count == 0)
        {
            GameObject obj = GameObject.Instantiate(original, position, rotation) as GameObject;
            obj.transform.parent = _instance._containerActive;
            return obj;
        }
        else
        {
            GameObject obj = pool.Dequeue();
            if (obj != null)
            {
                obj.transform.position = position;
                obj.transform.rotation = rotation;
                obj.transform.parent = _instance._containerActive;
                obj.SetActive(true);
            }
            else
            {
                Debug.LogWarning(string.Format("We had a null object in our queue. ({0:s})", original.name));
            }
            return obj;
        }
    }

    public static void Unspawn(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.parent = _instance._containerPooled;
        _instance.GetPool(obj).Enqueue(obj);
    }

    private Queue<GameObject> GetPool(GameObject original)
    {
        if (!_objectPools.ContainsKey(original))
        {
            _objectPools.Add(original, new Queue<GameObject>());
        }

        return _objectPools[original];
    }
}