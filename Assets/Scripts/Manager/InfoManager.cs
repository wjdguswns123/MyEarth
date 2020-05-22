using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Def;

public abstract class InfoClass<T>
{
    public T ID;
    public virtual void Write(string textData) { }
}

//무기 정보.
public class InfoWeapon : InfoClass<int>
{
    public string             name;
    public string             description;
    public DefEnum.MoveType   moveType;
    public DefEnum.AttackType attackType;
    public float              speed;
    public float              loadTime;
    public float              loadTimeUpg;
    public float              loadTimeMax;
    public float              coolTime;
    public float              range;
    public float              length;
    public float              lifeTime;
    public float              lifeTimeUpg;
    public float              lifeTimeMax;
    public int                attack;
    public float              attackUpg;
    public int                count;
    public bool               haveCollider;
    public string             iconPath;
    public string             bulletPath;
    public string             weaponPath;
    public string             effectPath;
    public string             fireSFXPath;
    public string             hitSFXPath;
    public int                difficulty;
    public int                isNormal;

    //텍스트 정보를 무기 정보로 변환.
    public override void Write(string textData)
    {
        string[] textRows = textData.Split('\t');
        if(textRows.Length > 1)
        {
            ID           = System.Convert.ToInt32(textRows[0]);
            name         = textRows[1];
            description  = textRows[2];
            moveType     = (DefEnum.MoveType)System.Convert.ToInt32(textRows[3]);
            attackType   = (DefEnum.AttackType)System.Convert.ToInt32(textRows[4]);
            speed        = (float)System.Convert.ToDouble(textRows[5]);
            loadTime     = (float)System.Convert.ToDouble(textRows[6]);
            loadTimeUpg  = (float)System.Convert.ToDouble(textRows[7]);
            loadTimeMax  = (float)System.Convert.ToDouble(textRows[8]);
            coolTime     = (float)System.Convert.ToDouble(textRows[9]);
            range        = (float)System.Convert.ToDouble(textRows[10]);
            length       = (float)System.Convert.ToDouble(textRows[11]);
            lifeTime     = (float)System.Convert.ToDouble(textRows[12]);
            lifeTimeUpg  = (float)System.Convert.ToDouble(textRows[13]);
            lifeTimeMax  = (float)System.Convert.ToDouble(textRows[14]);
            attack       = System.Convert.ToInt32(textRows[15]);
            attackUpg    = (float)System.Convert.ToDouble(textRows[16]);
            count        = System.Convert.ToInt32(textRows[17]);
            haveCollider = string.Equals(textRows[18], '1') ? true : false;
            iconPath     = textRows[19];
            bulletPath   = textRows[20];
            weaponPath   = textRows[21];
            effectPath   = textRows[22];
            fireSFXPath  = textRows[23];
            hitSFXPath   = textRows[24];
            difficulty   = System.Convert.ToInt32(textRows[25]);
            isNormal     = System.Convert.ToInt32(textRows[26]);
        }
    }
}

//적 정보.
public class InfoEnemy : InfoClass<int>
{
    public int      HP;
    public int      HPUpg;
    public float    speed;
    public float    attackSpeed;
    public int      attack;
    public int      attackUpg;
    public int      score;
    public int      resource;
    public int      resourceUpg;
    public string   prefabPath;
    public string   destroySFXPath;
    public int      difficulty;

    //텍스트 정보를 적 정보로 변환.
    public override void Write(string textData)
    {
        string[] textRows = textData.Split('\t');
        if(textRows.Length > 1)
        {
            ID             = System.Convert.ToInt32(textRows[0]);
            HP             = System.Convert.ToInt32(textRows[2]);
            HPUpg          = System.Convert.ToInt32(textRows[3]);
            speed          = (float)System.Convert.ToDouble(textRows[4]);
            attackSpeed    = (float)System.Convert.ToDouble(textRows[5]);
            attack         = System.Convert.ToInt32(textRows[6]);
            attackUpg      = System.Convert.ToInt32(textRows[7]);
            score          = System.Convert.ToInt32(textRows[8]);
            resource       = System.Convert.ToInt32(textRows[9]);
            resourceUpg    = System.Convert.ToInt32(textRows[10]);
            prefabPath     = textRows[11];
            destroySFXPath = textRows[12];
            difficulty     = System.Convert.ToInt32(textRows[13]);
        }
    }
}

//웨이브 정보.
public class InfoWave : InfoClass<int>
{
    public float  spawnDelay;
    public int    spawnCount;
    public int    spawnPosRate1;
    public int    spawnPosRate2;
    public int    spawnPosRate3;
    public int    spawnOnceCount;
    public int[]  spawnCountRate;
    public int    enemySpawnGroup;
    public int    difficulty;

