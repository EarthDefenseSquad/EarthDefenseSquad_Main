using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Linq;
using UnityEngine.EventSystems;
public class ChatManager : MonoBehaviourPunCallbacks,  IPointerClickHandler
{
    public TMP_InputField MessageInput;   // 메시지 입력 필드
    
    public TextMeshProUGUI chatLog; //채팅 로그를 표시할 텍스트 UI
    public Button sendBtn;        // 메시지 전송 버튼

    public ScrollRect chatScrollRect;
    public static bool isChatInputActive = false;
    const int maxLines = 10;
    PhotonView pv;

    void Start()
    {
        Debug.Log("챗 매니저 시작");
        pv = GetComponent<PhotonView>();
        PhotonNetwork.IsMessageQueueRunning = true;
        sendBtn.onClick.AddListener(SendButtonOnClicked);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        MessageInput.ActivateInputField();
    }
    // 메시지 전송 버튼 클릭 시 호출
    public void SendButtonOnClicked()
    {
        Debug.Log("send button clicked");
        if (string.IsNullOrEmpty(MessageInput.text)) return;

        string msg = $"[{PhotonNetwork.NickName}]: {MessageInput.text}";
        pv.RPC("ReceiveMsg", RpcTarget.All, msg); // 모든 플레이어에게 전송
        MessageInput.text = "";
    }

    // 엔터키로도 전송
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && MessageInput.isFocused)
        {
            SendButtonOnClicked();
        }
        MessageInput.onSelect.AddListener(delegate { isChatInputActive = true; });
        MessageInput.onDeselect.AddListener(delegate { isChatInputActive = false; });
    }

    // RPC로 메시지 수신
    [PunRPC]
    public void ReceiveMsg(string msg)
    {
        chatLog.text += "\n" + msg;
        Canvas.ForceUpdateCanvases();
        if (chatScrollRect != null)
        chatScrollRect.verticalNormalizedPosition = 0f;
        var lines = chatLog.text.Split('\n');
        if (lines.Length > maxLines)
            chatLog.text = string.Join("\n", lines.Skip(lines.Length - maxLines));
    }
}
