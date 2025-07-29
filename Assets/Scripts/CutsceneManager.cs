using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager Instance;

    [Header("컷씬 UI 구성요소")]
    public CanvasGroup cutsceneCanvas;
    public TextMeshProUGUI cutsceneText;
    public Image cutsceneImage;  // ✅ 이미지 추가

    [Header("컷씬 이미지")]
    public Sprite bossStartSprite;
    public Sprite bossEndSprite;

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
        StartCoroutine(CutsceneSequence("Now \n The REVENGE Begins", bossStartSprite, onComplete));
    }

    IEnumerator CutsceneSequence(string message, Sprite image, Action onComplete)
    {
        cutsceneText.text = message;
        cutsceneImage.sprite = image; // ✅ 이미지 설정
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
        cutsceneImage.sprite = bossEndSprite; // ✅ 엔딩 이미지 설정

        cutsceneCanvas.alpha = 1;
        cutsceneCanvas.blocksRaycasts = true;

        yield return new WaitForSeconds(3f);

        cutsceneCanvas.alpha = 0;
        cutsceneCanvas.blocksRaycasts = false;
    }
}
