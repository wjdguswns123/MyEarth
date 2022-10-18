using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Def;

public class BattleManager : Singleton<BattleManager>
{
    #region Inspector

    public IngameUI ingameUI;
    public Planet planet;
    public GameObject[] attackPoints;     //목적지 배열.

    #endregion

    public DefEnum.GameState GameState { get; private set; }

    public bool IsEnableDlbTouchFire { get; private set; }

    private List<Enemy> _liveEnemyList;
    private Enemy[] _arrivalEnemyAtkPoints; //목적지에 도착한 적 배열.
    
    private int _currentScore;
    private int _currentResources;
    private int _upgradeCost;
    private int _upgradeLV;
    private int _mainWeaponID;
    private bool _isGameResult;

    public void Awake()
    {
        //더블 터치로 전략무기 발사 가능 로컬에 저장된 값을 불러온다.
        IsEnableDlbTouchFire = PlayerPrefs.GetInt("OnDblTouchFire") != 0;
    }

    public void Init()
    {
        if(_liveEnemyList == null)
        {
            _liveEnemyList = new List<Enemy>();
        }
        _liveEnemyList.Clear();

        _arrivalEnemyAtkPoints = new Enemy[attackPoints.Length];
        for(int i = 0; i < _arrivalEnemyAtkPoints.Length; ++i)
        {
            _arrivalEnemyAtkPoints[i] = null;
        }

        //인트로 화면 출력.
        GameState = DefEnum.GameState.INTRO;
        UIManager.Instance.LoadUI("IntroUI");
    }

    //현재 살아있는 적 리스트에 적 추가.
    public GameObject AddEnemy(Enemy enemy)
    {
        _liveEnemyList.Add(enemy);

        //목적지 검색해서 반환.
        return SearchAttackPoint(enemy);
    }

    //해당 적 위치에서 가장 가까운 빈 공격 지점을 검색. 없으면 null 반환.
    public GameObject SearchAttackPoint(Enemy enemy)
    {
        int index = -1;
        float minLength = -1f;

        for(int i = 0; i < attackPoints.Length; ++i)
        {
            if(_arrivalEnemyAtkPoints[i] != null)
            {
                continue;
            }

            float dist = Vector3.Distance(enemy.transform.position, attackPoints[i].transform.position);
            if(minLength == -1f || minLength > dist)
            {
                minLength = dist;
                index = i;
            }
        }

        return index >= 0 ? attackPoints[index] : null;
    }

    //해당 공격 지점에 적이 도달하여 해당 지점을 목표로 가지던 모든 적에게 알림.
    public void ArrivalAttackPoint(Enemy enemy, GameObject point)
    {
        for(int i = 0; i < attackPoints.Length; ++i)
        {
            if(attackPoints[i] == point)
            {
                _arrivalEnemyAtkPoints[i] = enemy;

                for(int j = 0; j < _liveEnemyList.Count; ++j)
                {
                    _liveEnemyList[j].FullDestinationAttackPoint(attackPoints[i]);
                }
                break;
            }
        }
    }

    //가장 가까운 적 찾기.
    public Transform SearchNearEnemy(Transform searcher, float range)
    {
        int index = -1;
        float minLength = -1f;

        for(int i = 0; i < _liveEnemyList.Count; ++i)
        {
            float dist = Vector3.Distance(searcher.transform.position, _liveEnemyList[i].transform.position);
            if(range >= dist && (minLength == -1f || minLength > dist))
            {
                minLength = dist;
                index = i;
            }
        }

        return index >= 0 ? _liveEnemyList[index].transform : null;
    }

