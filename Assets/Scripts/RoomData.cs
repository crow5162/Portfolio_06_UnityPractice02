using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomData : MonoBehaviour
{
    //외부접근을 위해 Public 으로 선언했지만 Inspector에 노출하지않음.
    [HideInInspector]
    public string roomName = "";
    [HideInInspector]
    public int connectPlayer = 0;
    [HideInInspector]
    public int maxPlayer = 0;

    //Room 이름 표시할 Text UI 항목
    public Text textRoomName;

    //Room 접속자 수와 최대 접속자 수를 표시할 Text UI 항목입니다.
    public Text textConnectInfo;


    public void DispRoomData()
    {
        textRoomName.text = roomName;
        textConnectInfo.text = "(" + connectPlayer.ToString() + "/" + maxPlayer.ToString() + ")";
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
