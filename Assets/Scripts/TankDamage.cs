using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TankDamage : MonoBehaviour
{
    //탱크폭발후 투명 처리를 위한 MeshRenderer 컴포넌트 배열
    private MeshRenderer[] renderers;
    private TankMove _tankMove;

    //탱크 폭발효과 Prefab을 연결할 변수
    private GameObject expEffect = null;

    //초기 생명치
    private int initHp = 100;
    //현재 생명치
    private int currentHp = 0;
    //플레이어가 Respawn될때 새로운좌표를 받게합니다.
    private Transform playerTr;

    //탱크 하위의 Canvas 객제를 연결할 변수
    public Canvas hudCanvas;
    //Filled 타입의  Image UI 항목을 연결할 변수.
    public Image hpBar;
    
    private Rigidbody _rigid;

    //Photon view 컴포넌트를 할당할 변수.
    private PhotonView pv = null;
    public int playerId = -1;
    public int killCount = 0;
    public Text txtKillCount;

    private void Awake()
    {
        //Tank의 GameObject의 하위에있는 모든 Models의 MeshRenderer 컴포넌트를 추출한 후 배열에 할당합니다.
        renderers = GetComponentsInChildren<MeshRenderer>();
        _tankMove = GetComponent<TankMove>();
        playerTr = GetComponent<Transform>();
        _rigid = GetComponent<Rigidbody>();

        //pv = GetComponent<PhotonView>();
        //pv.synchronization = ViewSynchronization.UnreliableOnChange;
        //pv.ObservedComponents[1] = this;

        //현재 생명치를 초기 생명치로 초기화합니다.
        currentHp = initHp;

        //Tank 폴발시 생성시킬 폴발효과를 로드합니다.
        expEffect = Resources.Load<GameObject>("PlasmaExplosionEffect");

        //Filled 이미지 색상을 녹색으로 설정합니다.
        hpBar.color = Color.green;

        pv = GetComponent<PhotonView>();
        playerId = pv.ownerId;

        //pv.synchronization = ViewSynchronization.UnreliableOnChange;
        //0번째는 TankMove로 설정되어있습니다.
        //pv.ObservedComponents[1] = this;
  
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider coll)
    {
        if(currentHp > 0 && coll.tag == "CANNON")
        {
            currentHp -= 20;

            //현재 생명력 백분율 = (현재생명치) / (초기 생명치)
            hpBar.fillAmount = (float)currentHp / (float)initHp;

            //생명수치에 따라 Filled 이미지의 색상을 변경합니다.
            if (hpBar.fillAmount <= 0.4f)
                hpBar.color = Color.red;
            else if (hpBar.fillAmount <= 0.6f)
                hpBar.color = Color.yellow;

            if(currentHp <= 0)
            {
                SaveKillCount(coll.GetComponent<Cannon>().playerId);
                StartCoroutine(this.ExplosionTank());
            }
        }

        else if (currentHp > 0 && coll.tag == "FRAG")
        {
            currentHp -= 10;

            //현재 생명력 백분율 = (현재생명치) / (초기 생명치)
            hpBar.fillAmount = (float)currentHp / (float)initHp;

            //생명수치에 따라 Filled 이미지의 색상을 변경합니다.
            if (hpBar.fillAmount <= 0.4f)
                hpBar.color = Color.red;
            else if (hpBar.fillAmount <= 0.6f)
                hpBar.color = Color.yellow;

            if (currentHp <= 0)
            {
                SaveKillCount(coll.GetComponent<FragSc>().playerId);
                StartCoroutine(this.ExplosionTank());
            }
        }
    }

    IEnumerator ExplosionTank()
    {
        Object effect = GameObject.Instantiate(expEffect,
            transform.position,
            Quaternion.identity);

        Destroy(effect, 2.0f);


        float spawnPos = Random.Range(-100.0f, 100.0f);
        playerTr.position = new Vector3(spawnPos, 10.0f, spawnPos);

        //hud를 비활성화
        hudCanvas.enabled = false;

        SetTankVisible(false);

        _tankMove.enabled = false;

        //탱크터졌을때는 Kinematics true 가된다.
        _rigid.isKinematic = true;

        //5초 대기후 다시 활성화하는 로직을 수행합니다.
        yield return new WaitForSeconds(5.0f);

        //Filled 이미지를 초기값으로 환원 합니다.
        hpBar.fillAmount = 1.0f;
        //Filled 이미지의 색상을 녹색으로 설정합니다.
        hpBar.color = Color.green;
        //HUD 활성화
        hudCanvas.enabled = true;

        currentHp = initHp;

        SetTankVisible(true);
        _tankMove.enabled = true;

        _rigid.isKinematic = false;
    }

    void SetTankVisible(bool isVisible)
    {
        foreach(MeshRenderer _renderer in renderers)
        {
            _renderer.enabled = isVisible;
        }
    }

    //void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    //{
    //    //로컬 플레이어의 위치정보 송신.
    //    if (stream.isWriting)
    //    {
    //        stream.SendNext(playerTr.position);
    //        stream.SendNext(playerTr.rotation);
    //    }
    //    //원격 플레이어의 위치정보 수신.
    //    else
    //    {
    //        currentPos = (Vector3)stream.ReceiveNext();
    //        currentRot = (Quaternion)stream.ReceiveNext();
    //    }
    //}

    //자신을 파괴시킨 탱크를 검색해 스코어를 증가시키는 함수
    void SaveKillCount(int firePlayerId)
    {
        GameObject[] tanks = GameObject.FindGameObjectsWithTag("TANK");

        foreach(GameObject tank in tanks)
        {
            var tankDamage = tank.GetComponent<TankDamage>();
            //탱크의 playerId가 포탄의 playerId와 동일한지 판단합니다.

            if(tankDamage != null && tankDamage.playerId == firePlayerId)
            {
                tankDamage.incKillCount();

                break;
            }
        }
    }

    void incKillCount()
    {
        ++killCount;
        //UI 항목에 스코어 표시
        txtKillCount.text = killCount.ToString();

        if(pv.isMine)
        {
            PhotonNetwork.player.AddScore(1);
        }
    }
}
