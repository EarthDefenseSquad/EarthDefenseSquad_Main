using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using ExitGames.Client.Photon;
using Microsoft.SqlServer.Server;

public class StageSelectUI : MonoBehaviourPun
{
    // 1960년대 스테이지 버튼 배열 (UI 버튼들)
    public Button[] stageButtons1960;

    // 각 스테이지의 해금(잠금 해제) 여부를 저장하는 배열
    private bool[] unlockedStages;

    // 현재 선택된 스테이지 번호
    public int stageNumber;

    // 게임 전반 관리 매니저 참조
    private GameManager gameManager;

    [Header("스테이지 버튼들")]
    // 실제 모든 스테이지 버튼 (예: 1960, 1970, 1980 등 포함 가능)
    public Button[] stageButtons;

    [Header("게임 씬 이름")]
    // 선택된 스테이지를 로드할 씬 이름 (모든 스테이지가 포함된 하나의 씬이라고 가정)
    public string gameSceneName = "StageScene"; 
    public  const string FinishItemKey = "FinishItemCount";

    void Awake()
    {
        // 게임 매니저 찾기
        gameManager = FindObjectOfType<GameManager>();

        // PlayerMove에서 static으로 관리되는 clearedStage 값을 가져와 현재 스테이지 번호 설정
        stageNumber = PlayerMove.clearedStage;
    }

    void Start()
    {
        
        
        if (PhotonNetwork.IsMasterClient)
        {
            // 모든 스테이지 버튼을 순회하면서 초기화
            for (int i = 0; i < stageButtons.Length; i++)
            {

                Button button = stageButtons[i];

                // 각 버튼에 붙어 있는 StageButtonData 컴포넌트 가져오기
                StageButtonData data = button.GetComponent<StageButtonData>();
            
                if (data == null) continue; // StageButtonData가 없으면 패스

                // --- 스테이지 1(첫 번째)은 무조건 해금 ---
                /*if (data.stageIndex == 0)
                {
                    isUnlocked = true;
                }
                else
                {
                    // 기존 해금 여부 로직
                    isUnlocked = string.IsNullOrEmpty(data.requiredFinishID)
                        || PlayerPrefs.GetInt(data.requiredFinishID, 0) == 1;
                }*/

                // --- 스테이지 해금 여부 확인 ---
                // requiredFinishID가 없거나, PlayerPrefs에 저장된 값이 1이면 해금
                bool isUnlocked = string.IsNullOrEmpty(data.requiredFinishID)
                    || PlayerPrefs.GetInt(data.requiredFinishID, 0) == 1;

                photonView.RPC("RPC_stageUpdateUI", RpcTarget.AllBuffered, i, isUnlocked);

            }
        }
    } 


    [PunRPC]
    public void RPC_ComparerequiredFinishID(int clientValue)
    {
        for (int i = 0; i < stageButtons.Length; i++)
        {
            Button button = stageButtons[i];
            StageButtonData data = button.GetComponent<StageButtonData>();
            int myValue = PlayerPrefs.GetInt(data.requiredFinishID, 0);

            // 규칙: 더 큰 값을 기준으로 맞추기
            int syncValue = Mathf.Max(myValue, clientValue);

            int ItemCount = syncValue;
            PlayerPrefs.SetInt(data.requiredFinishID, ItemCount); //이제 두 명의 값이 같아짐.
        }
    }
    
    [PunRPC]
    void RPC_stageUpdateUI(int index, bool isUnlocked)
    {
            Button button = stageButtons[index];
            StageButtonData data = button.GetComponent<StageButtonData>();

            button.interactable = isUnlocked;
            button.enabled = isUnlocked;
            
            // --- 스테이지 1(첫 번째)은 항상 해금 ---
            /*if (data.stageIndex == 0)
            {
                isUnlocked = true;
            }*/


            // --- 자물쇠 아이콘 처리 ---
            Transform lockIcon = button.transform.Find("LockIcon");
            if (lockIcon != null)
            {
                // 잠겨있으면 자물쇠 켜기, 해금이면 끄기
                lockIcon.gameObject.SetActive(!isUnlocked);

                // RaycastTarget 꺼서 자물쇠 아이콘이 클릭을 방해하지 않게 함
                Image img = lockIcon.GetComponent<Image>();
                if (img != null)
                    img.raycastTarget = false;
            }

            // 클릭 이벤트 등록
            int selectedIndex = data.stageIndex; // 캡쳐 문제 방지 위해 지역 변수 사용
            button.onClick.RemoveAllListeners(); // 중복 방지
            button.onClick.AddListener(() =>
            {
                // 버튼이 해금된 상태일 때만 실행
                if (isUnlocked)
                {
                    // 선택된 스테이지 인덱스를 포톤 CustomProperties에 저장 (동기화 용도)
                    Hashtable props = new Hashtable
                    {
                        { "SelectedStageIndex", selectedIndex }
                    };
                    PhotonNetwork.LocalPlayer.SetCustomProperties(props);

                    Debug.Log($"selectedIndex: {selectedIndex}");

                    // 게임 씬 로드
                    PhotonNetwork.LoadLevel(gameSceneName);
                }
            });
        
    }
    // --- 씬 이동용 버튼 함수들 (UI Button에서 직접 연결 가능) ---
    public void Go1960Scene() => PhotonNetwork.LoadLevel("StageScene");
    public void Go1970Scene() => PhotonNetwork.LoadLevel("StageScene");
    public void Go1980Scene() => PhotonNetwork.LoadLevel("StageScene");
    public void Go1990Scene() => PhotonNetwork.LoadLevel("StageScene");
    public void Go2000Scene() => PhotonNetwork.LoadLevel("StageScene");
    public void Go2010Scene() => PhotonNetwork.LoadLevel("StageScene");
    public void GoWaitingScene() => PhotonNetwork.LoadLevel("WaitingScene");

    // --- 특정 스테이지 해금 함수 ---
    public void UnlockStage(int stageNum)
    {
        int idx = stageNum;
        if (idx >= 0 && idx < unlockedStages.Length)
        {
            unlockedStages[idx] = true; // 해당 스테이지 언락 처리
            UpdateStageButton(idx);     // 버튼 UI 갱신
        }
    }

    // --- 버튼과 LockIcon UI 상태 갱신 ---
    public void UpdateStageButton(int i)
    {
        Button btn = stageButtons1960[i];
        bool isUnlocked = unlockedStages[i];

        // 버튼 활성화 여부 갱신
        btn.interactable = isUnlocked;

        // 잠금 아이콘 처리
        Transform lockIcon = btn.transform.Find("LockIcon");
        Image bg = btn.GetComponent<Image>();

        if (isUnlocked)
        {
            if (bg) bg.color = Color.white;           // 해금된 버튼은 흰색 처리
            if (lockIcon) lockIcon.gameObject.SetActive(false);
        }
        else
        {
            // if (bg) bg.color = Color.gray;         // 잠겨있을 때는 회색 처리할 수도 있음
            if (lockIcon) lockIcon.gameObject.SetActive(true);
        }
    }
}
