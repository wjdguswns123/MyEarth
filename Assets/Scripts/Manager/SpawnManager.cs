using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Def;

public class SpawnManager : Singleton<SpawnManager>
{
    private const int SPECIAL_ENEMY_ADD_RATE = 100;

    #region Inspector

    public Transform[] lv1SpawnPositions;
    public Transform[] lv2SpawnPositions;
    public Transform[] lv3SpawnPositions;

    #endregion

    private float _spawnTimer;
    private int _remainSpawnCount;
    private int _waveCountIndex;

    private bool _updateWave;
        
    private List<InfoEnemySpawn> _enemySpawnRate;
    private List<InfoEnemySpawn> _specialEnemySpawnRate;

    private List<InfoWave> _waveInfos;
    private InfoWave _currentWaveInfo;

    /// <summary>
    /// 적 생성 정보, 웨이브 초기화.
    /// </summary>
    public void Init()
    {
        _spawnTimer = 0f;
        _updateWave = false;

        _waveInfos = new List<InfoWave>(InfoManager.Instance.infoWaveList.Values);

        InitWaveIndex();

        BattleManager.Instance.NextWave(_waveCountIndex);

        foreach (InfoEnemy info in InfoManager.Instance.infoEnemyList.Values)
        {
            ResourceManager.Instance.CreateObjectPool(ResourcePath.ENEMY_PATH, info.prefabPath, 20, 5);
        }
    }

    /// <summary>
    /// 선택한 난이도에 해당하는 웨이브 인덱스 첫 지점 검색.
    /// </summary>
    private void InitWaveIndex()
    {
        for(int i = 0; i < _waveInfos.Count; ++i)
        {
            var info = _waveInfos[i];
            if (info.difficulty == (int)BattleManager.Instance.GameDifficulty)
            {
                _currentWaveInfo = info;
                _remainSpawnCount = info.spawnCount;

                SetEnemySpawnRate(info.enemySpawnGroup);
                _updateWave = true;
                break;
            }
        }

        _waveCountIndex = 1;
    }

    /// <summary>
    /// 해당 그룹의 출현할 적과 출현 확률 리스트 작성.
    /// </summary>
    /// <param name="group"></param>
    private void SetEnemySpawnRate(int group)
    {
        if (_enemySpawnRate == null)
        {
            _enemySpawnRate = new List<InfoEnemySpawn>();
        }
        else
        {
            _enemySpawnRate.Clear();
        }

        if (_specialEnemySpawnRate == null)
        {
            _specialEnemySpawnRate = new List<InfoEnemySpawn>();
        }
        else
        {
            _specialEnemySpawnRate.Clear();
        }

        foreach (InfoEnemySpawn info in InfoManager.Instance.infoEnemySpawnList.Values)
        {
            if (info.group == group)
            {
                if(info.spawnRate > SPECIAL_ENEMY_ADD_RATE)
                {
                    _specialEnemySpawnRate.Add(info);
                }
                else
                {
                    _enemySpawnRate.Add(info);
                }
            }
        }
    }

    private void Update()
    {
        //게임 실행 상태일 때 적 생성.
        if(BattleManager.Instance.GameState == Def.DefEnum.GameState.PLAY)
        {
            if (_remainSpawnCount > 0 && _spawnTimer >= _currentWaveInfo.spawnDelay)
            {
                if(_updateWave)
                {
                    BattleManager.Instance.NextWave(_waveCountIndex);
                    _updateWave = false;
                }

                int spawnCnt = SetSpawnOnceCount();
                SpawnEnemy(spawnCnt);

                //해당 웨이브의 적 출현이 다 끝났으면 다음 웨이브 정보 설정.
                if (_remainSpawnCount == 0)
                {
                    SetNextWave();
                }

                _spawnTimer = 0f;
            }
            else
            {
                _spawnTimer += Time.deltaTime;
            }
        }
    }

