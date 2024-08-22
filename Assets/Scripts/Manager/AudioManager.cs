
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using UnityEngine;

public class AudioManager : MonoSingleton<AudioManager>
{
    // [ArrayElementName(typeof(ESfx), (int)ESfx.Max)]
    // public List<AudioClip> listAudioClip = new List<AudioClip>();

    // private class AudioSourceClip
    // {
    //     public AudioSource Source;
    //     public AudioClip Clip;

    //     public AudioSourceClip(AudioSource source, AudioClip clip)
    //     {
    //         Source = source;
    //         Clip = clip;
    //     }
    // }

    [Header("#BGM")]
    public AudioClip bgmClip;
    public float bgmVolume;
    AudioSource bgmPlayer;

    [Header("#SFX")]
    [SerializeField] SfxData[] sfxDataList;
    public float sfxVolume;
    public float sfxVoiceVolume;
    public int channels;
    AudioSource[] sfxPlayers;
    int channelIndex;


    bool m_IsOn = true;
    bool m_IsBgmOn = true;

    const string SOUND_ONOFF = "SOUND_ONOFF";
    const string BGM_ONOFF = "BGM_ONOFF";

    //private readonly List<AudioSourceClip> m_ListAudioSourceClip = new List<AudioSourceClip>();

    public bool IsOn
    {
        get { return m_IsOn;}
        set 
        {
            m_IsOn = value;
            PlayerPrefs.SetInt(SOUND_ONOFF, m_IsOn ? 1 : 0);
        }
    }

    public bool IsBgmOn
    {
        get { return m_IsBgmOn;}
        set 
        {
            m_IsBgmOn = value;
            PlayerPrefs.SetInt(BGM_ONOFF, m_IsBgmOn ? 1 : 0);
            PlayBgm(m_IsBgmOn);
        }
    }

    [Serializable]
    public class SfxData
    {
        public ESfx sfx;
        public AudioClip audioClip;
    }

    void Awake()
    {
        m_IsOn = PlayerPrefs.GetInt(SOUND_ONOFF, m_IsOn ? 1 : 0) == 1;
        m_IsBgmOn = PlayerPrefs.GetInt(BGM_ONOFF, m_IsBgmOn ? 1 : 0) == 1;
        
        var bgmObj = new GameObject("BgmPlayer");
        bgmObj.transform.SetParent(transform);
        bgmPlayer = bgmObj.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        bgmPlayer.volume = bgmVolume;
        bgmPlayer.clip = bgmClip;

        var sfxObj = new GameObject("SfxPlayer");
        sfxObj.transform.SetParent(transform);
        sfxPlayers = new AudioSource[channels];

        for (var i = 0; i < channels; i++)
        {
            sfxPlayers[i] = sfxObj.AddComponent<AudioSource>();
            sfxPlayers[i].playOnAwake = false;
            sfxPlayers[i].volume = sfxVolume;
        }        

        // var audioParent = new GameObject { name = "audioSource" };
        // audioParent.transform.SetParent(transform);
        // audioParent.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        // audioParent.transform.localScale = Vector3.one;

        // for (var i = 0; i < channels; ++i)
        // {
        //     var sc = new AudioSourceClip(audioParent.AddComponent<AudioSource>(), null);
        //     m_ListAudioSourceClip.Add(sc);
        // }
    }

    public void PlayBgm(bool isOn)
    {
        if (isOn)
        {
            bgmPlayer.Play();
        }
        else
        {
            bgmPlayer.Stop();
        }
    }

    public AudioClip PlayAudioClip(AudioClip audioClip, bool isLoop, float volume)
    {
        if (!IsOn)
            return null;
            
        return null;
    }

    public void PlaySfx(int sfx)
    {
        PlaySfx((ESfx)sfx);
    }

    public void PlaySfx(ESfx sfx)
    {
        if (!IsOn)
            return;

        var list = sfxDataList.Where(x => x.sfx == sfx);
        foreach (var data in list)
        {
            for (var i = 0; i < channels; i++)
            {
                var loopIndex = (i + channelIndex) % channels;
                if (sfxPlayers[loopIndex].isPlaying)
                    continue;

                channelIndex = loopIndex;
                if (sfx.IsVoice())
                    sfxPlayers[loopIndex].volume = sfxVoiceVolume;
                else
                    sfxPlayers[loopIndex].volume = sfxVolume;            
    
                sfxPlayers[loopIndex].clip = data.audioClip;
                sfxPlayers[loopIndex].Play();
                //Debug.Log($"loopIndex={loopIndex}, sfx={sfx}");
                break;
            }
        }
    }

    public void PlayCongratulationVoice(ECongratulationType congratulationType)
    {
        switch (congratulationType)
        {
            case ECongratulationType.GoodJob:
                PlaySfx(ESfx.GoodJob);
                break;
            case ECongratulationType.Great:
                PlaySfx(ESfx.Great);
                break;
            case ECongratulationType.Excellent:
                PlaySfx(ESfx.Excellent);
                break;
            case ECongratulationType.Perfect:
                PlaySfx(ESfx.Perfect);
                break;
        }
    }

}
