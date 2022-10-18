using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ResourceManager : Singleton<ResourceManager>
{
    private Dictionary<string, ObjectPool> _objectPoolList;

    //해당 경로의 프리팹 미리 로딩.
    public void PreLoadReaource(string path)
    {
        if(_objectPoolList == null)
        {
            _objectPoolList = new Dictionary<string, ObjectPool>();
        }

        if (!_objectPoolList.ContainsKey(path))
        {
            CreateObjectPool(path);
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

    /// <summary>
    /// 리소스 로드.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="pos"></param>
    /// <param name="rot"></param>
    /// <param name="scale"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public GameObject LoadResource(string path, Vector3 pos, Quaternion rot, Vector3 scale, Transform parent)
    {
        if (_objectPoolList == null)
        {
            _objectPoolList = new Dictionary<string, ObjectPool>();
        }

        //해당 경로의 풀이 생성되어 있지 않으면 풀 생성.
        if (!_objectPoolList.ContainsKey(path))
        {
            CreateObjectPool(path);
        }
        GameObject go = _objectPoolList[path].GetObject();
        
        if(parent != null)
        {
            go.transform.parent = parent;
        }
        go.transform.localPosition = pos;
        go.transform.localRotation = rot;
        go.transform.localScale = scale;

        return go;
    }

    public void CreateObjectsPool(string path, int initCount = 1, int addCount = 1)
    {
        if (_objectPoolList == null)
        {
            _objectPoolList = new Dictionary<string, ObjectPool>();
        }

        var objs = Resources.LoadAll<GameObject>(path);
        for (int i = 0; i < objs.Length; i++)
        {
            ObjectPool pool = new ObjectPool();
            pool.InitPool(objs[i], initCount, addCount);
            string name = path + objs[i].name;
            _objectPoolList.Add(name, pool);
        }
    }

    /// <summary>
    /// 오브젝트 풀 생성.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="initCount"></param>
    /// <param name="addCount"></param>
    public void CreateObjectPool(string path, int initCount = 1, int addCount = 1)
    {
        if (_objectPoolList != null && !_objectPoolList.ContainsKey(path))
        {
            ObjectPool pool = new ObjectPool();
            pool.InitPool(path, initCount, addCount);
            _objectPoolList.Add(path, pool);
        }
    }

    /// <summary>
    /// 해당 경로의 오브젝트 풀 해제.
    /// </summary>
    /// <param name="path"></param>
    public void DeleteResource(string path)
    {
        if(_objectPoolList.ContainsKey(path))
        {
            _objectPoolList[path].DestroyPool();
            _objectPoolList.Remove(path);
        }
    }

    /// <summary>
    /// 오브젝트 풀 전체 해제.
    /// </summary>
    public void ClearObjectPools()
    {
        foreach(ObjectPool pool in _objectPoolList.Values)
        {
            pool.DestroyPool();
        }
        _objectPoolList.Clear();
    }
}
