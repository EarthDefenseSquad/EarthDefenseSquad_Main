using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using Photon.Pun.UtilityScripts;
using Unity.VisualScripting;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    public const byte BID_EVENT = 1;
    public const byte AUCTION_COMPLETE_EVENT = 3;
    public GameObject CharacterSelect_Panel;
    public GameObject RoomLoadingPanel;
    public GameObject Start_Panel;
    public Button Button_Start;
    public Button Button_Back;
    public bool isGameStartRequested = false;
  

    private void Start() //게임 시작 버튼 클릭과 함께 스크립트 활성화.
    {
        PhotonNetwork.ConnectUsingSettings();
        Button_Start.onClick.AddListener(OnStartButtonClicked);
    }

    public override void OnConnectedToMaster() //연결+로비 진입을 디폴트로 포함.
    {
        Debug.Log("포톤 마스터 서버 연결 후 로비 진입 성공");

        if (isGameStartRequested)
        {
            PhotonNetwork.JoinRandomRoom();
            Debug.Log("방 참가를 시도합니다.");
            // UI 패널 전환
            Start_Panel.SetActive(false);
            RoomLoadingPanel.SetActive(false);
            CharacterSelect_Panel.SetActive(true);
            Debug.Log("CharacterSelect_Panel 활성화", this);
            Button_Back.onClick.RemoveAllListeners(); // 중복 방지
            Button_Back.onClick.AddListener(OnBackButtonClicked);
            isGameStartRequested = false;
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("방 참가에 실패하였습니다. 방을 새로 만듭니다.");
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 2 }); //null은 room 이름, 참가자 최대 2명.
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("방 입장 성공");
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



        if (playerObject == null)
        {
            Debug.LogError("플레이어 객체 생성 실패!");
            return;
        }

        Debug.Log("플레이어 생성 완료");

        // Camera 세팅
        if (Camera.main == null)
        {
            Debug.LogError("Main Camera가 없습니다.");
            return;
        }


        //Camera.main.GetComponent<CameraController>().Initalize(playerObject.transform);
        //PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    public void OnStartButtonClicked()
    {
        StartCoroutine(DelayTime(3.5f));
        if (isGameStartRequested)
        {
            PhotonNetwork.JoinRandomRoom();
            Debug.Log("방 참가를 시도합니다.");
            // UI 패널 전환
            Start_Panel.SetActive(false);
            RoomLoadingPanel.SetActive(false);
            CharacterSelect_Panel.SetActive(true);
            Debug.Log("CharacterSelect_Panel 활성화", this);
            Button_Back.onClick.RemoveAllListeners(); // 중복 방지
            Button_Back.onClick.AddListener(OnBackButtonClicked);
            isGameStartRequested = false;
        }
        else
        {
            isGameStartRequested = true;
        }
    }

    IEnumerator DelayTime(float time)
    {
        yield return new WaitForSeconds(time);
    }
    public void OnBackButtonClicked()
    {
        Debug.Log("방을 나갑니다.");
        if (PhotonNetwork.InRoom) //내가 방에 있는 게 확실한 경우,
        {
            PhotonNetwork.LeaveRoom();
            //룸을 나가 마스터 서버와 연결만 된 상태로, 다시 룸에 입장하려면 로비에 진입부터 해야 함. 
            CharacterSelect_Panel.SetActive(false);
            Debug.Log("CharacterSelect_Panel 비활성화", this);
            RoomLoadingPanel.SetActive(false);
            Start_Panel.SetActive(true);
            Button_Start.onClick.RemoveAllListeners();
            Button_Start.onClick.AddListener(OnStartButtonClicked);
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) // 플레이어가 방을 나갔을 때 다른 플레이어들에게 그 결과를 알려주는 콜백함수.
    {
        Debug.Log("상대 플레이어가 방을 나갔습니다.");
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LeaveRoom();
            CharacterSelect_Panel.SetActive(false);
            Debug.Log("CharacterSelect_Panel 비활성화", this);
            RoomLoadingPanel.SetActive(false);
            Start_Panel.SetActive(true);
            Button_Start.onClick.RemoveAllListeners();
            Button_Start.onClick.AddListener(OnStartButtonClicked);
        }
    }

    /*public override void OnLeftRoom()
    {
        CharacterSelect_Panel.SetActive(false);
        Debug.Log("CharacterSelect_Panel 비활성화", this);
        RoomLoadingPanel.SetActive(false);
        Start_Panel.SetActive(true);
        Button_Start.onClick.RemoveAllListeners();
        Button_Start.onClick.AddListener(OnStartButtonClicked);
        
    }*/
} 


//경우의 수
//내가 나가는데 내가 마스터(내가 마스터면 다 destroy하고 나가기), 내가 일반
//남이 나가는데 남이 마스터, 남이 일반(내가 destroy하고 나도 나가기)
//OnPlayerLeftRoom에 콜백이 오면서 자동으로 남아있는 사람은 마스터 클라이언트가 됨.