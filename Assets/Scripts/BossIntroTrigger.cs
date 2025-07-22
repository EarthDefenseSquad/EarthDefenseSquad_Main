using System.Collections;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class BossIntroTrigger : MonoBehaviour
{
    [Header("보스 고유 설정")]
    [SerializeField] private string bossName = "디바이드";
    [TextArea]
    [SerializeField] private string bossDialogue = "여기까지 오다니... 하지만 이 다음을 갈 수 있을 거라 생각했나?";
    [SerializeField] private float dialogueDuration = 3f;

    [Header("보스 대사 UI")]
    [SerializeField] private GameObject bossDialogueUI;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("보스 관련 연결")]
    [SerializeField] private BossTimerController timerController;
    [SerializeField] private GameObject bossAI;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;
        StartCoroutine(BossIntroSequence());
    }

    private IEnumerator BossIntroSequence()
    {
        bossDialogueUI.SetActive(true);
        dialogueText.text = $"{bossName}: {bossDialogue}";

        yield return new WaitForSeconds(dialogueDuration);

        bossDialogueUI.SetActive(false);

        timerController.StartBossTimer();
        bossAI.SetActive(true);
    }
}
