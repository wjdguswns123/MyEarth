using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IngameSceneManager : Singleton<IngameSceneManager>
{
    private Dictionary<string, IngameScene> _sceneList;

    public IngameScene CurrentScene { get; private set; }

    private void Awake()
    {
        //현재 씬에 있는 모든 연출 등록.
        IngameScene[] scenes = gameObject.GetComponentsInChildren<IngameScene>();
        _sceneList = new Dictionary<string, IngameScene>();

        int count = scenes.Length;
        for(int i = 0; i < count; ++i)
        {
            if(!_sceneList.ContainsKey(scenes[i].name))
            {
                scenes[i].gameObject.SetActive(false);
                _sceneList.Add(scenes[i].name, scenes[i]);
            }
        }
    }

    /// <summary>
    /// 해당 이름의 연출 출력.
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="endCallback"></param>
    public void PlayIngameScene(string sceneName, System.Action endCallback = null)
    {
        if(_sceneList.ContainsKey(sceneName))
        {
            var scene = _sceneList[sceneName];
            scene.gameObject.SetActive(true);
            scene.SetEndCallback(endCallback);

            CurrentScene = scene;
        }
    }
}