    //현재 살아있는 적 리스트에서 해당 적 제거.
    public void RemoveEnemy(Enemy enemy)
    {
        if(_liveEnemyList.Contains(enemy))
        {
            _liveEnemyList.Remove(enemy);
        }

        //제거한 적이 공격 지점에 있었으면 해당 지점에서 제일 가까운 목적지가 없는 적에게 목적지 변경 요청.
        for(int i = 0; i < _arrivalEnemyAtkPoints.Length; ++i)
        {
            if(enemy == _arrivalEnemyAtkPoints[i])
            {
                _arrivalEnemyAtkPoints[i] = null;
                Enemy result = SearchNearEnemy(i);
                if(result != null)
                {
                    result.ResetDestinationAttackPoint(attackPoints[i]);
                }
                break;
            }
        }
    }

    //해당 인덱스의 공격 지점과 가장 가까운 적 탐색.
    Enemy SearchNearEnemy(int atkIndex)
    {
        int index = -1;
        float minLength = -1f;
        for(int i = 0; i < _liveEnemyList.Count; ++i)
        {
            if(_liveEnemyList[i].State == DefEnum.EnemyState.MOVE)
            {
                float dist = Vector3.Distance(_liveEnemyList[i].transform.position, attackPoints[atkIndex].transform.position);
                if(minLength == -1f || minLength > dist)
                {
                    minLength = dist;
                    index = i;
                }
            }
        }

        return index >= 0 ? _liveEnemyList[index] : null;
    }

    //점수 추가.
    public void AddScore(int score)
    {
        _currentScore += score;
        ingameUI.SetScore(_currentScore);
    }

    //획득 자원 추가.
    public void AddResources(int resources)
    {
        _currentResources += resources;
        ingameUI.SetResources(_currentResources, _upgradeCost);
    }

    //전방 범위 공격. 해당 각도와 거리 안에 있는 적 공격.
    public void AngleRangeAttack(Transform atker, float angleRange, float range, int atk)
    {
        List<Enemy> attackedList = new List<Enemy>();
        for(int i = 0; i < _liveEnemyList.Count; ++i)
        {
            float dist = Vector3.Distance(_liveEnemyList[i].transform.position, atker.transform.position);
            float angle = Vector3.Angle(atker.forward, _liveEnemyList[i].transform.position - atker.position);
            if (angle <= angleRange / 2f && dist <= range)
            {
                //공격 대상 리스트를 따로 만들어서 우선 저장.
                attackedList.Add(_liveEnemyList[i]);
            }
        }

        //해당 공격으로 파괴되어도 배열 반복에 지장이 없도록 하기 위해 리스트 뒤에서부터 공격 처리.
        for(int j = attackedList.Count - 1; j >= 0; --j)
        {
            attackedList[j].Attacked(atk);
        }
    }

    //전방 직선 공격. 전방 거리/가로 범위 안의 적 전체 공격. 맞은 적이 있으면 true 반환.
    public bool FrontRangeAttack(Transform atker, float range, float length, int atk)
    {
        List<Enemy> attackedList = new List<Enemy>();
        for(int i = 0; i < _liveEnemyList.Count; ++i)
        {
            Vector3 toEnemyVec = _liveEnemyList[i].transform.position - atker.position;
            float hDist = Mathf.Sin(Vector3.Angle(atker.forward, toEnemyVec) * Mathf.Deg2Rad) * toEnemyVec.magnitude;
            float vDist = Mathf.Cos(Vector3.Angle(atker.forward, toEnemyVec) * Mathf.Deg2Rad) * toEnemyVec.magnitude;
            bool front = Vector3.Dot(atker.forward, toEnemyVec) / toEnemyVec.magnitude >= 0;
            if(front && hDist <= range / 2f && vDist <= length)
            {
                //공격 대상 리스트를 따로 만들어서 우선 저장.
                attackedList.Add(_liveEnemyList[i]);
            }
        }

        bool result = attackedList.Count > 0;

        //해당 공격으로 파괴되어도 배열 반복에 지장이 없도록 하기 위해 리스트 뒤에서부터 공격 처리.
        for (int j = attackedList.Count - 1; j >= 0; --j)
        {
            attackedList[j].Attacked(atk);
        }

        return result;
    }

