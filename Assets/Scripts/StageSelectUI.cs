using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

// UnityEngine.UI.Button 이름 충돌방지, using안에서만 별칭 사용
using Image = UnityEngine.UI.Image;
using Button = UnityEngine.UI.Button;

public class StageSelectUI : MonoBehaviourPun
{
    [Header("Stage Buttons Per Year")]
    public Button[] stageButtons1960;
    public Button[] yearButtons;

    private bool[] unlockedStages;
    public int stageNumber;
    public Button backButton;
    public GameObject yearSelect;
    public GameObject[] stageSelect1960, stageSelect1970, stageSelect1980, stageSelect1990, stageSelect2000, stageSelect2010;
    private GameObject[][] stageSelects;

    void Start()
    {
        // GameObject 2차원 배열 구성
        stageSelects = new GameObject[][] {
            stageSelect1960, stageSelect1970, stageSelect1980, stageSelect1990, stageSelect2000, stageSelect2010
        };

        // 해금 배열 초기화
        unlockedStages = new bool[stageButtons1960.Length];

        // 게임매니저 상태 반영
        stageNumber = GameManager.Instance.stageIndex;
        if (stageNumber == 10) {
            GameManager.Instance.year++;
            GameManager.Instance.stageIndex = 0;
            stageNumber = 0;
        }

        // 스테이지 언락 처리
        for (int i = 0; i <= stageNumber && i < unlockedStages.Length; i++) {
            unlockedStages[i] = true;
            UpdateStageButton(i);
        }

        // 연도/스테이지 버튼 초기화
        if (PhotonNetwork.IsMasterClient) {
            SetYearButtonListeners(true);
        }
        else {
            SetYearButtonListeners(false);
            backButton.gameObject.SetActive(false);
        }

        SetStageButtonListeners();
    }

    private void SetYearButtonListeners(bool interactable)
    {
        for (int i = 0; i < yearButtons.Length; i++)
        {
            yearButtons[i].interactable = interactable;
            int idx = i;
            yearButtons[i].onClick.RemoveAllListeners();
            yearButtons[i].onClick.AddListener(() =>
            {
                photonView.RPC("OnYearButtonClicked", RpcTarget.All, idx);
            });
        }
    }

    private void SetStageButtonListeners()
    {
        for (int i = 0; i < stageButtons1960.Length; i++)
        {
            int idx = i;
            stageButtons1960[i].onClick.RemoveAllListeners();
            stageButtons1960[i].onClick.AddListener(() =>
            {
                if (unlockedStages[idx])
                {
                    Debug.Log($"스테이지 {idx + 1} 선택됨");
                    if (PhotonNetwork.IsMasterClient)
                        photonView.RPC("LoadStageScene", RpcTarget.All, idx);
                    else
                        photonView.RPC("LoadStageScene", RpcTarget.MasterClient, idx);
                }
                else Debug.Log($"스테이지 {idx + 1}은 잠겨 있음");
            });
        }
    }

    [PunRPC]
    void OnYearButtonClicked(int yearIdx)
    {
        yearSelect.SetActive(false);
        for (int i = 0; i < stageSelects.Length; i++)
        {
            foreach (GameObject go in stageSelects[i])
                go.SetActive(i == yearIdx);
        }
        // 버튼 상태 감지, 연도별 추가 처리 시 여기에
    }

    [PunRPC]
    void LoadStageScene(int stageIdx)
    {
        GameManager.Instance.stageIndex = stageIdx;
        PhotonNetwork.LoadLevel("StageScene");
    }

    // 스테이지 언락
    public void UnlockStage(int stageIdx)
    {
        if (stageIdx >= 0 && stageIdx < unlockedStages.Length)
        {
            unlockedStages[stageIdx] = true;
            UpdateStageButton(stageIdx);
        }
    }

    // 버튼 및 LockIcon UI상태 동기화
    public void UpdateStageButton(int i)
    {
        Button btn = stageButtons1960[i];
        bool isUnlocked = unlockedStages[i];
        btn.interactable = isUnlocked;

        Transform lockIcon = btn.transform.Find("LockIcon");
        Image bg = btn.GetComponent<Image>();

        if (isUnlocked) {
            if (bg) bg.color = Color.white;
            if (lockIcon) lockIcon.gameObject.SetActive(false);
        } else {
            if (bg) bg.color = Color.gray;
            if (lockIcon) lockIcon.gameObject.SetActive(true);
        }
    }
}
