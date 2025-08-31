using UnityEngine;

public class TypingTrigger : MonoBehaviour
{
    public GameObject targetObject; // 사라질 B 오브젝트
    public string keyword; // 정답 키워드

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            TypingManager manager = FindObjectOfType<TypingManager>();
            if (manager != null)
            {
                Debug.Log("플레이어가 A에 닿았음, 입력필드 열기 시도 중");
                manager.ShowInputField(targetObject, keyword); // B 오브젝트 전달
            }
        }
    }
}
