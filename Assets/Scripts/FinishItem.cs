using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishItem : MonoBehaviour
{
    public string itemID;  // 고유 ID: 예) "Stage1_Finish"

    private bool collected = false;

    void Start()
    {
        int localState = PlayerPrefs.GetInt(itemID, 0);

        // 투명화 처리
        if (localState == 1)
        {
            collected = true;
            SetCollectedVisual();
        }

        // GameManager에 동기화 요청 (자기 값 보내기)
        GameManager.Instance?.RequestItemSync(itemID, localState);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || collected) return;

        // 로컬 저장
        PlayerPrefs.SetInt(itemID, 1);
        PlayerPrefs.Save();

        // 시각 효과
        collected = true;
        SetCollectedVisual();

        // FinishItem 카운트 증가
        GameManager.Instance?.AddFinishItem();

        // 동기화 전파
        GameManager.Instance?.SendItemCollected(itemID);
    }

    void SetCollectedVisual()
    {
        // SpriteRenderer를 투명하게
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color c = sr.color;
            c.a = 0.3f;
            sr.color = c;
        }
    }
}
