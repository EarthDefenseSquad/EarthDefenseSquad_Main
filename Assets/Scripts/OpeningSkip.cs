using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class OpeningSkip : MonoBehaviourPun
{
    [Header("플레이어별 버튼")]
    public Button player1Button; // 방장용 버튼
    public Button player2Button; // 클라이언트용 버튼

    [Header("버튼 색상")]
    public Color readyColor = Color.green;   // 준비 완료 시 버튼 색상
    public Color defaultColor = Color.white; // 기본 버튼 색상

    [Header("텍스트 색상 (TMP)")]
    public Color readyTextColor = Color.black;   // 준비 완료 시 텍스트 색상
    public Color defaultTextColor = Color.black; // 기본 텍스트 색상

    [Header("다음 씬 이름")]
    public string nextSceneName = "WaitingScene";

    private bool player1Ready = false;
    private bool player2Ready = false;

    void Start()
    {   // 초기 색상 세팅
        ResetButtonStyles();

        // 🔸 로컬 플레이어가 방장인지 확인
        bool isMaster = PhotonNetwork.IsMasterClient;
        
        // 🔹 내 버튼만 클릭 가능하도록 처리하되, 시각적으로는 그대로 유지
        LockButtonClick(player1Button, !isMaster); // 마스터가 아니면 1P 버튼 클릭 차단
        LockButtonClick(player2Button, isMaster);  // 마스터면 2P 버튼 클릭 차단

        // 🔹 클릭 리스너 등록
        if (player1Button != null)
        {
            player1Button.onClick.RemoveAllListeners();
            player1Button.onClick.AddListener(() => {
                if (isMaster)
                    OnPlayerButtonClicked(1);
            });
        }

        if (player2Button != null)
        {
            player2Button.onClick.RemoveAllListeners();
            player2Button.onClick.AddListener(() => {
                if (!isMaster)
                    OnPlayerButtonClicked(2);
            });
        }

        
    }

    private void LockButtonClick(Button btn, bool locked)
    {
        if (btn == null) return;

        // 🔸 버튼 색상은 유지 (interactable을 false로 하지 않음)
        btn.interactable = true;

        // ✅ 클릭만 막기
        var cg = btn.GetComponent<CanvasGroup>();
        if (cg == null) cg = btn.gameObject.AddComponent<CanvasGroup>();
        cg.interactable = true;          // 🔸 여기 true 유지 (시각적 유지)
        cg.blocksRaycasts = !locked;     // 🔸 클릭 차단
        
    }
        
        
    private void OnPlayerButtonClicked(int playerNumber)
    {
        photonView.RPC(nameof(RPC_SetReady), RpcTarget.AllBuffered, playerNumber);
    }

    [PunRPC]
    private void RPC_SetReady(int playerNumber)
    {
        if (playerNumber == 1)
        {
            player1Ready = true;
            SetButtonStyle(player1Button, readyColor, readyTextColor);
        }
        else if (playerNumber == 2)
        {
            player2Ready = true;
            SetButtonStyle(player2Button, readyColor, readyTextColor);
        }

        Debug.Log($"Player{playerNumber} 준비 완료. (player1={player1Ready}, player2={player2Ready})");

        // 🔸 두 명 모두 준비 완료 시 마스터가 씬 전환
        if (player1Ready && player2Ready && PhotonNetwork.IsMasterClient)
        {
            photonView.RPC(nameof(RPC_LoadNextScene), RpcTarget.AllBuffered, nextSceneName);
        }
        
    }

    [PunRPC]
    private void RPC_LoadNextScene(string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName);
    }

    private void SetButtonStyle(Button button, Color buttonColor, Color textColor)
    {
        if (button == null) return;

        // 버튼 배경 색상 변경
        if (button.image != null)
            button.image.color = buttonColor;

        // TMP 텍스트 색상 변경
        TextMeshProUGUI tmpText = button.GetComponentInChildren<TextMeshProUGUI>();
        if (tmpText != null)
            tmpText.color = textColor;
    }

    private void ResetButtonStyles()
    {
        SetButtonStyle(player1Button, defaultColor, defaultTextColor);
        SetButtonStyle(player2Button, defaultColor, defaultTextColor);
    }
}