    //원형 범위 공격. range 지름으로 하는 원형 범위 안의 적 전체 공격. 맞은 적이 있으면 true 반환.
    public bool CircleRangeAttack(Transform atker, float range, int atk)
    {
        List<Enemy> attackedList = new List<Enemy>();
        for (int i = 0; i < _liveEnemyList.Count; ++i)
        {
            float toEnemyDist = (_liveEnemyList[i].transform.position - atker.position).magnitude;
            if(toEnemyDist <= range / 2f)
            {
                //공격 대상 리스트를 따로 만들어서 우선 저장.
                attackedList.Add(_liveEnemyList[i]);
            }
        }

        bool result = attackedList.Count > 0;

        //해당 공격으로 파괴되어도 배열 반복에 지장이 없도록 하기 위해 리스트 뒤에서부터 공격 처리.
        for (int j = attackedList.Count - 1; j >= 0; --j)
        {
            attackedList[j].Attacked(atk);
        }

        return result;
    }
    
    //게임 시작. 무기 선택 부터 시작.
    public void GameStart()
    {
        GameState = DefEnum.GameState.SELECT_WEAPON;

        if (ingameUI == null)
        {
            ingameUI = UIManager.Instance.LoadUI("IngameUI").GetComponent<IngameUI>();
        }
        else
        {
            ingameUI.gameObject.SetActive(true);
        }

        planet.Init();
        _currentScore = 0;
        _currentResources = 0;
        _upgradeCost = 0;
        _upgradeLV = 1;
        SetMainWeaponID();
        GetUpgradeCost();

        ingameUI.SetScore(_currentScore);
        ingameUI.SetResources(_currentResources, _upgradeCost);

        SpawnManager.Instance.Init();
        EffectManager.Instance.InitIngameEffects();

        //시작할 때 무기 선택 창 열기.
        UIManager.Instance.LoadPopupUI("WeaponUI");
    }

    //게임 일시 정지.
    public void Pause()
    {
        GameState = DefEnum.GameState.PAUSE;
        EffectManager.Instance.Pause();
        SoundManager.Instance.Pause();
    }

    //게임 일시 정지 해제.
    public void Play()
    {
        GameState = DefEnum.GameState.PLAY;
        EffectManager.Instance.Resume();
        SoundManager.Instance.Resume();
    }

    //인게임 튜토리얼 출력.
    public void ShowTutorial()
    {
        //이미 튜토리얼 봤으면 바로 게임 진행.
        if(DataManager.Instance.playedTutorial)
        {
            Play();
            return;
        }

        GameState = DefEnum.GameState.TUTORIAL;
        IngameSceneManager.Instance.PlayIngameScene("Tutorial_02", EndTutorial);
    }

    //튜토리얼 종료 설정.
    //로컬에 튜토리얼 완료 값 저장.
    void EndTutorial()
    {
        DataManager.Instance.playedTutorial = true;
        PlayerPrefs.SetInt("PlayedTutorial", 1);
        Play();
    }

    //게임 종료 처리.
    public void GameEnd(bool success)
    {
        GameState = DefEnum.GameState.END;
        EffectManager.Instance.Pause();
        IngameSceneManager.Instance.PlayIngameScene(success ? "Clear" : "Defeat", ShowResultUI);
        _isGameResult = success;
        _currentScore += planet.CurrentHP * 50;
    }

    //결과창 출력.
    void ShowResultUI()
    {
        ResultUI ui = UIManager.Instance.LoadPopupUI("ResultUI").GetComponent<ResultUI>();
        ui.Init(_isGameResult, _currentScore);

        if (DataManager.Instance.bestScore < _currentScore)
        {
            DataManager.Instance.bestScore = _currentScore;
            PlayerPrefs.SetInt("BestScore", _currentScore);
        }
    }

    //인트로 화면으로 전환.
    public void GoIntro()
    {
        DataManager.Instance.ClearEnemyLevelDataList();
        EffectManager.Instance.ClearEffectList();
        SoundManager.Instance.AllSFXStop();
        ResourceManager.Instance.ClearObjectPools();
        ClearEnemyList();
        planet.Clear();

        GameState = DefEnum.GameState.INTRO;
        ingameUI.gameObject.SetActive(false);
        UIManager.Instance.LoadUI("IntroUI");
    }

