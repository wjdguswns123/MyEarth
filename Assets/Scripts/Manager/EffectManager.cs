using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class EffectManager : Singleton<EffectManager>
{
    List<GameObject> effectList = new List<GameObject>();

    public void InitIngameEffects()
    {
        string path = Application.dataPath + "/Resources/Effects";

        if (!Directory.Exists(path))
        {
            //해당 경로 없으면 종료.
            return;
        }

        DirectoryInfo info = new DirectoryInfo(path);

        //에셋 번들 파일만 골라낸 파일 배열.
        FileInfo[] files = info.GetFiles().Where(f => (f.Extension != ".meta")).ToArray();

        foreach (FileInfo file in files)
        {
            string filename = "Effects/" + file.Name.Replace(".prefab", "");
            ResourceManager.Instance.InitObjectPool(filename, 5);
        }
    }

    //이펙트 이름, 생성될 위치, 회전값을 받아 이펙트 생성.
    public GameObject LoadEffect(string name, Vector3 pos, Quaternion rot)
    {
        string path = string.Format("Effects/{0}", name);
        GameObject effect = ResourceManager.Instance.LoadResource(path, pos, rot);
        effect.AddComponent<Effect>();
        effectList.Add(effect);
        return effect;
    }

    //이펙트 이름, 생성 시 상위 객체 받아 이펙트 생성.
    public GameObject LoadEffect(string name, Transform parent)
    {
        string path = string.Format("Effects/{0}", name);
        GameObject effect = ResourceManager.Instance.LoadResource(path, parent);
        effect.AddComponent<Effect>();
        effectList.Add(effect);
        return effect;
    }

    //전체 이펙트 움직임 정지.
    public void Pause()
    {
        int count = effectList.Count;
        for(int i = 0; i < count; ++i)
        {
            UITweener[] tps = effectList[i].GetComponentsInChildren<UITweener>();
            for(int j = 0; j < tps.Length; ++j)
            {
                tps[j].Pause();
            }

            ParticleSystem[] particles = effectList[i].GetComponentsInChildren<ParticleSystem>();
            for (int k = 0; k < particles.Length; ++k)
            {
                particles[k].Pause();
            }
        }
    }

    //전체 이펙트 움직임 정지 해제.
    public void Play()
    {
        int count = effectList.Count;
        for(int i = 0; i < count; ++i)
        {
            UITweener[] tps = effectList[i].GetComponentsInChildren<UITweener>();
            for(int j = 0; j < tps.Length; ++j)
            {
                tps[j].PauseEnd();
            }

            ParticleSystem[] particles = effectList[i].GetComponentsInChildren<ParticleSystem>();
            for (int k = 0; k < particles.Length; ++k)
            {
                particles[k].Play();
            }
        }
    }

    //해당 이펙트 리스트에서 제거.
    public void RemoveEffect(GameObject obj)
    {
        if(effectList.Contains(obj))
        {
            effectList.Remove(obj);
        }
    }

    //현재 생성되어 있는 이펙트 모두 제거.
    public void ClearEffectList()
    {
        int count = effectList.Count;
        for(int i = count - 1; i >= 0; --i)
        {
            GameObject effect = effectList[i];
            effectList.RemoveAt(i);
            Destroy(effect);
        }
    }
}
