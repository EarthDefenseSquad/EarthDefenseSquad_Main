using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishItem : MonoBehaviour
{
    public string itemID;  // 고유 ID: 예) "Stage1_Finish"

    private bool collected = false;

    void Start()
    {
        // 이미 수집한 적 있다면 투명화 처리
        if (PlayerPrefs.GetInt(itemID, 0) == 1)
        {
            collected = true;
            SetCollectedVisual();  // 투명하게 만들기
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // GameManager 가져오기
        GameManager gm = FindObjectOfType<GameManager>();

        // Finish 개수는 한 번만 증가
        if (!collected)
        {
            PlayerPrefs.SetInt(itemID, 1);
            gm.AddFinishItem();  // 실제 Finish 개수 증가
            collected = true;
        }

        // 항상 다음 스테이지로는 넘어가게
        //gm.NextStage();
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
