using UnityEngine;
using System.Collections;

public class ResultUI : MonoBehaviour
{
    private const float UPDATE_SCORE_TIME = 0.04f;

    #region Inspector

    public UILabel successLabel;
    public UILabel failLabel;
    public UILabel scoreLabel;

    #endregion

    private int _destinationScore;
    private int _currentScore;
    private float _showUITime;

    /// <summary>
    /// 성공/실패 텍스트 출력, 점수 서서히 올라가도록 
    /// </summary>
    /// <param name="success"></param>
    /// <param name="score"></param>
    public void Init(bool success, int score)
    {
        successLabel.gameObject.SetActive(success);
        failLabel.gameObject.SetActive(!success);

        _destinationScore = score;
        _currentScore = 0;
        scoreLabel.text = _currentScore.ToString();

        _showUITime = float.Parse(InfoManager.Instance.infoGlobalList["ShowResultUI"].value);

        StartCoroutine(UpdateScoreText());
    }

    /// <summary>
    /// 점수 텍스트 점차적으로 증가.
    /// </summary>
    /// <returns></returns>
    private IEnumerator UpdateScoreText()
    {
        int UpdateScore = (int)(_destinationScore * 0.04f);
        while (_destinationScore > _currentScore)
        {
            yield return YieldCache.WaitForSeconds(UPDATE_SCORE_TIME);

            _currentScore += UpdateScore;
            scoreLabel.text = _currentScore.ToString();
        }

        scoreLabel.text = _destinationScore.ToString();

        StartCoroutine(EndShowUI());
    }

    /// <summary>
    /// 일정 시간 후 또는 화면 터치하면 인트로 화면으로 전환.
    /// </summary>
    /// <returns></returns>
    private IEnumerator EndShowUI()
    {
        yield return YieldCache.WaitForSeconds(_showUITime);

        UIManager.Instance.ClosePopupUI();
        BattleManager.Instance.GoIntro();
        AdManager.Instance.ShowInterstitialAd();
    }
}
