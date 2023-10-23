using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Def;

//적 레벨 정보 데이터.
public class EnemyLevelData
{
    public int enemyID;
    public int level;
    public int prevWave;

    public EnemyLevelData()
    {
        enemyID  = 0;
        level    = 0;
        prevWave = 0;
    }

    public EnemyLevelData(int id, int wave)
    {
        enemyID  = id;
        level    = 1;
        prevWave = wave;
    }
}

public class DataManager : Singleton<DataManager>
{
    public Dictionary<int, EnemyLevelData> enemyLevelDataList = new Dictionary<int, EnemyLevelData>();

    public int                bestScore;        //최고 점수.
    public bool               playedTutorial;   //튜토리얼 플레이 여부.

    public void Init()
    {
        bestScore = PlayerPrefs.GetInt("BestScore");
        playedTutorial = PlayerPrefs.GetInt("PlayedTutorial") > 0;
    }

    //적 레벨 정보 데이터 리스트에 추가.
    public void AddEnemyLevelData(int id, int wave)
    {
        EnemyLevelData data = new EnemyLevelData(id, wave);
        enemyLevelDataList.Add(id, data);
    }

    public void ClearEnemyLevelDataList()
    {
        enemyLevelDataList.Clear();
    }
}
