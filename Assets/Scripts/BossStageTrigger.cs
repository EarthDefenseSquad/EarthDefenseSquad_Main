using UnityEngine;

public class BossStageTrigger : MonoBehaviour
{
    public CutsceneManager cutsceneManager;
    public GameObject boss;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            cutsceneManager.PlayCutscene(() =>
            {
                boss.SetActive(true); // 컷씬 후 보스 등장
                GameManager.Instance.SetBossActive(true); // 보스 활성화 상태 설정
            });
            gameObject.SetActive(false); // 트리거 꺼주기
        }
    }
}
