using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class TypingManager : MonoBehaviour
{
    public TMP_InputField inputField;
    public CameraShake_tmp cameraShake;

    private Dictionary<string, GameObject> activeTargetMap = new Dictionary<string, GameObject>();
    private string currentKeyword = "";
    private GameObject currentTarget;

    private void Start()
    {
        inputField.gameObject.SetActive(false); // 처음엔 비활성화
        inputField.onEndEdit.AddListener(CheckInput);
    }

    public void ActivateTyping(string keyword, GameObject target)
    {
        currentKeyword = keyword.Trim().ToLower();
        currentTarget = target;
        inputField.text = "";
        inputField.gameObject.SetActive(true);
        inputField.ActivateInputField();
    }

    private void CheckInput(string input)
    {
        string userInput = input.Trim().ToLower();

        if (userInput == currentKeyword)
        {
            if (currentTarget != null)
            {
                currentTarget.SetActive(false);
                Debug.Log("정답 입력: 오브젝트 비활성화");
            }

            inputField.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("오답 입력: 화면 흔들림");
            StartCoroutine(cameraShake.Shake(0.3f, 0.2f));
            inputField.text = "";
            inputField.ActivateInputField();
        }
    }
}
