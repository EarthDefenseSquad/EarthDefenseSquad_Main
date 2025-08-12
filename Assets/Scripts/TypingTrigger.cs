using UnityEngine;

public class TypingTrigger : MonoBehaviour
{
    public string keyword;
    private bool isUsed = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isUsed) return;

        if (other.CompareTag("Player"))
        {
            TypingManager manager = FindObjectOfType<TypingManager>();
            if (manager != null)
            {
                //manager.ActivateTyping(keyword, gameObject);
                isUsed = true;
            }
        }
    }
}
