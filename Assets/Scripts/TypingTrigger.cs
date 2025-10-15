using UnityEngine;

public class TypingTrigger : MonoBehaviour
{
    public GameObject[] targetObjects; // 제거 대상
    public string keyword; // 정답
    public GameObject questionPanel; // 질문 UI

    public string questionText; // 문제 문장

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            TypingManager manager = FindObjectOfType<TypingManager>();
            if (manager != null)
            {
                //manager.ShowInputField(gameObject, targetObjects, keyword, questionPanel);
                manager.ShowInputField(gameObject, targetObjects, keyword, questionPanel, questionText);

            }
        }
    }
}
