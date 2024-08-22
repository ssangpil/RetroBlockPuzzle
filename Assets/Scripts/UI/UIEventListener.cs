using UnityEngine;

[AddComponentMenu("LunchLunch/UI/Event Listener")]
public class UIEventListener : MonoBehaviour
{
    public delegate void VoidDelegate(GameObject go);
    
    public VoidDelegate onClick;
    public bool needsActiveCollider = true;

    bool IsColliderEnabled
    {
        get 
        {
            if (!needsActiveCollider) 
                return true;
            if (TryGetComponent<Collider>(out var c)) 
                return c.enabled;
            var b = GetComponent<Collider2D>();
            return b != null && b.enabled;
        }
    }

    //public void OnClick() { if (IsColliderEnabled && onClick != null) onClick(gameObject); }
    public void OnClick() { onClick?.Invoke(gameObject); }

    public void Clear()
    {
        onClick = null;
    }

    public static UIEventListener Get(GameObject go)
    {
        if (!go.TryGetComponent<UIEventListener>(out var listener)) 
            listener = go.AddComponent<UIEventListener>();
        return listener;
    }
}