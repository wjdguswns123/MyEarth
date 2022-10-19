﻿using UnityEngine;
using System.Collections;

public class IntroUI : MonoBehaviour
{
    public UILabel bestScoreLabel;
    public UILabel difficulltyLabel;

    private Def.DefEnum.Difficulty _difficulty;

    private void Start()
    {
        //로컬에 저장된 최고 점수 불러오기.
        bestScoreLabel.text = DataManager.Instance.bestScore.ToString();

        _difficulty = Def.DefEnum.Difficulty.EASY;
        difficulltyLabel.text = _difficulty.ToString();
    }

    //시작 버튼 클릭 처리.
    public void OnStartBtnClick()
    {
        BattleManager.Instance.StartProcess(_difficulty);
        //튜토리얼 안봤으면 튜토리얼 출력.
        if(!DataManager.Instance.playedTutorial)
        {
            IngameSceneManager.Instance.PlayIngameScene("Tutorial_01");
        }
        Destroy(this.gameObject);
    }

    //기능 버튼 클릭 처리.
    public void OnOptionBtnClick()
    {
        OptionUI ui = UIManager.Instance.LoadPopupUI("OptionUI").GetComponent<OptionUI>();
        ui.Init();
    }

    //더 높은 난이도 선택 버튼 클릭 처리.
    public void OnHighDifficultyBtnClick()
    {
        if(_difficulty < Def.DefEnum.Difficulty.HARD)
        {
            ++_difficulty;
            difficulltyLabel.text = _difficulty.ToString();
        }
        //Debug.Log("난이도 높게 " + DataManager.Instance.GameDifficulty.ToString());
    }

    //더 낮은 난이도 선택 버튼 클릭 처리.
    public void OnLowDifficultyBtnClick()
    {
        if (_difficulty > Def.DefEnum.Difficulty.EASY)
        {
            --_difficulty;
            difficulltyLabel.text = _difficulty.ToString();
        }
        //Debug.Log("난이도 낮게 " + DataManager.Instance.GameDifficulty.ToString());
    }
}
