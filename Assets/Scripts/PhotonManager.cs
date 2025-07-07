using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using Photon.Pun.UtilityScripts;
using Unity.VisualScripting;

//PhotonManager 스크립트 기능:
//게임 시작하자마자 연결-로비 진입 상태
//start버튼 누르면 룸 진입.
//플레이어 중 한 명이라도 back버튼 누르면 룸 파괴. - start버튼 누르면 다시 새로운 룸 진입
//ok버튼 누르면 YearSelectPanel이동-1960년도 선택-StageSelect패널 이동-stage1선택-튜토리얼 패널 이동-게임씬 이동
public class PhotonManager : MonoBehaviourPunCallbacks
{
    public const byte BID_EVENT = 1;
    public const byte AUCTION_COMPLETE_EVENT = 3;
    public GameObject Start_Panel;
    public GameObject RoomLoadingPanel;
    public GameObject CharacterSelect_Panel;
    public GameObject YearSelectPanel;
    public GameObject StageSelectPanel;
    public GameObject Tutorial_Panel;
    public Button Button_Start;
    public Button Button_Back, Button_YearBack, Button_StageBack, Button_TutorialBack;
    public Button Button_OK, Button_TutorialOK;
    public Button Button_1960;
    public Button Button_Stage1;

    public StageSelectUI stageSelectUI;
    public bool isGameStartRequested = false;

