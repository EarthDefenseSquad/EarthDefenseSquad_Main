using UnityEngine;
using UnityEngine.SceneManagement;

public class NPCInteractionTrigger : MonoBehaviour
{
    public GameObject interactionUI;      // 머리 위 UI 오브젝트
    public GameObject outlineObject;      // 테두리 오브젝트
    private bool playerInRange = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            interactionUI.SetActive(true);
            outlineObject.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            interactionUI.SetActive(false);
            outlineObject.SetActive(false);
        }
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.F))
        {
            LoadNextScene(); // 씬 전환
        }
    }

    void LoadNextScene()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentScene + 1);
    }
}
