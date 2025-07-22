using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BossController : MonoBehaviour
{
    public int lives = 3;
    public Image[] heartImages; // UI에 연결
    public GameObject cutscenePanel;
    public Text cutsceneText;

    public void TakeSpecialDamage()
    {
        if (lives <= 0) return;

        lives--;
        UpdateHeartUI();

        if (lives <= 0)
        {
            StartCoroutine(PlayDeathCutscene());
        }
    }

    void UpdateHeartUI()
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            heartImages[i].enabled = i < lives;
        }
    }

    IEnumerator PlayDeathCutscene()
    {
        cutscenePanel.SetActive(true);
        cutsceneText.text = "크윽… 분하다… 다음 기회를 논하지...";
        yield return new WaitForSeconds(3f);
        cutscenePanel.SetActive(false);
        Destroy(gameObject); // 보스 삭제
    }
}
