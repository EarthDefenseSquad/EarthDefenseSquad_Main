using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class StageSelectNetworkManager : MonoBehaviourPun
{
    public Button yearButton_1970;
    public GameObject panel_1970;

    public Button stageButton_1970_1;
    public Button stageButton_1970_2;
    public Button stageButton_1970_3;
    public Button stageButton_1970_4;
    public Button stageButton_1970_5;

    void Start()
    {
        // 초기 패널 비활성화
        if (panel_1970 != null)
            panel_1970.SetActive(false);

        if (yearButton_1970 != null)
            yearButton_1970.onClick.AddListener(OnYearButton1970Clicked);

        // 마스터 클라이언트만 스테이지 버튼 클릭 가능하도록 설정
        bool isMaster = PhotonNetwork.IsMasterClient;
        SetStageButtonsInteractable(isMaster);

        // 각 스테이지 버튼에 클릭 리스너 등록
        if (stageButton_1970_1 != null)
            stageButton_1970_1.onClick.AddListener(() => OnStageButtonClicked(1));
        if (stageButton_1970_2 != null)
            stageButton_1970_2.onClick.AddListener(() => OnStageButtonClicked(2));
        if (stageButton_1970_3 != null)
            stageButton_1970_3.onClick.AddListener(() => OnStageButtonClicked(3));
        if (stageButton_1970_4 != null)
            stageButton_1970_4.onClick.AddListener(() => OnStageButtonClicked(4));
        if (stageButton_1970_5 != null)
            stageButton_1970_5.onClick.AddListener(() => OnStageButtonClicked(5));
    }


    private void SetStageButtonsInteractable(bool interactable)
    {
        stageButton_1970_1.interactable = interactable;
        stageButton_1970_2.interactable = interactable;
        stageButton_1970_3.interactable = interactable;
        stageButton_1970_4.interactable = interactable;
        stageButton_1970_5.interactable = interactable;
    }

    private void OnYearButton1970Clicked()
    {
        // 마스터 클라이언트만 패널 활성화 관련 RPC 전송 가능
        if (PhotonNetwork.IsMasterClient)
            photonView.RPC(nameof(RPC_ShowPanel1970), RpcTarget.All);
        else
            Debug.LogWarning("Year button click은 마스터 클라이언트만 가능합니다.");
    }

    [PunRPC]
    private void RPC_ShowPanel1970()
    {
        if (panel_1970 != null)
            panel_1970.SetActive(true);
    }

    private void OnStageButtonClicked(int stageIndex)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogWarning("스테이지 선택은 마스터 클라이언트만 가능합니다.");
            return;
        }

        // 선택된 스테이지에 따라 필요 시 다른 처리를 할 수 있음
        Debug.Log($"마스터가 스테이지 {stageIndex} 선택");

        
    }

    
}

