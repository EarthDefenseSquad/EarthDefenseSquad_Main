using UnityEngine;
using UnityEngine.UI;

public class StageSelectUI : MonoBehaviour
{
    public Button[] stageButtons;


    // 각 스테이지의 해금 상태를 저장
    private bool[] unlockedStages;
    public int stageNumber;
    void Start()
    {
        unlockedStages = new bool[stageButtons.Length]; //해금 상태를 스테이지 버튼의 길이만큼 1(언락)과 0(락)으로 저장.
        for (int i = 0; i < stageButtons.Length; i++)
        {
            // 첫 번째 스테이지만 기본 해금. 즉 i가 0일 때만 언락이 true. 
            unlockedStages[i] = (i == 0);

            UpdateStageButton(i);

            stageNumber = i + 1; // 1번부터 시작
            stageButtons[i].onClick.AddListener(() =>
            {
                if (unlockedStages[stageNumber - 1])
                    Debug.Log($"스테이지 {stageNumber} 선택됨");
                else
                    Debug.Log($"스테이지 {stageNumber}은 잠겨 있음");
            });
        }
    }
    // 특정 스테이지를 해금(잠금 해제)하는 public 메서드
    public void UnlockStage(int stageNumber)
    {
        int idx = stageNumber - 1;
        if (idx >= 0 && idx < unlockedStages.Length)
        {
            unlockedStages[idx] = true; //언락됨.
            UpdateStageButton(idx); //보여지는 상태도 같이 업데이트.
        }
    }

    // 버튼과 LockIcon UI 상태 갱신
    private void UpdateStageButton(int i)
    {
        Button btn = stageButtons[i];
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
