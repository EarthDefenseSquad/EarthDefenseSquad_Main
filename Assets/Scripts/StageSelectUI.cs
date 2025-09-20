using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using ExitGames.Client.Photon;

public class StageSelectUI : MonoBehaviourPunCallbacks
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
        stageNumber = PlayerMove.clearedStage;

    }
    void Start()
    {
        for (int i = 0; i < stageButtons.Length; i++)
        {
            Button button = stageButtons[i];
            StageButtonData data = button.GetComponent<StageButtonData>();

            if (data == null) continue;

            //bool isUnlocked = string.IsNullOrEmpty(data.requiredFinishID)
            //    || PlayerPrefs.GetInt(data.requiredFinishID, 0) == 1;
            string key = data.requiredFinishID;
            bool isUnlocked = string.IsNullOrEmpty(key) || (PhotonNetwork.CurrentRoom != null
                      && PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(key)
                      && (int)PhotonNetwork.CurrentRoom.CustomProperties[key] == 1);
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
                    Hashtable props = new Hashtable //포톤으로 저장 동기화
                {
                    { "SelectedStageIndex", selectedIndex }
                };
                    PhotonNetwork.LocalPlayer.SetCustomProperties(props);
                    Debug.Log($"selectedIndex: {selectedIndex}");
                    PhotonNetwork.LoadLevel(gameSceneName);
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
    public void UnlockStage(string requiredFinishID)
    {
        if (PhotonNetwork.CurrentRoom == null) return;

        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
        {
            { requiredFinishID, 1 }
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);

        Debug.Log($"{requiredFinishID} 언락됨");
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
    
    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
{
    foreach (Button button in stageButtons)
    {
        StageButtonData data = button.GetComponent<StageButtonData>();
        if (data == null) continue;

        string key = data.requiredFinishID;
        bool isUnlocked = string.IsNullOrEmpty(key) ||
                          (PhotonNetwork.CurrentRoom != null &&
                           PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(key) &&
                           (int)PhotonNetwork.CurrentRoom.CustomProperties[key] == 1);

        button.interactable = isUnlocked;
        button.enabled = isUnlocked;

        Transform lockIcon = button.transform.Find("LockIcon");
        if (lockIcon != null)
        {
            lockIcon.gameObject.SetActive(!isUnlocked);
        }
    }

    Debug.Log("[StageSelectUI] RoomProperties 갱신됨 → 버튼 상태 업데이트 완료");
}

}
