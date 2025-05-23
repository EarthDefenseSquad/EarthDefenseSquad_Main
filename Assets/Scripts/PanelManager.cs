using System.Collections;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    public GameObject CharacterPanel;
    public GameObject LoadingPanel;

    // 2초(또는 원하는 시간) 후 지정한 패널 켜기
    public void GoCharacterPanel()
    {
        StartCoroutine(DelayTime(3.5f));
    }

    IEnumerator DelayTime(float time)
    {
        yield return new WaitForSeconds(time);
        LoadingPanel.SetActive(false);
        CharacterPanel.SetActive(true);
    }
}
