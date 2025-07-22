using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class hmGM : MonoBehaviour
{

    public GameObject Pausepanel;
    private bool isPaused = false;

    // 세대 = 씬, 스테이지 = 패널로 생각
    // 로드할 씬(세대) 이름
    public string YearScene;
    // 씬이 로드된 후 활성화할 패널(스테이지) 이름 (씬 내에 있어야 함)
    public string Stage1panel; //스테이지 여러개라면 어떻게..?

    
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

    public void RestartGame()
    {
        Time.timeScale = 1f; // 반드시 시간 되돌리기
        SceneManager.LoadScene("StartScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }


    // *** 세대씬 > 스테이지 패널 로드 ***
    // 스테이지 1만 구현돼있는데 2~10스테이지는 그러면 코드하나하나 다짜야되나? 일단나중에생각하기 ㅎ
    
    // 버튼(스테이지선택UI)에 연결할 함수
    public void LoadSceneAndActivatePanel()
    {
        StartCoroutine(LoadSceneAndActivate());
    }

    private IEnumerator LoadSceneAndActivate()
    {
        // 씬 비동기 로드 시작
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(YearScene);

        // 씬이 완전히 로드될 때까지 대기
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // 씬 로드 완료 후 모든 패널을 비활성화
        GameObject[] panels = GameObject.FindGameObjectsWithTag("StagePanel"); // 'Panel' 태그를 가진 모든 오브젝트 찾기
        foreach (var panel in panels)
        {
            panel.SetActive(false);
        }

        // 특정 패널만 활성화
        GameObject targetPanel = GameObject.Find(Stage1panel);
        if (targetPanel != null)
        {
            targetPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning($"씬 내에 '{Stage1panel}' 패널을 찾을 수 없습니다.");
        }
    }


}