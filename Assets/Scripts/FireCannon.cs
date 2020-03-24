using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireCannon : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject cannon = null;
    public Transform firePos;
    //포탄발사 사운드 파일
    private AudioClip firesfx = null;
    //AudioSource 컴포넌트를 할당할 변수.
    private AudioSource sfx = null;

    //PhotonView 컴포넌트를 할당할 변수
    private PhotonView pv = null;

    void Awake()
    {
        cannon = (GameObject)Resources.Load("Cannon");
        firesfx = Resources.Load<AudioClip>("CannonFire");
        sfx = GetComponent<AudioSource>();

        //PhotonView 컴포넌트를 pv변수에 할당합니다.
        pv = GetComponent<PhotonView>();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (MouseHover.instance.isUIHover) return;

        //PhotonView가 자신의 것이고 마우스 왼쪽 버튼 클릭시 발사 로직을 수행합니다.
        if(Input.GetMouseButtonDown(0) && pv.isMine)
        {
            //자신의 탱크일 경우는 로컬함수를 호출해 포탄을 발사합니다.
            Fire();

            //원격 네트워크 플레이어의 탱크에 RPC로 원격으로 Fire함수를 호출합니다.
            pv.RPC("Fire", PhotonTargets.Others, null);
        }
    }

    //RPC로 호출할 함수는 반드시 PunRPC Attribute를 함수앞에 명기해야합니다.
    [PunRPC]
    void Fire()
    {
        sfx.PlayOneShot(firesfx, 1.0f);
        //Instantiate(cannon, firePos.position, firePos.rotation);

        GameObject _cannon = (GameObject)Instantiate(cannon, firePos.position, firePos.rotation);
        _cannon.GetComponent<Cannon>().playerId = pv.ownerId;
    }
}
