using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    private Queue<GameObject> objectQueue = new Queue<GameObject>();

    private Object     pooledObject;
    private Transform  pooledObjectsParent;

    private int        initCount;
    private int        addCount;

    public void InitPool(Object obj, int count, int addCnt)
    {
        if(pooledObjectsParent == null)
        {
            pooledObjectsParent = new GameObject(obj.name + " Pool").transform;
        }

        pooledObject = obj;
        initCount    = count;
        addCount     = addCnt;

        ClearPool();

        CreateObject(initCount);
    }

    public GameObject GetObject()
    {
        if(objectQueue.Count <= 0)
        {
            CreateObject(addCount);
        }

        GameObject obj = objectQueue.Dequeue();
        obj.SetActive(true);

        return obj;
    }

    public void ReturnObject(GameObject go)
    {
        objectQueue.Enqueue(go);
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        go.SetActive(false);
    }

    void CreateObject(int count)
    {
        for (int i = 0; i < count; ++i)
        {
            GameObject go = Instantiate(pooledObject) as GameObject;
            go.SetActive(false);
            go.transform.SetParent(pooledObjectsParent);
            objectQueue.Enqueue(go);
        }
    }

    public void ClearPool()
    {
        while(objectQueue.Count > 0)
        {
            GameObject obj = objectQueue.Dequeue();
            Destroy(obj);
        }
    }

    public void DestroyPool()
    {
        ClearPool();

        if (pooledObjectsParent != null)
        {
            Destroy(pooledObjectsParent.gameObject);
        }
    }
}
