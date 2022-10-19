using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Def;

public class BattleManager : Singleton<BattleManager>
{
    #region Inspector

    public IngameUI ingameUI;
    public Planet planet;
    public AttackPoint[] attackPoints;     //목적지 배열.

    #endregion

    public DefEnum.GameState GameState { get; private set; }

    private List<Enemy> _liveEnemyList;
    
    private int _currentScore;
    private int _currentResources;
    private int _upgradeCost;
    private int _upgradeLV;
    private int _mainWeaponID;
    private bool _isGameResult;

    #region Process

    /// <summary>
    /// 인게임 프로세스 시작.
    /// </summary>
    public void StartProcess()
    {
        if (_liveEnemyList == null)
        {
            _liveEnemyList = new List<Enemy>();
        }
        _liveEnemyList.Clear();

        for(int i = 0; i < attackPoints.Length; ++i)
        {
            attackPoints[i].Clear();
        }

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
        ResourceManager.Instance.PreLoadReaource(InfoManager.Instance.infoWeaponList[_mainWeaponID].bulletPath);    // 기본 무기 총알 풀 생성.

        SelectWeapon();
    }

    /// <summary>
    /// 무기 선택 진행.
    /// </summary>
    private void SelectWeapon()
    {
        Debug.Log("BattleManager SelectWeapon");

        GameState = DefEnum.GameState.SELECT_WEAPON;

        //시작할 때 무기 선택 창 열기.
        var weaponUI = UIManager.Instance.LoadPopupUI("WeaponUI").GetComponent<WeaponUI>();
        weaponUI.Set(() =>
        {
            GamePlayStart();
        });
    }

    /// <summary>
    /// 게임 플레이 시작.
    /// </summary>
    private void GamePlayStart()
    {
        Debug.Log("BattleManager GamePlayStart");

        if (!DataManager.Instance.playedTutorial)
        {
            ShowTutorial();
        }
        else
        {
            //튜토리얼 봤으면 바로 게임 진행.
            Play();
        }
    }

    /// <summary>
    /// 인게임 튜토리얼 출력.
    /// </summary>
    private void ShowTutorial()
    {
        Debug.Log("BattleManager ShowTutorial");

        GameState = DefEnum.GameState.TUTORIAL;
        IngameSceneManager.Instance.PlayIngameScene("Tutorial_02", EndTutorial);
    }

    /// <summary>
    /// 튜토리얼 종료 설정.
    /// </summary>
    private void EndTutorial()
    {
        Debug.Log("BattleManager EndTutorial");

        DataManager.Instance.playedTutorial = true;
        PlayerPrefs.SetInt("PlayedTutorial", 1);    //로컬에 튜토리얼 완료 값 저장.
        Play();
    }

    /// <summary>
    /// 게임 플레이.
    /// </summary>
    private void Play()
    {
        Debug.Log("BattleManager Play");
        GameState = DefEnum.GameState.PLAY;
    }

    /// <summary>
    /// 게임 일시 정지.
    /// </summary>
    public void Pause()
    {
        GameState = DefEnum.GameState.PAUSE;
        EffectManager.Instance.Pause();
        SoundManager.Instance.Pause();
    }

    /// <summary>
    /// 게임 일시 정지 해제.
    /// </summary>
    public void Resume()
    {
        GameState = DefEnum.GameState.PLAY;
        EffectManager.Instance.Resume();
        SoundManager.Instance.Resume();
    }

    /// <summary>
    /// 게임 종료 처리.
    /// </summary>
    /// <param name="success"></param>
    public void GameEnd(bool success)
    {
        Debug.Log("BattleManager GameEnd");

        GameState = DefEnum.GameState.END;
        EffectManager.Instance.Pause();
        IngameSceneManager.Instance.PlayIngameScene(success ? "Clear" : "Defeat", ShowResultUI);
        _isGameResult = success;
        _currentScore += planet.CurrentHP * 50;
    }

    /// <summary>
    /// 결과창 출력.
    /// </summary>
    private void ShowResultUI()
    {
        ResultUI ui = UIManager.Instance.LoadPopupUI("ResultUI").GetComponent<ResultUI>();
        ui.Init(_isGameResult, _currentScore);

        if (DataManager.Instance.bestScore < _currentScore)
        {
            DataManager.Instance.bestScore = _currentScore;
            PlayerPrefs.SetInt("BestScore", _currentScore);
        }
    }

