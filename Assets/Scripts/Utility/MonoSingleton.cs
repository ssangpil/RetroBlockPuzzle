using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T m_Instance = null;

    public static T Instance 
    {
        get 
        {
            if (null == m_Instance)
            {
                m_Instance = FindObjectOfType<T>();
                if (null == m_Instance)
                {
                    Debug.LogWarning(typeof(T).Name + " created form MonoSingleton");
                    m_Instance = new GameObject("_" + typeof(T).Name).AddComponent<T>();
                }
            }
            return m_Instance;
        }
    }

    void Awake()
    {
        m_Instance = this as T;
        OnAwake();
    }

    protected virtual void OnAwake()
    {

    }
}

public abstract class MonoObjectSingletonInScene<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T m_Instance = null;

    public static T Instance 
    {
        get 
        {
            if (m_Instance == null)
                m_Instance = Utility.FindObjectOfType<T>();

            return m_Instance;
        }
    }

    private void Awake()
    {
        m_Instance = this as T;
        OnAwake();
    }

    protected virtual void OnAwake()
    {
    }
}
