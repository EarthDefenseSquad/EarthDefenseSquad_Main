using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToPlayScene : MonoBehaviour
{
    // 이 함수는 UI 버튼에 연결하면 됩니다.
    public void LoadPlayScene()
    {
        SceneManager.LoadScene(3);  // 씬 인덱스 3번 = Play 씬
    }
}
