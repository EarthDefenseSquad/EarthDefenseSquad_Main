using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Collections;
public class PlayerBubble : MonoBehaviour
{
    public Canvas bubbleCanvas; // World Space Canvas
    public TextMeshProUGUI bubbleText;
    public float showTime = 3f; // 말풍선 표시 시간

    public void ShowBubble(string msg)
    {
        bubbleText.text = msg;
        bubbleCanvas.enabled = true;
        StopAllCoroutines();
        StartCoroutine(AutoHide());
    }

    IEnumerator AutoHide()
    {
        yield return new WaitForSeconds(showTime);
        bubbleCanvas.enabled = false;
    }
}
