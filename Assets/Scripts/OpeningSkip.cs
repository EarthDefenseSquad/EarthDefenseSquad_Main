using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class OpeningSkip : MonoBehaviourPunCallbacks
{
    public Button player1Button;
    public Button player2Button;
    public Color readyColor = Color.green;
    public Color readyTextColor = Color.black;
    public string nextSceneName = "WaitingScene";

    private bool player1Ready = false;
    private bool player2Ready = false;

    void Awake()
    {
        if (player1Button != null)
        {
            player1Button.onClick.RemoveAllListeners();
            player1Button.onClick.AddListener(() => OnPlayerButtonClicked(1));
        }

        if (player2Button != null)
        {
            player2Button.onClick.RemoveAllListeners();
            player2Button.onClick.AddListener(() => OnPlayerButtonClicked(2));
        }
    }

    private void OnPlayerButtonClicked(int playerNumber)
    {
        int myActor = PhotonNetwork.LocalPlayer.ActorNumber; // 1=방장, 2=클라

        // 🔐 권한 체크: 자기 버튼만 누를 수 있도록 제한
        if (playerNumber != myActor)
        {
            Debug.LogWarning($"[OpeningSkip] Player{myActor}가 Player{playerNumber} 버튼을 누르려 했지만 권한 없음.");
            return;
        }

        // 선택된 캐릭터 index 가져오기
        int selectedIndex = (playerNumber == 1)
            ? CharacterSelectionData.player1SelectedIndex
            : CharacterSelectionData.player2SelectedIndex;

        Debug.Log($"[OpeningSkip] Player{playerNumber} 버튼 클릭 → selectedIndex={selectedIndex}");

        // 네트워크로 Ready 전달
        photonView.RPC(nameof(RPC_SetReady), RpcTarget.AllBuffered, playerNumber, selectedIndex);
    }

    [PunRPC]
    private void RPC_SetReady(int playerNumber, int selectedIndex)
    {
        if (playerNumber == 1)
        {
            // ✅ 항상 선택값 갱신
            CharacterSelectionData.player1SelectedIndex = selectedIndex;

            if (!player1Ready)
            {
                player1Ready = true;
                SetButtonStyle(player1Button, readyColor, readyTextColor);
            }
        }
        else if (playerNumber == 2)
        {
            // ✅ 항상 선택값 갱신
            CharacterSelectionData.player2SelectedIndex = selectedIndex;

            if (!player2Ready)
            {
                player2Ready = true;
                SetButtonStyle(player2Button, readyColor, readyTextColor);
            }
        }

        Debug.Log($"[OpeningSkip] Player{playerNumber} 준비 완료. " +
                  $"(p1={CharacterSelectionData.player1SelectedIndex}, " +
                  $"p2={CharacterSelectionData.player2SelectedIndex})");

        // ✅ 두 명 다 Ready → 방장만 씬 전환 호출
        if (player1Ready && player2Ready && PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(nextSceneName); // RPC 대신 이거만 호출
        }

    }

    [PunRPC]
    private void RPC_LoadNextScene(string sceneName)
    {
        Debug.Log($"[OpeningSkip] 모든 플레이어 Ready. 씬 로드: {sceneName}");
        PhotonNetwork.LoadLevel(sceneName);
    }

    private void SetButtonStyle(Button button, Color bgColor, Color textColor)
    {
        if (button == null) return;

        var img = button.GetComponent<Image>();
        if (img != null) img.color = bgColor;

        var text = button.GetComponentInChildren<TextMeshProUGUI>();
        if (text != null) text.color = textColor;
    }
}
