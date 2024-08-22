using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoSingleton<GameManager>
{
    [System.Serializable]
    public class GameData
    {
        public int TurnCnt;
        public int StarCnt;
        public int Score;
        public int LimitReviveCnt;
        public GridSquareData[] GridSquareDataList;
        public ShapeSquareData[] ShapeSquareDataList;
    }

    [Serializable]
    public class BestScoreData
    {
        public int Value;
    }


    [Serializable]
    public class GridSquareData
    {
        public int SquareIndex;
        public bool SquareOccupied;
        public bool IsBonusIncluded;
        public ESquareColor ActiveSquareColor;
    }

    [Serializable]
    public class ShapeSquareData
    {
        public int shapeDataId;
        public int BonusSquareIndex;
        public ESquareColor SquareColor;
    }

    public class CreateShapeData
    {
        public int Index;
        public int BonusSquareIndex;
        public ShapeData ShapeData;
        public SquareTextureData.TextureData TextureData;
    }

    public const string GAME_DATA_KEY = "game_data";
    public const string BEST_SCORE_DATA_KEY = "best_score_data";

    public SquareTextureData squareTextureData;  
    public List<ShapeData> shapeDataList;

    [HideInInspector] public bool isTouchDefense;
    [HideInInspector] public bool isGameOver = false;
    [HideInInspector] public List<CreateShapeData> InitShapeDataList = new List<CreateShapeData>();

    // 현재 콤보 카운트
    private int m_CurrentComboCnt;
    private int m_CheckComboAllowedCnt = 0;
    private int m_CheckComboCnt;
    private int m_ComboTurnCnt = 3;
    
    private int m_TurnCnt = 1;
    private int m_StarCnt = 0;
    private int m_Score = 0;
    private int m_BestScore = 0;
    private bool m_IsBonusTime = false;
    private int m_LimitReviveCnt;
    private bool m_IsPaused = false;
    private int m_PlayCnt = 0;

    public int TurnCnt => m_TurnCnt;
    public int StarCnt => m_StarCnt;
    public int Score => m_Score;
    public int BestScore => m_BestScore;
    public bool IsBonusTime => m_IsBonusTime;
    public int LimitReviveCnt => m_LimitReviveCnt;
    public int PlayCnt => m_PlayCnt;

    protected override void OnAwake()
    {
        Application.targetFrameRate = 60;
        Input.multiTouchEnabled = false;

        if (!Application.isEditor)
            Debug.unityLogger.logEnabled = false;   

        DOTween.SetTweensCapacity(500, 50);

        m_PlayCnt = PlayerPrefs.GetInt("PlayCnt", 0);
    }    

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            m_IsPaused = true;   
        }
        else
        {
            if (m_IsPaused)
            {
                m_IsPaused = false;

                if (isGameOver)
                {
                    var obj = PrefabManager.Get<PanelPopupGameOver>("PanelPopupGameOver");  
                    if (null != obj)
                        PanelManager.Play(obj, EPanelShowBehaviour.HIDE_PREVIOUS);    
                }
            }
        }
    }

    public void Init()
    {
        m_CurrentComboCnt = 0;
        m_CheckComboAllowedCnt = 0;
        m_CheckComboCnt = 0;
        m_ComboTurnCnt = 3;
        m_StarCnt = 0;
        m_Score = 0;
        m_BestScore = 0;
        m_IsPaused = false;
        m_IsBonusTime = false;     
        m_LimitReviveCnt = Config.LimitReviveCnt;        
    }

    public void SaveGameData()
    {
        var gameData = new GameData
        {
            TurnCnt = TurnCnt,
            Score = Score,
            StarCnt = StarCnt,     
            LimitReviveCnt = LimitReviveCnt,
        };

        var panelGame = PanelManager.Instance.FindPanel<PanelGame>("PanelGame");
        if (null == panelGame)
            return;

        var gridSquares = panelGame.Grid.gridSquares;
        gameData.GridSquareDataList = new GridSquareData[gridSquares.Count];
        for (var i = 0; i < gridSquares.Count; i++)
        {
            var obj = gridSquares[i].GetComponent<GridSquare>();
            gameData.GridSquareDataList[i] = new GridSquareData
            {
                SquareIndex = obj.SquareIndex,
                SquareOccupied = obj.SquareOccupied,
                IsBonusIncluded = obj.IsBonusIncluded,
                ActiveSquareColor = obj.ActiveSquareColor
            };
        }
        
        var shapeList = panelGame.ShapeList;
        gameData.ShapeSquareDataList = new ShapeSquareData[shapeList.Count];
        for (var i = 0; i < shapeList.Count; i++)
        {
            var data = new ShapeSquareData();
            var shape = shapeList[i];            
            if (shape.IsOnStartPosition() && shape.IsAnyOfShapeSquareActive())
            {
                data.shapeDataId = shape.shapeData.id;
                data.SquareColor = shape.textureData.squareColor;
                data.BonusSquareIndex = shape.bonusSquareIndex;
            }

            gameData.ShapeSquareDataList[i] = data;
        }

        BinaryDataStream.Save(gameData, GAME_DATA_KEY);

        var bestScoreData = new BestScoreData { Value = BestScore };
        BinaryDataStream.Save(bestScoreData, BEST_SCORE_DATA_KEY);
    }

    public void LoadGameData()
    {
        var panelGame = PanelManager.Instance.FindPanel<PanelGame>("PanelGame");
        if (null == panelGame)
            return;

        var gameData = BinaryDataStream.Read<GameData>(GAME_DATA_KEY);
        if (null != gameData)
        {
            m_TurnCnt = gameData.TurnCnt;
            m_StarCnt = gameData.StarCnt;
            m_Score = gameData.Score;
            m_LimitReviveCnt = gameData.LimitReviveCnt;
            
            panelGame.UpdateScoreText(0, m_Score);
            panelGame.EnableStar(m_StarCnt);

            var gridSquares = panelGame.Grid.gridSquares;
            for (var i = 0; i < gameData.GridSquareDataList.Length; i++)
            {
                var data = gameData.GridSquareDataList[i];
                var obj = gridSquares[i].GetComponent<GridSquare>();
                
                obj.SquareIndex = data.SquareIndex;
                obj.SquareOccupied = data.SquareOccupied;       
                obj.IsBonusIncluded = data.IsBonusIncluded;
                var textureData = squareTextureData.GetTextureData(data.ActiveSquareColor);
                if (null != textureData)
                {   
                    obj.ActiveSquareColor = data.ActiveSquareColor;
                    obj.activeImage.sprite = textureData.texture;
                    obj.activeImage.gameObject.SetActive(true);
                }
            }

            for (var index = 0; index < gameData.ShapeSquareDataList.Length; index++)
            {
                var data = gameData.ShapeSquareDataList[index];
                if (0 >= data.shapeDataId)
                    continue;
   
                InitShapeDataList.Add(new CreateShapeData 
                {
                    Index = index,
                    BonusSquareIndex = data.BonusSquareIndex,
                    ShapeData = shapeDataList[data.shapeDataId - 1],
                    TextureData = squareTextureData.GetTextureData(data.SquareColor)
                });
            }
        }

        var bestScoreData = BinaryDataStream.Read<BestScoreData>(BEST_SCORE_DATA_KEY);
        if (null != bestScoreData)
        {
            m_BestScore = bestScoreData.Value;
            panelGame.UpdateBestScoreText(0, m_BestScore);
        }        
    }

    public void ClearGameData()
    {
        BinaryDataStream.Delete(GAME_DATA_KEY);
        PanelManager.Instance.DeactivateAllShapes();
    }

    public void IncreaseTurn()
    {
        m_TurnCnt++;
    }

    public void IncreasePlayCnt()
    {
        m_PlayCnt++;
        PlayerPrefs.SetInt("PlayCnt", m_PlayCnt);
        PlayerPrefs.SetInt("IsNRU", 0);
    }

    public void GainStar(int starCnt)
    {
        var panelGame = PanelManager.Instance.FindPanel<PanelGame>("PanelGame");
        if (null != panelGame)
        {
            panelGame.GainStar(starCnt);
            m_StarCnt = panelGame.StarCnt;
        }
    }

    public void ResetCombo()
    {
        if (0 < m_CheckComboAllowedCnt)
        {
            m_CheckComboAllowedCnt--;
            if (0 == m_CheckComboAllowedCnt)
            {
                m_CurrentComboCnt = 0;
                m_CheckComboCnt = 0;
            }
        } 
    }

    public bool IncreaseCombo()
    {
        m_CheckComboAllowedCnt = m_ComboTurnCnt;
        if (0 < m_CheckComboCnt++)
        {
            m_CurrentComboCnt++;
            return true;
        }

        return false;
    }

    public int GetComboCnt()
    {
        return m_CurrentComboCnt;
    }

    public void SetIsBonusTime(bool state)
    {
        m_IsBonusTime = state;
        if (!state)
        {
            SaveGameData();
        }
    }

    public void ResultShapePlaced(int shapeCnt, int lineCnt, Vector3 position)
    {
        var isShowCombo = false;
        if (0 < lineCnt)
        {
            // 콤보 증가
            isShowCombo = IncreaseCombo();
        }
        else
        {   
            // 콤보 초기화
            ResetCombo();  
        }

        // 점수 계산
        var comboCnt = GetComboCnt();
        var shapeScore = shapeCnt;
        var lineScore = 0;

        // 줄 점수
        if (0 < lineCnt)
        {
            lineScore += 1 == lineCnt ? (comboCnt + 1) * 10 : (comboCnt + 1) * lineCnt * (lineCnt - 1) * 10;    
        }        

        // 보너스 타임 점수
        if (m_IsBonusTime)
        {
            shapeScore *= Config.BonusEffectMultiplier;
            lineScore *= Config.BonusEffectMultiplier;
        }
        
        var prevScore = m_Score;
        m_Score += shapeScore + lineScore;

        var prevBestScore = m_BestScore;
        var isBestScore = false;
        if (m_Score > m_BestScore)
        {
            isBestScore = true;
            m_BestScore = m_Score;                   
        }

        SaveGameData();
   
        var panelGame = PanelManager.Instance.FindPanel<PanelGame>("PanelGame");
        if (null != panelGame)
        {
            panelGame.UpdateScoreText(prevScore, m_Score);

            if (isBestScore)
                panelGame.UpdateBestScoreText(prevBestScore, m_BestScore);

            if (!panelGame.IsShowBonusTime)
            {
                var seq = DOTween.Sequence();
                // 콤보 표시
                if (isShowCombo)
                    seq.Append(panelGame.ShowCombo(comboCnt));

                // 점수, 멘트 표시
                if (0 < lineScore)
                    seq.Append(panelGame.ShowCongratulation(lineScore, lineCnt, position));

                StartCoroutine(seq.WaitForCompletionCoroutine());
            }

            if (panelGame.CheckAndCreateShapes())
                IncreaseTurn();

            if (0 >= panelGame.GetPlacedOnGridShapeCnt())
                GameOver();
            else
                isTouchDefense = false;
        }
    }

    public void GameOver()
    {
        if (isGameOver)
            return;

        isGameOver = true;
        isTouchDefense = true;

        SaveGameData();
        AudioManager.Instance.PlayBgm(false);

        StartCoroutine(GameOverAnimation());
    }

    public void Revive()
    {
        isGameOver = false;
        isTouchDefense = true;
        m_LimitReviveCnt--;

        if (AudioManager.Instance.IsBgmOn)
            AudioManager.Instance.PlayBgm(true);
        
        StartCoroutine(ReviveAnimation());
    }

    IEnumerator ReviveAnimation()
    {
        var panelGame = PanelManager.Instance.FindPanel<PanelGame>("PanelGame");
        if (null != panelGame)
        {
            var sequence = panelGame.CreateTopDownShapeAnimation();
            yield return sequence.WaitForCompletion();

            panelGame.CreateShapes();
        }

        isTouchDefense = false;
    }

    public void Restart(bool gameOvered)
    {
        if (gameOvered)
        {
            isGameOver = false;
            isTouchDefense = false;

            if (AudioManager.Instance.IsBgmOn)
                AudioManager.Instance.PlayBgm(true);
        }
        
        var count = m_PlayCnt;
        IncreasePlayCnt();

        if (Config.ShowAdPlayCnt <= count)
        {
            // 전면 광고 보기
            AdMobManager.Instance.ShowInterstitialAd((error) =>
            {
                if (0 != error.ErrorCode)
                {
                    PanelManager.Instance.SetNoticePopup(ELogLevel.ERROR.ToString(), error.Message, MovePhaseGame);
                }
                else
                {
                    MovePhaseGame();
                }
            });
        }
        else
        {
            MovePhaseGame();
        }
    }

    private void MovePhaseGame()
    {
        ClearGameData();
        PhaseManager.Instance.MovePhaseGame(); 
    }

    IEnumerator GameOverAnimation()
    {   
        var panelGame = PanelManager.Instance.FindPanel<PanelGame>("PanelGame");
        if (null == panelGame)
            yield return null;     

        var sequence = DOTween.Sequence();
        sequence.AppendInterval(1f);
        sequence.Append(panelGame.CreateDownTopShapeAnimation());
        sequence.AppendInterval(1f);
        yield return sequence.WaitForCompletion();    

        DOTween.KillAll();
        if (m_IsBonusTime)
            panelGame.StopBonusTime();

        var obj = PrefabManager.Get<PanelPopupGameOver>("PanelPopupGameOver");  
        if (null != obj)
            PanelManager.Play(obj, EPanelShowBehaviour.HIDE_PREVIOUS);                     
    }

    public void SetTouchDefense(bool state)
    {
        isTouchDefense = state;
    }    
}
