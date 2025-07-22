using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneManager : MonoBehaviour
{
    public GameObject panel;       // UI 패널
    public Text cutsceneText;      // 텍스트 컴포넌트

    public void PlayCutscene(string message, float duration, System.Action onComplete = null)
    {
        StartCoroutine(CutsceneRoutine(message, duration, onComplete));
    }

    IEnumerator CutsceneRoutine(string message, float duration, System.Action onComplete)
    {
        panel.SetActive(true);
        cutsceneText.text = message;
        yield return new WaitForSeconds(duration);
        panel.SetActive(false);
        onComplete?.Invoke();
    }
}
