using UnityEngine;

public class MonoBehaviourObject : MonoBehaviour
{
    private GameObject m_GameObject;
    private Transform m_Transform;
    private Rigidbody2D m_Rigidbody;
    private Collider2D m_Collider;

    public GameObject GameObject
    {
        get 
        {
            if (null == m_GameObject)
                m_GameObject = gameObject;

            return m_GameObject;
        }
    }
    public Transform Transform
    {
        get 
        {
            if (null == m_Transform)
                m_Transform = transform;

            return m_Transform; 
        }
    }

    public Vector3 Position 
    {
        get {return m_Transform.position;}
        set {m_Transform.position = value;}
    }

    public Vector3 LocalPosition 
    {
        get {return m_Transform.localPosition;}
        set {m_Transform.localPosition = value;}
    }

    public Quaternion Rotation
    {
        get { return m_Transform.rotation; }
        set { m_Transform.rotation = value; }
    }

    public Vector3 LocalScale
    {
        get { return m_Transform.localScale; }
        set { m_Transform.localScale = value; }
    }

    public bool ActiveSelf => m_GameObject.activeSelf;
    public bool ActiveInHierarchy => m_GameObject.activeInHierarchy;

    public virtual void SetActive(bool state)
    {
        m_GameObject.SetActive(state);
    }
}