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
            });
            gameObject.SetActive(false); // 트리거 꺼주기
        }
    }
}
