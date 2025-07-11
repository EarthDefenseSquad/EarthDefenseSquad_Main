using UnityEngine;
using UnityEngine.SceneManagement;

public class NPCInteraction : MonoBehaviour
{
    public float interactRange = 2f;        // 플레이어와의 상호작용 거리
    public string goodEndingScene = "GoodEnding";
    public string badEndingScene = "BadEnding";
    private bool isPlayerNearby = false;
    private GameObject player;

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.F))
        {
            int collected = PlayerPrefs.GetInt("FinishItemCount", 0);  // 저장된 finish 개수 불러오기
            int required = FindObjectOfType<GameManager>().totalStages;

            if (collected >= required)
            {
                Debug.Log("🎉 Good Ending으로 이동");
                SceneManager.LoadScene(goodEndingScene);
            }
            else
            {
                Debug.Log("💀 Bad Ending으로 이동");
                SceneManager.LoadScene(badEndingScene);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = true;
            player = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = false;
            player = null;
        }
    }
}
