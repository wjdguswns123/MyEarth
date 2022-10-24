using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    private List<GameObject> _objectList;

    private GameObject _originalObject;
    private Transform  _objectsParent;

    private int        _initCount;
    private int        _addCount;   // 풀에 사용 가능한 오브젝트가 모자랄 때 한번에 생성해 줄 추가 오브젝트 갯수.

    /// <summary>
    /// 해당 오브젝트의 풀 생성.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="initCount"></param>
    /// <param name="addCnt"></param>
    /// <param name="parent"></param>
    public void InitPool(string path, int initCount, int addCnt = 1, Transform parent = null)
    {
        GameObject obj = Resources.Load<GameObject>(path);
        InitPool(obj, initCount, addCnt, parent);
    }

    /// <summary>
    /// 해당 오브젝트의 풀 생성.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="initCount"></param>
    /// <param name="addCnt"></param>
    /// <param name="parent"></param>
    public void InitPool(GameObject obj, int initCount, int addCnt = 1, Transform parent = null)
    {
        if(_objectList == null)
        {
            _objectList = new List<GameObject>();
        }

        // 오브젝트 부모 할당 또는 생성.
        if(_objectsParent == null)
        {
            _objectsParent = parent != null ? parent : new GameObject(obj.name + " Pool").transform;
        }

        _originalObject = obj;
        _initCount = initCount;
        _addCount = addCnt;

        ClearPool();

        CreateObject(_initCount);
    }

    /// <summary>
    /// 오브젝트 얻어오기.
    /// </summary>
    /// <returns></returns>
    public GameObject GetObject()
    {
        if(_objectList == null || _objectList.Count <= 0)
        {
            CreateObject(_addCount);
        }

        GameObject go = null;
        for (int i = 0; i < _objectList.Count; ++i)
        {
            var obj = _objectList[i];
            if(!obj.activeSelf)
            {
                go = obj;
                go.SetActive(true);
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                break;
            }
        }

        if(go == null)
        {
            go = CreateObject(_addCount);
        }

        return go;
    }

    /// <summary>
    /// 오브젝트 반환 받기.
    /// </summary>
    /// <param name="go"></param>
    public void ReturnObject(GameObject go)
    {
        go.transform.SetParent(_objectsParent);
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        go.SetActive(false);
    }

    /// <summary>
    /// 오브젝트 생성.
    /// </summary>
    /// <param name="count"></param>
    private GameObject CreateObject(int count)
    {
        if (_objectList == null)
        {
            _objectList = new List<GameObject>();
        }

        GameObject result = null;
        for (int i = 0; i < count; ++i)
        {
            GameObject go = Object.Instantiate(_originalObject);
            go.SetActive(false);
            go.transform.SetParent(_objectsParent);
            _objectList.Add(go);
            if(result == null)
            {
                result = go;
            }
        }
        return result;
    }

    /// <summary>
    /// 풀 클리어.
    /// </summary>
    public void ClearPool()
    {
        if (_objectList != null)
        {
            while (_objectList.Count > 0)
            {
                GameObject obj = _objectList[0];
                _objectList.RemoveAt(0);
                Object.Destroy(obj);
            }
        }
    }

    /// <summary>
    /// 풀 삭제.
    /// </summary>
    public void DestroyPool()
    {
        ClearPool();

        if (_objectsParent != null)
        {
            Object.Destroy(_objectsParent.gameObject);
        }
    }
}
