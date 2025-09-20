using UnityEngine;

public class TypingTrigger : MonoBehaviour
{
    public GameObject[] targetObjects; // 제거 대상
    public string keyword; // 정답
    public GameObject questionPanel; // 이 트리거에서 띄울 질문 UI

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            TypingManager manager = FindObjectOfType<TypingManager>();
            if (manager != null)
            {
                manager.ShowInputField(targetObjects, keyword, questionPanel);
            }
        }
    }
}
