using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMgr : MonoBehaviour
{
    //접속된 플레이어의 수를 표시할 TextUI 항목 변수.
    public Text textConnect;
    private void Awake()
    {
        CreateTank();

        PhotonNetwork.isMessageQueueRunning = true;

        GetConnectPlayerCount();
    }

    IEnumerator Start()
    {
        //룸에있는 네트워크 객체간에 통신이 완료될때까지 잠시 대기.
        yield return new WaitForSeconds(1.0f);

        SetConnectPlayerScore();
    }

    //모든 탱크의 스코어 UI에 점수를 표시하는 함수를 호출
    void SetConnectPlayerScore()
    {
        //현재 룸에 입장한 플레이어의 정보를 저장합니다.
        PhotonPlayer[] players = PhotonNetwork.playerList;

        foreach(PhotonPlayer _player in players)
        {
            Debug.Log("[" + _player.ID + "]" + _player.name + " " + _player.GetScore() + "kills");
        }

        //모든 Tank프리팹을 배열에 저장합니다.
        GameObject[] tanks = GameObject.FindGameObjectsWithTag("TANK");

        foreach(GameObject _tank in tanks)
        {
            //각 탱크별 Score 조회합니다.
            int currentKillCount = _tank.GetComponent<PhotonView>().owner.GetScore();
            //해당 Tank의 UI에 스코어 표시.
            _tank.GetComponent<TankDamage>().txtKillCount.text = currentKillCount.ToString();
        }
    }

    void CreateTank()
    {
        float pos = Random.Range(-100.0f, 100.0f);
        PhotonNetwork.Instantiate("Tank",
            new Vector3(pos, 10.0f, pos),
            Quaternion.identity, 0);
    }

    void GetConnectPlayerCount()
    {
        //현재 입장한 룸의 정보를 받아옵니다.
        Room currentRoom = PhotonNetwork.room;

        //현재 룸 접속자의 수와 최재 접속 가능한 수를 문자열로 구성한후 text UI 항목에 출력합니다.
        textConnect.text = currentRoom.playerCount.ToString()
        + "/" +
            currentRoom.maxPlayers.ToString();
    }

    //새 플레이어가 접속했을때 호출되는 함수
    void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        GetConnectPlayerCount();
    }

    //플레이어가 나가거나 룸에서 접속이 끊어졌을때 호출되는함수.
    void OnPhotonPlayerDisconnected(PhotonPlayer outPlayer)
    {
        GetConnectPlayerCount();
    }

    public void OnClickExitButton()
    {
        PhotonNetwork.LeaveRoom();
    }

    void OnLeftRoom()
    {
        SceneManager.LoadScene("scLobby");
    }
 
}
