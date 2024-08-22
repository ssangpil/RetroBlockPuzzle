using System;
using System.IO.Pipes;
using System.Transactions;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class PanelPopupGameOver : UIPanelPopup
{
    public ScoreBoard scoreBoard;
    public BestScoreBar bestScoreBar;
    public GameObject bestScoreText;
    public GameObject btnRevive;
    public GameObject btnRestart;
    public GameObject gameOverImage;
    public GameObject bestScoreImage;
    public GameObject bestScoreEffect;

    private Vector3 m_OriginalRestartPosition;

    void Awake()
    {
        btnRevive.SafeSetClickEvent(OnClick);
        btnRestart.SafeSetClickEvent(OnClick);
        m_OriginalRestartPosition = btnRestart.transform.localPosition;
    }

    protected override Tween OnIn()
    {
        scoreBoard.UpdateScoreText(0, GameManager.Instance.Score);
        if (GameManager.Instance.Score >= GameManager.Instance.BestScore)
        {
            gameOverImage.SetActive(false);
            bestScoreImage.SetActive(true);
            bestScoreEffect.SetActive(true);
            bestScoreBar.gameObject.SetActive(false);
            bestScoreText.SetActive(true);
        }
        else
        {
            gameOverImage.SetActive(true);
            bestScoreImage.SetActive(false);
            bestScoreEffect.SetActive(false);
            bestScoreText.SetActive(false);
            bestScoreBar.gameObject.SetActive(true);
            bestScoreBar.UpdateBestScoreText(0, GameManager.Instance.BestScore);
        }

        if (0 < GameManager.Instance.LimitReviveCnt)
        {
            btnRevive.SetActive(true);
            btnRestart.transform.localPosition = m_OriginalRestartPosition;
        }
        else
        {
            btnRevive.SetActive(false);
            btnRestart.transform.localPosition = btnRevive.transform.localPosition;
        }
        
        return null;
    }

    protected override Tween OnOut()
    {
        bestScoreBar.gameObject.SetActive(true);
        return base.OnOut();
    }

    private void OnClick(GameObject obj)
    {
        if (obj == btnRevive)
        {
            // 기회 부여
            AdMobManager.Instance.ShowRewardedAd(Revive);
        }
        else if (obj == btnRestart)
        {
            // 재시작
            GameManager.Instance.Restart(true);
        }
    }

    private void Revive(Error error)
    {
        switch (error.ErrorCode)
        {
            case 0: 
                {
                    // 게임오버 팝업창 닫기
                    PanelManager.HideLastPanel();
                    GameManager.Instance.Revive();
                }
                break;
            default: 
                {
                    // 에러 문구 출력
                    PanelManager.Instance.SetNoticePopup(ELogLevel.ERROR.ToString(), error.Message, () => 
                    {
                        btnRevive.SetActive(false);
                        btnRestart.transform.localPosition = btnRevive.transform.localPosition;
                    });                    
                }
                break;
        }
    }
}
