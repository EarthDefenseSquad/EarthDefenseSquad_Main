using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TypingManager : MonoBehaviour
{
    public TMP_InputField inputField;
    public CameraShake_tmp cameraShake; // CameraShake 스크립트 참조

    [System.Serializable]
    public class KeywordObjectPair
    {
        public string keyword;
        public GameObject targetObject;
    }

    public List<KeywordObjectPair> keywordObjectPairs;
    private Dictionary<string, GameObject> keywordToObject = new Dictionary<string, GameObject>();

    private void Start()
    {
        foreach (var pair in keywordObjectPairs)
        {
            string keyword = pair.keyword.Trim().ToLower();
            if (!keywordToObject.ContainsKey(keyword))
                keywordToObject.Add(keyword, pair.targetObject);
        }

        inputField.onEndEdit.AddListener(CheckInput);
    }

    private void CheckInput(string userInput)
    {
        userInput = userInput.Trim().ToLower();

        if (keywordToObject.ContainsKey(userInput))
        {
            GameObject target = keywordToObject[userInput];
            if (target != null && target.activeSelf)
            {
                target.SetActive(false);
                Debug.Log($"'{userInput}' 입력으로 오브젝트 비활성화됨");
            }
        }
        else
        {
            Debug.Log($"'{userInput}' 은/는 오답입니다.");
            StartCoroutine(cameraShake.Shake(0.3f, 0.2f)); // 흔들기 호출
        }

        inputField.text = "";
        inputField.ActivateInputField();
    }
}
