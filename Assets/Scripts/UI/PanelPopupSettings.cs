
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class PanelPopupSettings : UIPanelPopup
{
    [Header("Sound")]
    public GameObject btnSound;
    public GameObject objSoundOn;
    public GameObject objSoundOff;

    [Header("BGM")]
    public GameObject btnBGM;
    public GameObject objBGMOn;
    public GameObject objBGMOff;

    [Header("Vibration")]
    public GameObject btnVibration;
    public GameObject objVibrationOn;
    public GameObject objVibrationOff;

    [Header("Home")]
    public GameObject btnHome;

    [Header("Replay")]
    public GameObject btnReplay;

    [Header("Other")]
    public GameObject objPopup;
    public GameObject btnBack;

    void Awake()
    {
        btnBack.SafeSetClickEvent(OnClick);
        btnSound.SafeSetClickEvent(OnClick);
        btnBGM.SafeSetClickEvent(OnClick);
        btnVibration.SafeSetClickEvent(OnClick);
        btnHome.SafeSetClickEvent(OnClick);
        btnReplay.SafeSetClickEvent(OnClick);
    }

    protected override Tween OnIn()
    {         
        SetSoundOnOff();
        SetBGMOnOff();
        SetVibrationOnOff();

        var intro = objPopup.transform.DOScale(1f, 0.2f).From(0f).SetEase(Ease.OutCubic); 
        return intro;
    }

    protected override Tween OnOut()
    {     
        var outro = objPopup.transform.DOScale(0f, 0.2f).SetEase(Ease.OutCubic);
        return outro;
    }

    private void SetSoundOnOff()
    {
        var isOn = AudioManager.Instance.IsOn;
        objSoundOn.SetActive(isOn);
        objSoundOff.SetActive(!isOn);
    }

    private void SetBGMOnOff()
    {
        var isOn = AudioManager.Instance.IsBgmOn;
        objBGMOn.SetActive(isOn);
        objBGMOff.SetActive(!isOn);
    }

    private void SetVibrationOnOff()
    {
        var isOn = VibrationManager.Instance.IsOn;
        objVibrationOn.SetActive(isOn);
        objVibrationOff.SetActive(!isOn);
    }

    void OnClick(GameObject obj)
    {
        if (obj == btnSound)
        {
            var isOn = !AudioManager.Instance.IsOn;
            objSoundOn.SetActive(isOn);
            objSoundOff.SetActive(!isOn);
            AudioManager.Instance.IsOn = isOn; 
        }
        else if (obj == btnBack)
        {
            GameManager.Instance.isTouchDefense = false;
            PanelManager.HideLastPanel();
        }
        else if (obj == btnBGM)
        {
            var isOn = !AudioManager.Instance.IsBgmOn;
            objBGMOn.SetActive(isOn);
            objBGMOff.SetActive(!isOn);
            AudioManager.Instance.IsBgmOn = isOn;
        }
        else if (obj == btnVibration)
        {
            var isOn = !VibrationManager.Instance.IsOn;
            objVibrationOn.SetActive(isOn);
            objVibrationOff.SetActive(!isOn);
            VibrationManager.Instance.IsOn = isOn;
        }
        else if (obj == btnReplay)
        {
            // 재시작
            GameManager.Instance.Restart(false);
        }
        else if (obj == btnHome)
        {
            // 메인 페이즈 이동
            PhaseManager.Instance.MovePhaseMain();
        }
    }
}