    //텍스트 정보를 웨이브 정보로 변환.
    public override void Write(string textData)
    {
        string[] textRows = textData.Split('\t');
        if (textRows.Length > 1)
        {
            ID              = System.Convert.ToInt32(textRows[0]);
            spawnDelay      = (float)System.Convert.ToDouble(textRows[1]);
            spawnCount      = System.Convert.ToInt32(textRows[2]);
            spawnPosRate1   = System.Convert.ToInt32(textRows[3]);
            spawnPosRate2   = System.Convert.ToInt32(textRows[4]);
            spawnPosRate3   = System.Convert.ToInt32(textRows[5]);
            spawnOnceCount  = System.Convert.ToInt32(textRows[6]);
            enemySpawnGroup = System.Convert.ToInt32(textRows[8]);
            difficulty      = System.Convert.ToInt32(textRows[9]);
            string tempStr = textRows[7];
            string[] tempArr = tempStr.Split('_');
            spawnCountRate = new int[spawnOnceCount];
            for(int i = 0; i < spawnOnceCount; ++i)
            {
                spawnCountRate[i] = System.Convert.ToInt32(tempArr[i]);
            }     
        }
    }
}

//적 출현 확률 정보.
public class InfoEnemySpawn : InfoClass<int>
{
    public int group;
    public int enemyID;
    public int spawnRate;

    //텍스트 정보를 적 출현 확률 정보로 변환.
    public override void Write(string textData)
    {
        string[] textRows = textData.Split('\t');
        if (textRows.Length > 1)
        {
            ID         = System.Convert.ToInt32(textRows[0]);
            group      = System.Convert.ToInt32(textRows[1]);
            enemyID    = System.Convert.ToInt32(textRows[2]);
            spawnRate  = System.Convert.ToInt32(textRows[3]);
        }
    }
}

//적 출현 확률 정보.
public class InfoGlobal : InfoClass<string>
{
    public string value;

    //텍스트 정보를 적 출현 확률 정보로 변환.
    public override void Write(string textData)
    {
        string[] textRows = textData.Split('\t');
        if (textRows.Length > 1)
        {
            ID    = textRows[0];
            value = textRows[1];
        }
    }
}

public class InfoManager : Singleton<InfoManager>
{
    public Dictionary<int, InfoWeapon>      infoWeaponList      = new Dictionary<int, InfoWeapon>();
    public Dictionary<int, InfoEnemy>       infoEnemyList       = new Dictionary<int, InfoEnemy>();
    public Dictionary<int, InfoWave>        infoWaveList        = new Dictionary<int, InfoWave>();
    public Dictionary<int, InfoEnemySpawn>  infoEnemySpawnList  = new Dictionary<int, InfoEnemySpawn>();
    public Dictionary<string, InfoGlobal>   infoGlobalList      = new Dictionary<string, InfoGlobal>();

    public void Init()
    {
        infoWeaponList     = Read<InfoWeapon, int>("InfoDatas/InfoWeapon");
        infoEnemyList      = Read<InfoEnemy, int>("InfoDatas/InfoEnemy");
        infoWaveList       = Read<InfoWave, int>("InfoDatas/InfoWave");
        infoEnemySpawnList = Read<InfoEnemySpawn, int>("InfoDatas/InfoEnemySpawn");
        infoGlobalList     = Read<InfoGlobal, string>("InfoDatas/InfoGlobal");
    }

    //public void Read(string infoFile)
    //{
    //    TextAsset rawData = Resources.Load(infoFile) as TextAsset;
    //    string textData = rawData.text;

    //    string[] textLnes = textData.Split(new string[] { "\r\n" }, System.StringSplitOptions.RemoveEmptyEntries);

    //    for(int i = 1; i < textLnes.Length; ++i)
    //    {
    //        InfoWeapon info = new InfoWeapon();
    //        info.Write(textLnes[i]);
    //        InfoWeaponList.Add(info.ID, info);
    //    }
    //}

    public Dictionary<Q, T> Read<T, Q>(string infoFile) where T : InfoClass<Q>, new()
    {
        TextAsset rawData = Resources.Load(infoFile) as TextAsset;
        string textData = rawData.text;

        string[] textLnes = textData.Split(new string[] { "\r\n" }, System.StringSplitOptions.RemoveEmptyEntries);

        Dictionary<Q, T> list = new Dictionary<Q, T>();

        for (int i = 1; i < textLnes.Length; ++i)
        {
            T info = new T();
            info.Write(textLnes[i]);
            list.Add(info.ID, info);
        }

        return list;
    }
}
