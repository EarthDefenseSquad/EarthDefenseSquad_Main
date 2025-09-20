using UnityEngine;

public class TypingTrigger : MonoBehaviour
{
    public GameObject[] targetObjects; // 사라질 오브젝트들
    public string keyword;             // 정답 키워드
    [TextArea] public string questionText; // 문제 텍스트

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            TypingManager manager = FindObjectOfType<TypingManager>();
            if (manager != null)
            {
                Debug.Log("플레이어가 아이템에 닿음, 문제창 표시");
                manager.ShowInputField(targetObjects, keyword, questionText);
            }
        }
    }
}
