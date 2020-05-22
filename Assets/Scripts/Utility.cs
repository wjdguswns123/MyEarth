using UnityEngine;
using System.Collections;

public static class Utility
{
    //리소스 불러오기.
    public static GameObject LoadResource(string path, Vector3 pos, Quaternion rot)
    {
        Object obj = Resources.Load(path);
        GameObject go = Object.Instantiate(obj) as GameObject;
        go.transform.position = pos;
        go.transform.rotation = rot;

        return go;
    }
}
