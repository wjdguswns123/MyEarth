using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IngameSceneManager : Singleton<IngameSceneManager>
{
    Dictionary<string, IngameScene> sceneList = new Dictionary<string, IngameScene>();

    IngameScene        currentScene;
    public IngameScene CurrentScene { get { return currentScene; } }

    private void Awake()
    {
        //현재 씬에 있는 모든 연출 등록.
        IngameScene[] scenes = gameObject.GetComponentsInChildren<IngameScene>();

        int count = scenes.Length;
        for(int i = 0; i < count; ++i)
        {
            if(!sceneList.ContainsKey(scenes[i].name))
            {
                scenes[i].gameObject.SetActive(false);
                sceneList.Add(scenes[i].name, scenes[i]);
            }
        }
    }

    //해당 이름의 연출 출력.
    public void PlayIngameScene(string sceneName, System.Action endCallback = null)
    {
        if(sceneList.ContainsKey(sceneName))
        {
            sceneList[sceneName].gameObject.SetActive(true);
            sceneList[sceneName].Init(endCallback);

            currentScene = sceneList[sceneName];
        }
    }
}
