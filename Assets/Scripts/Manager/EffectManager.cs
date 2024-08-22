using System.Collections.Generic;
using UnityEngine;

public enum EEffectType
{
    Shape,
    LineX,
    LineY,
    BonusTimeGauge,    
    Star,
    Background,
    Combo,
    Max,
}

public class EffectManager : MonoSingleton<EffectManager>
{
    [ArrayElementName(typeof(EEffectType), (int)EEffectType.Max)]
    public List<EffectLifeTime> Effects = new List<EffectLifeTime>();
    private readonly Dictionary<EEffectType, List<EffectLifeTime>> m_DictUseEffect = new Dictionary<EEffectType, List<EffectLifeTime>>();

    public EffectLifeTime ShowEffect(EEffectType type, Transform parent)
    {
        var effect = ShowEffect(type);
        if (null == effect)
        {
            Debug.LogError("Failed to show effect: type=" + type);
            return null;
        }

        effect.transform.SetParent(parent);
        effect.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        effect.transform.localScale = Vector3.one;
        effect.SetActive(true);
        return effect;
    }

    public EffectLifeTime ShowEffect(EEffectType type, Transform parent, Vector3 position, Vector3 rotate, float scale)
    {
        var effect = ShowEffect(type);
        if (null == effect)
        {
            Debug.LogError("Failed to show effect: type=" + type);
            return null;
        }

        effect.transform.SetParent(parent);
        effect.transform.SetLocalPositionAndRotation(position, Quaternion.Euler(rotate));
        effect.SetParticleSize(scale);
        effect.SetActive(true);
        return effect;
    }

    public EffectLifeTime ShowEffect(EEffectType type, Vector3 position)
    {
        var effect = ShowEffect(type);
        if (null == effect)
        {
            Debug.LogError("Failed to show effect: type=" + type);
            return null;
        }

        effect.transform.localPosition = position;
        effect.transform.localScale = Vector3.one;
        effect.SetActive(true);
        return effect;
    }

    public EffectLifeTime ShowEffect(EEffectType type, Transform parent, Vector3 position)
    {
        var effect = ShowEffect(type);
        if (null == effect)
        {
            Debug.LogError("Failed to show effect: type=" + type);
            return null;
        }

        effect.transform.SetParent(parent);
        effect.transform.localPosition = position;
        effect.transform.localScale = Vector3.one;
        effect.SetActive(true);
        return effect;
    }

    private EffectLifeTime ShowEffect(EEffectType type)
    {
        var effect = GetReUseEffect(type);
        if (null == effect)
            effect = CreateEffect(type);

        return effect;
    }

    private EffectLifeTime GetReUseEffect(EEffectType type)
    {
        if (!m_DictUseEffect.ContainsKey(type))
            return null;

        var count = m_DictUseEffect[type].Count;
        for (var i = 0; i < count; i++)
        {
            if (null == m_DictUseEffect[type][i] || m_DictUseEffect[type][i].gameObject.activeSelf)
                continue;

            return m_DictUseEffect[type][i];
        }

        return null;
    }

    private EffectLifeTime CreateEffect(EEffectType type)
    {
        if (Effects.Count <= 0 || (int)type >= Effects.Count)
            return null;

        var lifeTime = CloneEffect(Effects[(int)type]);
        if (null == lifeTime)
            return null;

        if (m_DictUseEffect.ContainsKey(type))
            m_DictUseEffect[type].Add(lifeTime);
        else
            m_DictUseEffect[type] = new List<EffectLifeTime> { lifeTime };

        return lifeTime;
    }
    
    private EffectLifeTime CloneEffect(EffectLifeTime effect)
    {
        if (effect == null)
            return null;

        var original = effect.gameObject;

        var go = Instantiate(original);
        go.transform.SetParent(transform);
        go.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        go.transform.localScale = Vector3.one;
        go.SetActive(false);

        if (go.TryGetComponent<EffectLifeTime>(out var lifeTime))
        {
            lifeTime.SetPrevParent(transform);
            return lifeTime;
        }
        else
        {
            Destroy(go);
            return null;
        }
    }

    public void Clear()
    {
        for (EEffectType type = 0; type < EEffectType.Max; ++type)
        {
            if (m_DictUseEffect.ContainsKey(type))
            {
                for (var i = 0; i < m_DictUseEffect[type].Count; ++i)
                {
                    if (m_DictUseEffect[type][i] != null)
                    {
                        m_DictUseEffect[type][i].SetActive(false);
                        Destroy(m_DictUseEffect[type][i].gameObject);
                        m_DictUseEffect[type][i] = null;
                    }
                }
                m_DictUseEffect[type].Clear();
            }
        }
        m_DictUseEffect.Clear();
    }
}