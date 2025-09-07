using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class StageSelectUI : MonoBehaviour
{
     public Button[] stageButtons1960;
    // 각 스테이지의 해금 상태를 저장
    private bool[] unlockedStages;
    public int stageNumber;
    private GameManager gameManager;

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }
        void Start()
    {
        unlockedStages = new bool[stageButtons1960.Length];

            // 기본: 첫번째 스테이지 언락
            //unlockedStages[0] = true;
            stageNumber = gameManager.stageIndex;
            // 현재 stageNumber까지 해금 상태로 설정
            for (int i = 0; i <= stageNumber && i < unlockedStages.Length; i++)
            {
                unlockedStages[i] = true;
                UpdateStageButton(i);
                Debug.Log("STAGE" + i + "열렸습니다.");
            }
        
        // 각 버튼마다 개별 인덱스 복사해서 클릭 이벤트 등록
        for (int i = 0; i < stageButtons1960.Length; i++)
        {
            int index = i; // 클로저 문제 해결용 로컬 변수 복사
            stageButtons1960[i].onClick.AddListener(() =>
            {
                if (unlockedStages[index])
                    Debug.Log($"스테이지 {index + 1} 선택됨");
                else
                    Debug.Log($"스테이지 {index + 1}은 잠겨 있음");
            });
        }
    }


    public void Go1960Scene()
    {
        PhotonNetwork.LoadLevel("StageScene");
        int playerIndex = PhotonNetwork.IsMasterClient ? 0 : 1;
        gameManager.SpawnPlayer(playerIndex);
    }

    public void Go1970Scene()
    {
        PhotonNetwork.LoadLevel("StageScene");
        int playerIndex = PhotonNetwork.IsMasterClient ? 0 : 1;
        gameManager.SpawnPlayer(playerIndex);
    }

    public void Go1980Scene()
    {
        PhotonNetwork.LoadLevel("StageScene");
    }

    public void Go1990Scene()
    {
        PhotonNetwork.LoadLevel("StageScene");
    }

    public void Go2000Scene()
    {
        PhotonNetwork.LoadLevel("StageScene");
    }

    public void Go2010Scene()
    {
        PhotonNetwork.LoadLevel("StageScene");
    }

    public void GoWaitingScene()
    {
        PhotonNetwork.LoadLevel("WaitingScene");
    }


    // 특정 스테이지를 해금(잠금 해제)하는 public 메서드
    public void UnlockStage(int stageNum)
    {
        int idx = stageNum;
        if (idx >= 0 && idx < unlockedStages.Length)
        {
            unlockedStages[idx] = true; //언락됨.
            UpdateStageButton(idx); //보여지는 상태도 같이 업데이트.
        }

    }

    // 버튼과 LockIcon UI 상태 갱신
    public void UpdateStageButton(int i)
    {
        Button btn = stageButtons1960[i];
        bool isUnlocked = unlockedStages[i];
        btn.interactable = isUnlocked;

        Transform lockIcon = btn.transform.Find("LockIcon");
        Image bg = btn.GetComponent<Image>();

        if (isUnlocked)
        {
            if (bg) bg.color = Color.white;
            if (lockIcon) lockIcon.gameObject.SetActive(false);
        }
        else
        {
            //if (bg) bg.color = Color.gray;
            if (lockIcon) lockIcon.gameObject.SetActive(true);
        }
    }
}
