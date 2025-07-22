using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStageTrigger : MonoBehaviour
{
    public CutsceneManager cutsceneManager; // 컷씬 UI 관리자
    public GameObject bossItemObject;       // 보스 전용 아이템 (처음엔 비활성화 상태)

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        CameraShake shaker = Camera.main.GetComponent<CameraShake>();

        cutsceneManager.PlayCutscene("드디어 여기까지 왔군... 날 이기면 데이터를 넘겨주지...", 3f, () =>
        {
            StartCoroutine(shaker.Shake(0.5f, 0.3f)); // 화면 흔들기
            bossItemObject.SetActive(true);           // 아이템 등장
        });

        Destroy(gameObject); // 이 트리거는 1회용
    }
}
