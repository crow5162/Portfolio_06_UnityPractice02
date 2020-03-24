using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PhotonInit : MonoBehaviour
{
    public string version = "v1.0";

    //User의 ID를 입력하는 부분
    public InputField userId;

    //룸이름을 입력받을 UI항목 연결 변수
    public InputField roomName;

    public GameObject scrollContents;
    //룸목록만큼 생성될  RoomItem 프리팹
    public GameObject roomItem;

    private void Awake()
    {
        //서버에 접속해있지않다면 .
        if (!PhotonNetwork.connected)
        {
            //포콘 클라우드에 접속합니다
            PhotonNetwork.ConnectUsingSettings(version);
        }

        userId.text = GetUserId();

        roomName.text = "ROOM_" + Random.Range(0, 999).ToString();
    }

    //포톤 클라우드에 정상적으로 접속한 후 로비에 입장하면 호출되는 Callback 함수.
    void OnJoinedLobby()
    {
        Debug.Log("Entered Lobby !!");

        userId.text = GetUserId();
    }


    //로컬플레이어에 저장된 플레이어의 이름을 반환하거나 생성하는 함수.
    string GetUserId()
    {
        string userId = PlayerPrefs.GetString("USER_ID");

        if(string.IsNullOrEmpty(userId))
        {
            userId = "USER_" + Random.Range(0, 999).ToString("000");
        }

        return userId;
    }

    //로비에 입장후 Room에 입장실패시 호출되는 CallBack 함수.
    void OnPhotonRandomJoinFailed()
    {
        Debug.Log("No Rooms !!");
        //룸생성
        PhotonNetwork.CreateRoom("MyRoom");
    }

    //Room에 입장하면 호출되는 Callback 함수
    void OnJoinedRoom()
    {
        //CreateTank();
        Debug.Log("Enter Room !!");
        StartCoroutine(this.LoadBattleField());
    }

    IEnumerator LoadBattleField()
    {
        //Scene을 이동하는동안 포톤클라우드 서버로 부터 네트워크 메세지 수신 중단합니다.
        PhotonNetwork.isMessageQueueRunning = false;
        //백그라운드 씬 로딩.
        AsyncOperation ao = SceneManager.LoadSceneAsync("scBattleField 1");

        yield return ao;
    }
    private void OnGUI()
    {
        //화면 좌측 상단에 접속과정에 대한 로그를 출력합니다.
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
    }

    public void OnClickJoinRandomRoom()
    {
        //로컬 네트워크의 이름을 설정합니다.
        PhotonNetwork.player.name = userId.text;

        //플레이어의 이름을 저장.
        PlayerPrefs.SetString("USER_ID", userId.text);

        //무작위로 추출된 룸으로 입장.
        PhotonNetwork.JoinRandomRoom();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateTank()
    {
        float pos = Random.Range(-100.0f, 100.0f);
        PhotonNetwork.Instantiate("Tank", new Vector3(pos, 10.0f, pos), Quaternion.identity, 0);
    }

    public void OnClickCreateRoom()
    {
        string _roomName = roomName.text;

        //룸이름이 없거나 NULL 일 경우에는 룸 이름 지정
        if(string.IsNullOrEmpty(roomName.text))
        {
            _roomName = "ROOM_" + Random.Range(0, 999).ToString("000");
        }

        //로컬플레이어의 이름을 설정합니다.
        PhotonNetwork.player.name = userId.text;
        //플레이어 이름을 저장 합니다.
        PlayerPrefs.SetString("USER_ID", userId.text);

        //생성할 룸의 조건 설정

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.isOpen = true;
        roomOptions.isVisible = true;
        roomOptions.MaxPlayers = 20;

        //지정한 조건에 맞는 룸 생성 함수.
        PhotonNetwork.CreateRoom(_roomName, roomOptions, TypedLobby.Default);
    }

    void OnPhotonCreateRoomFailed(object[] codeAndMsg)
    {
        Debug.Log("Create Room Failed = " + codeAndMsg[1]);
    }

    //생성된 룸 목록이 변경되었을 때 호출되는 CallBack함수.
    void OnReceivedRoomListUpdate()
    {
        foreach(GameObject obj in GameObject.FindGameObjectsWithTag("ROOM_ITEM"))
        {
            Destroy(obj);
        }

        //Grid Layout Group 컴포넌트의 Constraint Count 값을 증가시킬 변수
        int rowCount = 0;
        //스크롤 영역 초기화.
        scrollContents.GetComponent<RectTransform>().sizeDelta = Vector2.zero;

        //수신받은 room 목록의 정보로 RoomItem을 호출합니다.
        foreach(RoomInfo _room in PhotonNetwork.GetRoomList())
        {
            Debug.Log(_room.name);
            //RoomItem 프리팹을 동적으로 생성합니다.
            GameObject room = (GameObject)Instantiate(roomItem);
            //생성한 roomItem 프리팹의 parent를 지정합니다.
            room.transform.SetParent(scrollContents.transform, false);

            //생성한 RoomItem에 표시하기위한 Text 전달.
            RoomData roomData = room.GetComponent<RoomData>();
            roomData.roomName = _room.name;
            roomData.connectPlayer = _room.PlayerCount;
            roomData.maxPlayer = _room.maxPlayers;
            //Text 정보를 표시합니다.
            roomData.DispRoomData();
            //RoomItem의 Button 컴포넌트에 클릭이벤트를 동적을 연결합니다.
            roomData.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { OnClickRoomItem(roomData.roomName); });

            //Grid Layout Group컴포넌트의 Constrain Count 값을 증가 시킵니다.
            scrollContents.GetComponent<GridLayoutGroup>().constraintCount = ++rowCount;
            //스크롤 영역의 높이를 증가시킴.
            scrollContents.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 20);
        }
    }

    void OnClickRoomItem(string roomName)
    {
        //로컬플레이어의 이름을 설정합니다.
        PhotonNetwork.player.name = userId.text;

        //플레이어의 이름을 저장합니다.
        PlayerPrefs.SetString("USER_ID", userId.text);

        //인자로 전달된 이름에 해당하는 룸으로 입장합니다.
        PhotonNetwork.JoinRoom(roomName);
    }
}
