using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    public const byte BID_EVENT = 1;
    public const byte AUCTION_COMPLETE_EVENT = 3;
    public GameObject CharacterSelect_Panel;
    public GameObject RoomLoadingPanel;
    public Button Button_Start;
    private bool isGameStartRequested=false;

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        Button_Start.onClick.AddListener(OnGameStartButtonClicked);

    }
    
    public override void OnConnectedToMaster()
    {
        Debug.Log("포톤 마스터 서버 연결 성공");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("로비 입장 성공");
        if (isGameStartRequested)
            {
                // 방 입장 시도
                PhotonNetwork.JoinRandomRoom();
                // UI 패널 전환
                RoomLoadingPanel.SetActive(false);
                CharacterSelect_Panel.SetActive(true);
                isGameStartRequested = false; // 중복 방지
            }
    }

    public void OnGameStartButtonClicked()
    {

        // 이미 로비에 들어가 있다면 바로 진행
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinRandomRoom();
            RoomLoadingPanel.SetActive(false);
            CharacterSelect_Panel.SetActive(true);
        }
        else
        {
            // 아직 로비에 안 들어가 있으면, 콜백에서 처리하도록 플래그만 켜둠
            isGameStartRequested = true;
        }
    }

     public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("방 생성 시도");
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 2 });
    }
   
    public override void OnJoinedRoom()
    {
        Debug.Log($"방 입장: {PhotonNetwork.CurrentRoom.Name}");
        // 예시: 마스터 클라이언트는 0번, 나머지는 1번 위치에 생성
        int playerIndex = PhotonNetwork.IsMasterClient ? 0 : 1;
        photonView.RPC("SpawnPlayer", RpcTarget.AllBuffered, playerIndex);
    }


    [PunRPC]
    void SpawnPlayer(int player_index)
    {
        var spawnPositions = new Vector3[]
        {
        new Vector3(50.0f, 50.0f, 0.0f),
        new Vector3(970.0f, 50.0f, 0.0f)
        };
        GameObject playerObject = PhotonNetwork.Instantiate("1PInfo", spawnPositions[player_index], Quaternion.identity);
        //"PlayerPrefab"이라는 오브젝트 스폰포지션에 생성. 
        //유니티에는 생성자(instantiate)와 파괴자(destroy)가 존재. 오브젝트 생성시 사용. 
        //GameObject obj = Resources.Load<GameObject>("PlayerPrefab");
        //Instantiate(obj, spawnPosition, Quaternion.identity);
        //위의 두 줄이 의미하는 게 포톤에서는 PhotonNetwork.Instantiate~~저걸로 리소스에서 "PlayerPrefab"이라는 이름의 프리팹 가져옴.
        playerObject.transform.SetParent(CharacterSelect_Panel.transform, false);


        
        Debug.Log("SpawnPlayer 시작");  // 이게 안 뜨면 함수가 아예 호출 안 됨



        if (playerObject == null) {
            Debug.LogError("플레이어 객체 생성 실패!");
            return;
        }

        Debug.Log("플레이어 생성 완료");

        // Camera 세팅
        if (Camera.main == null) {
            Debug.LogError("Main Camera가 없습니다.");
            return;
        }

    
        //Camera.main.GetComponent<CameraController>().Initalize(playerObject.transform);
        //PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }
}