    //게임 도중 강제 종료.
    public void ForcedEnd()
    {
        planet.ForcedEnd();
    }

    //현재 살아있는 모든 적 제거.
    public void ClearEnemyList()
    {
        int count = _liveEnemyList.Count;
        for (int i = count - 1; i >= 0; --i)
        {
            GameObject enemy = _liveEnemyList[i].gameObject;
            _liveEnemyList.RemoveAt(i);
            Destroy(enemy);
        }
    }

    //전략 무기 설정.
    public void SetSubWeapon(InfoWeapon info)
    {
        planet.SetWeapon(_mainWeaponID, info);
        ResourceManager.Instance.PreLoadReaource(InfoManager.Instance.infoWeaponList[_mainWeaponID].bulletPath);
        ResourceManager.Instance.PreLoadReaource(info.bulletPath);
    }

    //무기 업그레이드 실행. 일정 자원 이상 있어야지 실행.
    public void UpgradeWeaponLevel()
    {
        if(_currentResources >= _upgradeCost)
        {
            _currentResources -= _upgradeCost;
            _upgradeLV++;
            GetUpgradeCost();
            ingameUI.SetResources(_currentResources, _upgradeCost);
            planet.UpgradeWeaponLevel();
        }
    }

    //무기 강화 가격 계산 후 반환.
    //레벨 * 104 + (50 + 2 * (레벨 제곱 + (레벨 - 2)))
    void GetUpgradeCost()
    {
        //upgradeCost = upgradeCost == 0 ? System.Convert.ToInt32(InfoManager.Instance.InfoGlobalList["WeaponUpgCost"].Value)
        //    : upgradeCost + (int)(upgradeCost * ((float)System.Convert.ToDouble(InfoManager.Instance.InfoGlobalList["WeaponUpgCostIncreaseRate"].Value) / 100f));
        _upgradeCost = _upgradeLV * 104 + (50 + 2 * (_upgradeLV * _upgradeLV + (_upgradeLV - 2)));
    }
    
    //다음 웨이브 UI 설정.
    public void NextWave(int wave)
    {
        //첫 웨이브 제외하고 다음 웨이브로 넘어갈 때 마다 추가 점수 부여.
        if(wave > 1)
        {
            _currentScore += System.Convert.ToInt32(InfoManager.Instance.infoGlobalList["WaveBonusScore"].value);
            ingameUI.SetScore(_currentScore);
        }
        ingameUI.SetNextWaveUI(wave);
    }

    //마지막 웨이브인지 확인.
    public void CheckFinishWave()
    {
        if(SpawnManager.Instance.CheckFinalWave() && _liveEnemyList.Count <= 0)
        {
            GameEnd(true);
        }
    }

    //전략 무기 발사 처리.
    public void FireSubweapon()
    {
        planet.FireSubweapon();
    }

    //더블 터치로 전략 무기 발사 켜기/끄기.
    public void SetDblTouchFire()
    {
        IsEnableDlbTouchFire = !IsEnableDlbTouchFire;
        PlayerPrefs.SetInt("OnDblTouchFire", IsEnableDlbTouchFire ? 1 : 0);
    }

    //기본 무기 ID 설정.
    void SetMainWeaponID()
    {
        foreach (InfoWeapon info in InfoManager.Instance.infoWeaponList.Values)
        {
            if (info.difficulty == (int)DataManager.Instance.gameDifficulty && info.isNormal == 1)
            {
                _mainWeaponID = info.ID;
                break;
            }
        }
    }

    ////공격 지점 기즈모 표시.
    //private void OnDrawGizmos()
    //{
    //    for (int i = 0; i < attackPoints.Length; ++i)
    //    {
    //        Gizmos.DrawSphere(attackPoints[i].transform.position, 0.1f);
    //    }
    //}
}
