using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class UIManager : MonoBehaviour
{

    public GameObject Pausepanel;

    private bool isPaused = false;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }


    public void PauseGame()
    {
        Pausepanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        Pausepanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void RestartStage()
    {
        Time.timeScale = 1f; // 반드시 시간 되돌리기
        SceneManager.LoadScene("StageScene");
    }

    public void GotoMain()
    {
        Time.timeScale = 1f; // 반드시 시간 되돌리기
        SceneManager.LoadScene("StartScene");
    }

    public void GotoStageSelect()
    {
        Time.timeScale = 1f; // 반드시 시간 되돌리기
        SceneManager.LoadScene("StageSelect");
    }


    public void QuitGame()
    {
        Application.Quit();
    }
    
    public void SkipStory()
    {
        Time.timeScale = 1f; // 반드시 시간 되돌리기
        SceneManager.LoadScene("StageSelect");
    }


}