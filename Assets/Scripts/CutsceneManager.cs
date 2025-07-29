using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager Instance;

    [Header("컷씬 UI 구성요소")]
    public CanvasGroup cutsceneCanvas;
    public TextMeshProUGUI cutsceneText;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // 시작 시 캔버스 비활성화
        cutsceneCanvas.alpha = 0;
        cutsceneCanvas.blocksRaycasts = false;
    }

    public void PlayCutscene(Action onComplete)
    {
        StartCoroutine(CutsceneSequence("Now" + "The REVENGE Begins", onComplete));
    }

    IEnumerator CutsceneSequence(string message, Action onComplete)
    {
        cutsceneText.text = message;
        cutsceneCanvas.alpha = 1;
        cutsceneCanvas.blocksRaycasts = true;

        yield return new WaitForSeconds(3f);

        cutsceneCanvas.alpha = 0;
        cutsceneCanvas.blocksRaycasts = false;

        onComplete?.Invoke();
    }

    public IEnumerator PlayEndingCutscene()
    {
        cutsceneText.text = "Next Time... The Final Battle!";
        cutsceneCanvas.alpha = 1;
        cutsceneCanvas.blocksRaycasts = true;

        yield return new WaitForSeconds(3f);

        cutsceneCanvas.alpha = 0;
        cutsceneCanvas.blocksRaycasts = false;
    }
}
