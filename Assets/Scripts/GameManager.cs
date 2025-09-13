using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    public int totalPoint=0;
    public int stagePoint=0;
    public int stageIndex=-1;
    public int health=3;
    public GameObject playerObj;
    public PlayerMove player;

    public GameObject[] Stages;

    public Image[] UIhealth;
    public Text UIPoint;
    public Text UIStage;
    public GameObject RestartButton;

    public GameObject StageSelectPanel;
    public Button Button_StageBack, Button_Stage1, Button_Stage2;
    public bool gameClear = false;


    // 현재까지 먹은 Finish 아이템 개수 (게임 재시작 시에도 유지됨)
    public int finishItemCount = 0;

    // 총 Finish 아이템의 개수 (엔딩 분기 기준값) - 인스펙터에서 설정 가능
    public int totalStages = 3;

    // Finish 아이템 개수를 화면에 표시할 UI 텍스트 (왼쪽 하단에 위치한 Text 오브젝트)
    public Text finishItemText;

    // PlayerPrefs 저장 키 이름 (로컬 저장용 키)
    private const string FinishItemKey = "FinishItemCount";

    public bool colorRestoreMode = false;
    public GameObject goalObject; // Goal 오브젝트 연결


    [Header("개발용 설정 - 즉사 모드")]
    public bool isInstantDeathMode=false;

    //플레이어들 동기화 스폰
    //플레이어들 동기화 이동
    //플레이어들 특정 조건 만족 시 DB로 클리어 기록 전송.(나중에 DB스크립트에서 클리어 기록이 있다면 게임 스테이지 변경)
    
    public static GameManager Instance;
    void Awake()
    {
        if (isInstantDeathMode)
            {
                health = 1;
                UpdateHealthUI(); // ✅ UI 반영
            }
            else
            {
                health = 3;
            }

        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == "WaitingScene" || currentScene == "StageScene") {
          if (photonView.IsMine)
            {
                int playerIndex = PhotonNetwork.IsMasterClient ? 0 : 1;
                photonView.RPC("SpawnPlayer", RpcTarget.All, playerIndex);
            }      
        }
    }
    


    void Update()
    {
        if (totalPoint != 0 && stagePoint != 0)
        {
          UIPoint.text = (totalPoint + stagePoint).ToString();   
        }
        // 로컬 저장된 아이템 개수를 불러옴
        LoadFinishItemCount();

        // UI에 현재 수치 표시
        UpdateFinishItemUI();

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.R))
        {
            PlayerPrefs.DeleteAll();     // 저장 데이터 초기화
            PlayerPrefs.Save();
            Debug.Log("[개발용] data모은 정도 초기화 완료");
        }
