using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ResourceManager : Singleton<ResourceManager>
{
    public string[]            preloadPaths = new string[1];

    Dictionary<string, Object> loadObjectList = new Dictionary<string, Object>();
    Dictionary<string, Sprite> spritePool     = new Dictionary<string, Sprite>();

    //ObjectPool m_BulletPool;
    //string bulletKey;
    Dictionary<string, ObjectPool> objectPoolList = new Dictionary<string, ObjectPool>();

    List<GameObject> usingPooledObjList = new List<GameObject>();

    //미리 지정된 경로의 프리팹들 미리 로딩.
    public void PreLoadResources()
    {
        int count = preloadPaths.Length;
        for(int i = 0; i < count; ++i)
        {
            Object[] objs = Resources.LoadAll(preloadPaths[i]);
            int objCnt = objs.Length;
            for(int j = 0; j < objCnt; ++j)
            {
                string path = string.Format("{0}/{1}", preloadPaths[i], objs[j].name);
                if(!loadObjectList.ContainsKey(path))
                {
                    loadObjectList.Add(path, objs[j]);
                }
            }
        }
    }

    //해당 경로의 프리팹 미리 로딩.
    public void PreLoadReaource(string path)
    {
        if (!loadObjectList.ContainsKey(path))
        {
            loadObjectList.Add(path, Resources.Load(path));
        }
    }

    //리소스 불러오기.
    public GameObject LoadResource(string path, Vector3 pos, Quaternion rot)
    {
        return LoadResource(path, pos, rot, Vector3.one, null);
    }

    //리소스 부모 밑에 불러오기.
    public GameObject LoadResource(string path, Transform parent)
    {
        return LoadResource(path, Vector3.zero, Quaternion.identity, Vector3.one, parent);
    }

    public GameObject LoadResource(string path, Vector3 pos, Quaternion rot, Vector3 scale, Transform parent)
    {
        GameObject go;
        //미리 불러온 오브젝트가 있으면 불러와서 복제, 없으면 불러와서 리스트에 저장.
        //if (bulletKey == path)
        if(objectPoolList.ContainsKey(path))
        {
            //go = m_BulletPool.GetObject();
            go = objectPoolList[path].GetObject();
            usingPooledObjList.Add(go);
        }
        else
        {
            Object obj;
            if (!loadObjectList.ContainsKey(path))
            {
                obj = Resources.Load(path);
                loadObjectList.Add(path, obj);
            }
            else
            {
                obj = loadObjectList[path];
            }

            go = Instantiate(obj) as GameObject;
        }
        
        if(parent != null)
        {
            go.transform.parent = parent;
        }
        go.transform.localPosition = pos;
        go.transform.localRotation = rot;
        go.transform.localScale = scale;

        return go;
    }

    //스프라이트 불러오기.
    public Sprite LoadSprite(string path)
    {
        if(!spritePool.ContainsKey(path))
        {
            spritePool.Add(path, Resources.Load<Sprite>(path));
        }

        return spritePool[path];
    }

    //해당 경로에서 불러온 오브젝트 해제.
    public void DeleteResource(string path)
    {
        if(loadObjectList.ContainsKey(path))
        {
            Object temp = loadObjectList[path];
            loadObjectList.Remove(path);
            Resources.UnloadAsset(temp);
        }
    }

    public void InitObjectPool(string path, int initCount = 10, int addCount = 10)
    {
        if(objectPoolList.ContainsKey(path))
        {
            return;
        }

        Object obj;
        //미리 불러온 오브젝트가 있으면 불러와서 복제, 없으면 불러와서 리스트에 저장.
        if (!loadObjectList.ContainsKey(path))
        {
            obj = Resources.Load(path);
            loadObjectList.Add(path, obj);
        }
        else
        {
            obj = loadObjectList[path];
        }

        ObjectPool pool = new ObjectPool();
        pool.InitPool(obj, initCount, addCount);
        objectPoolList.Add(path, pool);

        //m_BulletPool = new ObjectPool();

        //bulletKey = path;

        //m_BulletPool.InitPool(obj, 10);
    }

    public void DestroyObject(string path, GameObject go)
    {
        if(usingPooledObjList.Contains(go))
        {
            usingPooledObjList.Remove(go);
            //m_BulletPool.ReturnObject(go);
            objectPoolList[path].ReturnObject(go);
        }
        else
        {
            Destroy(go);
        }
    }

    public void EndUseObjectPool()
    {
        //m_BulletPool.DestroyPool();
        //m_BulletPool = null;

        foreach(ObjectPool pool in objectPoolList.Values)
        {
            pool.DestroyPool();
        }
        objectPoolList.Clear();
    }
}
