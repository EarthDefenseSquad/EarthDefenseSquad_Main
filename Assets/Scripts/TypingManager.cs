using UnityEngine;
using TMPro;
using System.Collections;

public class TypingManager : MonoBehaviour
{
    public TMP_InputField inputField;
    public CameraShake_tmp cameraShake;

    private GameObject currentTarget; // B 오브젝트
    private string correctKeyword;

    private void Start()
    {
        inputField.gameObject.SetActive(false);
        inputField.onSubmit.AddListener(CheckInput); // Enter 입력 감지
    }

    public void ShowInputField(GameObject targetObject, string keyword)
    {
        currentTarget = targetObject;
        correctKeyword = keyword.Trim().ToLower();

        inputField.text = "";
        inputField.gameObject.SetActive(true);
        inputField.ActivateInputField();
    }

    private void CheckInput(string input)
    {
        input = input.Trim().ToLower();

        if (input == correctKeyword)
        {
            if (currentTarget != null)
            {
                currentTarget.SetActive(false); // B 오브젝트 비활성화
                Debug.Log("정답 입력됨, 오브젝트 비활성화");
            }

            inputField.gameObject.SetActive(false); // 입력창 닫기
        }
        else
        {
            Debug.Log("오답!");
            StartCoroutine(cameraShake.Shake(0.3f, 0.2f));
            inputField.text = "";
            inputField.ActivateInputField();
        }
    }
}
