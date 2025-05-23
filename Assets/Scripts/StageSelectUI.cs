using UnityEngine;
using UnityEngine.UI;

public class StageSelectUI : MonoBehaviour
{
    public Button[] stageButtons;

    void Start()
    {
        for (int i = 0; i < stageButtons.Length; i++)
        {
            Button btn = stageButtons[i];
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
}
