using UnityEngine;
using System.Collections;

//게임 내 연출 정보 클래스.
[System.Serializable]
public class IngameSceneInfo
{
    public enum SceneType { LOAD_EFFECT, PLAY_SOUND, SET_ACTIVE, LOAD_UI, PLAY_ANIMATION, END_SCENE }

    public float      startTime;            //연출 시작 시간.
    public SceneType  type;                 //연출 유형.
    public GameObject targetObject;         //대상 오브젝트.
    public string     loadObjectName;       //불러올 오브젝트 이름.
    public bool       active;               //활성화 여부.
}

//게임 내 연출 재생 클래스.
public class IngameScene : MonoBehaviour
{
    public IngameSceneInfo[] ingameScenes = new IngameSceneInfo[1];

    float         timer;
    int           currentIndex;
    GameObject    currentUI;

    System.Action sceneEndCallback;

    // Use this for initialization
    void OnEnable ()
    {
        timer = 0f;
        currentIndex = 0;
    }

    public void Init(System.Action endCallback)
    {
        //연출 종료 콜백 함수 설정.
        sceneEndCallback = endCallback;
    }

    private void Update()
    {
        if(timer >= ingameScenes[currentIndex].startTime)
        {
            switch(ingameScenes[currentIndex].type)
            {
                //이펙트 출력.
                case IngameSceneInfo.SceneType.LOAD_EFFECT:
                    EffectManager.Instance.LoadEffect(ingameScenes[currentIndex].loadObjectName, ingameScenes[currentIndex].targetObject.transform.position, Quaternion.identity);
                    break;
                //사운드 재생.
                case IngameSceneInfo.SceneType.PLAY_SOUND:
                    SoundManager.Instance.PlaySound(ingameScenes[currentIndex].loadObjectName);
                    break;
                //오브젝트 활성화/비활성화.
                case IngameSceneInfo.SceneType.SET_ACTIVE:
                    ingameScenes[currentIndex].targetObject.SetActive(ingameScenes[currentIndex].active);
                    break;
                //UI 출력.
                case IngameSceneInfo.SceneType.LOAD_UI:
                    if(currentUI != null)
                    {
                        Destroy(currentUI);
                        currentUI = null;
                    }
                    currentUI = UIManager.Instance.LoadUI(ingameScenes[currentIndex].loadObjectName);
                    break;
                //애니메이션 재생.
                case IngameSceneInfo.SceneType.PLAY_ANIMATION:
                    ingameScenes[currentIndex].targetObject.GetComponent<Animator>().enabled = true;
                    ingameScenes[currentIndex].targetObject.GetComponent<Animator>().Play(ingameScenes[currentIndex].loadObjectName);
                    break;
                //연출 종료.
                case IngameSceneInfo.SceneType.END_SCENE:
                    if (currentUI != null)
                    {
                        Destroy(currentUI);
                        currentUI = null;
                    }
                    this.gameObject.SetActive(false);
                    if(sceneEndCallback != null)
                    {
                        sceneEndCallback();
                    }
                    return;
            }
            currentIndex++;
        }
        else
        {
            timer += Time.deltaTime;
        }
    }

    //다음 이벤트 바로 출력.
    public void NextEvent()
    {
        timer = ingameScenes[currentIndex].startTime;
    }
}
