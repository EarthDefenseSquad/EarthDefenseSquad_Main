using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class CutsceneManager : MonoBehaviour
{
    public CanvasGroup cutsceneCanvas;

    public static CutsceneManager Instance;

    public GameObject CutScenePanel;
    public TextMeshProUGUI cutsceneText;


    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void PlayCutscene(Action onComplete)
    {
        StartCoroutine(CutsceneSequence(onComplete));
    }

    IEnumerator CutsceneSequence(Action onComplete)
    {
        cutsceneCanvas.alpha = 1;
        cutsceneCanvas.blocksRaycasts = true;

        yield return new WaitForSeconds(3f); // 컷씬 길이

        cutsceneCanvas.alpha = 0;
        cutsceneCanvas.blocksRaycasts = false;

        onComplete?.Invoke();
    }

    public IEnumerator PlayEndingCutscene()
    {
        CutScenePanel.SetActive(true);
        cutsceneText.text = "크윽… 분하다… 다음 기회를…";
        yield return new WaitForSeconds(3f);
        CutScenePanel.SetActive(false);
        // 엔딩 처리 or 스테이지 클리어
    }
}