    /// <summary>
    /// 한번에 생성되는 적 갯수 결정.
    /// InfoWave의 SpawnCountRate 항목의 확률로 갯수 결정.
    /// </summary>
    /// <returns></returns>
    private int SetSpawnOnceCount()
    {
        int rand = Random.Range(0, 100);
        int maxCount = _currentWaveInfo.spawnOnceCount;
        int[] rates = _currentWaveInfo.spawnCountRate;
        int sum = 0;
        for (int i = 0; i < maxCount; ++i)
        {
            sum += rates[i];
            if (rand < sum)
            {
                return i + 1;
            }
        }

        return 1;
    }

    /// <summary>
    /// 적 생성 갯수 만큼 적 생성.
    /// </summary>
    /// <param name="spawnOnceCnt"></param>
    private void SpawnEnemy(int spawnOnceCnt)
    {
        for (int i = 0; i < spawnOnceCnt; ++i)
        {
            if(_remainSpawnCount > 0)
            {
                Transform spawnPos = SetSpawnPosition();
                if(spawnPos == null)
                {
                    // 모든 스폰 지점이 다 찼으면 더 생성하지 않음.
                    break;
                }
                InfoEnemy enemyInfo = GetEnemyInfo();
                Enemy enemyObj = ResourceManager.Instance.LoadResource(ResourcePath.ENEMY_PATH, enemyInfo.prefabPath, spawnPos.position, spawnPos.rotation).GetComponent<Enemy>();
                enemyObj.Init(enemyInfo);
                //생성 후 해당 생성 위치 비활성화 처리.
                spawnPos.gameObject.SetActive(false);
                _remainSpawnCount -= 1;
            }
        }

        //해당 생성 시간에 적 갯수 만큼 생성이 완료 되면 비활성화 처리 했던 생성 위치 다시 모두 활성화 처리.
        ResetAllSpawnPos();
    }

    /// <summary>
    /// 스폰 위치 난이도별 출현 확률로 위치 결정.
    /// </summary>
    /// <returns></returns>
    private Transform SetSpawnPosition()
    {
        int rand = Random.Range(0, 100);
        int rate1 = _currentWaveInfo.spawnPosRate1;
        int rate2 = _currentWaveInfo.spawnPosRate2;
        int rate3 = _currentWaveInfo.spawnPosRate3;
        if (rand < rate1)
        {
            if(HaveEmptySpawnPosition(lv1SpawnPositions))
            {
                return SetSpawnPosition(lv1SpawnPositions);
            }
        }
        if (rand < rate1 + rate2)
        {
            if (HaveEmptySpawnPosition(lv2SpawnPositions))
            {
                return SetSpawnPosition(lv2SpawnPositions);
            }
        }
        if (rand < rate1 + rate2 + rate3)
        {
            if (HaveEmptySpawnPosition(lv3SpawnPositions))
            {
                return SetSpawnPosition(lv3SpawnPositions);
            }
        }

        return null;
    }

