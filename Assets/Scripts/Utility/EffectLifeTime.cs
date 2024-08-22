
using System.Collections.Generic;
using UnityEngine;

public class EffectLifeTime : MonoBehaviour
{
    public float lifeTime = -1f;
    public AudioClip[] audioClips = null;

    private readonly List<ParticleSystem> m_ListParticleSystem = new List<ParticleSystem>();
    private float m_CheckTime = 0f;
    private Vector3 m_Scale = Vector3.one;
    private Transform m_PrevParent = null;
    private bool m_IsEffectShowPlayAudio = true;
    private float m_Volume = 1;

    public List<ParticleSystem> ParticleSystems => m_ListParticleSystem;

    void Awake()
    {
        if (TryGetComponent<ParticleSystem>(out var particle))
            m_ListParticleSystem.Add(particle);

        var particles = GetComponentsInChildren<ParticleSystem>();
        if (null != particles && 0 < particles.Length)
        {
            for (var i = 0; i < particles.Length; i++)
                m_ListParticleSystem.Add(particles[i]);
        }
    }
    
    void OnDisable()
    {
        m_IsEffectShowPlayAudio = true;    
    }

    void Update()
    {
        if (0f < lifeTime)
        {
            if (0f >= m_CheckTime)
                m_CheckTime = Time.realtimeSinceStartup + lifeTime;
            else if (m_CheckTime < Time.realtimeSinceStartup)
                SetActive(false);             
        }
    }

    public void PlayAudio()
    {
        // if (null != audioClips)
        // {
        //     for (var i = 0; i < audioClips.Length; ++i)
        //     {
        //         if (null == audioClips[i])
        //             continue;

        //         AudioManager.Instance.PlaySfx
        //     }
        // }
    }

    public void Play() 
    {
        for (var i = 0; i < m_ListParticleSystem.Count; i++)
        {
            m_ListParticleSystem[i].Play();
        }
    }

    public void SetParticleColor(Color color)
    {
        for (var i = 0; i < m_ListParticleSystem.Count; i++)
        {
            var main = m_ListParticleSystem[i].main;
            main.startColor = color;
        }
    }

    public void SetLifeTime(float time, bool isParticle = false, bool isRoot = false)
    {
        if (0 <= time)
        {
            lifeTime = time + 0.5f;
            m_CheckTime = Time.realtimeSinceStartup + lifeTime;
        }
        else
        {
            lifeTime = time;
            m_CheckTime = 0f;
        }

        if (isParticle)
        {
            for (var i = 0; i < m_ListParticleSystem.Count; ++i)
            {
                var main = m_ListParticleSystem[i].main;
                main.startLifetime = time;
                if (isRoot && 0 == i)
                    break;
            }
        }
    }

    public void SetPrevParent(Transform parent)
    {
        m_PrevParent = parent;
    }

    public void SetParticleSize(float size)
    {
        if (m_Scale.x != size)
        {
            m_Scale = new Vector3(size, size, size);
            transform.localScale = m_Scale;
        }
    }

    public void SetActive(bool state)
    {
        if (!state)
        {
            if (null != m_PrevParent)
            {
                transform.SetParent(m_PrevParent);
                transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                transform.localScale = Vector3.one;
                m_Scale = Vector3.one;
            }
        }

        m_CheckTime = 0f;
        m_Volume = 1f;
        gameObject.SetActive(state);
    }

    public EffectLifeTime SetOrderInLayer(int order)
    {
        if (!gameObject.TryGetComponent<Canvas>(out var canvas))
        {
            canvas = gameObject.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.vertexColorAlwaysGammaSpace = true;
        }

        canvas.sortingOrder = order;
        return this;
    }

    public void Activate(float volume)
    {
        SetActive(true);
        m_Volume = volume;
    }
}
