using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Def;

public class IngameUI : MonoBehaviour
{
    #region Inspector

    public UISlider HPBar;
    public UILabel  scoreText;
    public UILabel  resourcesText;
    public UILabel  waveText;
    public UILabel  nextWaveText;
    public UILabel  loadedSubWeaponBulletCnt;
    public UIButton weaponUpgBtn;
    public UIButton fireSubWeaponBtn;
    public UISprite subWeaponLoadCoolTimerImg;
    public UISprite subWeaponFireCoolTimerImg;

    #endregion

    private void Start()
    {
        nextWaveText.gameObject.SetActive(false);
        SetSubWeaponFireCoolTimer(1, 1);
    }

    /// <summary>
    /// HP바 값 설정.
    /// </summary>
    /// <param name="maxHP"></param>
    /// <param name="curHP"></param>
    public void SetHPBar(int maxHP, int curHP)
    {
        HPBar.value = (float)curHP / (float)maxHP;
    }

    /// <summary>
    /// 점수 텍스트 설정.
    /// </summary>
    /// <param name="score"></param>
    public void SetScore(int score)
    {
        scoreText.text = score.ToString();
    }

    /// <summary>
    /// 획득 자원 텍스트 설정.
    /// </summary>
    /// <param name="resources"></param>
    /// <param name="needResc"></param>
    public void SetResources(int resources, int needResc)
    {
        //획득 자원이 강화 필요 자원보다 많으면 강화 버튼 활성화.
        resourcesText.text = string.Format("{0}/{1}", resources, needResc);
        weaponUpgBtn.isEnabled = resources >= needResc;
    }

    /// <summary>
    /// 다음 웨이브 표시 UI 설정.
    /// </summary>
    /// <param name="wave"></param>
    public void SetNextWaveUI(int wave)
    {
        SetWaveUI(wave);
    }

    /// <summary>
    /// 현재 웨이브 UI 표시.
    /// </summary>
    /// <param name="wave"></param>
    private void SetWaveUI(int wave)
    {
        nextWaveText.gameObject.SetActive(false);
        waveText.text = string.Format("WAVE {0}", wave);
    }

    /// <summary>
    /// 무기 업그레이드 버튼 클릭 설정.
    /// </summary>
    public void OnUpgradeBtnClick()
    {
        BattleManager.Instance.UpgradeWeaponLevel();
        TweenPosition tween = UIManager.Instance.LoadUI("UpgradeLabel", transform).GetComponent<TweenPosition>();
        tween.PlayForward();
    }

    /// <summary>
    /// 정지 버튼 클릭 설정.
    /// </summary>
    public void OnPauseBtnClick()
    {
        UIManager.Instance.LoadPopupUI("PauseUI");
        BattleManager.Instance.Pause();
    }

    /// <summary>
    /// 전략 무기 발사 버튼 클릭 설정.
    /// </summary>
    public void OnFireSubweaponBtnClick()
    {
        BattleManager.Instance.FireSubweapon();
    }

    /// <summary>
    /// 전략 무기 장전 타이머 값 설정.
    /// </summary>
    /// <param name="coolTime"></param>
    /// <param name="curTime"></param>
    public void SetSubWeaponLoadCoolTimer(float coolTime, float curTime)
    {
        subWeaponLoadCoolTimerImg.fillAmount = (coolTime - curTime) / coolTime;
    }

    /// <summary>
    /// 전략 무기 발사 딜레이 타이머 값 설정.
    /// </summary>
    /// <param name="coolTime"></param>
    /// <param name="curTime"></param>
    public void SetSubWeaponFireCoolTimer(float coolTime, float curTime)
    {
        subWeaponFireCoolTimerImg.fillAmount = (coolTime - curTime) / coolTime;
    }

    /// <summary>
    /// 전략 무기 장전 갯수 레이블 설정.
    /// </summary>
    /// <param name="count"></param>
    public void SetSubWeaponLoadedCnt(int count)
    {
        loadedSubWeaponBulletCnt.text = count.ToString();
    }
}
