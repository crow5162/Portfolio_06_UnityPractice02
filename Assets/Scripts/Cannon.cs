using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    public float speed = 6000.0f;

    public GameObject expEffect;
    private GameObject frag = null;

    private CapsuleCollider _capsule;
    private Rigidbody _rigid;

    //포탄을 발사한 플레이어의 ID저장
    public int playerId = -1;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().AddForce(transform.forward * speed);

        _capsule = GetComponent<CapsuleCollider>();
        _rigid = GetComponent<Rigidbody>();

        frag = (GameObject)Resources.Load("Fragment");

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "TERR")
        {
            Instantiate(frag, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
        else if (other.tag == "TANK")
        {
            //지면 또는 적 탱크에 충돌한 경우 즉시 폴발하도록 코루틴 실행.
            StartCoroutine(this.ExplosionCannon(0.0f));
        }

    }

    IEnumerator ExplosionCannon(float tm)
    {
        yield return new WaitForSeconds(tm);

        //충돌 콜백 함수가 발생하지않도록 collider를 비활성화.
        _capsule.enabled = false;
        //물리엔진의 영향을 받을 필요가 없음.
        _rigid.isKinematic = true;

        //폭발이펙트 랜덤하게 회전시키시.
        Quaternion effectRot = Quaternion.Euler(0, Random.Range(0, 360), 0);

        //폭발 프리펩 동적 생성
        GameObject obj = (GameObject)Instantiate(expEffect,
            transform.position,
            effectRot);

        Destroy(obj, 2.0f);
        Destroy(this.gameObject, 1.0f);

    }
}
