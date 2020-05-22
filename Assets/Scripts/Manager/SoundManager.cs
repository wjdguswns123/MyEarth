using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : Singleton<SoundManager>
{
    public AudioSource BGMSource;
    public GameObject  audioSources;
    AudioSource[]      audioSourceList;

    Dictionary<string, AudioClip> audioClipList = new Dictionary<string, AudioClip>();

    bool              onBGM;
    public bool       OnBGM { set { onBGM = value; } get { return onBGM; } }
    bool              onSFX;
    public bool       OnSFX { set { onSFX = value; } get { return onSFX; } }

    private void Awake()
    {
        audioSourceList = audioSources.GetComponentsInChildren<AudioSource>();
        //로컬에 저장된 배경음과 효과음 켜짐 상태 불러옴.
        onBGM = PlayerPrefs.GetInt("OnBGM") == 0;
        onSFX = PlayerPrefs.GetInt("OnSFX") == 0;
        PlayBGM();
    }
    
    //사운드 클립 미리 불러와서 저장.
    public void PreLoadSound()
    {
        AudioClip[] objs = Resources.LoadAll<AudioClip>("Sounds");
        int count = objs.Length;
        for (int i = 0; i < count; ++i)
        {
            if (!audioClipList.ContainsKey(objs[i].name))
            {
                audioClipList.Add(objs[i].name, objs[i]);
            }
        }
    }

    public void PlaySound(string fileName)
    {
        PlaySound(fileName, true);
    }

    //해당 파일명의 사운드 출력.
    public void PlaySound(string fileName, bool overlapPlay)
    {
        //효과음 끈 상태면 출력하지 않음.
        if(!onSFX)
        {
            return;
        }

        if(fileName.Length == 0)
        {
            return;
        }

        AudioClip clip;
        if(!audioClipList.ContainsKey(fileName))
        {
            clip = Resources.Load(string.Format("Sounds/{0}", fileName)) as AudioClip;
            audioClipList.Add(fileName, clip);
        }
        else
        {
            clip = audioClipList[fileName];
        }

        int existIndex = ContainsAudioClip(clip);
        if (existIndex > -1)
        {
            //해당 클립을 가지고 있는 오디오 소스가 있고,
            //오디오 소스가 해당 클립 재생이 끝난 상태이거나, 해당 클립이 재생중인데 overlap 인자가 true이면,
            //재생 중인 클립 멈추고 처음부터 재생.
            if(!audioSourceList[existIndex].isPlaying || (audioSourceList[existIndex].isPlaying && overlapPlay))
            {
                audioSourceList[existIndex].Stop();
                audioSourceList[existIndex].Play();
            }
        }
        else
        {
            int count = audioSourceList.Length;
            for (int i = 0; i < count; ++i)
            {
                //오디오 소스가 클립을 가지고 있지 않거나 가지고 있어도 재생이 끝난 상태이면 클립 교체 후 재생.
                if (audioSourceList[i].clip == null || !audioSourceList[i].isPlaying)
                {
                    audioSourceList[i].clip = clip;
                    audioSourceList[i].Play();
                    break;
                }
            }
        }
    }

    //해당 오디오 클립을 가진 오디오 소스가 있는지 검색.
    int ContainsAudioClip(AudioClip clip)
    {
        int count = audioSourceList.Length;
        for (int i = 0; i < count; ++i)
        {
            if (audioSourceList[i].clip == clip)
            {
                return i;
            }
        }

        return -1;
    }

    //재생중인 모든 사운드 일시 정지.
    public void Pause()
    {
        int count = audioSourceList.Length;
        for(int i = 0; i < count; ++i)
        {
            if(audioSourceList[i].clip != null && audioSourceList[i].isPlaying)
            {
                audioSourceList[i].Pause();
            }
        }
    }
    
    //일시 정지중인 모든 사운드 다시 재생 시작.
    public void Play()
    {
        int count = audioSourceList.Length;
        for (int i = 0; i < count; ++i)
        {
            if (audioSourceList[i].clip != null)
            {
                audioSourceList[i].UnPause();
            }
        }
    }

    //모든 효과음 정지.
    public void AllSFXStop()
    {
        int count = audioSourceList.Length;
        for (int i = 0; i < count; ++i)
        {
            if (audioSourceList[i].clip != null)
            {
                audioSourceList[i].Stop();
            }
        }
    }

    //배경음 켜기/끄기.
    public void SetBGM()
    {
        onBGM = !onBGM;
        PlayerPrefs.SetInt("OnBGM", onBGM ? 0 : 1);
        PlayBGM();
    }

    //배경음 켰으면 배경음 재생, 껐으면 정지.
    void PlayBGM()
    {
        if(onBGM)
        {
            BGMSource.Play();
        }
        else
        {
            BGMSource.Stop();
        }
    }

    //효과음 켜기/끄기.
    public void SetSFX()
    {
        onSFX = !onSFX;
        PlayerPrefs.SetInt("OnSFX", onSFX ? 0 : 1);
    }
}