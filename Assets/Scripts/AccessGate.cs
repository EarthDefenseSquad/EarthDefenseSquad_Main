using UnityEngine;

public class AccessGate : MonoBehaviour
{
    [Header("점수 조건")]
    public int requiredScore;

    private BoxCollider2D col;
    private SpriteRenderer sr;

    private void Awake()
    {
        col = GetComponent<BoxCollider2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerMove player = collision.gameObject.GetComponent<PlayerMove>();

        if (player != null)
        {
            if (!player.hasAccessPass)
            {
                Debug.Log("❌ AccessPass 아이템이 없습니다.");
                return;
            }

            if (GameManager.Instance.stagePoint >= requiredScore)
            {
                GameManager.Instance.stagePoint -= requiredScore;
                Debug.Log($"✅ {requiredScore}점 차감 후 통과. 남은 점수: {GameManager.Instance.stagePoint}");

                col.enabled = false;
                if (sr != null) sr.enabled = false; // 시각적 제거

                // 필요하면 사운드, 애니메이션 등 추가 가능
            }
            else
            {
                Debug.Log($"❌ 점수 부족 ({GameManager.Instance.stagePoint} / 필요: {requiredScore})");
            }
        }
    }
}
