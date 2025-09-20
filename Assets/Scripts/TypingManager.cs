using UnityEngine;
using TMPro;
using System.Collections;

public class TypingManager : MonoBehaviour
{
    public TMP_InputField inputField;
    public TMP_Text questionText;
    public CameraShake_tmp cameraShake;

    private GameObject[] currentTargets;
    private string correctKeyword;
    private PlayerMove player;

    private void Start()
    {
        inputField.gameObject.SetActive(false);
        questionText.gameObject.SetActive(false);

        inputField.onSubmit.AddListener(CheckInput);
        player = FindLocalPlayer();
    }

    // 내 로컬 캐릭터 찾기 (PhotonView.isMine)
    private PlayerMove FindLocalPlayer()
{
    PlayerMove[] allPlayers = FindObjectsOfType<PlayerMove>();
    foreach (var p in allPlayers)
    {
        var view = p.GetComponent<Photon.Pun.PhotonView>();
        if (view == null || view.IsMine)  // Photon이 없거나 내 캐릭터면 통과
            return p;
    }
    return null;
}


    // 문제 텍스트, 정답 키워드, 타겟 오브젝트들 받아서 UI 띄우기
    public void ShowInputField(GameObject[] targetObjects, string keyword, string question)
    {
        currentTargets = targetObjects;
        correctKeyword = keyword.Trim().ToLower();

        questionText.text = question;
        questionText.gameObject.SetActive(true);

        inputField.text = "";
        inputField.gameObject.SetActive(true);
        inputField.ActivateInputField();

        if (player != null)
            player.enabled = false; // 조작 비활성화
    }

    // 엔터 입력 시 호출
    private void CheckInput(string input)
    {
        input = input.Trim().ToLower();

        if (input == correctKeyword)
        {
            if (currentTargets != null)
            {
                foreach (GameObject obj in currentTargets)
                {
                    obj.SetActive(false);
                }
                Debug.Log("정답! 오브젝트 제거 완료");
            }

            HideUI();
        }
        else
        {
            Debug.Log("오답! 흔들림");
            StartCoroutine(cameraShake.Shake(0.3f, 0.2f));
            inputField.text = "";
            inputField.ActivateInputField();
        }
    }

    // 문제 & 입력창 숨기고 조작 다시 허용
    private void HideUI()
    {
        questionText.gameObject.SetActive(false);
        inputField.gameObject.SetActive(false);

        if (player != null)
            player.enabled = true;
    }
}
