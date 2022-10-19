using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Effect : MonoBehaviour
{
    #region Inspector

    public float duration;

    #endregion

    private UITweener[] _tweeners;
    private ParticleSystem[] _particles;

    private float _time;
    private bool _isPause;

    private void Awake()
    {
        _tweeners = this.GetComponentsInChildren<UITweener>();
        _particles = this.GetComponentsInChildren<ParticleSystem>();
    }

    private void OnEnable()
    {
        _time = 0f;
        _isPause = false;

        for(int i = 0; i < _tweeners.Length; ++i)
        {
            _tweeners[i].enabled = true;
            _tweeners[i].ResetToBeginning();
        }
    }

    private void Update()
    {
        if(!_isPause && duration > 0f)
        {
            _time += Time.deltaTime;
            if(_time > duration)
            {
                this.gameObject.SetActive(false);
            }
        }
    }

    private void OnDisable()
    {
        //비활성화 될 때 이펙트 매니저 안의 리스트에서 제거 요청.
        EffectManager.Instance.RemoveEffect(this);
    }

    /// <summary>
    /// 일시 정지.
    /// </summary>
    public void Pause()
    {
        _isPause = true;
        if (_tweeners != null)
        {
            for (int i = 0; i < _tweeners.Length; ++i)
            {
                _tweeners[i].Pause();
            }
        }
        
        if(_particles != null)
        {
            for (int j = 0; j < _particles.Length; ++j)
            {
                _particles[j].Pause();
            }
        }
    }

    /// <summary>
    /// 일시 정지 해제.
    /// </summary>
    public void Resume()
    {
        _isPause = false;
        if (_tweeners != null)
        {
            for (int i = 0; i < _tweeners.Length; ++i)
            {
                _tweeners[i].PauseEnd();
            }
        }

        if (_particles != null)
        {
            for (int j = 0; j < _particles.Length; ++j)
            {
                _particles[j].Play();
            }
        }
    }
}
