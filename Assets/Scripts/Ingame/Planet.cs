using UnityEngine;
using System.Collections;
using Def;

public class Planet : MonoBehaviour
{
    #region Inspector

    public Transform planetBody;
    public Transform turretPos;
    public Transform planetMesh;
    public float     notInEnemyAreaRadius;

    #endregion

    public int CurrentHP { get; private set; }

    private int _maxHP;    
    private Turret _turret;

    /// <summary>
    /// 플레이어 정보 초기화.
    /// </summary>
    public void Init()
    {
        if(_maxHP == 0)
        {
            _maxHP = System.Convert.ToInt32(InfoManager.Instance.infoGlobalList["PlayerHP"].value);
        }

        CurrentHP = _maxHP;
        BattleManager.Instance.ingameUI.SetHPBar(_maxHP, CurrentHP);
    }

    #region Control

    private void Update()
    {
#if UNITY_EDITOR
        //전체 화면 마우스 클릭 조작. UI 콜라이더 걸리는 부분은 제외하고 클릭 처리.
        RaycastHit hit = new RaycastHit();
        Ray ray = UIManager.Instance.UICamera.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
        {
            if (!Physics.Raycast(ray, out hit))
            {
                OnMouseDrag();
            }
        }
#elif UNITY_ANDROID
        //전체 화면 터치 조작. UI 콜라이더 걸리는 부분은 제외하고 터치 처리.
        RaycastHit hit = new RaycastHit();

        if (Input.touchCount > 0)
        {
            Ray ray = UIManager.Instance.UICamera.ScreenPointToRay(Input.GetTouch(0).position);
            if (Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                if (!Physics.Raycast(ray, out hit))
                {
                    OnMouseDrag();
                }
            }
        }
#endif
    }

    /// <summary>
    /// 마우스 클릭/터치 드래그.
    /// </summary>
    private void OnMouseDrag()
    {
        if (BattleManager.Instance.GameState == DefEnum.GameState.PLAY)
        {
#if UNITY_EDITOR
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
#elif UNITY_ANDROID
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 0));
#endif
            RotatePlanet(mousePos);
        }
    }

    /// <summary>
    /// 행성 회전.
    /// </summary>
    /// <param name="mousePoint"></param>
    private void RotatePlanet(Vector3 mousePoint)
    {
        //행성의 윗 벡터를 터치 위치를 향하여 회전하듯 보이도록 설정.
        Vector3 currentVector = mousePoint - transform.position;
        currentVector.z = 0;
        transform.up = currentVector;
    }

    #endregion

    /// <summary>
    /// 사용할 무기 설정.
    /// </summary>
    /// <param name="mainWeaponId"></param>
    /// <param name="subInfo"></param>
    public void SetWeapon(InfoWeapon mainWeaponInfo)
    {
        if(_turret != null)
        {
            ResourceManager.Instance.ReleaseResource(_turret.gameObject);
            _turret = null;
        }
        InfoWeapon mainInfo = mainWeaponInfo;
        _turret = ResourceManager.Instance.LoadResource(ResourcePath.WEAPON_PATH, mainInfo.weaponPath, turretPos).GetComponent<Turret>();
        _turret.Init(mainInfo);
    }

    /// <summary>
    /// 전략 무기 설정.
    /// </summary>
    /// <param name="subInfo"></param>
    public void SetSubWeapon(InfoWeapon subInfo)
    {
        _turret.SetSubWeapon(subInfo);
    }

    /// <summary>
    /// 피격 처리.
    /// </summary>
    /// <param name="atk"></param>
    public void Attacked(int atk)
    {
        CurrentHP -= atk;

        if(CurrentHP <= 0)
        {
            CurrentHP = 0;
            EffectManager.Instance.LoadEffect("Bang_06", transform.position, Quaternion.identity);
            BattleManager.Instance.GameEnd(false);
        }

        BattleManager.Instance.ingameUI.SetHPBar(_maxHP, CurrentHP);
    }

    /// <summary>
    /// 무기 업그레이드 처리.
    /// </summary>
    public void UpgradeWeaponLevel()
    {
        _turret.UpgradeLevel();
    }

    /// <summary>
    /// 포대 모델링 삭제.
    /// </summary>
    public void Clear()
    {
        if(_turret != null)
        {
            ResourceManager.Instance.ReleaseResource(_turret.gameObject);
            _turret = null;
        }

        planetMesh.gameObject.SetActive(true);
    }

    /// <summary>
    /// 전략 무기 발사 처리.
    /// </summary>
    public void FireSubweapon()
    {
        _turret.FireSubweapon();
    }

    /// <summary>
    /// 남은 체력만큼의 공격력으로 강제 공격하여 강제 종료.
    /// </summary>
    public void ForcedEnd()
    {
        Attacked(CurrentHP);
    }

    ////적 접근 불가 거리 기즈모 그리기.
    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawWireSphere(transform.position, notInEnemyAreaRadius);
    //}
}