    /// <summary>
    /// 해당 레벨의 소환 위치에 활성화된 지역이 있는지 확인.
    /// </summary>
    /// <param name="spawnPos"></param>
    /// <returns></returns>
    private bool HaveEmptySpawnPosition(Transform[] spawnPos)
    {
        for (int i = 1; i < spawnPos.Length; ++i)
        {
            if (spawnPos[i].gameObject.activeSelf)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 생성 위치가 활성화된 곳을 대상으로 임의로 위치 결정.
    /// </summary>
    /// <param name="spawnPos"></param>
    /// <returns></returns>
    private Transform SetSpawnPosition(Transform[] spawnPos)
    {
        List<Transform> emptys = new List<Transform>();
        for(int i = 0; i < spawnPos.Length; ++i)
        {
            if(spawnPos[i].gameObject.activeSelf)
            {
                emptys.Add(spawnPos[i]);
            }
        }

        int randIndex = Random.Range(0, emptys.Count);
        return emptys[randIndex];
    }

    /// <summary>
    /// 적 정보 랜덤으로 가져오기.
    /// </summary>
    /// <returns></returns>
    private InfoEnemy GetEnemyInfo()
    {
        int rand = Random.Range(0, 100);
        int sum = 0;
        // 스페셜 적.
        if(_specialEnemySpawnRate.Count == _remainSpawnCount)
        {
            // 이번 웨이브의 남은 적 수가 아직 나오지 않은 스페셜 적 수 만큼 남아있으면, 바로 스페셜 적 정보 반환.
            var info = _specialEnemySpawnRate[0];
            _specialEnemySpawnRate.Remove(info);
            SetEnemyLevel(info.enemyID);
            return InfoManager.Instance.infoEnemyList[info.enemyID];
        }
        else
        {
            for (int i = 0; i < _specialEnemySpawnRate.Count; ++i)
            {
                var info = _specialEnemySpawnRate[i];
                if (rand < info.spawnRate - SPECIAL_ENEMY_ADD_RATE)
                {
                    _specialEnemySpawnRate.Remove(info);
                    SetEnemyLevel(info.enemyID);
                    return InfoManager.Instance.infoEnemyList[info.enemyID];
                }
            }
        }

        // 일반 적.
        for(int j = 0; j < _enemySpawnRate.Count; ++j)
        {
            var info = _enemySpawnRate[j];
            sum += info.spawnRate;
            if (rand <= sum)
            {
                SetEnemyLevel(info.enemyID);
                return InfoManager.Instance.infoEnemyList[info.enemyID];
            }
        }

        return InfoManager.Instance.infoEnemyList[1];
    }

    /// <summary>
    /// 적 레벨 설정.
    /// </summary>
    /// <param name="emenyId"></param>
    private void SetEnemyLevel(int emenyId)
    {   
        //해당 ID의 적 레벨 정보 설정. 해당 정보가 없으면 새로 적 레벨 정보 추가, 있으면 레벨 증가.
        if (!DataManager.Instance.enemyLevelDataList.ContainsKey(emenyId))
        {
            DataManager.Instance.AddEnemyLevelData(emenyId, _currentWaveInfo.ID);
        }
        else
        {
            var enemyLvData = DataManager.Instance.enemyLevelDataList[emenyId];
            if(enemyLvData.PrevWave < _currentWaveInfo.ID)
            {
                enemyLvData.PrevWave = _currentWaveInfo.ID;
                enemyLvData.Level++;
            }
        }
    }

    /// <summary>
    /// 다음 웨이브 정보 설정.
    /// </summary>
    private void SetNextWave()
    {   
        int curIndex = _waveInfos.FindIndex(i => i.ID == _currentWaveInfo.ID);
        _currentWaveInfo = null;
        if (curIndex + 1 < _waveInfos.Count)
        {
            var info = _waveInfos[curIndex + 1];
            if(info.difficulty == (int)BattleManager.Instance.GameDifficulty)
            {
                _currentWaveInfo = info;
                _remainSpawnCount = info.spawnCount;
                ++_waveCountIndex;

                SetEnemySpawnRate(info.enemySpawnGroup);
                _updateWave = true;
            }
        }
    }

    /// <summary>
    /// 모든 생성 위치 활성화.
    /// </summary>
    private void ResetAllSpawnPos()
    {
        ResetSpawnPos(lv1SpawnPositions);
        ResetSpawnPos(lv2SpawnPositions);
        ResetSpawnPos(lv3SpawnPositions);
    }

    private void ResetSpawnPos(Transform[] spawnPos)
    {
        for(int i = 1; i < spawnPos.Length; ++i)
        {
            spawnPos[i].gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 마지막 웨이브인지 확인.
    /// </summary>
    /// <returns></returns>
    public bool CheckFinalWave()
    {
        return _currentWaveInfo == null;
    }
}
