using UnityEngine;
using System.Collections;

/// <summary>
/// 게임 내 연출 정보 클래스.
/// </summary>
[System.Serializable]
public class IngameSceneInfo
{
    public enum SceneType { LOAD_EFFECT, PLAY_SOUND, SET_ACTIVE, LOAD_UI, PLAY_ANIMATION, END_SCENE }

    public float      startTime;            // 연출 시작 시간.
    public SceneType  type;                 // 연출 유형.
    public GameObject targetObject;         // 대상 오브젝트.
    public string     loadObjectName;       // 불러올 오브젝트 이름.
    public bool       active;               // 활성화 여부.
}

/// <summary>
/// 게임 내 연출 재생 클래스.
/// </summary>
public class IngameScene : MonoBehaviour
{
    #region Inspector

    public IngameSceneInfo[] ingameScenes;

    #endregion

    private float _timer;
    private int   _currentIndex;
    private GameObject _currentUI;

    private System.Action _sceneEndCallback;

    // Use this for initialization
    private void OnEnable ()
    {
        _timer = 0f;
        _currentIndex = 0;
    }

    /// <summary>
    /// 종료 콜백 설정.
    /// </summary>
    /// <param name="endCallback"></param>
    public void SetEndCallback(System.Action endCallback)
    {
        //연출 종료 콜백 함수 설정.
        _sceneEndCallback = endCallback;
    }

    private void Update()
    {
        var currentScene = ingameScenes[_currentIndex];
        if (_timer >= currentScene.startTime)
        {
            switch(currentScene.type)
            {
                //이펙트 출력.
                case IngameSceneInfo.SceneType.LOAD_EFFECT:
                    EffectManager.Instance.LoadEffect(currentScene.loadObjectName, currentScene.targetObject.transform.position, Quaternion.identity);
                    break;
                //사운드 재생.
                case IngameSceneInfo.SceneType.PLAY_SOUND:
                    SoundManager.Instance.PlaySound(currentScene.loadObjectName);
                    break;
                //오브젝트 활성화/비활성화.
                case IngameSceneInfo.SceneType.SET_ACTIVE:
                    currentScene.targetObject.SetActive(currentScene.active);
                    break;
                //UI 출력.
                case IngameSceneInfo.SceneType.LOAD_UI:
                    if(_currentUI != null)
                    {
                        Destroy(_currentUI);
                        _currentUI = null;
                    }
                    _currentUI = UIManager.Instance.LoadUI(currentScene.loadObjectName);
                    break;
                //애니메이션 재생.
                case IngameSceneInfo.SceneType.PLAY_ANIMATION:
                    currentScene.targetObject.GetComponent<Animator>().enabled = true;
                    currentScene.targetObject.GetComponent<Animator>().Play(currentScene.loadObjectName);
                    break;
                //연출 종료.
                case IngameSceneInfo.SceneType.END_SCENE:
                    if (_currentUI != null)
                    {
                        Destroy(_currentUI);
                        _currentUI = null;
                    }
                    this.gameObject.SetActive(false);
                    _sceneEndCallback?.Invoke();
                    return;
            }
            _currentIndex++;
        }
        else
        {
            _timer += Time.deltaTime;
        }
    }

    /// <summary>
    /// 다음 이벤트 바로 출력.
    /// </summary>
    public void NextEvent()
    {
        _timer = ingameScenes[_currentIndex].startTime;
    }
}
