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

}
