using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PanelGame : UIPanelPopup
{   
    [SerializeField] GameObject m_BtnSettings;
    [SerializeField] ScoreBoard m_ScoreBoard;
    [SerializeField] BestScoreBar m_BestScoreBar;
    [SerializeField] BonusTimeGaugeBar m_BonusTimeGaugeBar;
    [SerializeField] List<Shape> m_ShapeList;
    [SerializeField] Grid m_Grid;
    [SerializeField] ComboWriting m_ComboWriting;
    [SerializeField] CongratulationWriting m_CongratulationWriting;
    [SerializeField] GameObject m_BonusTimeWriting;

    public Grid Grid => m_Grid;
    public List<Shape> ShapeList => m_ShapeList;
    public bool IsShowBonusTime => m_BonusTimeWriting.activeSelf;
    public int StarCnt => m_BonusTimeGaugeBar.StarIndex + 1;

    private float m_OnApplicationTime = -1f;

    void Awake()
    {
        m_BtnSettings.SafeSetClickEvent(OnClick);
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            m_OnApplicationTime = TimeHelper.realtimeSinceStartup;
        }
        else
        {
            if (m_OnApplicationTime >= 0f 
                && (TimeHelper.realtimeSinceStartup - m_OnApplicationTime >= 60 * 10)
                && Config.ShowAdPlayCnt <= GameManager.Instance.PlayCnt)
            {
                // 전면 광고 보기
                AdMobManager.Instance.ShowInterstitialAd((error) =>
                {
                    Debug.LogError("Failed to show ad: error=" + error.Message);
                });
            }
        }
    }

    protected override void OnUpdate()
    {
        foreach (var shape in m_ShapeList)
        {
            if (!shape.IsMoving && shape.IsOnStartPosition() && shape.IsAnyOfShapeSquareActive())
            {
                shape.MoveToStartPosition();
            }
        }
    }

    protected override Tween OnIn()
    {
        EffectManager.Instance.ShowEffect(EEffectType.Background, transform);

        if (AudioManager.Instance.IsBgmOn)
        {
            AudioManager.Instance.PlayBgm(true);
        }
        
        InitUI();

        // 보드판 클리어
        m_Grid.Clear();

        // 게임 정보 로드
        GameManager.Instance.LoadGameData();     

        // 스타트 블럭 생성
        var seq = DOTween.Sequence();
        seq.AppendInterval(0.5f);
        seq.Append(CreateDownTopShapeAnimation());
        seq.Append(CreateTopDownShapeAnimation());
        seq.OnComplete(CreateShapes);
        return seq;
    }

    public Sequence CreateDownTopShapeAnimation()
    {
        var sequences = DOTween.Sequence();

        for (var row = m_Grid.rows - 1; row >= 0; row--)
        {
            var seq = DOTween.Sequence();
            for (var column = 0; column < m_Grid.columns; column++)
            {
                var obj = m_Grid.GetGridSquare(row, column);          
                if (obj.SquareOccupied)
                    continue;

                seq.Join(obj.activeImage.DOFade(0f, 0f));
                seq.AppendCallback(() => 
                {
                    obj.activeImage.sprite = GameManager.Instance.squareTextureData.GetRandomTextureData().texture;
                    obj.activeImage.gameObject.SetActive(true);
                });
                seq.Join(obj.activeImage.DOFade(1f, 0f));
            }

            sequences.AppendInterval(0.1f);
            sequences.AppendCallback(() => AudioManager.Instance.PlaySfx(ESfx.Filled));
            sequences.Append(seq);
        }

        return sequences;
    }

    public Sequence CreateTopDownShapeAnimation()
    {
        var sequences = DOTween.Sequence();
        for (var row = 0; row < m_Grid.rows; row++)
        {
            var seq = DOTween.Sequence();
            for (var column = 0; column < m_Grid.columns; column++)
            {
                var obj = m_Grid.GetGridSquare(row, column);    
                if (obj.SquareOccupied)
                    continue;

                seq.Join(obj.activeImage.DOFade(0f, 0f));
            }

            sequences.AppendInterval(0.1f);
            sequences.AppendCallback(() => AudioManager.Instance.PlaySfx(ESfx.Filled));
            sequences.Append(seq);
        }
        sequences.AppendCallback(() =>
        {
            foreach (var square in m_Grid.gridSquares)
            {
                var obj = square.GetComponent<GridSquare>();
                if (obj.SquareOccupied)
                    continue;

                obj.activeImage.gameObject.SetActive(false);
                obj.activeImage.DOFade(1f, 0f);
            }
        });
        return sequences;
    }

    protected override Tween OnOut()
    {
        return base.OnOut();
    }

    private void InitUI()
    {
        m_ScoreBoard.UpdateScoreText(0, 0);
        m_BestScoreBar.UpdateBestScoreText(0, 0);
        m_BonusTimeGaugeBar.Clear();
        m_ComboWriting.gameObject.SetActive(false);
        m_CongratulationWriting.gameObject.SetActive(false);
        m_BonusTimeWriting.SetActive(false);
    }

    private void OnClick(GameObject obj)
    {
        if (obj == m_BtnSettings)
        {
            var settings = PrefabManager.Get<PanelPopupSettings>("PanelPopupSettings");
            if (null != settings)
            {
                GameManager.Instance.isTouchDefense = true;
                PanelManager.Play(settings, EPanelShowBehaviour.KEEP_PREVIOUS);
            }
        }
    }

    public void UpdateScoreText(int prevScore, int currentScore)
    {
        m_ScoreBoard.UpdateScoreText(prevScore, currentScore);
    }

    public void UpdateBestScoreText(int prevBestScore, int currentBestScore)
    {
        m_BestScoreBar.UpdateBestScoreText(prevBestScore, currentBestScore);
    }

    public void EnableStar(int starCnt)
    {
        m_BonusTimeGaugeBar.EnableStar(starCnt);
    }

    public void CreateShapes()
    {
        if (GameManager.Instance.InitShapeDataList.Any())
        {
            foreach (var data in GameManager.Instance.InitShapeDataList)
            {
                var shape = m_ShapeList[data.Index];
                shape.Create(data.ShapeData, data.TextureData);
                shape.ActivateShape();
                if (-1 != data.BonusSquareIndex)
                {
                    shape.SetBonus(data.BonusSquareIndex);
                }
            }

            GameManager.Instance.InitShapeDataList.Clear();
        }
        else
        {
            foreach (var shape in m_ShapeList)
            {
                if (!shape.IsMoving && shape.IsOnStartPosition() && shape.IsAnyOfShapeSquareActive())
                {
                    // 시작 포인트에 있는 블럭은 비활성함
                    shape.DeactivateShape();
                }
            }

            Board.AllocateShapeData(m_Grid, m_ShapeList);
        }

        if (0 >= GetPlacedOnGridShapeCnt())
        {
            GameManager.Instance.GameOver();
            return;
        }

        GameManager.Instance.SaveGameData();
    }

    public void DeactivateAllShapes()
    {
        foreach (var shape in m_ShapeList)
        {
            if (shape.IsOnStartPosition() && shape.IsAnyOfShapeSquareActive())
            {
                shape.DeactivateShape();
            }
        }
    }

    public Shape GetCurrentSelectedShape()
    {
        foreach (var shape in m_ShapeList)
        {
            if (!shape.IsOnStartPosition() && shape.IsAnyOfShapeSquareActive())
                return shape;
        }

        return null;
    }

    public bool CheckAndCreateShapes()
    {
        var usableCnt = 0;
        foreach (var shape in m_ShapeList)
        {
            if ((shape.IsMoving || shape.IsOnStartPosition()) && shape.IsAnyOfShapeSquareActive())
                usableCnt++;
        }

        if (0 >= usableCnt)
        {
            CreateShapes();
            return true;
        }

        return false;
    }

    public int GetPlacedOnGridShapeCnt()
    {
        var validShapeCnt = 0;
        // 왼쪽, 가운데, 오른쪽 블록들이 모두 놓을 수 있는지 확인
        foreach (var shape in m_ShapeList)
        {
            if (!shape.IsAnyOfShapeSquareActive())
                continue;

            // 보드 위에 놓을 곳이 있는지 검사
            var check = m_Grid.CheckIfShapeCanBePlacedOnGrid(shape.shapeData);
            if (check)
            {
                shape.ActivateShape();
                validShapeCnt++;
            }

            //Debug.Log("[CheckIfPlayerLost] index=" + index + ", validShapes=" + validShapes + ", check=" + check + ", total=" + shape.TotalSquareNumber);
        }
        return validShapeCnt;
    }

    public void GainStar(int starCnt)
    {
       m_BonusTimeGaugeBar.GainStar(starCnt);
    }

    public Tween ShowBonusTimeWriting()
    {
        var seq = DOTween.Sequence();
        seq.AppendCallback(() => 
        { 
            m_BonusTimeWriting.SetActive(true);        
            m_ComboWriting.gameObject.SetActive(false);
            m_CongratulationWriting.gameObject.SetActive(false);
            AudioManager.Instance.PlaySfx(ESfx.BonusTimed);
        });
        seq.Append(m_BonusTimeWriting.transform.DOScale(1.5f, 0.15f));
        seq.Append(m_BonusTimeWriting.transform.DOScale(1f, 0.15f));
        seq.AppendInterval(1f);
        seq.AppendCallback(() => m_BonusTimeWriting.SetActive(false));
        return seq;
    }

    public Tween ShowCombo(int comboCnt)
    {
        return m_ComboWriting.ShowCombo(comboCnt);
    }

    public Tween ShowCongratulation(int score, int lineCnt, Vector3 position)
    {
        return m_CongratulationWriting.ShowCongratulation(score, lineCnt, position);
    }

    public void StopBonusTime()
    {
        m_BonusTimeGaugeBar.StopBonusTime();
    }
}