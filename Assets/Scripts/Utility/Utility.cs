
using System.Collections;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Utility
{
    public static GameObject InstantiatePanel(string path, Transform parent, Vector3 pos)
    {
        var obj = Resources.Load(path);
        if (null == obj)
        {
            Debug.LogError("Failed to load: path=" + path);
            return null;
        }

        var go = (GameObject)Object.Instantiate(obj);
        go.transform.SetParent(parent);
        go.transform.localPosition = pos;
        go.transform.localScale = Vector3.one;
        go.SetActive(false);
        return go;
    }

    public static GameObject InstantiateGameObject(Object instance, Transform parent, Vector3 position)
    {
        var obj = (GameObject)Object.Instantiate(instance);
        if (null != obj)
        {
            obj.SetActive(true);
            obj.transform.SetParent(parent);
            obj.transform.localPosition = position;
            obj.transform.localScale = Vector3.one;
        }
        return obj;
    }

    public static T FindObjectOfType<T>(string sceneName = "Main") where T : MonoBehaviour
    {
        var scene = SceneManager.GetSceneByName(sceneName);
        if (null != scene)
        {
            var rootObjects = scene.GetRootGameObjects();
            if (null != rootObjects)
            {
                for (int i = 0; i < rootObjects.Length; i++)
                {
                    if (rootObjects[i].name.Contains("UI"))
                        continue;

                    var childObj = rootObjects[i].GetComponentInChildren<T>();
                    if (null != childObj)
                        return childObj;
                }
            }
        }
        return null;
    }

    public static void SafeSetClickEvent(this GameObject btn, UIEventListener.VoidDelegate callback)
    {
        if (null != btn)
            UIEventListener.Get(btn).onClick = callback;
    }

    public static void SafeText(this GameObject obj, string text)
    {
        if (obj.TryGetComponent<TextMeshProUGUI>(out var o))
        {
            o.text = text;
        }
    }
    
    public static void SafeInvoke(this System.Action action)
    {
        action?.Invoke();
    }
}

public static class UtilCoroutine
{
    public static void PlayCoroutine(ref IEnumerator variable, IEnumerator func, MonoBehaviour script)
    {
        if (null != variable)
        {
            script.StopCoroutine(variable);
        }

        if (!script.gameObject.activeSelf)
            return;

        variable = func;
        script.StartCoroutine(variable);
    }

    public static void ClearCoroutine(ref IEnumerator variable, IEnumerator func, MonoBehaviour script)
    {
        if (null != variable)
        {
            script.StopCoroutine(variable);
            variable = null;
        }
    }

    public static IEnumerator PlayCoroutine(this IEnumerator variable, IEnumerator func, MonoBehaviour script)
    {
        variable.ClearCoroutine(script);
        script.StartCoroutine(func);
        return func;
    }

    public static IEnumerator ClearCoroutine(this IEnumerator variable, MonoBehaviour script)
    {
        if (null != variable)
        {
            script.StopCoroutine(variable);
        }

        return null;
    }
}