#endif
       

    }


    [PunRPC]
    public void SpawnPlayer(int player_index)
    {
        var spawnPositions = new Vector3[]
        {
        new Vector3(-1.0f, -0.5f, 0.0f),
        new Vector3(0.0f, -0.5f, 0.0f)
        };
        GameObject playerObject = PhotonNetwork.Instantiate("Player", spawnPositions[player_index], Quaternion.identity);
        player = playerObject.GetComponent<PlayerMove>();
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
        { StageSelectUI.UnlockStage(clearedStage); });
    }

    public void NextStage()
    {
        if (stageIndex < Stages.Length - 1)
        {
            Stages[stageIndex].SetActive(false);
            stageIndex++;
            Stages[stageIndex].SetActive(true);
            PlayerReposition();

            UIStage.text = "STAGE " + (stageIndex + 1);
        }
        else
        {
            //Time.timeScale = 0; // Pause the game
            Debug.Log("모든 스테이지를 클리어했습니다.");
            //Text btnText = RestartButton.GetComponentInChildren<Text>();
            //btnText.text = "Clear!";
            //RestartButton.SetActive(true);
        }

        totalPoint += stagePoint;

        stagePoint = 0;
    }

    public void HealthDown()
    {
        // if (health > 0)
        // {
        //     health--;
        //     UIhealth[health].color = new Color(1, 0, 0, 0.2f);
        // }
        // else
        // {
        //     UIhealth[0].color = new Color(1, 0, 0, 0.2f);

        //     player.OnDie();

        //     Debug.Log("플레이어가 죽었습니다.");

        //     RestartButton.SetActive(true);
        // }

        // if (oneHitKill || health <= 1)
        // {
        //     // 즉사 또는 체력 1 남은 경우
        //     health = 0;

        //     if (UIhealth.Length > 0)
        //         UIhealth[0].color = new Color(1, 0, 0, 0.2f);

        //     player.OnDie();
        //     Debug.Log("☠️ 즉사 처리됨 또는 체력 소진");
        //     RestartButton.SetActive(true);
        // }
        // else
        // {
        //     // 일반 모드에서는 체력 감소
        //     health--;
        //     if (UIhealth.Length > health)
        //         UIhealth[health].color = new Color(1, 0, 0, 0.2f);
        // }


        {
            if (health > 0)
            {
                health--;
                Debug.Log("생명 감소");
                Debug.Log(health);
                UpdateHealthUI(); // ✅ UI 업데이트
                //photonView.RPC("PlayerReposition", RpcTarget.All);
            }

            if (health <= 0)
            {   
                Debug.Log(health);
                player.OnDie();
                Debug.Log("플레이어가 죽었습니다.");
                RestartButton.SetActive(true);
            }
        }

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {

            if (health > 1)
            {
                PlayerReposition();
            }


            HealthDown();
        }
    }

    [PunRPC]
    void PlayerReposition()
    {
        if (player != null)
        {
            player.transform.position = new Vector3(-1.0f, -0.5f, 0);
            //player.VelocityZero();
        }
         
    }


    public void RestartGame()
    {
        Time.timeScale = 1; // Resume the game
        PhotonNetwork.LoadLevel(3);
    }

    public void EnableColorRestoreMode(bool enable)
    {
        colorRestoreMode = enable;
    }


    public IEnumerator GoalAppearEffect()
    {
        if (goalObject == null)
        {
            Debug.LogWarning("Goal 오브젝트가 비어있습니다.");
            yield break;
        }

        SpriteRenderer sr = goalObject.GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogWarning("Goal 오브젝트에 SpriteRenderer가 없습니다.");
            yield break;
        }

        goalObject.SetActive(true); // 활성화는 하지만
        float blinkInterval = 0.2f;
        int blinkCount = 5;

        for (int i = 0; i < blinkCount; i++)
        {
            sr.enabled = false;
            yield return new WaitForSeconds(blinkInterval);
            sr.enabled = true;
            yield return new WaitForSeconds(blinkInterval);
        }

        // 최종적으로 보이도록 유지
        sr.enabled = true;
        Debug.Log("Goal 깜빡임 연출 완료");
    }

    // Finish 아이템을 하나 먹었을 때 호출하는 함수
    public void AddFinishItem()
    {
        // 수치 1 증가
        finishItemCount++;

        // PlayerPrefs에 저장 (로컬 디스크에 저장됨)
        PlayerPrefs.SetInt(FinishItemKey, finishItemCount);
        PlayerPrefs.Save(); // 강제로 저장

        // UI 업데이트
        UpdateFinishItemUI();
    }

    // 로컬 저장된 아이템 개수를 불러오는 함수
    public void LoadFinishItemCount()
    {
        // 만약 저장된 값이 없다면 기본값 0을 반환함
        finishItemCount = PlayerPrefs.GetInt(FinishItemKey, 0);
    }

    // 아이템 수치를 초기화하는 함수 (버튼이나 디버그 용도)
    public void ResetFinishItemData()
    {
        // PlayerPrefs에서 해당 키 제거
        PlayerPrefs.DeleteKey(FinishItemKey);

        // 메모리 상의 수치도 0으로 초기화
        finishItemCount = 0;

        // UI 반영
        UpdateFinishItemUI();
    }

    // UI에 Finish 아이템 수치를 업데이트하는 함수
    public void UpdateFinishItemUI()
    {
        // 텍스트 컴포넌트가 정상 연결되어 있으면 숫자를 표시함
        if (finishItemText != null)
            finishItemText.text = finishItemCount.ToString();
    }

    void UpdateHealthUI()
    {
        if (UIhealth == null)
        {
            Debug.LogError("UIhealth 배열이 null입니다!");
            return;
        }

        for (int i = 0; i < UIhealth.Length; i++)
        {
            if (UIhealth[i] == null)
            {
                Debug.LogError($"UIhealth[{i}]가 null입니다!");
                continue;
            }
            UIhealth[i].gameObject.SetActive(i < health);
        }
    }
}




