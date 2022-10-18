using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnManager : Singleton<SpawnManager>
{
    Transform[]          lv1SpawnPositions;
    Transform[]          lv2SpawnPositions;
    Transform[]          lv3SpawnPositions;

    Planet               planetObj;

    float                spawnTimer = 0f;
    int                  currentWaveIndex = 0;
    int                  remainSpawnCount = 0;
    int                  waveCountIndex = 0;
    bool                 updateWave = false;
    bool                 isSpawnedSpecialEnemy = false;
    Dictionary<int, int> enemySpawnRate = new Dictionary<int, int>();
    int[]                difficultyEnemyIndexList;
     
	// Use this for initialization
	void Start ()
    {
        planetObj = FindObjectOfType<Planet>();

        lv1SpawnPositions = transform.Find("Level1").GetComponentsInChildren<Transform>();
        lv2SpawnPositions = transform.Find("Level2").GetComponentsInChildren<Transform>();
        lv3SpawnPositions = transform.Find("Level3").GetComponentsInChildren<Transform>();
    }

    private void Update()
    {
        //게임 실행 상태일 때 적 생성.
        if(BattleManager.Instance.GameState == Def.DefEnum.GameState.PLAY)
        {
            if(remainSpawnCount > 0 && spawnTimer >= InfoManager.Instance.infoWaveList[currentWaveIndex].spawnDelay)
            {
                if(updateWave)
                {
                    //BattleManager.Instance.NextWave(currentWaveIndex);
                    BattleManager.Instance.NextWave(waveCountIndex);
                    updateWave = false;
                }

                SpawnEnemy(RandSpawnOnceCount());

                //해당 웨이브의 적 출현이 다 끝났으면 다음 웨이브 정보 설정.
                if (remainSpawnCount == 0)
                {
                    SetNextWave();
                }
            }
            else
            {
                spawnTimer += Time.deltaTime;
            }
        }
    }

    //적 생성 정보, 웨이브 초기화.
    public void Init()
    {
        spawnTimer = 0f;
        //currentWaveIndex = 0;
        InitWaveIndex();
        InitEnemyId();
        isSpawnedSpecialEnemy = false;
        //SetNextWave();
        BattleManager.Instance.NextWave(waveCountIndex);
        updateWave = false;

        foreach(InfoEnemy info in InfoManager.Instance.infoEnemyList.Values)
        {
            ResourceManager.Instance.CreateObjectPool(info.prefabPath, 20, 5);
        }
    }

    //선택한 난이도에 해당하는 웨이브 인덱스 첫 지점 검색.
    void InitWaveIndex()
    {
        foreach(KeyValuePair<int, InfoWave> info in InfoManager.Instance.infoWaveList)
        {
            if(info.Value.difficulty == (int)DataManager.Instance.gameDifficulty)
            {
                currentWaveIndex = info.Key;
                remainSpawnCount = info.Value.spawnCount;

                SetEnemySpawnRate(info.Value.enemySpawnGroup);
                updateWave = true;
                isSpawnedSpecialEnemy = false;
                break;
            }
        }

        waveCountIndex = 1;
    }

    //선택한 난이도에 해당하는 적 정보 리스트 생성.
    void InitEnemyId()
    {
        int count = 0;
        foreach(KeyValuePair<int, InfoEnemy> info in InfoManager.Instance.infoEnemyList)
        {
            if(info.Value.difficulty == (int)DataManager.Instance.gameDifficulty)
            {
                ++count;
            }
        }

        difficultyEnemyIndexList = new int[count];
        count = 0;
        foreach (KeyValuePair<int, InfoEnemy> info in InfoManager.Instance.infoEnemyList)
        {
            if (info.Value.difficulty == (int)DataManager.Instance.gameDifficulty)
            {
                difficultyEnemyIndexList[count++] = info.Key;
            }
        }
    }

    //한번에 생성되는 적 갯수 결정.
    //InfoWave의 SpawnCountRate 항목의 확률로 갯수 결정.
    int RandSpawnOnceCount()
    {
        int rand = Random.Range(1, 101);
        int count = InfoManager.Instance.infoWaveList[currentWaveIndex].spawnOnceCount;
        int[] rates = InfoManager.Instance.infoWaveList[currentWaveIndex].spawnCountRate;
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
            if(remainSpawnCount > 0)
            {
                Transform spawnPos = RandSpawnPos();
                InfoEnemy enemyInfo = RandEnemy();
                Enemy enemyObj = ResourceManager.Instance.LoadResource(enemyInfo.prefabPath, spawnPos.position, spawnPos.rotation).GetComponent<Enemy>();
                enemyObj.Init(planetObj, enemyInfo);
                spawnTimer = 0f;
                //생성 후 해당 생성 위치 비활성화 처리.
                spawnPos.gameObject.SetActive(false);
                remainSpawnCount -= 1;
            }
        }

        //해당 생성 시간에 적 갯수 만큼 생성이 완료 되면 비활성화 처리 했던 생성 위치 다시 모두 활성화 처리.
        ResetAllSpawnPos();
    }

    //스폰 위치 난이도별 출현 확률로 위치 결정.
    Transform RandSpawnPos()
    {
        int rand = Random.Range(1, 101);
        int rate1 = InfoManager.Instance.infoWaveList[currentWaveIndex].spawnPosRate1;
        int rate2 = InfoManager.Instance.infoWaveList[currentWaveIndex].spawnPosRate2;
        int rate3 = InfoManager.Instance.infoWaveList[currentWaveIndex].spawnPosRate3;
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
        foreach(KeyValuePair<int, int> spawn in enemySpawnRate)
        {
            if(spawn.Value > 100)
            {
                int sp_rand = Random.Range(1, 101);
                if((remainSpawnCount == 1 && !isSpawnedSpecialEnemy) || (sp_rand <= spawn.Value - 100))
                {
                    isSpawnedSpecialEnemy = true;
                    return GetEnemyInfo(difficultyEnemyIndexList[spawn.Key - 1]);
                }
            }
        }

        int rand = Random.Range(1, 101);
        int sum = 0;
        foreach(KeyValuePair<int, int> spawn in enemySpawnRate)
        {
            sum += spawn.Value;
            if(rand <= sum)
            {
                return GetEnemyInfo(difficultyEnemyIndexList[spawn.Key - 1]);
            }
        }

        return InfoManager.Instance.infoEnemyList[1];
    }

    InfoEnemy GetEnemyInfo(int key)
    {
        //해당 ID의 적 레벨 정보 설정. 해당 정보가 없으면 새로 적 레벨 정보 추가, 있으면 레벨 증가.
        if (!DataManager.Instance.enemyLevelDataList.ContainsKey(key))
        {
            DataManager.Instance.AddEnemyLevelData(key, currentWaveIndex);
        }
        else if (DataManager.Instance.enemyLevelDataList[key].PrevWave < currentWaveIndex)
        {
            DataManager.Instance.enemyLevelDataList[key].PrevWave = currentWaveIndex;
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
            if(next && (int)DataManager.Instance.gameDifficulty == info.Value.difficulty)
            {
                currentWaveIndex = info.Key;
                remainSpawnCount = info.Value.spawnCount;
                ++waveCountIndex;

                SetEnemySpawnRate(info.Value.enemySpawnGroup);
                updateWave = true;
                isSpawnedSpecialEnemy = false;
                break;
            }
            else if(currentWaveIndex == info.Key && (int)DataManager.Instance.gameDifficulty == info.Value.difficulty)
            {
                next = true;
            }
        }

        //더이상 받아올 웨이브 정보가 없으면 남은 스폰 수 -1로 처리.
        if(remainSpawnCount <= 0)
        {
            remainSpawnCount = -1;
        }
    }

    //해당 그룹의 출현할 적과 출현 확률 리스트 작성.
    void SetEnemySpawnRate(int group)
    {
        enemySpawnRate.Clear();
        foreach (InfoEnemySpawn info in InfoManager.Instance.infoEnemySpawnList.Values)
        {
            if(info.group == group)
            {
                enemySpawnRate.Add(info.enemyID, info.spawnRate);
            }
        }
    }

    //마지막 웨이브인지 확인.
    public bool CheckFinalWave()
    {
        return remainSpawnCount == -1;
    }
}
