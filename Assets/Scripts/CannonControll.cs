using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonControll : MonoBehaviour
{
    // Start is called before the first frame update

    private Transform tr;
    private float rotSpeed = 100.0f;

    private PhotonView pv = null;
    private Quaternion currentRot = Quaternion.identity;

    void Awake()
    {
        tr = GetComponent<Transform>();
        pv = GetComponent<PhotonView>();

        //PhotonView의 Observed속성을 이 스크립트로 지정합니다
        pv.ObservedComponents[0] = this;
        //PhotonView의 동기화 속성을 설정합니다
        pv.synchronization = ViewSynchronization.UnreliableOnChange;

        //초기 회전값 설정.
        currentRot = tr.localRotation;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (pv.isMine)
        {
            float angle = -Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * rotSpeed;
            tr.Rotate(angle, 0, 0);
        }
        else
        {
            //현재 회전각도에서 수신받은 회전 각도로 부드럽게 회전합니다.
            tr.localRotation = Quaternion.Slerp(tr.localRotation, currentRot, Time.deltaTime * 3.0f);
        }
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.isWriting)
        {
            stream.SendNext(tr.localRotation);
        }
        else
        {
            currentRot = (Quaternion) stream.ReceiveNext();
        }
    }
}
