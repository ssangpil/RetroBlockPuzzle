using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    static ObjectPool m_Instance;

    [Serializable]
    public class Pool
    {
        public string name;
        public GameObject prefab;
        public int size;
    }

    [SerializeField] Pool[] pools;
    List<GameObject> m_SpawnObjects = new List<GameObject>();
    Dictionary<string, Queue<GameObject>> m_DictPool = new Dictionary<string, Queue<GameObject>>();

    void Awake()
    {
        m_Instance = this;
    }

    void Start()
    {
        // foreach (var pool in pools)
        // {
        //     m_DictPool.Add(pool.name, new Queue<GameObject>());
        //     for (var i = 0; i < pool.size; i++)
        //     {
        //         var obj = CreateObject(pool.name, pool.prefab);
        //         ArrangePool(obj);
        //     }
        // }
    }

    public static GameObject SpawnFromPool(string name)
        => m_Instance.Get(name, Vector3.zero, Quaternion.identity);

    public static GameObject SpawnFromPool(string name, Vector3 position)
        => m_Instance.Get(name, position, Quaternion.identity);

    public static GameObject SpawnFromPool(string name, Vector3 position, Quaternion rotation)
        => m_Instance.Get(name, position, rotation);

    public static T SpawnFromPool<T>(string name) where T : Component
        => SpawnFromPool<T>(name, Vector3.zero, Quaternion.identity);
        
    public static T SpawnFromPool<T>(string name, Vector3 position) where T : Component
        => SpawnFromPool<T>(name, position, Quaternion.identity);

    public static T SpawnFromPool<T>(string name, Vector3 position, Quaternion rotation) where T : Component
    {
        var obj = m_Instance.Get(name, position, rotation);
        if (obj.TryGetComponent(out T component))
        {
            return component;
        }
        else
        {
            obj.SetActive(false);
            throw new Exception($"Component not found");
        }
    }

    public static List<GameObject> GetAllPools(string name)
    {
        if (!m_Instance.m_DictPool.ContainsKey(name))
            throw new Exception($"Pool with name {name} doesn't exists.");

        return m_Instance.m_SpawnObjects.FindAll(x => x.name == name);
    }

    public static void ReturnToPool(GameObject obj)
    {
        if (m_Instance.m_DictPool.ContainsKey(obj.name))
        {
            m_Instance.m_DictPool[obj.name].Enqueue(obj);
        }
    }

    GameObject Get(string name, Vector3 position, Quaternion rotation)
    {
        if (!m_DictPool.ContainsKey(name))
        {
            if (null == Array.Find(pools, x => x.name == name))
                throw new Exception($"Pool with name {name} doesn't exists.");
            
            m_DictPool.Add(name, new Queue<GameObject>());
        }

        var queue = m_DictPool[name];
        if (0 >= queue.Count)
        {
            var pool = Array.Find(pools, x => x.name == name);
            var newObject = CreateObject(pool.name, pool.prefab);
            ArrangePool(newObject);
        }

        var obj = queue.Dequeue();
        obj.transform.SetPositionAndRotation(position, rotation);
        obj.SetActive(true);
        return obj;
    }

    GameObject CreateObject(string name, GameObject prefab)
    {
        var obj = Instantiate(prefab, transform);
        obj.name = name;
        obj.SetActive(false);
        return obj;
    }

    void ArrangePool(GameObject obj)
    {
        var isFind = false;
        for (var i = 0; i < transform.childCount; i++)
        {
            if (i == transform.childCount - 1)
            {
                obj.transform.SetSiblingIndex(i);
                m_SpawnObjects.Insert(i, obj);
                break;
            }
            else if (transform.GetChild(i).name == obj.name)
            {
                isFind = true;
            }
            else if (isFind)
            {
                obj.transform.SetSiblingIndex(i);
                m_SpawnObjects.Insert(i, obj);
                break;
            }
        }
    }
}

// public class ObjectPool : MonoSingleton<ObjectPool>
// {
//     public List<GameObject> prefabs;
//     private readonly List<GameObject> m_PooledObjects = new List<GameObject>();

//     public GameObject GetFromPool(string name)
//     {
//         var instance = m_PooledObjects.FirstOrDefault(x => x.name == name);
//         if (null != instance)
//         {
//             instance.SetActive(true);
//             m_PooledObjects.Remove(instance);
//             return instance;
//         }

//         var prefab = prefabs.FirstOrDefault(x => x.name == name);
//         if (null != prefab)
//         {
//             instance = Instantiate(prefab, transform);
//             instance.name = name;
//             return instance;
//         }

//         Debug.Log($"Not found pool: name={name}");
//         return null;
//     }

//     public void ReturnPool(GameObject obj)
//     {
//         obj.SetActive(false);
//         m_PooledObjects.Add(obj);
//     }
// }
