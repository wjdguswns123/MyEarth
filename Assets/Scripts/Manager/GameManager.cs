using UnityEngine;
using System.Collections;

public class GameManager : Singleton<GameManager>
{
    public bool playedTutorial;

    public bool IsEnableDlbTouchFire { get; private set; }

    private void Awake()
    {
        Application.targetFrameRate = 60;
        InfoManager.Instance.Init();
        DataManager.Instance.Init();
        SoundManager.Instance.PreLoadSound();

        playedTutorial = DataManager.Instance.playedTutorial;

        //더블 터치로 전략무기 발사 가능 로컬에 저장된 값을 불러온다.
        IsEnableDlbTouchFire = PlayerPrefs.GetInt("OnDblTouchFire") != 0;

        LobbyManager.Instance.ShowLobby();
    }

    // Update is called once per frame
    private void Update ()
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

    /// <summary>
    /// 더블 터치로 전략 무기 발사 켜기/끄기.
    /// </summary>
    public void SetDblTouchFire()
    {
        IsEnableDlbTouchFire = !IsEnableDlbTouchFire;
        PlayerPrefs.SetInt("OnDblTouchFire", IsEnableDlbTouchFire ? 1 : 0);
    }
}
