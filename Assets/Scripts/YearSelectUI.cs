using UnityEngine;
using UnityEngine.UI;

public class YearSelectUI : MonoBehaviour
{
    public Button[] yearButtons;

    void Start()
    {
        for (int i = 0; i < yearButtons.Length; i++)
        {
            Button btn = yearButtons[i];

            bool isUnlocked = (i == 0); // 첫 번째 세대만 해금

            btn.interactable = isUnlocked;

            Transform lockIcon = btn.transform.Find("LockIcon");
            Image bg = btn.GetComponent<Image>();

            if (isUnlocked)
            {
                //if (bg) bg.color = Color.white;
                if (lockIcon) lockIcon.gameObject.SetActive(false);
            }
            else
            {
                //if (bg) bg.color = Color.gray;
                if (lockIcon) lockIcon.gameObject.SetActive(true);
            }

            int yearNumber = i + 1;
            btn.onClick.AddListener(() =>
            {
                if (isUnlocked)
                    Debug.Log($"시대 {yearNumber} 선택됨");
                else
                    Debug.Log($"시대 {yearNumber}은 잠겨 있음");
            });
        }
    }
}
