using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Utility;

public class TankMove : MonoBehaviour
{
    public float moveSpeed = 20.0f;
    public float rotSpeed = 50.0f;

    private Rigidbody rbody;
    private Transform tr;

    //Photon view 컴포넌트를 할당할 변수.
    private PhotonView pv = null;
    //메인카메라가 추적할 CamPivot 게임 오브젝트
    public Transform camPivot;

    private float h, v;

    //위치 정보를 송, 수신할때 사용할 변수 선언및 초기값 설정.
    private Vector3 currentPos = Vector3.zero;
    private Quaternion currentRot = Quaternion.identity;

    void Awake()
    {
        rbody = GetComponent<Rigidbody>();
        tr = GetComponent<Transform>();

        pv = GetComponent<PhotonView>();

        //데이터 전송 타입을 설정합니다.
        pv.synchronization = ViewSynchronization.UnreliableOnChange;
        //PhotonView Observed Components 속성에 TankMove컴포넌트를연결합니다.
        pv.ObservedComponents[0] = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        if(pv.isMine)
        {
            Camera.main.GetComponent<SmoothFollow>().target = camPivot;

            //Rigid Body의 무게 중심을 낮게 설정합니다.
            rbody.centerOfMass = new Vector3(0.0f, -0.5f, 0.0f);
        }
        else
        {
            //자신이 아닌 다른 원격 플레이어의 탱크는 물리력을 이용하지 않습니다.
            rbody.isKinematic = true;
        }

        //원격 탱크의 위치 및 회전값을 처리할 변수의 초깃값 설정.
        currentPos = tr.position;
        currentRot = tr.rotation;
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //로컬 플레이어의 위치정보 송신.
        if(stream.isWriting)
        {
            stream.SendNext(tr.position);
            stream.SendNext(tr.rotation);
        }
        //원격 플레이어의 위치정보 수신.
        else
        {
            currentPos = (Vector3)stream.ReceiveNext();
            currentRot = (Quaternion)stream.ReceiveNext();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (pv.isMine)   //자신의 탱크는 직접 이동/회전시킵니다.
        {
            v = Input.GetAxis("Vertical");
            h = Input.GetAxis("Horizontal");

            tr.Rotate(Vector3.up * rotSpeed * h * Time.deltaTime);
            tr.Translate(Vector3.forward * v * moveSpeed * Time.deltaTime);
        }
        else
        {
            //원격 플레이어의 탱크를 수신받은 위치까지 부드럽게 이동시킵니다.
            tr.position = Vector3.Lerp(tr.position, currentPos, Time.deltaTime * 3.0f);
            //원격 플레이어의 탱크를 수신받은 각도만큼 부드럽게 회전시킵니다.
            tr.rotation = Quaternion.Slerp(tr.rotation, currentRot, Time.deltaTime * 3.0f);
        }
    }
}
