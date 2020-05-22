using UnityEngine;
using System.Collections;

public class ResultUI : MonoBehaviour
{
    public UILabel successLabel;
    public UILabel failLabel;
    public UILabel scoreLabel;

    int            destinationScore;
    int            currentScore;
    float          showUITime;
    float          updateTime;

    //성공/실패 텍스트 출력, 점수 서서히 올라가도록 
    public void Init(bool success, int score)
    {
        successLabel.gameObject.SetActive(success);
        failLabel.gameObject.SetActive(!success);

        destinationScore = score;
        currentScore = 0;
        scoreLabel.text = currentScore.ToString();

        showUITime = (float)System.Convert.ToDouble(InfoManager.Instance.infoGlobalList["ShowResultUI"].value);

        StartCoroutine(UpdateScoreText());
    }

    //점수 텍스트 점차적으로 증가.
    IEnumerator UpdateScoreText()
    {
        int UpdateScore = destinationScore / 25;
        while (destinationScore > currentScore)
        {
            yield return new WaitForEndOfFrame();

            updateTime += Time.deltaTime;

            if (updateTime >= 0.04f)
            {
                currentScore += UpdateScore;
                scoreLabel.text = currentScore.ToString();

                updateTime = 0f;
            }
        }

        scoreLabel.text = destinationScore.ToString();

        StartCoroutine(EndShowUI());
    }

    //일정 시간 후 또는 화면 터치하면 인트로 화면으로 전환.
    IEnumerator EndShowUI()
    {
        updateTime = 0f;
        while(updateTime < showUITime)
        {
            if (Input.GetMouseButtonDown(0))
            {
                updateTime = showUITime;
            }
            else
            {
                updateTime += Time.deltaTime;
            }
            yield return null;
        }

        UIManager.Instance.ClosePopupUI();
        BattleManager.Instance.GoIntro();
        AdManager.Instance.ShowInterstitialAd();
    }
}
