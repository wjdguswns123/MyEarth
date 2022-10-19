using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnManager : Singleton<SpawnManager>
{
    #region Inspector

    public Transform[] lv1SpawnPositions;
    public Transform[] lv2SpawnPositions;
    public Transform[] lv3SpawnPositions;

    #endregion

    private float _spawnTimer;
    private int _currentWaveIndex;
    private int _remainSpawnCount;
    private int _waveCountIndex;

    private bool _updateWave;
    private bool _isSpawnedSpecialEnemy;

    private List<int> _difficultyEnemyIndexList;

    private Dictionary<int, int> _enemySpawnRate;

    /// <summary>
    /// 적 생성 정보, 웨이브 초기화.
    /// </summary>
    public void Init()
    {
        _spawnTimer = 0f;
        _updateWave = false;
        _isSpawnedSpecialEnemy = false;

        InitWaveIndex();
        InitEnemyId();

        BattleManager.Instance.NextWave(_waveCountIndex);

        foreach (InfoEnemy info in InfoManager.Instance.infoEnemyList.Values)
        {
            ResourceManager.Instance.CreateObjectPool(info.prefabPath, 20, 5);
        }
    }

    /// <summary>
    /// 선택한 난이도에 해당하는 웨이브 인덱스 첫 지점 검색.
    /// </summary>
    private void InitWaveIndex()
    {
        foreach (KeyValuePair<int, InfoWave> info in InfoManager.Instance.infoWaveList)
        {
            if (info.Value.difficulty == (int)BattleManager.Instance.GameDifficulty)
            {
                _currentWaveIndex = info.Key;
                _remainSpawnCount = info.Value.spawnCount;

                SetEnemySpawnRate(info.Value.enemySpawnGroup);
                _updateWave = true;
                _isSpawnedSpecialEnemy = false;
                break;
            }
        }

        _waveCountIndex = 1;
    }

    private void Update()
    {
        //게임 실행 상태일 때 적 생성.
        if(BattleManager.Instance.GameState == Def.DefEnum.GameState.PLAY)
        {
            if(_remainSpawnCount > 0 && _spawnTimer >= InfoManager.Instance.infoWaveList[_currentWaveIndex].spawnDelay)
            {
                if(_updateWave)
                {
                    //BattleManager.Instance.NextWave(currentWaveIndex);
                    BattleManager.Instance.NextWave(_waveCountIndex);
                    _updateWave = false;
                }

                SpawnEnemy(RandSpawnOnceCount());

                //해당 웨이브의 적 출현이 다 끝났으면 다음 웨이브 정보 설정.
                if (_remainSpawnCount == 0)
                {
                    SetNextWave();
                }
            }
            else
            {
                _spawnTimer += Time.deltaTime;
            }
        }
    }

    /// <summary>
    /// 선택한 난이도에 해당하는 적 정보 리스트 생성.
    /// </summary>
    private void InitEnemyId()
    {
        if(_difficultyEnemyIndexList == null)
        {
            _difficultyEnemyIndexList = new List<int>();
        }
        else
        {
            _difficultyEnemyIndexList.Clear();
        }

        foreach (KeyValuePair<int, InfoEnemy> info in InfoManager.Instance.infoEnemyList)
        {
            if (info.Value.difficulty == (int)BattleManager.Instance.GameDifficulty)
            {
                _difficultyEnemyIndexList.Add(info.Key);
            }
        }
    }

    //한번에 생성되는 적 갯수 결정.
    //InfoWave의 SpawnCountRate 항목의 확률로 갯수 결정.
    int RandSpawnOnceCount()
    {
        int rand = Random.Range(1, 101);
        int count = InfoManager.Instance.infoWaveList[_currentWaveIndex].spawnOnceCount;
        int[] rates = InfoManager.Instance.infoWaveList[_currentWaveIndex].spawnCountRate;
        int sum = 0;
        for (int i = 0; i < count; ++i)
        {
            sum += rates[i];
            if (rand <= sum)
            {
                return i + 1;
            }
        }

        return 1;
    }

    //적 생성 갯수 만큼 적 생성.
    void SpawnEnemy(int spawnOnceCnt)
    {
        for (int i = 0; i < spawnOnceCnt; ++i)
        {
            if(_remainSpawnCount > 0)
            {
                Transform spawnPos = RandSpawnPos();
                InfoEnemy enemyInfo = RandEnemy();
                Enemy enemyObj = ResourceManager.Instance.LoadResource(enemyInfo.prefabPath, spawnPos.position, spawnPos.rotation).GetComponent<Enemy>();
                enemyObj.Init(enemyInfo);
                _spawnTimer = 0f;
                //생성 후 해당 생성 위치 비활성화 처리.
                spawnPos.gameObject.SetActive(false);
                _remainSpawnCount -= 1;
            }
        }

        //해당 생성 시간에 적 갯수 만큼 생성이 완료 되면 비활성화 처리 했던 생성 위치 다시 모두 활성화 처리.
        ResetAllSpawnPos();
    }

    //스폰 위치 난이도별 출현 확률로 위치 결정.
    Transform RandSpawnPos()
    {
        int rand = Random.Range(1, 101);
        int rate1 = InfoManager.Instance.infoWaveList[_currentWaveIndex].spawnPosRate1;
        int rate2 = InfoManager.Instance.infoWaveList[_currentWaveIndex].spawnPosRate2;
        int rate3 = InfoManager.Instance.infoWaveList[_currentWaveIndex].spawnPosRate3;
        if (rand <= rate1)
        {
            if(HaveEmptySpawnPos(lv1SpawnPositions))
            {
                return RandSpawnPosLevel(lv1SpawnPositions);
            }
            else
            {
                rand = rate1;
            }
        }
        if (rand <= rate1 + rate2)
        {
            if (HaveEmptySpawnPos(lv2SpawnPositions))
            {
                return RandSpawnPosLevel(lv2SpawnPositions);
            }
            else
            {
                rand = rate1 + rate2;
            }
        }
        if (rand <= rate1 + rate2 + rate3)
        {
            if (HaveEmptySpawnPos(lv3SpawnPositions))
            {
                return RandSpawnPosLevel(lv3SpawnPositions);
            }
        }

        ResetAllSpawnPos();

        return RandSpawnPos();
    }

    //생성 위치가 활성화된 곳을 대상으로 임의로 위치 결정.
    Transform RandSpawnPosLevel(Transform[] spawnPos)
    {
        int randIndex = 0;
        do
        {
            randIndex = Random.Range(1, spawnPos.Length);
        } while (!spawnPos[randIndex].gameObject.activeSelf);

        return spawnPos[randIndex];
    }

    //해당 레벨의 소환 위치에 활성화된 지역이 있는지 확인.
    bool HaveEmptySpawnPos(Transform[] spawnPos)
    {
        for(int i = 1; i < spawnPos.Length; ++i)
        {
            if(spawnPos[i].gameObject.activeSelf)
            {
                return true;
            }
        }

        return false;
    }

    //모든 생성 위치 활성화.
    void ResetAllSpawnPos()
    {
        ResetSpawnPos(lv1SpawnPositions);
        ResetSpawnPos(lv2SpawnPositions);
        ResetSpawnPos(lv3SpawnPositions);
    }

    void ResetSpawnPos(Transform[] spawnPos)
    {
        for(int i = 1; i < spawnPos.Length; ++i)
        {
            spawnPos[i].gameObject.SetActive(true);
        }
    }

    //적 정보 랜덤으로 가져오기.
    InfoEnemy RandEnemy()
    {
        foreach(KeyValuePair<int, int> spawn in _enemySpawnRate)
        {
            if(spawn.Value > 100)
            {
                int sp_rand = Random.Range(1, 101);
                if((_remainSpawnCount == 1 && !_isSpawnedSpecialEnemy) || (sp_rand <= spawn.Value - 100))
                {
                    _isSpawnedSpecialEnemy = true;
                    return GetEnemyInfo(_difficultyEnemyIndexList[spawn.Key - 1]);
                }
            }
        }

        int rand = Random.Range(1, 101);
        int sum = 0;
        foreach(KeyValuePair<int, int> spawn in _enemySpawnRate)
        {
            sum += spawn.Value;
            if(rand <= sum)
            {
                return GetEnemyInfo(_difficultyEnemyIndexList[spawn.Key - 1]);
            }
        }

        return InfoManager.Instance.infoEnemyList[1];
    }

    InfoEnemy GetEnemyInfo(int key)
    {
        //해당 ID의 적 레벨 정보 설정. 해당 정보가 없으면 새로 적 레벨 정보 추가, 있으면 레벨 증가.
        if (!DataManager.Instance.enemyLevelDataList.ContainsKey(key))
        {
            DataManager.Instance.AddEnemyLevelData(key, _currentWaveIndex);
        }
        else if (DataManager.Instance.enemyLevelDataList[key].PrevWave < _currentWaveIndex)
        {
            DataManager.Instance.enemyLevelDataList[key].PrevWave = _currentWaveIndex;
            DataManager.Instance.enemyLevelDataList[key].Level++;
        }
        return InfoManager.Instance.infoEnemyList[key];
    }

    //다음 웨이브 정보 받아오기.
    void SetNextWave()
    {
        bool next = false;
        foreach(KeyValuePair<int, InfoWave> info in InfoManager.Instance.infoWaveList)
        {
            if(next && (int)BattleManager.Instance.GameDifficulty == info.Value.difficulty)
            {
                _currentWaveIndex = info.Key;
                _remainSpawnCount = info.Value.spawnCount;
                ++_waveCountIndex;

                SetEnemySpawnRate(info.Value.enemySpawnGroup);
                _updateWave = true;
                _isSpawnedSpecialEnemy = false;
                break;
            }
            else if(_currentWaveIndex == info.Key && (int)BattleManager.Instance.GameDifficulty == info.Value.difficulty)
            {
                next = true;
            }
        }

        //더이상 받아올 웨이브 정보가 없으면 남은 스폰 수 -1로 처리.
        if(_remainSpawnCount <= 0)
        {
            _remainSpawnCount = -1;
        }
    }

    /// <summary>
    /// 해당 그룹의 출현할 적과 출현 확률 리스트 작성.
    /// </summary>
    /// <param name="group"></param>
    private void SetEnemySpawnRate(int group)
    {
        if(_enemySpawnRate == null)
        {
            _enemySpawnRate = new Dictionary<int, int>();
        }
        else
        {
            _enemySpawnRate.Clear();
        }
        
        foreach (InfoEnemySpawn info in InfoManager.Instance.infoEnemySpawnList.Values)
        {
            if(info.group == group)
            {
                _enemySpawnRate.Add(info.enemyID, info.spawnRate);
            }
        }
    }

    //마지막 웨이브인지 확인.
    public bool CheckFinalWave()
    {
        return _remainSpawnCount == -1;
    }
}
