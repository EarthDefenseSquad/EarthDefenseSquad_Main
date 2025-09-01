using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;
using Button = UnityEngine.UI.Button;
using Unity.VisualScripting;

public class StageSelectUI : MonoBehaviourPun
{
    public UnityEngine.UI.Button[] stageButtons1960;
    public UnityEngine.UI.Button[] yearButtons;


    // 각 스테이지의 해금 상태를 저장
    private bool[] unlockedStages;
    public int stageNumber;

    public UnityEngine.UI.Button backButton;
    public GameObject yearSelect;
    public GameObject[] stageSelect1960,stageSelect1970,stageSelect1980,stageSelect1990,stageSelect2000,stageSelect2010;
    public GameObject[][] stageSelects;
    
    
    
    

    void Start()
{
        stageSelects = new GameObject[6][];
        stageSelects[0] = stageSelect1960;
        stageSelects[1] = stageSelect1970;
        stageSelects[2] = stageSelect1980;
        stageSelects[3] = stageSelect1990;
        stageSelects[4] = stageSelect2000;
        stageSelects[5] = stageSelect2010;

    unlockedStages = new bool[stageButtons1960.Length];

        // 기본: 첫번째 스테이지 언락
        //unlockedStages[0] = true;
        stageNumber = GameManager.Instance.stageIndex;
        if (stageNumber == 10)
        {
            GameManager.Instance.year++;
            GameManager.Instance.stageIndex = 0;
            stageNumber = 0;
        }
        // 현재 stageNumber까지 해금 상태로 설정
        for (int i = 0; i <= stageNumber && i < unlockedStages.Length; i++)
        {
            unlockedStages[i] = true;
            UpdateStageButton(i);
            Debug.Log("STAGE" + i + "열렸습니다.");
        }

        
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("ClickYear", RpcTarget.All); //방장이 year버튼 클릭하면, 
        }
        else
        {
            for (int j = 0; j < yearButtons.Length; j++)
            {
                 yearButtons[j].interactable = false;
            }
            backButton.gameObject.SetActive(false);
        }
}


    [PunRPC]
    void ClickYear()
    {
        for (int j = 0; j < yearButtons.Length; j++)
        {
            int index = j; // 클로저 문제 해결용 로컬 변수 복사
            yearButtons[j].onClick.AddListener(() =>
            {
                Debug.Log(yearButtons[index]);
                photonView.RPC("OnYearButtonClicked", RpcTarget.All);   
            }
            );
        }
        for (int i = 0; i < stageButtons1960.Length; i++)
        {
            int index = i; // 클로저 문제 해결용 로컬 변수 복사
            stageButtons1960[i].onClick.AddListener(() =>
            {
                if (unlockedStages[index])
                {
                    Debug.Log($"스테이지 {index + 1} 선택됨");
                    // 마스터 클라이언트만 씬 전환 RPC 호출
                    if (PhotonNetwork.IsMasterClient)
                    {
                        photonView.RPC("LoadStageScene", RpcTarget.All, i);
                    }
                    else
                    {
                        // 비마스터 클라이언트는 마스터에게 요청
                        photonView.RPC("LoadStageScene", RpcTarget.MasterClient, i);
                    } 
                }

                else
                    Debug.Log($"스테이지 {index + 1}은 잠겨 있음");
            });
        }
    }

    [PunRPC]
    void LoadStageScene(int stageIndex)
    {
        PhotonNetwork.LoadLevel("StageScene");
        // 필요시 GameManager 등에 스테이지 정보 전달
    }

    [PunRPC]
    void OnYearButtonClicked()
    {
        int i = GameManager.Instance.year;
        yearSelect.SetActive(false); //연도 선택 후에는 연도 선택 사라짐
        foreach (GameObject go in stageSelects[i])
        {
            go.SetActive(true);  //그리고 해당하는 연도의 스테이지들이 나타남.
            if (!PhotonNetwork.IsMasterClient)
            {
                if (stageSelects[i] == stageSelect1960)
                {
                    for (int j = 0; j < unlockedStages.Length; j++)
                    {
                        stageButtons1960[j].interactable = unlockedStages[j];
                        unlockedStages[j] = false;
                    }
                }
            }
        }
    }
        
    public void Go1960Scene()
{
    PhotonNetwork.LoadLevel("StageScene");
    //int playerIndex = PhotonNetwork.IsMasterClient ? 0 : 1;
    //GameManager.Instance.SpawnPlayer(playerIndex);
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
