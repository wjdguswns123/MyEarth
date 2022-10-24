namespace Def
{
    public static class DefVal
    {
        public const int MainWeaponInfoID = 1;      //일반 무기 정보 인덱스.
    }

    public static class DefEnum
    {
        public enum Difficulty { EASY = 1, NORMAL, HARD }                                     //게임 난이도.
        public enum GameState { SELECT_WEAPON, TUTORIAL, PLAY, PAUSE, END }        //게임 현재 상태.
        public enum EnemyState { MOVE, ATTACK }                                           //적 행동 상태.
        public enum MoveType { LINEAR = 1, GUIDED }                                       //적, 탄환 이동 유형.
        public enum AttackType { SINGLE = 1, ANGLE_RANGE, FRONT_RANGE, RANGE_EXPLOSION }  //탄환 공격 유형.
    }

    public static class ResourcePath
    {
        /// <summary>
        /// 리소스 경로.
        /// </summary>
        public const string BULLET_PATH = "Bullets/";
        public const string WEAPON_PATH = "Weapons/";
        public const string ENEMY_PATH = "Enemy/";
        public const string EFFECT_PATH = "Effects/";
    }
}
