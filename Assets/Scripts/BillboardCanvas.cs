using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardCanvas : MonoBehaviour
{
    private Transform _tr;
    private Transform mainCameraTr;
    // Start is called before the first frame update
    void Start()
    {
        _tr = GetComponent<Transform>();

        //스테이지에 있는 메인카메라의  Transform 컴포넌트 추출합니다
        mainCameraTr = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        _tr.LookAt(mainCameraTr);
    }
}
