using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public int bossHealth = 3;
    public float attackInterval = 5f;
    private bool isVulnerable = false;
    private bool isDead = false;

    public GameObject[] bossHealthUI; // 보스 체력 UI 오브젝트 3개

    void Start()
    {
        UpdateBossHealthUI();
        StartCoroutine(BossAttackCycle());
    }

    IEnumerator BossAttackCycle()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(attackInterval);
            TryAttackPlayer();
        }
    }

    void TryAttackPlayer()
    {
        if (!isVulnerable)
        {
            Debug.Log("👹 보스 특수 공격 발동!");
            GameManager.Instance.HealthDown();
            // 카메라 흔들림 등 연출 가능
            Camera.main.GetComponent<CameraShake>()?.ShakeCamera();
        }
        else
        {
            Debug.Log("🧃 보스 공격 무효화됨!");
            // 아무 일도 없음
        }

        isVulnerable = false; // 무적 초기화
    }

    public void OnBossHit()
    {
        Debug.Log("✅ OnBossHit() 호출됨");

        isVulnerable = true;

        bossHealth--;
        Debug.Log($"📉 보스 체력 감소됨: {bossHealth}");

        UpdateBossHealthUI();

        if (bossHealth <= 0)
        {
            isDead = true;
            Debug.Log("💥 보스 패배 시퀀스 시작");

            GameManager.Instance.isBossActive = false;

            if (CutsceneManager.Instance != null)
                StartCoroutine(CutsceneManager.Instance.PlayEndingCutscene());
            else
                Debug.LogWarning("❗ CutsceneManager.Instance가 null입니다!");

            Destroy(gameObject);
        }
    }   

    void UpdateBossHealthUI()
    {
        for (int i = 0; i < bossHealthUI.Length; i++)
        {
            bossHealthUI[i].SetActive(i < bossHealth);
        }
    }
}
