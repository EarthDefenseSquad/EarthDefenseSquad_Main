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
//start버튼 누르면 룸 로딩 패널 - 캐릭터 선택 패널로 룸 진입.
//플레이어 중 한 명이라도 back버튼 누르면 룸 파괴. - start버튼 누르면 다시 새로운 룸 진입
//캐릭터 선택 패널에서 ok버튼 누르면 튜토리얼 패널 이동-웨이팅씬-게임씬 이동
public class PhotonManager : MonoBehaviourPunCallbacks
{
    public const byte BID_EVENT = 1;
    public const byte AUCTION_COMPLETE_EVENT = 3;
    public GameObject Start_Panel;
    public GameObject RoomLoadingPanel;
    public GameObject CharacterSelect_Panel;
    public GameObject Tutorial_Panel;
    public GameObject Player1_Info_all, Player2_Info_all;
    public SpriteSwitch spriteSwitch;
    public Button Button_Start;
    public Button Button_CharacterSelect_Back, Button_Tutorial_Back;
    public Button Button_CharacterSelect_LB,Button_CharacterSelect_RB, Button_Tutorial_OK;
    public bool isGameStartRequested = false;
    public bool Button_CharacterSelect_LB_Pressed, Button_CharacterSelect_RB_Pressed = false;

    private void Start() //게임 시작 버튼 클릭과 함께 스크립트 활성화.
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
        GameDataManager.Instance.Start_Panel = this.Start_Panel;
        Button_Start.onClick.AddListener(OnGameStartButtonClicked);
    }

    IEnumerator DeactivateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
    }
    public override void OnConnectedToMaster() //게임 시작하자마자 서버 연결 - 로비 진입 성공 상태시 콜백
    {
        Debug.Log("포톤 마스터 서버 연결 후 로비 진입 성공");
        if (isGameStartRequested)
        {
            PhotonNetwork.JoinRandomRoom();
            Debug.Log("방 참가를 시도합니다.");
            StartCoroutine(DeactivateAfterDelay(0.2f));
            // UI 패널 전환
            Start_Panel.SetActive(false);
            RoomLoadingPanel.SetActive(false);
            CharacterSelect_Panel.SetActive(true);
            Player1_Info_all.SetActive(false); //처음엔 캐릭터들 비활성화
            Player2_Info_all.SetActive(false);
            Debug.Log("CharacterSelect_Panel 활성화", this);
            Button_CharacterSelect_Back.onClick.RemoveAllListeners(); // 중복 방지
            Button_CharacterSelect_Back.onClick.AddListener(OnCharacterSelect_BackButtonClicked);
            Start_Panel.SetActive(false);
            RoomLoadingPanel.SetActive(true);
        }
        isGameStartRequested = false;
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("방 참가에 실패하였습니다. 방을 새로 만듭니다.");
        StartCoroutine(DeactivateAfterDelay(0.2f));
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 2 }); //null은 room 이름, 참가자 최대 2명.
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("방 입장 성공");
        PhotonNetwork.NickName = "Player" + PhotonNetwork.LocalPlayer.ActorNumber; //닉네임 설정
        Debug.Log($"방 입장: {PhotonNetwork.CurrentRoom.Name}");
        // 예시: 마스터 클라이언트는 0번, 나머지는 1번 위치에 생성
        RoomLoadingPanel.SetActive(false);
        CharacterSelect_Panel.SetActive(true);
        Player1_Info_all.SetActive(false);
        Player2_Info_all.SetActive(false);
        Button_CharacterSelect_Back.onClick.RemoveAllListeners(); // 중복 방지
        Button_CharacterSelect_Back.onClick.AddListener(OnCharacterSelect_BackButtonClicked);
        //Button_CharacterSelect_LB.onClick.RemoveAllListeners(); // 중복 방지
        //Button_CharacterSelect_RB.onClick.RemoveAllListeners();
        //Button_CharacterSelect_LB.onClick.AddListener(() => photonView.RPC("OnButton_CharacterSelect_LB_Click", RpcTarget.All));
        //Button_CharacterSelect_RB.onClick.AddListener(() => photonView.RPC("OnButton_CharacterSelect_RB_Click", RpcTarget.All));
        int playerIndex = PhotonNetwork.IsMasterClient ? 0 : 1; //캐릭터 선택 패널에서의 스폰
        //int playerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;
        //photonView.RPC("SpawnRoomPlayer", RpcTarget.AllBuffered, playerIndex);
        photonView.RPC("SpawnCharacterRoom", RpcTarget.AllBuffered, playerIndex); //방입장 성공시 캐릭터들 활성화
    }
    [PunRPC]
    void SpawnCharacterRoom(int player_Index)
    {
        if (player_Index == 0 && Player1_Info_all.activeSelf == false)
        {
            Player1_Info_all.SetActive(true);
            spriteSwitch.set1.ownerPlayerIndex = 0;
            spriteSwitch.set1.currentIndex = 0;
            spriteSwitch.set1.ApplyCurrent();
            spriteSwitch.set1.Init(player_Index);
            spriteSwitch.set1.SetConfirmed(false);
        }
        if (player_Index == 1 && Player2_Info_all.activeSelf == false)
        {
            Player2_Info_all.SetActive(true);
            spriteSwitch.set2.ownerPlayerIndex = 1;
            spriteSwitch.set2.currentIndex = 0;
            spriteSwitch.set2.ApplyCurrent();
            spriteSwitch.set2.Init(player_Index);
            spriteSwitch.set2.SetConfirmed(false);
    }
        
    }
    
    [PunRPC]
    void SpawnRoomPlayer(int player_index)
    {
        var spawnPositions = new Vector3[]
        {
        new Vector3(50.0f, 50.0f, 0.0f),
        new Vector3(970.0f, 50.0f, 0.0f)
        };
        string prefabName = player_index == 0 ? "1P Info all" : "2P Info all";
        GameObject playerObject = PhotonNetwork.Instantiate(prefabName, spawnPositions[player_index], Quaternion.identity);
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

    public void OnCharacterSelect_BackButtonClicked()
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

    
}
