using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretControll : MonoBehaviour
{
    private Transform tr;
    private RaycastHit hit;

    public float rotSpeed = 5.0f;
    
    //PhotonView 컴포넌트 변수
    private PhotonView pv = null;
    //원격 플레이어 탱크의 터렛 회전값을 저장할 변수.
    private Quaternion currentRot = Quaternion.identity;

    void Awake()
    {
        tr = GetComponent<Transform>();
        pv = GetComponent<PhotonView>();

        //PhotonView의 Observed 속성을 이 스크립트로 지정합니다.
        pv.ObservedComponents[0] = this;
        //PhotonView의 동기화 속성을 설정합니다.
        pv.synchronization = ViewSynchronization.UnreliableOnChange;

        currentRot = tr.localRotation;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (pv.isMine)  //자신의 탱크일 경우
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            Debug.DrawRay(ray.origin, ray.direction * 300.0f, Color.red);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 8))
            {
                //Ray에 맞은 위치를 로컬좌표로 변환합니다.
                Vector3 relative = tr.InverseTransformPoint(hit.point);

                float angle = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;
                tr.Rotate(0, angle * Time.deltaTime * rotSpeed, 0);
            }
        }
        else
        {
            //원격플레이어의 탱크 일 경우
            //현재 회전각도에서 수신받은 회전각도로 부드럽게 회전합니다.
            tr.localRotation = Quaternion.Slerp(tr.localRotation, currentRot, Time.deltaTime * 3.0f);
        }
    }

    //송, 수신 콜백 함수
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.isWriting)
        {
            stream.SendNext(tr.localRotation);
        }
        else
        {
            currentRot = (Quaternion)stream.ReceiveNext();
        }
    }
}
