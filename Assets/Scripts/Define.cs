﻿namespace Def
{
    public static class DefVal
    {
        public const int MainWeaponInfoID = 1;      //일반 무기 정보 인덱스.
    }

    public static class DefEnum
    {
        public enum Difficulty { EASY = 1, NORMAL, HARD }                                     //게임 난이도.
        public enum GameState { INTRO, SELECT_WEAPON, TUTORIAL, PLAY, PAUSE, END }        //게임 현재 상태.
        public enum EnemyState { MOVE, ATTACK }                                           //적 행동 상태.
        public enum MoveType { LINEAR = 1, GUIDED }                                       //적, 탄환 이동 유형.
        public enum AttackType { SINGLE = 1, ANGLE_RANGE, FRONT_RANGE, RANGE_EXPLOSION }  //탄환 공격 유형.
    }
}
