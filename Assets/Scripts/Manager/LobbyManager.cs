using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : Singleton<LobbyManager>
{
    /// <summary>
    /// 인트로 UI 출력.
    /// </summary>
    public void ShowLobby()
    {
        UIManager.Instance.LoadUI("IntroUI");
    }
}
