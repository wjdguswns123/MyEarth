using UnityEngine;
using System.Collections;
using Def;

public class Planet : MonoBehaviour
{
    public Transform planetBody;
    public Transform turretPos;
    public Transform planetMesh;
    public float     notInEnemyAreaRadius;
    public int       maxHP;

    int              curHP;
    public int       CurrentHP { get { return curHP; } }
    Turret           turret;

	// Use this for initialization
	void Start ()
    {
        maxHP = System.Convert.ToInt32(InfoManager.Instance.infoGlobalList["PlayerHP"].value);
	}

    private void Update()
    {
#if UNITY_EDITOR
        //전체 화면 마우스 클릭 조작. UI 콜라이더 걸리는 부분은 제외하고 클릭 처리.
        RaycastHit hit = new RaycastHit();
        Ray ray = UIManager.Instance.UICamera.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            if (!Physics.Raycast(ray, out hit))
            {
                OnMouseDown();
            }
        }
        else if (Input.GetMouseButton(0))
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
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                if (!Physics.Raycast(ray, out hit))
                {
                    OnMouseDown();
                }
            }
            else if(Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                if (!Physics.Raycast(ray, out hit))
                {
                    OnMouseDrag();
                }
            }
        }
#endif
    }

    //플레이어 정보 초기화.
    public void Init()
    {
        curHP = maxHP;
        BattleManager.Instance.ingameUI.SetHPBar(maxHP, curHP);
        planetBody.rotation = Quaternion.identity;
    }

    //사용할 무기 설정.
    public void SetWeapon(int mainWeaponId, InfoWeapon subInfo)
    {
        if(turret != null)
        {
            Destroy(turret.gameObject);
        }
        InfoWeapon mainInfo = InfoManager.Instance.infoWeaponList[mainWeaponId];
        turret = ResourceManager.Instance.LoadResource(mainInfo.weaponPath, turretPos).GetComponent<Turret>();
        turret.Init(mainInfo, subInfo);
    }

    //시작 벡터 설정.
    void SetRotateVector(Vector3 mousePoint)
    {
        //행성의 윗 벡터를 입력 받은 곳을 향하도록 설정.
        //행성 몸체는 기존의 회전값을 유지하도록 설정.
        Vector3 currentVector = mousePoint - transform.position;
        currentVector.z = 0;

        Quaternion rot = planetBody.rotation;

        transform.up = currentVector;
        planetBody.rotation = rot;
    }

    //행성 회전.
    void RotatePlanet(Vector3 mousePoint)
    {
        //행성의 윗 벡터를 터치 위치를 향하여 회전하듯 보이도록 설정.
        Vector3 currentVector = mousePoint - transform.position;
        currentVector.z = 0;

        transform.up = currentVector;
    }

    //마우스 클릭/터치 시작.
    private void OnMouseDown()
    {
        //if(BattleManager.Instance.Playing)
        if (BattleManager.Instance.GameState == DefEnum.GameState.PLAY)
        {
#if UNITY_EDITOR
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
#elif UNITY_ANDROID
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 0));
#endif
            SetRotateVector(mousePos);
        }
    }

    //마우스 클릭/터치 드래그.
    private void OnMouseDrag()
    {
        //if (BattleManager.Instance.Playing)
        if(BattleManager.Instance.GameState == DefEnum.GameState.PLAY)
        {
#if UNITY_EDITOR
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
#elif UNITY_ANDROID
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 0));
#endif
            RotatePlanet(mousePos);
        }   
    }

    //피격 처리.
    public void Attacked(int atk)
    {
        curHP -= atk;

        if(curHP <= 0)
        {
            curHP = 0;
            //BattleManager.Instance.Playing = false;
            EffectManager.Instance.LoadEffect("Bang_06", transform.position, Quaternion.identity);
            BattleManager.Instance.GameEnd(false);
        }

        BattleManager.Instance.ingameUI.SetHPBar(maxHP, curHP);
    }

    //무기 업그레이드 처리.
    public void UpgradeWeaponLevel()
    {
        turret.UpgradeLevel();
    }

    //포대 모델링 삭제.
    public void Clear()
    {
        if(turret != null)
        {
            Destroy(turret.gameObject);
        }
        turret = null;

        planetMesh.gameObject.SetActive(true);
    }

    //전략 무기 발사 처리.
    public void FireSubweapon()
    {
        turret.FireSubweapon();
    }

    //남은 체력만큼의 공격력으로 강제 공격하여 강제 종료.
    public void ForcedEnd()
    {
        Attacked(curHP);
    }

    ////적 접근 불가 거리 기즈모 그리기.
    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawWireSphere(transform.position, notInEnemyAreaRadius);
    //}
}
