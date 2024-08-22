using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PrefabManager : MonoSingleton<PrefabManager>
{
    private Canvas m_Canvas; 
    private Transform m_TransCanvas;

    private readonly Dictionary<string, UIPanelPopup> m_DictPanelPopup = new Dictionary<string, UIPanelPopup>();
    private readonly Dictionary<string, MonoBehaviour> m_DictPrefab = new Dictionary<string, MonoBehaviour>();
    private static bool m_IsDestroyed = false;

    void Awake()
    {
        m_Canvas = FindTopCanvas();
        m_TransCanvas = m_Canvas.transform;
        m_IsDestroyed = false;
    }

    void OnDestroy()
    {
        m_IsDestroyed = true;   
    }

    Canvas FindTopCanvas()
    {
        Canvas[] allCanvases = FindObjectsOfType<Canvas>();
        Canvas topMostCanvas = null;

        foreach (Canvas canvas in allCanvases)
        {
            if (topMostCanvas == null || topMostCanvas.transform.IsChildOf(canvas.transform))
            {
                topMostCanvas = canvas;
            }
        }

        return topMostCanvas;
    }

    public static T Get<T>(string name = "", Transform parent = null, Vector3? pos = null) where T : UIPanelPopup
    {
        if (m_IsDestroyed)
            return null;

        T panel = null;
        if (string.IsNullOrEmpty(name))
        {
            name = typeof(T).ToString();
        }

        if (Instance.m_DictPanelPopup.ContainsKey(name))
        {
            panel = Instance.m_DictPanelPopup[name] as T;
        }
        else
        {
            var path = Path.Combine("UI/Prefabs", name);
            var objPanel = Utility.InstantiatePanel(path, parent == null ? Instance.m_TransCanvas : parent, pos ?? Vector3.zero);
            if (null != objPanel)
            {
                if (objPanel.TryGetComponent(out panel))
                {
                    objPanel.SetActive(false);
                    Instance.m_DictPanelPopup.Add(name, panel);
                }
                else    
                {
                    Destroy(objPanel);
                }
            }
            else    
            {
                Debug.LogError("Not found prefab=" + name);
            }
        }

        return panel;
    }

    public static T GetPrefab<T>(string name = "", Transform parent = null, Vector3? pos = null) where T : MonoBehaviour
    {
        if (m_IsDestroyed)
            return null;

        T prefab = null;
        if (string.IsNullOrEmpty(name))
            name = typeof(T).ToString();

        if (Instance.m_DictPrefab.ContainsKey(name))
        {
            prefab = Instance.m_DictPrefab[name] as T;
        }
        else
        {
            var path = Path.Combine("UI/Prefabs", name);
            var objPanel = Utility.InstantiatePanel(path, parent == null ? Instance.m_TransCanvas : parent, pos ?? Vector3.zero);
            if (null != objPanel)
            {
                if (objPanel.TryGetComponent(out prefab))
                {
                    objPanel.SetActive(false);
                    Instance.m_DictPrefab.Add(name, prefab);
                }
                else
                {
                    Destroy(objPanel);
                    Debug.LogError("Not found component prefab=" + name);
                }
            }
            else
            {
                Debug.LogError("Not found prefab=" + name);
            }
        }

        return prefab;
    }
}