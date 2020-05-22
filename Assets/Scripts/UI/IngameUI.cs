using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Def;

public class IngameUI : MonoBehaviour
{
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

    public WeaponIconUI weaponIcon;

    private void Start()
    {
        nextWaveText.gameObject.SetActive(false);
        SetSubWeaponFireCoolTimer(1, 1);
    }

    //무기 아이콘 설정.
    public void SetWeaponIcon(InfoWeapon weaponInfo)
    {
        weaponIcon.Init(weaponInfo.iconPath);
    }

    //HP바 값 설정.
    public void SetHPBar(int maxHP, int curHP)
    {
        HPBar.value = (float)curHP / (float)maxHP;
    }

    //점수 텍스트 설정.
    public void SetScore(int score)
    {
        scoreText.text = score.ToString();
    }

    //획득 자원 텍스트 설정.
    public void SetResources(int resources, int needResc)
    {
        //획득 자원이 강화 필요 자원보다 많으면 강화 버튼 활성화.
        resourcesText.text = string.Format("{0}/{1}", resources, needResc);
        weaponUpgBtn.isEnabled = resources >= needResc;
    }

    //다음 웨이브 표시 UI 설정.
    public void SetNextWaveUI(int wave)
    {
        //StartCoroutine(ShowNextWaveUI(wave));
        SetWaveUI(wave);
    }

    ////게임 실행중이 아니면 실행 될 때 까지 기다렸다가 중앙의 다음 웨이브 UI 표시.
    //IEnumerator ShowNextWaveUI(int wave)
    //{
    //    while(BattleManager.Instance.GameState != DefEnum.GameState.PLAY)
    //    {
    //        yield return new WaitForEndOfFrame();
    //    }

    //    nextWaveText.gameObject.SetActive(true);
    //    nextWaveText.text = string.Format("WAVE {0}", wave);

    //    StartCoroutine(SetWaveUI(wave));
    //}

    ////현재 웨이브 UI 표시.
    //IEnumerator SetWaveUI(int wave)
    //{
    //    yield return new WaitForSeconds(1f);

    //    nextWaveText.gameObject.SetActive(false);
    //    waveText.text = string.Format("WAVE {0}", wave);
    //}

    //현재 웨이브 UI 표시.
    void SetWaveUI(int wave)
    {
        nextWaveText.gameObject.SetActive(false);
        waveText.text = string.Format("WAVE {0}", wave);
    }

    //무기 업그레이드 버튼 클릭 설정.
    public void OnUpgradeBtnClick()
    {
        BattleManager.Instance.UpgradeWeaponLevel();
        TweenPosition tween = UIManager.Instance.LoadUI("UpgradeLabel", transform).GetComponent<TweenPosition>();
        tween.PlayForward();
    }

    //정지 버튼 클릭 설정.
    public void OnPauseBtnClick()
    {
        UIManager.Instance.LoadPopupUI("PauseUI");
        BattleManager.Instance.Pause();
    }

    //전략 무기 발사 버튼 클릭 설정.
    public void OnFireSubweaponBtnClick()
    {
        BattleManager.Instance.FireSubweapon();
    }

    //전략 무기 장전 타이머 값 설정.
    public void SetSubWeaponLoadCoolTimer(float coolTime, float curTime)
    {
        subWeaponLoadCoolTimerImg.fillAmount = (coolTime - curTime) / coolTime;
    }

    //전략 무기 발사 딜레이 타이머 값 설정.
    public void SetSubWeaponFireCoolTimer(float coolTime, float curTime)
    {
        subWeaponFireCoolTimerImg.fillAmount = (coolTime - curTime) / coolTime;
    }

    //전략 무기 장전 갯수 레이블 설정.
    public void SetSubWeaponLoadedCnt(int count)
    {
        loadedSubWeaponBulletCnt.text = count.ToString();
    }
}
