using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StageSelectUI : MonoBehaviour
{
    public Button[] stageButtons1960;

    void Start()
    {
        for (int i = 0; i < stageButtons1960.Length; i++)
        {
            Button btn = stageButtons1960[i];
            bool isUnlocked = (i == 0); // 첫 번째 스테이지만 해금

            btn.interactable = isUnlocked;

            Transform lockIcon = btn.transform.Find("LockIcon");
            Image bg = btn.GetComponent<Image>();

            if (isUnlocked)
            {
                if (bg) bg.color = Color.white;
                if (lockIcon) lockIcon.gameObject.SetActive(false);
            }
            else
            {
                //if (bg) bg.color = Color.gray;
                if (lockIcon) lockIcon.gameObject.SetActive(true);
            }

            int stageNumber = i + 1;
            btn.onClick.AddListener(() =>
            {
                if (isUnlocked)
                    Debug.Log($"스테이지 {stageNumber} 선택됨");
                else
                    Debug.Log($"스테이지 {stageNumber}은 잠겨 있음");
            });
        }
    }

    public void Go1960Scene()
    {
        SceneManager.LoadScene("StageScene");
    }

    public void Go1970Scene()
    {
        SceneManager.LoadScene("StageScene");
    }

    public void Go1980Scene()
    {
        SceneManager.LoadScene("StageScene");
    }

    public void Go1990Scene()
    {
        SceneManager.LoadScene("StageScene");
    }

    public void Go2000Scene()
    {
        SceneManager.LoadScene("StageScene");
    }

    public void Go2010Scene()
    {
        SceneManager.LoadScene("StageScene");
    }


}