    private void Start() //게임 시작 버튼 클릭과 함께 스크립트 활성화.
    {
        PhotonNetwork.ConnectUsingSettings();
        GameDataManager.Instance.Start_Panel = this.Start_Panel;
        GameDataManager.Instance.StageSelectPanel = this.StageSelectPanel;
        GameDataManager.Instance.stageSelectUI = this.stageSelectUI;
        Button_Start.onClick.AddListener(OnGameStartButtonClicked);
    }
    public override void OnConnectedToMaster() //게임 시작하자마자 서버 연결 - 로비 진입 성공 상태시 콜백
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
            Start_Panel.SetActive(false);
            RoomLoadingPanel.SetActive(true);
        }
        isGameStartRequested = false;
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("방 참가에 실패하였습니다. 방을 새로 만듭니다.");
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 2 }); //null은 room 이름, 참가자 최대 2명.
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("방 입장 성공");
        PhotonNetwork.NickName = "Player" + PhotonNetwork.LocalPlayer.ActorNumber;
        Debug.Log($"방 입장: {PhotonNetwork.CurrentRoom.Name}");
        // 예시: 마스터 클라이언트는 0번, 나머지는 1번 위치에 생성
        RoomLoadingPanel.SetActive(false);
        CharacterSelect_Panel.SetActive(true);
        Button_Back.onClick.RemoveAllListeners(); // 중복 방지
        Button_Back.onClick.AddListener(OnBackButtonClicked);
        Button_OK.onClick.RemoveAllListeners(); // 중복 방지
        Button_OK.onClick.AddListener(() => photonView.RPC("MoveTheYearPanel", RpcTarget.All));
        // MasterClient만 OK 버튼 활성화
        if (PhotonNetwork.IsMasterClient)
        {
            Button_OK.gameObject.SetActive(true);
        }
        else
        {
            Button_OK.gameObject.SetActive(false);
        }
        int playerIndex = PhotonNetwork.IsMasterClient ? 0 : 1;
        photonView.RPC("SpawnPlayer", RpcTarget.AllBuffered, playerIndex);
        //SpawnPlayer(playerIndex);

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
    public void OnGameStartButtonClicked() //start버튼 눌렀을 때, 로비 진입까지 마친 상태라면 룸 진입
    {
        if (PhotonNetwork.IsConnectedAndReady) //연결되어 있고 방 진입 준비가 되어 있다면,
        {
            PhotonNetwork.JoinRandomRoom();
            Debug.Log("방 참가를 시도합니다.");
            Start_Panel.SetActive(false);
            RoomLoadingPanel.SetActive(true);
            isGameStartRequested = false;
        }
        else
        {
            // 아직 로비에 안 들어가 있으면, 콜백에서 처리하도록 플래그만 켜둠
            isGameStartRequested = true;
        }
    }

    public void OnBackButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
        Debug.Log("방에서 나가는 중입니다.");
    }

    public override void OnLeftRoom() //내가 룸을 나갔을 때 나에게 오는 콜백함수.
    {
        Debug.Log("방에서 나왔습니다.");
        CharacterSelect_Panel.SetActive(false);
        Start_Panel.SetActive(true);
        Button_Start.onClick.RemoveAllListeners(); // 중복 방지
        Button_Start.onClick.AddListener(OnGameStartButtonClicked);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) //남이 나갔을 때 나에게 오는 콜백함수.
    {
        Debug.Log($"{otherPlayer.NickName} 님이 방을 나갔습니다.");
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LeaveRoom();
            Debug.Log("방에서 나가는 중입니다.");
        }
        else
        {
            Debug.Log("방에서 나가기 실패!");
        }
    }
    [PunRPC]
    void MoveTheYearPanel() //방장만 선택할 수 있으므로 다른 플레이어에게도 보이도록 연도 선택 패널 동기화
    {
        CharacterSelect_Panel.SetActive(false);
        StageSelectPanel.SetActive(false);
        YearSelectPanel.SetActive(true);
        Button_1960.onClick.RemoveAllListeners();
        Button_1960.onClick.AddListener(() => photonView.RPC("MoveTheStageSelectPanel", RpcTarget.All));
        Button_YearBack.onClick.RemoveAllListeners();
        Button_YearBack.onClick.AddListener(() => photonView.RPC("MoveThe_CharacterSelectPanel", RpcTarget.All));
        if (!PhotonNetwork.IsMasterClient) //만약 방장이 아니면 버튼 눌러도 이벤트 발생 안함.
        {
            Button_YearBack.gameObject.SetActive(false); //뒤로가기 버튼은 안보이도록.
            Button_1960.interactable = false;
        }
    }
    [PunRPC]
    void MoveTheStageSelectPanel() //방장만 선택할 수 있으므로 다른 플레이어에게도 보이도록 
    {                              //스테이지 선택 패널 동기화
        YearSelectPanel.SetActive(false);
        StageSelectPanel.SetActive(true);
        Button_Stage1.onClick.RemoveAllListeners();
        Button_Stage1.onClick.AddListener(() => photonView.RPC("MoveTheTutorialPanel", RpcTarget.All));
        Button_StageBack.onClick.RemoveAllListeners();
        Button_StageBack.onClick.AddListener(() => photonView.RPC("MoveTheYearPanel", RpcTarget.All));
        if (!PhotonNetwork.IsMasterClient) //만약 방장이 아니면 버튼 눌러도 이벤트 발생 안함.
        {
            Button_StageBack.gameObject.SetActive(false); //뒤로가기 버튼은 안보이도록.
            Button_Stage1.interactable = false;
        }
    }
    [PunRPC]
    void MoveTheTutorialPanel() //방장만 선택할 수 있으므로 다른 플레이어에게도 보이도록 
    {                           //튜토리얼 선택 패널 동기화
        StageSelectPanel.SetActive(false);
        Tutorial_Panel.SetActive(true);
        Button_TutorialBack.onClick.RemoveAllListeners(); //뒤로가기
        Button_TutorialBack.onClick.AddListener(() => photonView.RPC("MoveTheStageSelectPanel", RpcTarget.All));
        Button_TutorialOK.onClick.RemoveAllListeners(); //ok버튼
        Button_TutorialOK.onClick.AddListener(() => photonView.RPC("MoveTheGameScene", RpcTarget.All));
        if (!PhotonNetwork.IsMasterClient) //만약 방장이 아니면 버튼 눌러도 이벤트 발생 안함.
        {
            Button_TutorialBack.gameObject.SetActive(false); //뒤로가기 버튼은 안보이도록.
        }
    }
    [PunRPC]
    void MoveTheGameScene() //방장만 선택할 수 있으므로 다른 플레이어에게도 보이도록 
    {                       //게임씬 이동 동기화
        PhotonNetwork.LoadLevel("GameScene");
    }

    [PunRPC]
    void MoveThe_CharacterSelectPanel()
    {
        YearSelectPanel.SetActive(false);
        CharacterSelect_Panel.SetActive(true);
        Button_Back.onClick.RemoveAllListeners(); // 중복 방지
        Button_Back.onClick.AddListener(OnBackButtonClicked);
        Button_OK.onClick.RemoveAllListeners(); // 중복 방지
        Button_OK.onClick.AddListener(() => photonView.RPC("MoveTheYearPanel", RpcTarget.All));
        // MasterClient만 OK 버튼 활성화
        if (PhotonNetwork.IsMasterClient)
        {
            Button_OK.gameObject.SetActive(true);
        }
        else
        {
            Button_OK.gameObject.SetActive(false);
        }
        int playerIndex = PhotonNetwork.IsMasterClient ? 0 : 1;
        photonView.RPC("SpawnPlayer", RpcTarget.AllBuffered, playerIndex);
    }
    
}
