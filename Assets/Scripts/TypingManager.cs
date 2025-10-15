using UnityEngine;
using TMPro;
using System.Collections;
using Photon.Pun;

public class TypingManager : MonoBehaviourPun
{
    public TMP_InputField inputField;
    public CameraShake_tmp cameraShake;

    private GameObject[] currentTargets;
    private string correctKeyword;
    private GameObject currentQuestionPanel;
    private GameObject currentTriggerObject;
    private PlayerMove player;

    private void Start()
    {
        inputField.gameObject.SetActive(false);
        player = FindLocalPlayer();
    }

    void Update()
    {
        if (inputField.gameObject.activeSelf && Input.GetKeyDown(KeyCode.Return))
        {
            CheckInput(inputField.text);
        }
    }

    private PlayerMove FindLocalPlayer()
    {
        PlayerMove[] allPlayers = FindObjectsOfType<PlayerMove>();
        foreach (var p in allPlayers)
        {
            var view = p.GetComponent<Photon.Pun.PhotonView>();
            if (view == null || view.IsMine)
                return p;
        }
        return null;
    }

    public void ShowInputField(GameObject triggerObject, GameObject[] targetObjects, string keyword, GameObject questionPanel, string questionText)
{
        // if (player != null && player.playerType == PlayerMove.PlayerType.Player2)
        // {
        //     Debug.Log("🚫 Player2는 타이핑 퍼즐을 사용할 수 없습니다.");
        //     return;
        // }

        currentTriggerObject = triggerObject;
        currentTargets = targetObjects;
        correctKeyword = keyword.Trim().ToLower();
        currentQuestionPanel = questionPanel;

        // ✅ questionText로 UI에 표시 (정답 keyword는 노출하지 않음)
        TMP_Text text = currentQuestionPanel.GetComponentInChildren<TMP_Text>();
        if (text != null)
        text.text = questionText; 

        currentQuestionPanel.SetActive(true);

        inputField.text = "";
        inputField.gameObject.SetActive(true);
        inputField.ActivateInputField();

        PlayerMove[] allPlayers = FindObjectsOfType<PlayerMove>();
        foreach (var p in allPlayers)
        p.enabled = false;
    }

    private void CheckInput(string input)
    {
        input = input.Trim().ToLower();

        if (input == correctKeyword)
        {
            // if (currentTargets != null)
            // {
            //     foreach (GameObject obj in currentTargets)
            //         obj.SetActive(false);
            // }

            // if (currentTriggerObject != null)
            //     currentTriggerObject.SetActive(false);

            photonView.RPC("RPC_DeactivateObjects", RpcTarget.AllBuffered,
            currentTriggerObject != null ? currentTriggerObject.name : "",
            GetTargetNames());

            //HideUI();
        }
        else
        {
            StartCoroutine(cameraShake.Shake(0.3f, 0.2f));
            inputField.text = "";
            inputField.ActivateInputField();
        }
    }

    private void HideUI()
    {
        if (currentQuestionPanel != null)
            currentQuestionPanel.SetActive(false);

        inputField.gameObject.SetActive(false);

        // ✅ 모든 플레이어 이동 복원
        PlayerMove[] allPlayers = FindObjectsOfType<PlayerMove>();
        foreach (var p in allPlayers)
        p.enabled = true;
    }

    // --- RPC 함수: 모든 클라이언트에서 오브젝트 비활성화 ---
    [PunRPC]
    void RPC_DeactivateObjects(string triggerName, string[] targetNames)
    {
        // Trigger 비활성화
        if (!string.IsNullOrEmpty(triggerName))
        {
            GameObject trig = GameObject.Find(triggerName);
            if (trig != null) trig.SetActive(false);
        }

        // Target 비활성화
        foreach (var tName in targetNames)
        {
            GameObject obj = GameObject.Find(tName);
            if (obj != null) obj.SetActive(false);
        }

        // 🔥 UI도 같이 끄기
        HideUI();   
    }

    // --- 대상 오브젝트 이름 배열로 변환 ---
    private string[] GetTargetNames()
    {
        if (currentTargets == null) return new string[0];

        string[] names = new string[currentTargets.Length];
        for (int i = 0; i < currentTargets.Length; i++)
            names[i] = currentTargets[i].name;
        return names;
    }


}
