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

    [Header("스테이지 버튼들")]
    public Button[] stageButtons;

    [Header("게임 씬 이름")]
    public string gameSceneName = "StageScene"; // 예: 모든 스테이지가 포함된 하나의 씬


    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        stageNumber=PlayerMove.clearedStage;
        
    }
    void Start()
    {
        for (int i = 0; i < stageButtons.Length; i++)
        {
            Button button = stageButtons[i];
            StageButtonData data = button.GetComponent<StageButtonData>();

            if (data == null) continue;

            bool isUnlocked = string.IsNullOrEmpty(data.requiredFinishID)
                || PlayerPrefs.GetInt(data.requiredFinishID, 0) == 1;

            button.interactable = isUnlocked;
            button.enabled = isUnlocked;

            // 🔓 Lock 아이콘 처리
            Transform lockIcon = button.transform.Find("LockIcon");
            if (lockIcon != null)
            {
                lockIcon.gameObject.SetActive(!isUnlocked);

                // Raycast 막는 문제 해결
                Image img = lockIcon.GetComponent<Image>();
                if (img != null)
                    img.raycastTarget = false;
            }


            int selectedIndex = data.stageIndex;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                if (isUnlocked)
                {
                    PlayerPrefs.SetInt("SelectedStageIndex", selectedIndex);
                    PlayerPrefs.Save();
                    SceneManager.LoadScene(gameSceneName);
                }
            });
        }
    }



    public void Go1960Scene()
    {
        PhotonNetwork.LoadLevel("StageScene");

    }

    public void Go1970Scene()
    {
        PhotonNetwork.LoadLevel("StageScene");
        
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
