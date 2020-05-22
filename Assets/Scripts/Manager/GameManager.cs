using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public bool playedTutorial;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        InfoManager.Instance.Init();
        DataManager.Instance.Init();
        BattleManager.Instance.Init();
        ResourceManager.Instance.PreLoadResources();
        SoundManager.Instance.PreLoadSound();

        playedTutorial = DataManager.Instance.playedTutorial;
    }

    // Update is called once per frame
    void Update ()
    {
        //pc에선 esc, 모바일에선 뒤로가기 버튼 터치 설정.
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            //Application.Quit();
            UIManager.Instance.LoadPopupUI("ExitUI");
            if(BattleManager.Instance.GameState == Def.DefEnum.GameState.PLAY)
            {
                BattleManager.Instance.Pause();
            }
        }

#if UNITY_EDITOR
        //에디터 전용 튜토리얼 다시 볼 수 있도록.
        if(playedTutorial != DataManager.Instance.playedTutorial)
        {
            DataManager.Instance.playedTutorial = playedTutorial;
        }
#endif
    }
}