    /// <summary>
    /// 인트로 화면으로 전환.
    /// </summary>
    public void GoIntro()
    {
        DataManager.Instance.ClearEnemyLevelDataList();
        EffectManager.Instance.ClearEffectList();
        SoundManager.Instance.AllSFXStop();
        ResourceManager.Instance.ClearObjectPools();
        _liveEnemyList.Clear();
        planet.Clear();

        ingameUI.gameObject.SetActive(false);
        LobbyManager.Instance.ShowLobby();
    }

    #endregion

    /// <summary>
    /// 현재 살아있는 적 리스트에 적 추가.
    /// </summary>
    /// <param name="enemy"></param>
    public void AddEnemy(Enemy enemy)
    {
        _liveEnemyList.Add(enemy);
    }

    /// <summary>
    /// 해당 적 위치에서 가장 가까운 빈 공격 지점을 검색. 없으면 null 반환.
    /// </summary>
    /// <param name="enemy"></param>
    /// <returns></returns>
    public AttackPoint SearchAttackPoint(Enemy enemy)
    {
        AttackPoint result = null;
        float minLength = 0f;
        for (int i = 0; i < attackPoints.Length; ++i)
        {
            var atkPoint = attackPoints[i];
            if (atkPoint.IsArriveEnemy())
            {
                continue;
            }

            float dist = Vector3.SqrMagnitude(enemy.transform.position - atkPoint.transform.position);
            if(result == null || minLength * minLength > dist)
            {
                result = atkPoint;
                minLength = dist;
            }
        }
        return result;
    }

    /// <summary>
    /// 해당 공격 지점에 적이 도달하여 해당 지점을 목표로 가지던 모든 적에게 알림.
    /// </summary>
    /// <param name="enemy"></param>
    /// <param name="point"></param>
    public void ArrivalAttackPoint(Enemy enemy, AttackPoint point)
    {
        for (int i = 0; i < attackPoints.Length; ++i)
        {
            var atkPoint = attackPoints[i];
            if (atkPoint == point)
            {
                atkPoint.ArriveEnemy(enemy);
                for (int j = 0; j < _liveEnemyList.Count; ++j)
                {
                    _liveEnemyList[j].FullDestinationAttackPoint(atkPoint);
                }
                break;
            }
        }
    }

    /// <summary>
    /// 가장 가까운 적 찾기.
    /// </summary>
    /// <param name="searcher"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public Enemy SearchNearEnemy(Transform searcher, float range)
    {
        Enemy result = null;
        float minLength = 0f;
        for (int i = 0; i < _liveEnemyList.Count; ++i)
        {
            var enemy = _liveEnemyList[i];
            float dist = Vector3.SqrMagnitude(searcher.transform.position - enemy.transform.position);
            if (range * range >= dist && (result == null || minLength * minLength > dist))
            {
                result = enemy;
                minLength = dist;
            }
        }
        return result;
    }

    /// <summary>
    /// 해당 인덱스의 공격 지점과 가장 가까운 이동중인 적 탐색.
    /// </summary>
    /// <param name="atkIndex"></param>
    /// <returns></returns>
    private Enemy SearchNearMovingEnemy(AttackPoint attackPoint)
    {
        Enemy result = null;
        float minLength = 0f;
        for (int i = 0; i < _liveEnemyList.Count; ++i)
        {
            var enemy = _liveEnemyList[i];
            if (enemy.State == DefEnum.EnemyState.MOVE)
            {
                float dist = Vector3.SqrMagnitude(attackPoint.transform.position - enemy.transform.position);
                if (result == null || minLength * minLength > dist)
                {
                    result = enemy;
                    minLength = dist;
                }
            }   
        }
        return result;
    }

    /// <summary>
    /// 현재 살아있는 적 리스트에서 해당 적 제거.
    /// </summary>
    /// <param name="enemy"></param>
    public void RemoveEnemy(Enemy enemy)
    {
        if(_liveEnemyList.Contains(enemy))
        {
            _liveEnemyList.Remove(enemy);
        }

        //제거한 적이 공격 지점에 있었으면 해당 지점에서 제일 가까운 목적지가 없는 적에게 목적지 변경 요청.
        for (int i = 0; i < attackPoints.Length; ++i)
        {
            var atkPoint = attackPoints[i];
            if (atkPoint.IsArriveThisEnemy(enemy))
            {
                atkPoint.Clear();
                Enemy result = SearchNearMovingEnemy(atkPoint);
                if (result != null)
                {
                    result.ResetDestinationAttackPoint(atkPoint);
                }
                break;
            }
        }
    }

