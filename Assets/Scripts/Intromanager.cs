using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Intromanager : MonoBehaviour
{

    public GameObject StartPanel;
    public GameObject IntroPanel;
    public GameObject Optionpanel;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DelayTime(3.5f));
        Optionpanel.SetActive(false);
    }

    IEnumerator DelayTime(float time)
    {
        yield return new WaitForSeconds(time);

        IntroPanel.SetActive(false);
        StartPanel.SetActive(true);
    }

    // Update is called once per frame
    public void GoGameScene()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void Quit()
    {
        // 에디터에서는 실행 멈춤
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // 실제 빌드에서는 애플리케이션 종료
        Application.Quit();
#endif
    }
    
    //public void OptionButtonOn()
    //{
    //        OpenOption();
    //}

    public void OpenOption()
    {
        Time.timeScale = 0f;  // 게임 멈춤
        Optionpanel.SetActive(true);  // 메뉴 보이기
        //isPaused = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;  // 게임 재개
        Optionpanel.SetActive(false);  // 메뉴 숨기기
        //isPaused = false;
    }
}
