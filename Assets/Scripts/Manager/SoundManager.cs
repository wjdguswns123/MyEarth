using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : Singleton<SoundManager>
{
    private const string SOUND_PATH = "Sounds";

    #region Inspector

    public AudioSource BGMSource;
    public GameObject  audioSources;

    #endregion

    private AudioSource[] _audioSourceList;
    private Dictionary<string, AudioClip> _audioClipList;

    private bool _onBGM;
    public bool  OnBGM { set { _onBGM = value; } get { return _onBGM; } }
    
    private bool _onSFX;
    public bool  OnSFX { set { _onSFX = value; } get { return _onSFX; } }

    private void Awake()
    {
        _audioSourceList = audioSources.GetComponentsInChildren<AudioSource>();
        //로컬에 저장된 배경음과 효과음 켜짐 상태 불러옴.
        _onBGM = PlayerPrefs.GetInt("OnBGM") == 0;
        _onSFX = PlayerPrefs.GetInt("OnSFX") == 0;
        PlayBGM();
    }

    /// <summary>
    /// 사운드 클립 미리 불러와서 저장. 
    /// </summary>
    public void PreLoadSound()
    {
        if(_audioClipList == null)
        {
            _audioClipList = new Dictionary<string, AudioClip>();
        }

        AudioClip[] objs = Resources.LoadAll<AudioClip>(SOUND_PATH);
        int count = objs.Length;
        for (int i = 0; i < count; ++i)
        {
            var clip = objs[i];
            if (!_audioClipList.ContainsKey(clip.name))
            {
                _audioClipList.Add(clip.name, clip);
            }
        }
    }

    public void PlaySound(string fileName)
    {
        PlaySound(fileName, true);
    }

    /// <summary>
    /// 해당 파일명의 사운드 출력.
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="overlapPlay"></param>
    public void PlaySound(string fileName, bool overlapPlay)
    {
        //효과음 끈 상태면 출력하지 않음.
        if(!_onSFX)
        {
            return;
        }

        if(fileName.Length == 0)
        {
            return;
        }

        if (_audioClipList == null)
        {
            _audioClipList = new Dictionary<string, AudioClip>();
        }

        AudioClip clip;
        if(!_audioClipList.ContainsKey(fileName))
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(SOUND_PATH);
            sb.Append("/");
            sb.Append(fileName);
            clip = Resources.Load<AudioClip>(sb.ToString());
            _audioClipList.Add(fileName, clip);
        }
        else
        {
            clip = _audioClipList[fileName];
        }

        int existIndex = ContainsAudioClip(clip);
        if (existIndex > -1)
        {
            //해당 클립을 가지고 있는 오디오 소스가 있고,
            //오디오 소스가 해당 클립 재생이 끝난 상태이거나, 해당 클립이 재생중인데 overlap 인자가 true이면,
            //재생 중인 클립 멈추고 처음부터 재생.
            var source = _audioSourceList[existIndex];
            if(!source.isPlaying || (source.isPlaying && overlapPlay))
            {
                source.Stop();
                source.Play();
            }
        }
        else
        {
            int count = _audioSourceList.Length;
            for (int i = 0; i < count; ++i)
            {
                var source = _audioSourceList[i];
                //오디오 소스가 클립을 가지고 있지 않거나 가지고 있어도 재생이 끝난 상태이면 클립 교체 후 재생.
                if (source.clip == null || !source.isPlaying)
                {
                    source.clip = clip;
                    source.Play();
                    break;
                }
            }
        }
    }

    /// <summary>
    /// 해당 오디오 클립을 가진 오디오 소스가 있는지 검색.
    /// </summary>
    /// <param name="clip"></param>
    /// <returns></returns>
    private int ContainsAudioClip(AudioClip clip)
    {
        int count = _audioSourceList.Length;
        for (int i = 0; i < count; ++i)
        {
            if (_audioSourceList[i].clip.Equals(clip))
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// 재생중인 모든 사운드 일시 정지.
    /// </summary>
    public void Pause()
    {
        int count = _audioSourceList.Length;
        for(int i = 0; i < count; ++i)
        {
            var source = _audioSourceList[i];
            if (source.clip != null && source.isPlaying)
            {
                source.Pause();
            }
        }
    }

    /// <summary>
    /// 일시 정지중인 모든 사운드 다시 재생 시작.
    /// </summary>
    public void Resume()
    {
        int count = _audioSourceList.Length;
        for (int i = 0; i < count; ++i)
        {
            var source = _audioSourceList[i];
            if (source.clip != null)
            {
                source.UnPause();
            }
        }
    }

    /// <summary>
    /// 모든 효과음 정지.
    /// </summary>
    public void AllSFXStop()
    {
        int count = _audioSourceList.Length;
        for (int i = 0; i < count; ++i)
        {
            var source = _audioSourceList[i];
            if (source.clip != null)
            {
                source.Stop();
            }
        }
    }

    /// <summary>
    /// 배경음 켜기/끄기.
    /// </summary>
    public void SetBGM()
    {
        _onBGM = !_onBGM;
        PlayerPrefs.SetInt("OnBGM", _onBGM ? 0 : 1);
        PlayBGM();
    }

    /// <summary>
    /// 배경음 켰으면 배경음 재생, 껐으면 정지.
    /// </summary>
    void PlayBGM()
    {
        if(_onBGM)
        {
            BGMSource.Play();
        }
        else
        {
            BGMSource.Stop();
        }
    }

    /// <summary>
    /// 효과음 켜기/끄기.
    /// </summary>
    public void SetSFX()
    {
        _onSFX = !_onSFX;
        PlayerPrefs.SetInt("OnSFX", _onSFX ? 0 : 1);
    }
}