using UnityEngine;
using TMPro;
using System.Collections;

public class TypingManager : MonoBehaviour
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

    public void ShowInputField(GameObject triggerObject, GameObject[] targetObjects, string keyword, GameObject questionPanel)
    {
        if (player != null && player.playerType == PlayerMove.PlayerType.Player2)
        {
            Debug.Log("🚫 Player2는 타이핑 퍼즐을 사용할 수 없습니다.");
            return;
        }

        currentTriggerObject = triggerObject;
        currentTargets = targetObjects;
        correctKeyword = keyword.Trim().ToLower();
        currentQuestionPanel = questionPanel;

        TMP_Text text = currentQuestionPanel.GetComponentInChildren<TMP_Text>();
        if (text != null)
            text.text = keyword;

        currentQuestionPanel.SetActive(true);

        inputField.text = "";
        inputField.gameObject.SetActive(true);
        inputField.ActivateInputField();

        if (player != null)
            player.enabled = false;
    }

    private void CheckInput(string input)
    {
        input = input.Trim().ToLower();

        if (input == correctKeyword)
        {
            if (currentTargets != null)
            {
                foreach (GameObject obj in currentTargets)
                    obj.SetActive(false);
            }

            if (currentTriggerObject != null)
                currentTriggerObject.SetActive(false);

            HideUI();
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

        if (player != null)
            player.enabled = true;
    }
}
