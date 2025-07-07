using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

//플레이어들 동기화 스폰
//플레이어들 동기화 이동
//플레이어들 특정 조건 만족 시 DB로 클리어 기록 전송.(나중에 DB스크립트에서 클리어 기록이 있다면 게임 스테이지 변경)
public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject StageSelectPanel;
    public Button Button_StageBack, Button_Stage1, Button_Stage2;
    public bool gameClear = false;
    void Start()
    {
        int playerIndex = PhotonNetwork.IsMasterClient ? 0 : 1;
        //photonView.RPC("SpawnPlayer", RpcTarget.All, playerIndex);
        SpawnPlayer(playerIndex);
    }

    public void SpawnPlayer(int player_index)
    {
        var spawnPositions = new Vector3[]
        {
        new Vector3(-7.0f, -4.5f, 0.0f),
        new Vector3(-5.0f, -4.5f, 0.0f)
        };
        GameObject playerObject = PhotonNetwork.Instantiate("PlayerPrefab", spawnPositions[player_index], Quaternion.identity);
        //"PlayerPrefab"이라는 오브젝트 스폰포지션에 생성. 
        //유니티에는 생성자(instantiate)와 파괴자(destroy)가 존재. 오브젝트 생성시 사용. 
        //GameObject obj = Resources.Load<GameObject>("PlayerPrefab");
        //Instantiate(obj, spawnPosition, Quaternion.identity);
        //위의 두 줄이 의미하는 게 포톤에서는 PhotonNetwork.Instantiate~~저걸로 리소스에서 "PlayerPrefab"이라는 이름의 프리팹 가져옴.

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
    }
    public void OnGameClear()
    {
        gameClear = true;
    }
    [PunRPC]
    void MoveTheClearStageSelectPanel() //방장만 선택할 수 있으므로 다른 플레이어에게도 보이도록 
    {                                   //튜토리얼 선택 패널 동기화
        StageSelectPanel.SetActive(true);
    }

    [PunRPC]
    void DBonGameClear(int clearedStage)
    {
        //클리어 기록 관련
        GameObject obj = GameObject.Find("PlayFabDataManager");
        Debug.Log(obj);
        PlayFabDataManager playFabDataManager = obj.GetComponent<PlayFabDataManager>();
        Debug.Log(playFabDataManager);

        GameObject stageObj = GameObject.Find("StageSelectUI");
        Debug.Log(stageObj);
        
        StageSelectUI StageSelectUI = stageObj.GetComponent<StageSelectUI>();
        Debug.Log(StageSelectUI);

        playFabDataManager.SaveStageClear(clearedStage, clearedStage =>
        {StageSelectUI.UnlockStage(clearedStage);});   
    }
}
