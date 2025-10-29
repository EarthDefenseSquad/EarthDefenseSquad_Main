using UnityEngine;

public class AccessGate : MonoBehaviour
{
    [Header("점수 조건")]
    public int requiredScore;

    private BoxCollider2D col;
    private SpriteRenderer sr;

    public GameManager gameManager;

    private void Awake()
    {
        col = GetComponent<BoxCollider2D>();
        sr = GetComponent<SpriteRenderer>();
        gameManager = GameObject.FindGameObjectWithTag("MainManager").GetComponent<GameManager>();
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerMove player = collision.gameObject.GetComponent<PlayerMove>();

        if (player != null)
        {
            if (!player.hasAccessPass)
            {
                Debug.Log("AccessPass 아이템이 없습니다.");
                return;
            }



            if (gameManager.stagePoint >= requiredScore)
            {
                gameManager.stagePoint -= requiredScore;
                Debug.Log($"{requiredScore}점 차감 후 통과. 남은 점수: {gameManager.stagePoint}");

                col.enabled = false;
                // 오브젝트 전체 비활성화
                gameObject.SetActive(false);
            }
            else
            {
                Debug.Log($"점수 부족 ({gameManager.stagePoint} / 필요: {requiredScore})");
            }
        }
    }
}