    /// <summary>
    /// 점수 추가.
    /// </summary>
    /// <param name="score"></param>
    public void AddScore(int score)
    {
        _currentScore += score;
        ingameUI.SetScore(_currentScore);
    }

    /// <summary>
    /// 획득 자원 추가.
    /// </summary>
    /// <param name="resources"></param>
    public void AddResources(int resources)
    {
        _currentResources += resources;
        ingameUI.SetResources(_currentResources, _upgradeCost);
    }

    /// <summary>
    /// 게임 도중 강제 종료.
    /// </summary>
    public void ForcedEnd()
    {
        planet.ForcedEnd();
    }

    /// <summary>
    /// 전략 무기 설정.
    /// </summary>
    /// <param name="info"></param>
    public void SetSubWeapon(InfoWeapon info)
    {
        planet.SetWeapon(_mainWeaponID, info);
        ResourceManager.Instance.PreLoadReaource(info.bulletPath);
    }

    /// <summary>
    /// 무기 업그레이드 실행. 일정 자원 이상 있어야지 실행.
    /// </summary>
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

    /// <summary>
    /// 무기 강화 가격 계산 후 반환.
    /// 레벨 * 104 + (50 + 2 * (레벨 제곱 + (레벨 - 2)))
    /// </summary>
    private void GetUpgradeCost()
    {
        _upgradeCost = _upgradeLV * 104 + (50 + 2 * (_upgradeLV * _upgradeLV + (_upgradeLV - 2)));
    }

    /// <summary>
    /// 다음 웨이브 UI 설정.
    /// </summary>
    /// <param name="wave"></param>
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

    /// <summary>
    /// 마지막 웨이브인지 확인.
    /// </summary>
    public void CheckFinishWave()
    {
        if(SpawnManager.Instance.CheckFinalWave() && _liveEnemyList.Count <= 0)
        {
            GameEnd(true);
        }
    }

    /// <summary>
    /// 전략 무기 발사 처리.
    /// </summary>
    public void FireSubweapon()
    {
        planet.FireSubweapon();
    }

    /// <summary>
    /// 기본 무기 ID 설정.
    /// </summary>
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

    #region Attack

    //전방 범위 공격. 해당 각도와 거리 안에 있는 적 공격.
    public void AngleRangeAttack(Transform atker, float angleRange, float range, int atk)
    {
        List<Enemy> attackedList = new List<Enemy>();
        for (int i = 0; i < _liveEnemyList.Count; ++i)
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
        for (int j = attackedList.Count - 1; j >= 0; --j)
        {
            attackedList[j].Attacked(atk);
        }
    }

    //전방 직선 공격. 전방 거리/가로 범위 안의 적 전체 공격. 맞은 적이 있으면 true 반환.
    public bool FrontRangeAttack(Transform atker, float range, float length, int atk)
    {
        List<Enemy> attackedList = new List<Enemy>();
        for (int i = 0; i < _liveEnemyList.Count; ++i)
        {
            Vector3 toEnemyVec = _liveEnemyList[i].transform.position - atker.position;
            float hDist = Mathf.Sin(Vector3.Angle(atker.forward, toEnemyVec) * Mathf.Deg2Rad) * toEnemyVec.magnitude;
            float vDist = Mathf.Cos(Vector3.Angle(atker.forward, toEnemyVec) * Mathf.Deg2Rad) * toEnemyVec.magnitude;
            bool front = Vector3.Dot(atker.forward, toEnemyVec) / toEnemyVec.magnitude >= 0;
            if (front && hDist <= range / 2f && vDist <= length)
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
            if (toEnemyDist <= range / 2f)
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

    #endregion

    ////공격 지점 기즈모 표시.
    //private void OnDrawGizmos()
    //{
    //    for (int i = 0; i < attackPoints.Length; ++i)
    //    {
    //        Gizmos.DrawSphere(attackPoints[i].transform.position, 0.1f);
    //    }
    //}
}
