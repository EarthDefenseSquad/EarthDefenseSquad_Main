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
    void Start()
    {
        unlockedStages = new bool[stageButtons1960.Length]; //해금 상태를 스테이지 버튼의 길이만큼 1(언락)과 0(락)으로 저장.
        for (int i = 0; i < stageButtons1960.Length; i++)
        {
            // 첫 번째 스테이지만 기본 해금. 즉 i가 0일 때만 언락이 true. 
            unlockedStages[i] = (i == 0);

            UpdateStageButton(i);

            stageNumber = i + 1; // 1번부터 시작
            stageButtons1960[i].onClick.AddListener(() =>
            {
                if (unlockedStages[stageNumber - 1])
                    Debug.Log($"스테이지 {stageNumber} 선택됨");
                else
                    Debug.Log($"스테이지 {stageNumber}은 잠겨 있음");
            });
        }
    }

    public void Go1960Scene()
    {
        PhotonNetwork.LoadLevel("StageScene");
        int playerIndex = PhotonNetwork.IsMasterClient ? 0 : 1;
        GameManager.Instance.SpawnPlayer(playerIndex);
    }

    public void Go1970Scene()
    {
        PhotonNetwork.LoadLevel("StageScene");
        int playerIndex = PhotonNetwork.IsMasterClient ? 0 : 1;
        GameManager.Instance.SpawnPlayer(playerIndex);
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
    public void UnlockStage(int stageNumber)
    {
        int idx = stageNumber;
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
